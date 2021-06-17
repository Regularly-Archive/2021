using MessagePack;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Mvc.MessagePack
{
    internal class MessagePackOutputFormatter : OutputFormatter
    {
        private readonly MessagePackSerializerOptions _options;

        private static readonly StringSegment _mediaType = new StringSegment(FakeRpcMediaTypes.MessagePack);

        public MessagePackOutputFormatter(MessagePackSerializerOptions options = null)
        {
            _options = options ?? MessagePackSerializer.DefaultOptions;
            _options = _options.WithCompression(MessagePackCompression.Lz4Block);
            SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(_mediaType));
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            if (context.ObjectType != typeof(object))
            {
                await MessagePackSerializer.SerializeAsync(context.ObjectType, context.HttpContext.Response.Body, context.Object, _options);
            }
            else if (context.Object == null)
            {
                context.HttpContext.Response.Body.WriteByte(MessagePackCode.Nil);
            }
            else
            {
                await MessagePackSerializer.SerializeAsync(context.Object.GetType(), context.HttpContext.Response.Body, context.Object, _options);
            }

            context.ContentType = _mediaType;
            await Task.CompletedTask;
        }
    }
}
