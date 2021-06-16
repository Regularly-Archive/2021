using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Mvc
{
    public class MessagePackRpcCalls : IFakeRpcCalls
    {
        private readonly HttpClient _httpClient;
        public MessagePackRpcCalls(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse> CallAsync<TRequest, TResponse>(Uri uri, TRequest request)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(FakeRpcMediaTypes.MessagePack));
            var payload = Serizlize(request);
            var httpContent = new ByteArrayContent(payload);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(FakeRpcMediaTypes.MessagePack);
            var response = await _httpClient.PostAsync(uri, httpContent);
            payload = await response.Content.ReadAsByteArrayAsync();
            return Deserizlize<TResponse>(payload);
        }

        public byte[] Serizlize<T>(T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                MessagePackSerializer.Serialize<T>(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        public T Deserizlize<T>(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                return MessagePackSerializer.Deserialize<T>(memoryStream);
            }
        }
    }
}
