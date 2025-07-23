using System.Net;

namespace bank_accounts.Account.Exceptions;

public abstract class CustomExceptions : Exception
{
    public HttpStatusCode StatusCode;
    public string Type;
    public string Title;

    protected CustomExceptions(
        HttpStatusCode statusCode,
        string type,
        string title,
        string message) : base(message)
    {
        StatusCode = statusCode;
        Type = type;
        Title = title;
    }

    public class OwnerNotFoundException : CustomExceptions
    {
        public OwnerNotFoundException(Guid ownerId)
            : base(
                HttpStatusCode.NotFound,
                "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                "Игрок не найден",
                $"Игрок с таким id {ownerId} не найден")
        {
        }
    }
}