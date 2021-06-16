using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Mvc.MessagePack
{
    internal class MessagePackInputFormatter : InputFormatter
    {
        private readonly MessagePackSerializerOptions _options;

        private static readonly StringSegment _mediaType = new StringSegment(FakeRpcMediaTypes.MessagePack);

        public MessagePackInputFormatter(MessagePackSerializerOptions options = null)
        {
            _options = options ?? MessagePackSerializer.DefaultOptions;
            SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(_mediaType));
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var body = context.HttpContext.Request.Body;
            var result = MessagePackSerializer.Deserialize(context.ModelType, body, _options);
            return InputFormatterResult.SuccessAsync(result);
        }
    }
}
