using bank_accounts.Account.Enums;
using bank_accounts.Account.Exceptions;
using bank_accounts.Account.Interfaces;

namespace bank_accounts.Account.Services;

public class AccountService : IAccountService
{
    private readonly IList<Models.Account> _accounts = new List<Models.Account>();

    public void AddAccountToList(Models.Account account)
    {
        _accounts.Add(account);
    }

    public Models.Account CloseAccount(Guid accountId)
    {
        var account = _accounts.Where(a => a.Id == accountId).FirstOrDefault();
        if (account == null)
        {
            throw new CustomExceptions.AccountNotFoundException(accountId);
        }

        if (account.IsClosed)
        {
            return account;
        }
        
        account.ClosedAt = DateTime.Now;
        return account;
    }

    public IList<Models.Account> GetUserAccounts(Guid ownerId)
    {
        var accounts = _accounts.Where(a => a.OwnerId == ownerId).ToList();
        
        return accounts;
    }

    public Models.Account ChangeInterestRate(Guid accountId, decimal interestRate)
    {
        var account = _accounts.Where(a => a.Id == accountId).FirstOrDefault();
        if (account == null)
        {
            throw new CustomExceptions.AccountNotFoundException(accountId);
        }

        if (account.IsClosed)
        {
            throw new CustomExceptions.AccountClosedException(accountId);
        }

        if (account.AccountType == AccountType.Checking)
        {
            throw new CustomExceptions.CheckingAccountNotSupportInterestRateException();
        }
        account.InterestRate = interestRate;
        return account;
    }

    public Models.Account GetAccountById(Guid accountId)
    {
        var account = _accounts.Where(a => a.Id == accountId).FirstOrDefault();
        if (account == null)
        {
            throw new CustomExceptions.AccountNotFoundException(accountId);
        }
        
        return account;
    }
}
