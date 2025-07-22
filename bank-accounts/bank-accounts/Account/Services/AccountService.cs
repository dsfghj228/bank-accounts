using bank_accounts.Account.Interfaces;
using MediatR;

namespace bank_accounts.Account.Services;

public class AccountService : IAccountService
{
    private readonly IList<Models.Account> _accounts = new List<Models.Account>();

    public void AddAccountToList(Models.Account account)
    {
        _accounts.Add(account);
    }
}