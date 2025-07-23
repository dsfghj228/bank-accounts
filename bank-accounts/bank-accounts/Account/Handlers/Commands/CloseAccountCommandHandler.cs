using bank_accounts.Account.Commands;
using bank_accounts.Account.Interfaces;
using MediatR;

namespace bank_accounts.Account.Handlers.Commands;

public class CloseAccountCommandHandler : IRequestHandler<CloseAccountCommand, Models.Account>
{
    private readonly IAccountService _accountService;
    
    public CloseAccountCommandHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    public Task<Models.Account> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
    {
        var account =  _accountService.CloseAccount(request.AccountId);
        return Task.FromResult(account);
    }
}