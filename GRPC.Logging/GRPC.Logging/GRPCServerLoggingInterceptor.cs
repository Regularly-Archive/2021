﻿using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRPC.Logging
{
    public class GrpcServerLoggingInterceptor : Interceptor
    {
        private readonly ILogger<GrpcServerLoggingInterceptor> _logger;
        public GrpcServerLoggingInterceptor(ILogger<GrpcServerLoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var builder = new StringBuilder();

            // Call gRPC begin
            builder.AppendLine($"Call gRPC {context.Host}/{context.Method} begin.");

            // Logging Request
            builder.AppendLine(LogRequest(request));

            // Logging Response
            var reply = continuation(request, context);
            var response = reply.Result;
            var exception = reply.Exception;
            builder.AppendLine(LogResponse(response, exception));

            // Call gRPC finish
            builder.AppendLine($"Call gRPC {context.Host}/{context.Method} finish.");
            _logger.LogInformation(builder.ToString());

            return reply;
        }

        private string LogRequest<TRequest>(TRequest request)
        {
            var payload = string.Empty;
            if (request is IMessage)
                payload = JsonConvert.SerializeObject(
                    (request as IMessage)
                    .Descriptor.Fields.InDeclarationOrder()
                    .ToDictionary(x => x.Name, x => x.Accessor.GetValue(request as IMessage))
                );
            return $"Send request of {typeof(TRequest).Name}:{payload}";
        }

        private string LogResponse<TResponse>(TResponse response, AggregateException exception)
        {
            var payload = string.Empty;
            if (exception == null)
            {
                if (response is IMessage)
                    payload = JsonConvert.SerializeObject(
                        (response as IMessage)
                        .Descriptor.Fields.InDeclarationOrder()
                        .ToDictionary(x => x.Name, x => x.Accessor.GetValue(response as IMessage))
                    );
                return $"Receive response of {typeof(TResponse).Name}:{payload}";
            }
            else
            {
                var errorMsgs = string.Join(";", exception.InnerExceptions.Select(x => x.Message));
                return $"Receive response of {typeof(TResponse).Name} throws exceptions: {errorMsgs}";
            }
        }
    }
}
