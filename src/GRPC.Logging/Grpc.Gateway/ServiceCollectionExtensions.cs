using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using System;
using System.IO;
using GRPC.Logging;
using System.Text;
using System.Linq;
using Google.Protobuf;

namespace Grpc.Gateway
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGrpcGateway<TClient,TRequest,TResponse>(this IApplicationBuilder app, string route, Func<string, TRequest> requestBuilder, Func<TClient,TRequest,TResponse> responseBuilder)
        {
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
    }

}