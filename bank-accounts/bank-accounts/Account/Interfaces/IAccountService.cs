namespace bank_accounts.Account.Interfaces;

public interface IAccountService
{
   void AddAccountToList(Models.Account account); 
   Models.Account CloseAccount(Guid accountId);
}
    