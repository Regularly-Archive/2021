using CSRedis;
using FakeRpc.Core.Client;
using FakeRpc.Core.Discovery;
using FakeRpc.Core.Mvc;
using FakeRpc.Core.Mvc.MessagePack;
using FakeRpc.Core.Mvc.Protobuf;
using MessagePack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace FakeRpc.Core
{
    public static class ServiceCollectionExtension
    {
        public static string GetServiceName(this Type type)
        {
            if (!type.IsInterface)
                return type.Name;

            return type.Name.AsSpan().Slice(1).ToString();
        }

        public static void UseSwagger(IApplicationBuilder app)
        {

        }
    }
}
