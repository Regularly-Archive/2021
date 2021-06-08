using Grpc.Core;
using Grpc.Net.Client;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRPC.Logging
{
    class GrpcCallInvoker : CallInvoker
    {
        private readonly Channel _channel;
        public GrpcCallInvoker(
            Channel channel
            )
        {
            _channel = channel;
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var call = CreateCall(method, host, options);
            return Calls.AsyncClientStreamingCall(call);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            throw new NotImplementedException();
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            throw new NotImplementedException();
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            throw new NotImplementedException();
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            throw new NotImplementedException();
        }

        protected CallInvocationDetails<TRequest, TResponse> CreateCall<TRequest, TResponse>(
            Method<TRequest, TResponse> method, 
            string host, 
            CallOptions options
        )
            where TRequest : class
            where TResponse : class
        {
            return new CallInvocationDetails<TRequest, TResponse>(_channel, method, options);
        }
    }
}
