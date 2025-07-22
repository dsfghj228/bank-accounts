using System.ComponentModel;

namespace bank_accounts.Account.Enums
{
    public enum Currency
    {
        [Description("USD")] Usd = 840,
        [Description("EUR")] Eur = 978,
        [Description("GBP")] Gbp = 826,
        [Description("RUB")] Rub = 643
    }
}