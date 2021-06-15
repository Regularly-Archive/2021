using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Mvc.Protobuf
{
    internal class ProtobufOutputFormatter : OutputFormatter
    {
        private static Lazy<RuntimeTypeModel> _typeModel => new Lazy<RuntimeTypeModel>(CreateTypeModel);

        public static RuntimeTypeModel TypeModel => _typeModel.Value;

        public string ContentType => FakeRpcMediaTypes.Protobuf;

        public ProtobufOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ContentType));
        }

        private static RuntimeTypeModel CreateTypeModel()
        {
            var typeModel = RuntimeTypeModel.Create();
            typeModel.UseImplicitZeroDefaults = false;
            typeModel.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
            return typeModel;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;
            TypeModel.Serialize(response.Body, context.Object);
            return Task.FromResult(response);
        }
    }
}
