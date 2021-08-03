using FakeRpc.Core.Mvc.Protobuf;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Mvc.Protobuf
{
    internal class ProtobufInputFormatter : InputFormatter
    {
        private static Lazy<RuntimeTypeModel> _typeModel => new Lazy<RuntimeTypeModel>(CreateTypeModel);

        public static RuntimeTypeModel TypeModel => _typeModel.Value;

        private static readonly StringSegment _mediaType = new StringSegment(FakeRpcMediaTypes.Protobuf);

        public ProtobufInputFormatter()
        {
            SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(_mediaType));
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var type = context.ModelType;
            var request = context.HttpContext.Request;
            MediaTypeHeaderValue.TryParse(request.ContentType, out MediaTypeHeaderValue requestContentType);
            object result = TypeModel.Deserialize(context.HttpContext.Request.Body, null, type);
            return InputFormatterResult.SuccessAsync(result);
        }

        public override bool CanRead(InputFormatterContext context) => true;

        private static RuntimeTypeModel CreateTypeModel()
        {
            var typeModel = RuntimeTypeModel.Create();
            typeModel.UseImplicitZeroDefaults = false;
            typeModel.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
            return typeModel;
        }
    }
}
