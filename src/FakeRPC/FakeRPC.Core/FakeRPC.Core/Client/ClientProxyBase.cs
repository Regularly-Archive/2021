using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;

namespace FakeRpc.Core.Mvc
{
    public class ClientProxyBase : DispatchProxy
    {
        public string ServiceName { get; set; }

        public HttpClient HttpClient { get; set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var serviceUrl = $"{(HttpClient).BaseAddress.ToString()}rpc/{ServiceName}/{targetMethod.Name}";
            var rpcCalls = new FakeRpcCalls(HttpClient);

            var requestType = args[0].GetType();
            var responseType = targetMethod.ReturnType.GenericTypeArguments[0];
            var callMethod = typeof(FakeRpcCalls).GetMethod("CallAsync").MakeGenericMethod(requestType, responseType);

            dynamic caller = callMethod.Invoke(rpcCalls, new object[] { new Uri(serviceUrl), args[0] });
            dynamic result = caller.Result;
            return Task.FromResult(result);
        }
    }
}
