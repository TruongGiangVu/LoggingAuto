using Castle.DynamicProxy;
using EasyCaching.Core;
using System.Reflection;

namespace LoggingAuto.Interceptors;
public class PrefixCachingInterceptor : IInterceptor
{
    private readonly IEasyCachingProvider _cacheProvider;

    public PrefixCachingInterceptor(IEasyCachingProvider cacheProvider)
    {
        _cacheProvider = cacheProvider;
    }

    public void Intercept(IInvocation invocation)
    {
        CachingAttribute? cachingAttribute = GetCachingAttribute(invocation.Method);
        if (cachingAttribute is null)
        {
            invocation.Proceed();
            return;
        }

        string cacheKey = GenerateCacheKey(cachingAttribute.Prefix, cachingAttribute.CacheKey);
        TimeSpan expiration = TimeSpan.FromSeconds(cachingAttribute.ExpirationSeconds);

        if (_cacheProvider.Exists(cacheKey))
        {
            CacheValue<object>? cachedData = _cacheProvider.Get<object>(cacheKey);
            invocation.ReturnValue = cachedData;
        }
        else
        {
            invocation.Proceed();
            object result = invocation.ReturnValue;
            if (result is not null)
            {
                _cacheProvider.Set(cacheKey, result, expiration);
            }
        }
    }

    public void InvalidateCacheWithPrefix(string prefix)
    {
        _cacheProvider.RemoveByPrefix(prefix);
    }

    private CachingAttribute? GetCachingAttribute(MethodInfo methodInfo)
    {
        return methodInfo.GetCustomAttributes(typeof(CachingAttribute), true).FirstOrDefault() as CachingAttribute;
    }

    private string GenerateCacheKey(string prefix, string key)
    {
        return $"{prefix}:{key}";
    }
}
