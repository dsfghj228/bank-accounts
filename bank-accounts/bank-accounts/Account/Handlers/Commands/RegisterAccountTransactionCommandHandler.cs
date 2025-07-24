using AutoMapper;
using bank_accounts.Account.Commands;
using bank_accounts.Account.Dto;
using bank_accounts.Account.Interfaces;
using bank_accounts.Account.Models;
using MediatR;

namespace bank_accounts.Account.Handlers.Commands;

public class RegisterAccountTransactionCommandHandler : IRequestHandler<RegisterAccountTransactionCommand, ReturnTransactionDto>
{
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;
    
    public RegisterAccountTransactionCommandHandler(IAccountService accountService, IMapper mapper)
    {
        _accountService = accountService;
        _mapper = mapper;
    }
    
    public Task<ReturnTransactionDto> Handle(RegisterAccountTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = _accountService.RegisterAccountTransaction(request.AccountId,
            request.CounterpartyId,
            request.Amount,
            request.Currency,
            request.Description);
        
        return Task.FromResult(_mapper.Map<ReturnTransactionDto>(transaction));
    }
}