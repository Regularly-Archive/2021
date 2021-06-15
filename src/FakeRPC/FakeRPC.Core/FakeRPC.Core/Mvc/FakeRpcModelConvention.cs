using FakeRpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FakeRpc.Core.Mvc
{
    internal class FakeRpcModelConvention : IApplicationModelConvention
    {
        /// <summary>
        /// 路由前缀，默认为rpc
        /// </summary>
        private const string RoutePrefix = "rpc";

        /// <summary>
        /// HTTP方法，默认为POST
        /// </summary>
        private const string HttpMethod = "POST";


        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var type = controller.ControllerType.AsType();
                var fakeRpc = type.GetCustomAttribute<FakeRpcAttribute>();
                if (fakeRpc != null)
                {
                    ConfigureApiExplorer(controller);
                    ConfigureSelector(controller);
                    ConfigureParameters(controller);
                }
            }
        }

        private void ConfigureApiExplorer(ControllerModel controller)
        {
            if (string.IsNullOrEmpty(controller.ApiExplorer.GroupName))
                controller.ApiExplorer.GroupName = controller.ControllerName;

            if (controller.ApiExplorer.IsVisible == null)
                controller.ApiExplorer.IsVisible = true;

            controller.Actions.ToList().ForEach(action => ConfigureApiExplorer(action));
        }

        private void ConfigureApiExplorer(ActionModel action)
        {
            if (action.ApiExplorer.IsVisible == null)
                action.ApiExplorer.IsVisible = true;
        }

        private void ConfigureSelector(ControllerModel controller)
        {
            controller.Selectors.ToList().RemoveAll(selector =>
                selector.AttributeRouteModel == null && (selector.ActionConstraints == null || !selector.ActionConstraints.Any())
            );

            if (controller.Selectors.Any(selector => selector.AttributeRouteModel != null))
                return;

            var areaName = string.Empty;
            controller.Actions.ToList().ForEach(action => ConfigureSelector(areaName, controller.ControllerName, action));
        }

        private void ConfigureSelector(string areaName, string controllerName, ActionModel action)
        {
            action.Selectors.ToList().RemoveAll(selector =>
                selector.AttributeRouteModel == null && (selector.ActionConstraints == null || !selector.ActionConstraints.Any())
            );

            if (!action.Selectors.Any())
            {
                action.Selectors.Add(CreateActionSelector(areaName, controllerName, action));
            }
            else
            {
                action.Selectors.ToList().ForEach(selector =>
                {
                    var routePath = $"{RoutePrefix}/{areaName}/{controllerName}/{action.ActionName}".Replace("//", "/");
                    var routeModel = new AttributeRouteModel(new RouteAttribute(routePath));
                    selector.AttributeRouteModel = routeModel;
                    selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { HttpMethod }));
                });

            }
        }

        private void ConfigureParameters(ControllerModel controller)
        {
            controller.Actions.ToList().ForEach(action => ConfigureActionParameters(action));
        }

        private void ConfigureActionParameters(ActionModel action)
        {
            foreach (var parameter in action.Parameters)
            {
                if (parameter.BindingInfo != null)
                    continue;

                var type = parameter.ParameterInfo.ParameterType;
                //if (type.IsPrimitive || type.IsEnum ||
                //    (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                //{
                    if (IsFromBodyEnable(action, parameter))
                    {
                        parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                    }
                //}
            }
        }

        private SelectorModel CreateActionSelector(string areaName, string controllerName, ActionModel action)
        {
            var selectorModel = new SelectorModel();
            var routePath = $"{RoutePrefix}/{areaName}/{controllerName}/{action.ActionName}".Replace("//", "/");
            selectorModel.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(routePath));
            selectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { HttpMethod }));
            return selectorModel;
        }

        private bool IsFromBodyEnable(ActionModel action, ParameterModel parameter)
        {
            foreach (var selector in action.Selectors)
            {
                if (selector.ActionConstraints == null)
                    continue;

                var httpMethods = new string[] { HttpMethod };
                var actionConstraints = selector.ActionConstraints
                    .Select(ac => ac as HttpMethodActionConstraint)
                    .Where(ac => ac != null)
                    .SelectMany(ac => ac.HttpMethods).Distinct().ToList();
                if (actionConstraints.Any(ac => httpMethods.Contains(ac)))
                    return true;
            }

            return false;
        }
    }
}
