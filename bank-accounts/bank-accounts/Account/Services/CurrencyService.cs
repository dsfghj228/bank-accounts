using bank_accounts.Account.Enums;
using bank_accounts.Account.Interfaces;

namespace bank_accounts.Account.Services;

public class CurrencyService : ICurrencyService
{
    public bool IsCurrencySupported(Currency currency)
    {
        return Enum.IsDefined(typeof(Currency), currency);
    }
}