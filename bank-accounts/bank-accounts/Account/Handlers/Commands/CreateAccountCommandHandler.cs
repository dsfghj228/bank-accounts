using bank_accounts.Account.Commands;
using bank_accounts.Account.Enums;
using bank_accounts.Account.Interfaces;
using MediatR;

namespace bank_accounts.Account.Handlers.Commands;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Models.Account>
{
    private readonly IAccountService _accountService;
    
    public CreateAccountCommandHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
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
        
        _accountService.AddAccountToList(account);

        return Task.FromResult(account);
    }
}