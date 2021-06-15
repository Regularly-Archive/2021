using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core
{
    public class DefaultFakeRpcCalls : IFakeRpcCalls
    {
        private readonly HttpClient _httpClient;
        public DefaultFakeRpcCalls(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public virtual async Task<TResponse> CallAsync<TRequest, TResponse>(Uri uri, TRequest request)
        {
            var payload = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(payload);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(FakeRpcMediaTypes.Default);
            var response = await _httpClient.PostAsync(uri, httpContent);
            payload = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(payload);
        }
    }
}
