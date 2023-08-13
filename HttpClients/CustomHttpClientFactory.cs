namespace LoggingAuto.HttpClients;

public class CustomHttpClientFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CustomHttpClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CustomHttpClient Create()
    {
        return _serviceProvider.GetRequiredService<CustomHttpClient>();
    }
}