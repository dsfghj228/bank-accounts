using bank_accounts.Account.Commands;
using bank_accounts.Account.Enums;
using MediatR;

namespace bank_accounts.Account.Handlers.Commands;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Models.Account>
{
    
    public Task<Models.Account> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var isInterestAccount = request.AccountType == AccountType.Credit || 
                                request.AccountType == AccountType.Deposit;
        
        var account = new Models.Account
        {
            Id = Guid.NewGuid(),
            OwnerId = request.OwnerId,
            AccountType = request.AccountType,
            Currency = request.Currency,
            Balance = request.Balance,
            InterestRate = isInterestAccount ? request.InterestRate : null,
            CreatedAt = DateTime.Now
        };

        return Task.FromResult(account);
    }
}