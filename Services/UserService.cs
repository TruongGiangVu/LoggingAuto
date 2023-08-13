using LoggingAuto.Services;

namespace LoggingAuto.Services;

public class UserService : IUserService
{
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext(typeof(UserService));
    public int Add(int a, int b)
    {
        return a + b;
    }
    [NoIOLogging]
    public int Subtract(int a, int b)
    {
        return a - b;
    }
    public (Item, int) Hentai(Item input, int a)
    {
        _log.Information("int a =" + a);
        input.Id = Add(input.Id, a);
        return (input, a);
    }

    
}