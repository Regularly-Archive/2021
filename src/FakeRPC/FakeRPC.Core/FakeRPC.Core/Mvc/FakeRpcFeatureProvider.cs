using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FakeRpc.Core.Mvc
{
    internal class FakeRpcFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            var type = typeInfo.AsType();
            var fakeRpc = type.GetCustomAttribute<FakeRpcAttribute>();
            return fakeRpc != null;
        }
    }
}
