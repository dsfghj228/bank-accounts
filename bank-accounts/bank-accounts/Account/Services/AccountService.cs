using System.Data;
using bank_accounts.Account.Data;
using bank_accounts.Account.Enums;
using bank_accounts.Account.Exceptions;
using bank_accounts.Account.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            throw new CustomExceptions.InvalidTransferException("Account не counterparty могут быть одинаковыми");
        }
        
        await using var dbTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var totalBalanceBeforeTransaction = await _context.Accounts.SumAsync(a => a.Balance);
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
            var totalBalanceAfterTransaction = await _context.Accounts.SumAsync(a => a.Balance);
            if (!totalBalanceBeforeTransaction.Equals(totalBalanceAfterTransaction))
            {
                await dbTransaction.RollbackAsync();
                throw new CustomExceptions.InvalidBalanceStateException(totalBalanceBeforeTransaction, totalBalanceAfterTransaction);
            }

            await dbTransaction.CommitAsync(); 
        
            return transaction;
        }
        catch (Exception e)
        {
            await dbTransaction.RollbackAsync();
            throw new CustomExceptions.InvalidTransferException(e.Message);
        }
        
    }

    public async Task<Transaction> RegisterIncomingOrOutgoingTransactionsCommand(Guid accountId, decimal amount, Currency currency,
        TransactionType transactionType, string description = "")
    {
        await using var dbTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
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
            await dbTransaction.CommitAsync(); 
            
            return transaction;
        }
        catch (Exception e)
        {
            await dbTransaction.RollbackAsync();
            throw new CustomExceptions.InvalidTransferException(e.Message);
        }
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
    
    public async Task AccrueInterest(Guid accountId)
    {
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand("CALL accrue_interest(@account_id)", connection);
        cmd.Parameters.AddWithValue("account_id", accountId);
        await cmd.ExecuteNonQueryAsync();
    }


    public async Task AccrueInterestForAllAccounts()
    {
        var accountsId = await _context.Accounts.Select(a => a.Id).ToListAsync();

        foreach (var accountId in accountsId)
        {
            await AccrueInterest(accountId);
        }
    }
}
