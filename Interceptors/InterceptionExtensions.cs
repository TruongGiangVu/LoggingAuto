using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LoggingAuto.Interceptors;
public static class InterceptionExtensions
{
    /// <summary>
    /// The method registers the interceptor and implementation with lifetime,
    /// then sets up a factory to create instances of the interface using the interceptor and implementation.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <typeparam name="TInterceptor"></typeparam>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    public static void AddInterceptedService<TInterface, TImplementation, TInterceptor>(
        this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TInterface : class
        where TImplementation : class, TInterface
        where TInterceptor : class, IInterceptor
    {
        // Try Register the interceptor and ProxyGenerator if they haven't registered
        services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
        services.TryAddSingleton<TInterceptor>();

        // Register the implementation
        services.Add(new ServiceDescriptor(typeof(TImplementation), typeof(TImplementation), lifetime));

        // Add a factory to create intercepted instances
        services.Add(new ServiceDescriptor(typeof(TInterface), provider =>
        {
            var interceptor = provider.GetRequiredService<TInterceptor>();
            var implementation = provider.GetRequiredService<TImplementation>();

            var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
            var interceptedInstance = proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, interceptor);

            return interceptedInstance;
        }, lifetime));
    }
    public static void AddInterceptedSingleton<TInterface, TImplementation, TInterceptor>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TInterceptor : class, IInterceptor
    {
        services.AddInterceptedService<TInterface, TImplementation, TInterceptor>(ServiceLifetime.Singleton);
    }
    public static void AddInterceptedTransient<TInterface, TImplementation, TInterceptor>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TInterceptor : class, IInterceptor
    {
        services.AddInterceptedService<TInterface, TImplementation, TInterceptor>(ServiceLifetime.Transient);
    }
    public static void AddInterceptedScoped<TInterface, TImplementation, TInterceptor>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TInterceptor : class, IInterceptor
    {
        services.AddInterceptedService<TInterface, TImplementation, TInterceptor>(ServiceLifetime.Scoped);
    }
    /// <summary>
    /// Remember that when working with concrete classes, 
    /// the interceptor will be applied to all methods of the class, 
    /// including its inherited methods and methods defined in object. 
    /// Make sure to use this approach with awareness of how it might affect your application's behavior.
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <typeparam name="TInterceptor"></typeparam>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    public static void AddInterceptedServiceWithoutInterface<TImplementation, TInterceptor>(
        this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TImplementation : class
        where TInterceptor : class, IInterceptor
    {
        // Try Register the interceptor and ProxyGenerator if they haven't registered
        services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
        services.TryAddSingleton<TInterceptor>();

        // Register the implementation
        services.Add(new ServiceDescriptor(typeof(TImplementation), typeof(TImplementation), lifetime));

        // Add a factory to create intercepted instances
        services.Add(new ServiceDescriptor(typeof(TImplementation), provider =>
        {
            var interceptor = provider.GetRequiredService<TInterceptor>();
            var implementation = provider.GetRequiredService<TImplementation>();

            var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
            var interceptedInstance = proxyGenerator.CreateClassProxyWithTarget(implementation, interceptor);

            return interceptedInstance;
        }, lifetime));
    }
    public static void AddInterceptedSingletonWithoutInterface<TInterface, TImplementation, TInterceptor>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TInterceptor : class, IInterceptor
    {
        services.AddInterceptedServiceWithoutInterface<TImplementation, TInterceptor>(ServiceLifetime.Singleton);
    }
    public static void AddInterceptedTransientWithoutInterface<TInterface, TImplementation, TInterceptor>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TInterceptor : class, IInterceptor
    {
        services.AddInterceptedServiceWithoutInterface<TImplementation, TInterceptor>(ServiceLifetime.Transient);
    }
    public static void AddInterceptedScopedWithoutInterface<TInterface, TImplementation, TInterceptor>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TInterceptor : class, IInterceptor
    {
        services.AddInterceptedServiceWithoutInterface<TImplementation, TInterceptor>(ServiceLifetime.Scoped);
    }


    // public static void AddInterceptedSingleton<TInterface, TImplementation, TInterceptor>(this IServiceCollection services)
    //     where TInterface : class
    //     where TImplementation : class, TInterface
    //     where TInterceptor : class, IInterceptor
    // {
    //     // Register the interceptor and implementation
    //     services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
    //     services.AddSingleton<TImplementation>();
    //     services.TryAddTransient<TInterceptor>();
    //     services.AddSingleton(provider => 
    //     {
    //         var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
    //         var impl = provider.GetRequiredService<TImplementation>();
    //         var interceptor = provider.GetRequiredService<TInterceptor>();
    //         return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(impl, interceptor);
    //     });
    // }
}
