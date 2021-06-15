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
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
            var payload = Serizlize(request);
            var httpContent = new ByteArrayContent(payload);
            var response = await _httpClient.PostAsync(uri, httpContent);
            payload = await response.Content.ReadAsByteArrayAsync();
            return Deserizlize<TResponse>(payload);
        }

        public byte[] Serizlize<T>(T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(memoryStream, obj);
                var result = new byte[memoryStream.Length];
                memoryStream.Position = 0;
                memoryStream.Read(result, 0, result.Length);
                return result;
            }
        }

        public T Deserizlize<T>(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Position = 0;
                memoryStream.Write(bytes, 0, bytes.Length);
                return ProtoBuf.Serializer.Deserialize<T>(memoryStream);
            }
        }
    }
}
