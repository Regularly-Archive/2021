using Caching.AOP.Core.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caching.AOP.Core
{
    public class CacheInterceptor<TCacheService> : DispatchProxy
    {
        private TCacheService _realObject => ServiceProvider.GetRequiredService<TCacheService>();
        private ICacheSerializer _cacheSerializer => ServiceProvider.GetRequiredService<ICacheSerializer>();
        private IDistributedCache _distributedCache => ServiceProvider.GetRequiredService<IDistributedCache>();

        public IServiceProvider ServiceProvider { get; set; }

        public CacheInterceptor()
        {

        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            byte[] cacheValue;
            var returnType = targetMethod.ReturnType;

            // void && Task
            if (returnType == typeof(void) || returnType == typeof(Task))
                return targetMethod.Invoke(_realObject, args);

            if (IsAsyncReturnValue(targetMethod))
                returnType = targetMethod.ReturnType.GetGenericArguments()[0];

            var cacheableAttribute = targetMethod.GetCustomAttribute<CacheableAttribute>();
            if (cacheableAttribute != null)
            {
                var cacheKey = GetCacheKey(cacheableAttribute, targetMethod);
                cacheValue = _distributedCache.Get(cacheKey);
                if (cacheValue != null)
                {
                    // Task<T>
                    if (IsAsyncReturnValue(targetMethod))
                        return Task.FromResult(_cacheSerializer.Deserialize(cacheValue, returnType));

                    return _cacheSerializer.Deserialize(cacheValue, returnType);
                }

                dynamic returnValue = targetMethod.Invoke(_realObject, args);
                cacheValue = _cacheSerializer.Serialize(returnValue);

                // Task<T>
                if (IsAsyncReturnValue(targetMethod))
                    cacheValue = _cacheSerializer.Serialize(returnValue.Result);

                var cacheOptions = new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheableAttribute.Expiration) };
                _distributedCache.Set(cacheKey, cacheValue, cacheOptions);
                return returnValue;
            }

            return targetMethod.Invoke(_realObject, args);
        }

        private string GetCacheKey(CacheableAttribute cacheableAttribute, MethodInfo methodInfo)
        {
            var segments = new List<string>();

            if (!string.IsNullOrEmpty(cacheableAttribute.CacheKeyPrefix))
                segments.Add(cacheableAttribute.CacheKeyPrefix);

            segments.Add(methodInfo.DeclaringType.FullName.Replace(".", "_"));

            segments.Add(methodInfo.Name);

            methodInfo.GetParameters().ToList().ForEach(x => segments.Add(x.Name));

            return string.Join("_", segments);
        }

        private bool IsAsyncReturnValue(MethodInfo targetMethod)
        {
            return targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }
    }
}
