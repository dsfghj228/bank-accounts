using AutoMapper;
using bank_accounts.Account.Commands;
using bank_accounts.Account.Dto;
using bank_accounts.Account.Interfaces;
using MediatR;

namespace bank_accounts.Account.Handlers.Commands;

public class CloseAccountCommandHandler : IRequestHandler<CloseAccountCommand, ReturnAccountDto>
{
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;
    
    public CloseAccountCommandHandler(IAccountService accountService, IMapper mapper)
    {
        _accountService = accountService;
        _mapper = mapper;
    }
    
    public Task<ReturnAccountDto> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
    {
        var account =  _accountService.CloseAccount(request.AccountId);
        return Task.FromResult(_mapper.Map<ReturnAccountDto>(account));
    }
}