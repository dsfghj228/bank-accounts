using bank_accounts.Account.Exceptions;
using bank_accounts.Account.Interfaces;
using bank_accounts.Account.Queries;
using MediatR;

namespace bank_accounts.Account.Handlers.Queries;

public class GetUserAccountsHandler : IRequestHandler<GetUserAccountsQuery, IList<Models.Account>>
{
    private readonly IAccountService _accountService;
    private readonly IClientVerifyService _verifyService;
    
    public GetUserAccountsHandler(IAccountService accountService, IClientVerifyService verifyService)
    {
        _accountService = accountService;
        _verifyService = verifyService;
    }
    
    public Task<IList<Models.Account>> Handle(GetUserAccountsQuery request, CancellationToken cancellationToken)
    {
        if (!_verifyService.VerifyClient(request.OwnerId))
        {
            throw new CustomExceptions.OwnerNotFoundException(request.OwnerId);
        }
        
        var accounts = _accountService.GetUserAccounts(request.OwnerId);
        
        return Task.FromResult(accounts);
    }
}