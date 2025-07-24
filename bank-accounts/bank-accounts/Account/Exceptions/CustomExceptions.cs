using System.Net;
using bank_accounts.Account.Enums;

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
                "Владелец не найден",
                $"Владелец с таким id {ownerId} не найден")
        {
        }
    }
    
    public class CurrencyDoesNotSupportedException : CustomExceptions
    {
        public CurrencyDoesNotSupportedException(Currency currency)
            : base(
                HttpStatusCode.UnprocessableEntity,
                "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                "Валюта не поддерживается",
                $"Валюта {currency} не поддерживается")
        {
        }
    }
    
    public class AccountNotFoundException : CustomExceptions
    {
        public AccountNotFoundException(Guid accountId)
            : base(
                HttpStatusCode.NotFound,
                "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                "Аккаунт не найден",
                $"Аккаунт с таким id {accountId} не найден")
        {
        }
    }
    
    public class CheckingAccountNotSupportInterestRateException: CustomExceptions
    {
        public CheckingAccountNotSupportInterestRateException()
            : base(
                HttpStatusCode.NotFound,
                "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                "Аккаунт не поддерживает процентную ставку",
                $"Аккаунт с типом Checking не поддерживает процентную ставку")
        {
        }
    }
    
    public class AccountClosedException: CustomExceptions
    {
        public AccountClosedException(Guid accountId)
            : base(
                HttpStatusCode.NotFound,
                "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                "Аккаунт закрыт",
                $"Аккаунт с таким id {accountId} закрыт")
        {
        }
    }
}