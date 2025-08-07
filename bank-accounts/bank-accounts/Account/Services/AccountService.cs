using bank_accounts.Account.Data;
using bank_accounts.Account.Enums;
using bank_accounts.Account.Exceptions;
using bank_accounts.Account.Interfaces;
using Microsoft.EntityFrameworkCore;
using Transaction = bank_accounts.Account.Models.Transaction;

namespace bank_accounts.Account.Services;

public class AccountService : IAccountService
{
    private readonly List<Models.Account> _accounts = [];
    private readonly BankAccountsDbContext _context;
    
    public AccountService(BankAccountsDbContext context)
    {
        _context = context;
    }

    public async Task AddAccountToList(Models.Account account)
    {
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
    }

    public async Task<Models.Account> CloseAccount(Guid accountId)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account == null)
        {
            throw new CustomExceptions.AccountNotFoundException(accountId);
        }

        if (account.IsClosed)
        {
            return account;
        }
        
        account.ClosedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return account;
    }

    public async Task<IList<Models.Account>> GetUserAccounts(Guid ownerId)
    {
        var accounts = await _context.Accounts.Where(a => a.OwnerId == ownerId).ToListAsync();
        
        return accounts;
    }

    public async Task<Models.Account> ChangeInterestRate(Guid accountId, decimal interestRate)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
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
        await _context.SaveChangesAsync();
        
        return account;
    }

    public async Task<Models.Account> GetAccountById(Guid accountId)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account == null)
        {
            throw new CustomExceptions.AccountNotFoundException(accountId);
        }
        
        return account;
    }
    
    public async Task<Transaction> RegisterAccountTransaction(Guid accountId, Guid counterpartyId, decimal amount, Currency currency,
        string description = "")
    {
        if(accountId == counterpartyId)
        {
            throw new CustomExceptions.InvalidTransferException();
        }
        
        await using var dbTransaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                throw new CustomExceptions.AccountNotFoundException(accountId);
            }
            if (account.IsClosed)
            {
                throw new CustomExceptions.AccountClosedException(accountId);
            }
            
            var counterpartyAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == counterpartyId);
            if (counterpartyAccount == null)
            {
                throw new CustomExceptions.AccountNotFoundException(counterpartyId);
            }
            if (counterpartyAccount.IsClosed)
            {
                throw new CustomExceptions.AccountClosedException(counterpartyId);
            }

            if (account.Balance < amount)
            {
                throw new CustomExceptions.InsufficientBalanceException(accountId);
            }

            if (account.Currency != counterpartyAccount.Currency)
            {
                throw new CustomExceptions.CurriesDontMatchException();
            }
        
            account.Balance -= amount;
            counterpartyAccount.Balance += amount;
        
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                CounterpartyId = counterpartyId,
                Amount = amount,
                Currency = currency,
                TransactionType = TransactionType.Debit,
                Description = description,
                CommitedAt = DateTime.UtcNow
            };
        
            var counterpartyTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = counterpartyId,
                CounterpartyId = accountId,
                Amount = amount,
                Currency = currency,
                TransactionType = TransactionType.Credit,
                Description = description,
                CommitedAt = DateTime.UtcNow
            };
        
            await _context.Transactions.AddAsync(transaction);
            await _context.Transactions.AddAsync(counterpartyTransaction);
            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync(); 
        
            return transaction;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
        
    }

    public async Task<Transaction> RegisterIncomingOrOutgoingTransactionsCommand(Guid accountId, decimal amount, Currency currency,
        TransactionType transactionType, string description = "")
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account == null)
        {
            throw new CustomExceptions.AccountNotFoundException(accountId);
        }

        if (account.IsClosed)
        {
            throw new CustomExceptions.AccountClosedException(accountId);
        }

        switch (transactionType)
        {
            case TransactionType.Debit when account.Balance < amount:
                throw new CustomExceptions.InsufficientBalanceException(accountId);
            case TransactionType.Debit:
                account.Balance -= amount;
                break;
            case TransactionType.Credit:
                account.Balance += amount;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null);
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Amount = amount,
            Currency = currency,
            TransactionType = transactionType,
            Description = description,
            CommitedAt = DateTime.UtcNow
        };

        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }


    public async Task<List<Transaction>> GetAccountTransactions(Guid accountId, DateTime? startDate, DateTime? endDate)
    {
        bool accountExists = await _context.Accounts.AnyAsync(a => a.Id == accountId);
        if (!accountExists)
        {
            throw new CustomExceptions.AccountNotFoundException(accountId);
        }

        var query = _context.Transactions
            .Where(t => t.AccountId == accountId);

        if (startDate.HasValue)
        {
            var utcStartDate = startDate.Value.ToUniversalTime();
            query = query.Where(t => t.CommitedAt >= utcStartDate);
        }

        if (endDate.HasValue)
        {
            var utcEndDate = endDate.Value.ToUniversalTime();
            query = query.Where(t => t.CommitedAt <= utcEndDate);
        }

        return await query
                    .OrderByDescending(t => t.CommitedAt)
                    .ToListAsync();
    }
}
