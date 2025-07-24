namespace bank_accounts.Account.Interfaces;

public interface IAccountService
{
   void AddAccountToList(Models.Account account); 
   Models.Account CloseAccount(Guid accountId);
   IList<Models.Account> GetUserAccounts(Guid ownerId);
   Models.Account ChangeInterestRate(Guid accountId, decimal interestRate);
   Models.Account GetAccountById(Guid accountId);
}
    