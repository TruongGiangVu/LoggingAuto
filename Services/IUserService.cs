namespace LoggingAuto.Services;

public interface IUserService
{
    int Add(int a, int b);
    int Subtract(int a, int b);
    (Item, int) Hentai(Item input, int a);
}