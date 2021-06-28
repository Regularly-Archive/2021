using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using System;
using System.IO;
using GRPC.Logging;
using System.Text;
using System.Linq;
using Google.Protobuf;
using System.Collections.Generic;
using System.Reflection;
using Grpc.Core;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Logging;

namespace Grpc.Gateway
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGrpcGateway<TClient, TRequest, TResponse>(this IApplicationBuilder app, string route, Func<string, TRequest> requestBuilder, Func<TClient, TRequest, TResponse> responseBuilder)
        {
            var routeData =
            app.UseEndpoints(endpoints => endpoints.MapPost(route, async context =>
            {
                using (var streamReader = new StreamReader(context.Request.Body))
                {
                    var client = (TClient)app.ApplicationServices.GetService(typeof(TClient));

                    var payload = await streamReader.ReadToEndAsync();
                    var request = requestBuilder(payload);

                    var reply = responseBuilder(client, request);
                    var response = JsonConvert.SerializeObject(reply);

                    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(response));
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                }
            }));
        }

        public static void AddGrpcGateway(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("GrpcGateway");
            var clientTypes = FromThis().Where(x => x.BaseType != null && x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == typeof(ClientBase<>)).ToList();
            foreach (var clientType in clientTypes)
            {
                var serviceName = clientType.Name.Replace("Client", "");
                foreach (var method in clientType.GetMethods().Where(x => x.Name.EndsWith("Async") && x.GetParameters().Length == 4))
                {
                    var methodName = method.Name.Replace("Async", "");
                    var grpcRoute = $"{serviceName}/{methodName}";
                    logger.LogInformation($"Add gRPC Gateway: {grpcRoute}");
                    app.UseEndpoints(endpoints => endpoints.MapPost($"{grpcRoute}", async context =>
                    {
                        using (var streamReader = new StreamReader(context.Request.Body))
                        {
                            var client = app.ApplicationServices.GetService(clientType);

                            var payload = await streamReader.ReadToEndAsync();
                            var requestType = method.GetParameters()[0].ParameterType;
                            var messageParser = CreateMessageParser(requestType);
                            dynamic request = CallParseJson(messageParser, payload);

                            dynamic reply = CallRpcMethod(client, method, request);
                            var response = JsonConvert.SerializeObject(reply.ResponseAsync.Result);

                            context.Response.Headers.Add("X-Grpc-Service", $"{serviceName}Service");
                            context.Response.Headers.Add("X-Grpc-Method", $"{methodName}Async");

                            await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(response));
                            context.Response.StatusCode = 200;
                            context.Response.ContentType = "application/json";
                        }
                    }));
                }
            }
        }

        public static void AddGrpcClients(this IServiceCollection services, Action<GrpcClientFactoryOptions> configureClient)
        {
            var clientTypes = FromThis().Where(x => x.BaseType != null && x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == typeof(ClientBase<>)).ToList();
            foreach(var clientType in clientTypes)
            {
                CallAddGrpcClient(clientType, services, configureClient);
            }
        }

        public static IEnumerable<Type> FromThis()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var feferdAssemblies = entryAssembly.GetReferencedAssemblies().Select(x => Assembly.Load(x));
            var allAssemblies = new List<Assembly> { entryAssembly }.Concat(feferdAssemblies);
            return allAssemblies.SelectMany(x => x.DefinedTypes).ToList();
        }

        private static dynamic CreateMessageParser(Type type)
        {
            // () => new HelloRequest()
            var constructor = type.GetConstructor(Type.EmptyTypes);
            var messageNew = Expression.New(constructor);
            var messageLambda = Expression.Lambda(messageNew, null);

            // var parser = new MessageParser(() => new HelloRequest()); 
            var factoryType = typeof(Func<>).MakeGenericType(type);
            var parserType = typeof(MessageParser<>).MakeGenericType(type);
            constructor = parserType.GetConstructor(new Type[] { factoryType});
            var parserNew = Expression.New(constructor, messageLambda);
            var parserLambda = Expression.Lambda(parserNew, null);

            // return parser
            var parserFactory = parserLambda.Compile();
            return parserFactory.DynamicInvoke();
        }

        private static dynamic CallParseJson(object messageParser, string json)
        {
            // () => messageParser.ParseJson(json)
            var parserInstance = Expression.Constant(messageParser);
            var parseJsonMethod = Expression.Call(
                parserInstance,
                messageParser.GetType().GetMethod("ParseJson"),
                new List<Expression> { Expression.Constant(json)}
            );

            var lambdaExp = Expression.Lambda(parseJsonMethod, null);
            var callInvoker = lambdaExp.Compile();
            return callInvoker.DynamicInvoke();
        }

        private static dynamic CallRpcMethod(dynamic rpcClient, MethodInfo methodInfo, dynamic request)
        {
            // () => client.SayHelloAsync(request)
            var clientInstance = Expression.Constant(rpcClient, rpcClient.GetType());
            var parseJsonMethod = Expression.Call(
                clientInstance,
                methodInfo,
                new List<Expression> { 
                    Expression.Constant(request, request.GetType()),
                    Expression.Constant(null, typeof(Metadata)),
                    Expression.Constant(null, typeof(Nullable<System.DateTime>)),
                    Expression.Constant(default(global::System.Threading.CancellationToken)),
                }
            );

            var lambdaExp = Expression.Lambda(parseJsonMethod, null);
            var callInvoker = lambdaExp.Compile();
            return callInvoker.DynamicInvoke();
        }

        private static void CallAddGrpcClient(Type type, IServiceCollection services, Action<GrpcClientFactoryOptions> configureClient)
        {
            var methodCall = Expression.Call(
                null,
                typeof(GrpcClientServiceExtensions).GetMethod("AddGrpcClient", new Type[] { typeof(IServiceCollection), typeof(Action<GrpcClientFactoryOptions>) }).MakeGenericMethod(type),
                new List<Expression> {
                    Expression.Constant(services, services.GetType()),
                    Expression.Constant(configureClient, configureClient.GetType()),
                }
            );

            var lambdaExp = Expression.Lambda(methodCall, null);
            lambdaExp.Compile().DynamicInvoke();
        }
    }
}