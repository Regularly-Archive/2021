using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core
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
            var payload = await Serizlize(request);
            var httpContent = new ByteArrayContent(payload);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(FakeRpcMediaTypes.MessagePack);
            var response = await _httpClient.PostAsync(uri, httpContent);
            payload = await response.Content.ReadAsByteArrayAsync();
            return await Deserizlize <TResponse>(payload);
        }

        private async Task<byte[]> Serizlize<T>(T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                var options = ContractlessStandardResolver.Options;
                options = options.WithCompression(MessagePackCompression.Lz4Block);
                await MessagePackSerializer.SerializeAsync<T>(memoryStream, obj, options);
                return memoryStream.ToArray();
            }
        }

        private async Task<T> Deserizlize<T>(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                var options = ContractlessStandardResolver.Options;
                options = options.WithCompression(MessagePackCompression.Lz4Block);
                return await MessagePackSerializer.DeserializeAsync<T>(memoryStream, options);
            }
        }

        public Task<TResponse> CallAsync<TResponse>(Uri uri) => CallAsync<object, TResponse>(uri, Nil.Default);

        public static Func<HttpClient, IFakeRpcCalls> Factory =
            httpClient => new MessagePackRpcCalls(httpClient);
    }
}
