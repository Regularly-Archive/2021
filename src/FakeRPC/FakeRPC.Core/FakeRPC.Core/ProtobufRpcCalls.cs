using FakeRpc.Core.Mvc;
using ProtoBuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core
{
    public class ProtobufRpcCalls : IFakeRpcCalls
    {
        private readonly HttpClient _httpClient;
        public ProtobufRpcCalls(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse> CallAsync<TRequest, TResponse>(Uri uri, TRequest request)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(FakeRpcMediaTypes.Protobuf));
            var payload = Serizlize(request);
            var httpContent = new ByteArrayContent(payload);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(FakeRpcMediaTypes.Protobuf);
            var response = await _httpClient.PostAsync(uri, httpContent);
            payload = await response.Content.ReadAsByteArrayAsync();
            return Deserizlize<TResponse>(payload);
        }

        private byte[] Serizlize<T>(T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        private T Deserizlize<T>(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                return ProtoBuf.Serializer.Deserialize<T>(memoryStream);
            }
        }

        public Task<TResponse> CallAsync<TResponse>(Uri uri) => CallAsync<object, TResponse>(uri, new Empty());

        public static Func<HttpClient, IFakeRpcCalls> Factory =
            httpClient => new ProtobufRpcCalls(httpClient);
    }
}
