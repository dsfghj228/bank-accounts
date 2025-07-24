using AutoMapper;
using bank_accounts.Account.Dto;
using bank_accounts.Account.Exceptions;
using bank_accounts.Account.Interfaces;
using bank_accounts.Account.Queries;
using MediatR;

namespace bank_accounts.Account.Handlers.Queries;

public class GetUserAccountsHandler : IRequestHandler<GetUserAccountsQuery, IList<ReturnAccountDto>>
{
    private readonly IAccountService _accountService;
    private readonly IClientVerifyService _verifyService;
    private readonly IMapper _mapper;
    
    public GetUserAccountsHandler(IAccountService accountService, IClientVerifyService verifyService, IMapper mapper)
    {
        _accountService = accountService;
        _verifyService = verifyService;
        _mapper = mapper;
    }
    
    public Task<IList<ReturnAccountDto>> Handle(GetUserAccountsQuery request, CancellationToken cancellationToken)
    {
        if (!_verifyService.VerifyClient(request.OwnerId))
        {
            throw new CustomExceptions.OwnerNotFoundException(request.OwnerId);
        }
        
        var accounts = _accountService.GetUserAccounts(request.OwnerId);
        
        return Task.FromResult(_mapper.Map<IList<ReturnAccountDto>>(accounts));
    }
}