namespace Contool.Console.Infrastructure.UI;

public interface IErrorDisplayService
{
    void DisplayError(Exception exception);
}