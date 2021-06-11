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
        private readonly GrpcPollyPolicyOptions _pollyOptions;
        public GrpcCallInvoker(
            Channel channel,
            GrpcPollyPolicyOptions pollyOptions
        )
        {
            _channel = channel;
            _pollyOptions = pollyOptions;
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var policy = CreatePollyPolicy<AsyncClientStreamingCall<TRequest, TResponse>>();
            return policy.Execute(() => Calls.AsyncClientStreamingCall(CreateCall(method, host, options)));
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var policy = CreatePollyPolicy<AsyncDuplexStreamingCall<TRequest, TResponse>>();
            return policy.Execute(() => Calls.AsyncDuplexStreamingCall(CreateCall(method, host, options)));
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var policy = CreatePollyPolicy<AsyncServerStreamingCall<TResponse>>();
            return policy.Execute(() => Calls.AsyncServerStreamingCall(CreateCall(method, host, options), request));
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var policy = CreatePollyPolicy<AsyncUnaryCall<TResponse>>();
            return policy.Execute(() => Calls.AsyncUnaryCall(CreateCall(method, host, options), request));
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var policy = CreatePollyPolicy<TResponse>();
            return policy.Execute(() => Calls.BlockingUnaryCall(CreateCall(method, host, options), request));
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

        public virtual Policy<TResult> CreatePollyPolicy<TResult>()
        {
            Policy<TResult> policy = null; ;

            // 构造断路器策略
            if (_pollyOptions.CircuitBreakerCount > 0)
            {
                var policyBreaker = Policy<TResult>
                    .Handle<Exception>()
                    .CircuitBreaker(_pollyOptions.CircuitBreakerCount, _pollyOptions.CircuitBreakerTime);

                policy = policy == null ? policyBreaker :
                    policy.Wrap(policyBreaker) as Policy<TResult>;

                // 断路器降级
                var policyFallBack = Policy<TResult>
                    .Handle<Polly.CircuitBreaker.BrokenCircuitException>()
                    .Fallback(() =>
                    {
                        return default(TResult);
                    });
                policy = policyFallBack.Wrap(policy);
            }

            // 构造超时策略
            if (_pollyOptions.Timeout > TimeSpan.Zero)
            {
                var policyTimeout = Policy.Timeout(() => _pollyOptions.Timeout, Polly.Timeout.TimeoutStrategy.Pessimistic);

                policy = policy == null ? (Policy<TResult>)policyTimeout.AsPolicy<TResult>() :
                    policy.Wrap(policyTimeout);

                // 超时降级
                var policyFallBack = Policy<TResult>
                    .Handle<Polly.Timeout.TimeoutRejectedException>()
                    .Fallback(() =>
                    {
                        return default(TResult);
                    });
                policy = policyFallBack.Wrap(policy);
            }

            // 构造重试策略
            if (_pollyOptions.RetryCount > 0)//重试
            {
                var retryPolicy = Policy<TResult>.Handle<Exception>().WaitAndRetry(_pollyOptions.RetryCount, x => _pollyOptions.RetryInterval, (result, timeSpan, current, context) =>
                {
                    Console.WriteLine($"正在进行第{current}次重试，间隔{timeSpan.TotalMilliseconds}秒");
                });

                policy = policy == null ? retryPolicy :
                    policy.Wrap(retryPolicy) as Policy<TResult>;
            }

            return policy;
        }
    }
}
