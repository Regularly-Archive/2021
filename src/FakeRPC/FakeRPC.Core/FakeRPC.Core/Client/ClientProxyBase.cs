using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;
using FakeRpc.Core.Mvc;
using System.Linq;

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

            if (args.Length == 1)
            {
                var requestType = args[0].GetType();
                var responseType = targetMethod.ReturnType.GenericTypeArguments[0];
                dynamic result = CallUnaryRequest(RpcCalls, requestType, responseType, new Uri(serviceUrl), args[0]);
                return Task.FromResult(result.Result);
            }
            else if (args.Length == 0)
            {
                var responseType = targetMethod.ReturnType.GenericTypeArguments[0];
                dynamic result = CallEmptyRequest(RpcCalls, responseType, new Uri(serviceUrl));
                return Task.FromResult(result.Result);
            }

            throw new Exception("FakeRpc only support a RPC method with 0 or 1 parameter");
        }

        private dynamic CallUnaryRequest(IFakeRpcCalls rpcCalls, Type requestType, Type responseType, Uri uri, object request)
        {
            // IFakeRpcCalls.CallAsync<TRequest,TResponse>(uri, request);
            var callMethod = Expression.Call(
                Expression.Constant(rpcCalls),
                rpcCalls.GetType().GetMethods().ToList().First(x => x.Name == "CallAsync" && x.GetParameters().Length == 2).MakeGenericMethod(requestType, responseType),
                new Expression[]
                {
                    Expression.Constant(uri, uri.GetType()),
                    Expression.Constant(request,request.GetType()),
                }
            );

            // () => IFakeRpcCalls.CallAsync<TRequest, TResponse>(uri, request);
            var lambdaExp = Expression.Lambda(callMethod, null);

            var caller = lambdaExp.Compile();
            return caller.DynamicInvoke();
        }

        private dynamic CallEmptyRequest(IFakeRpcCalls rpcCalls, Type responseType, Uri uri)
        {
            // IFakeRpcCalls.CallAsync<TResponse>(uri);
            var callMethod = Expression.Call(
                Expression.Constant(rpcCalls),
                rpcCalls.GetType().GetMethods().ToList().First(x => x.Name == "CallAsync" && x.GetParameters().Length == 1).MakeGenericMethod(responseType), 
                new Expression[]
                {
                    Expression.Constant(uri, uri.GetType()),
                }
            );

            // () => IFakeRpcCalls.CallAsync<TResponse>(uri);
            var lambdaExp = Expression.Lambda(callMethod, null);

            var caller = lambdaExp.Compile();
            return caller.DynamicInvoke();
        }
    }
}
