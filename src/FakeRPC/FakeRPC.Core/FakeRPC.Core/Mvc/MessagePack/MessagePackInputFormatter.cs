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
            _options = options ?? MessagePackSerializerOptions.Standard; ;
            _options = _options.WithCompression(MessagePackCompression.Lz4Block);
            SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(_mediaType));
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var body = context.HttpContext.Request.Body;
            var result = await MessagePackSerializer.DeserializeAsync(context.ModelType, body, _options);
            return await InputFormatterResult.SuccessAsync(result);
        }
    }
}
