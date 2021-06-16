using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;
using FakeRpc.Core.Mvc;

namespace FakeRpc.Core.Client
{
    public class ClientProxyBase : DispatchProxy
    {
        public string ServiceName { get; set; }

        public HttpClient HttpClient { get; set; }

        public IFakeRpcCalls RpcCalls { get; set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var serviceUrl = $"{(HttpClient).BaseAddress.ToString()}rpc/{ServiceName}/{targetMethod.Name}";

            var requestType = args[0].GetType();
            var responseType = targetMethod.ReturnType.GenericTypeArguments[0];
            var callMethod = RpcCalls.GetType().GetMethod("CallAsync").MakeGenericMethod(requestType, responseType);

            dynamic caller = callMethod.Invoke(RpcCalls, new object[] { new Uri(serviceUrl), args[0] });
            dynamic result = caller.Result;
            return Task.FromResult(result);
        }
    }
}
