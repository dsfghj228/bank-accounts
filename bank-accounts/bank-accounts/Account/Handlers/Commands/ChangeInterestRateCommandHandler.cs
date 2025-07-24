using AutoMapper;
using bank_accounts.Account.Commands;
using bank_accounts.Account.Dto;
using bank_accounts.Account.Interfaces;
using MediatR;

namespace bank_accounts.Account.Handlers.Commands;

public class ChangeInterestRateCommandHandler : IRequestHandler<ChangeInterestRateCommand, ReturnAccountDto>
{
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;
    
    public ChangeInterestRateCommandHandler(IAccountService accountService, IMapper mapper)
    {
        _accountService = accountService;
        _mapper = mapper;
    }
    
    public Task<ReturnAccountDto> Handle(ChangeInterestRateCommand request, CancellationToken cancellationToken)
    {
        var account = _accountService.ChangeInterestRate(request.AccountId, request.InterestRate);
        return Task.FromResult(_mapper.Map<ReturnAccountDto>(account));
    }
}
