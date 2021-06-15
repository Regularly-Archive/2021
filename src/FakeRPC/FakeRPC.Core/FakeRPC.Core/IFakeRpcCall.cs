using System;
using System.Threading.Tasks;

namespace FakeRpc.Core
{
    public interface IFakeRpcCall
    {
        Task<TResponse> CallAsync<TRequest, TResponse>(Uri uri, TRequest request);
    }
}