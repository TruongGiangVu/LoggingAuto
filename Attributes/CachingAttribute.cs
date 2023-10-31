namespace LoggingAuto.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class CachingAttribute : Attribute
{
    public string Prefix { get; }
    public string CacheKey { get; }
    public int ExpirationSeconds { get; }

    public CachingAttribute(string prefix, string cacheKey, int expirationSeconds)
    {
        Prefix = prefix;
        CacheKey = cacheKey;
        ExpirationSeconds = expirationSeconds;
    }
}
