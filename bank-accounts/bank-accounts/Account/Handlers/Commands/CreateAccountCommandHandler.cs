using AutoMapper;
using bank_accounts.Account.Commands;
using bank_accounts.Account.Dto;
using bank_accounts.Account.Enums;
using bank_accounts.Account.Exceptions;
using bank_accounts.Account.Interfaces;
using MediatR;

namespace bank_accounts.Account.Handlers.Commands;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, ReturnAccountDto>
{
    private readonly IAccountService _accountService;
    private readonly IClientVerifyService _verifyService;
    private readonly ICurrencyService _currencyService;
    private readonly IMapper _mapper;
    
    public CreateAccountCommandHandler(
        IAccountService accountService, 
        IClientVerifyService verifyService,
        ICurrencyService currencyService,
        IMapper mapper)
    {
        _accountService = accountService;
        _verifyService = verifyService;
        _currencyService = currencyService;
        _mapper = mapper;
    }
    
    public Task<ReturnAccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var isInterestAccount = request.AccountType == AccountType.Credit || 
                                request.AccountType == AccountType.Deposit;
        
        if(!_verifyService.VerifyClient(request.OwnerId))
        {
            throw new CustomExceptions.OwnerNotFoundException(request.OwnerId);

        }

        if (!_currencyService.IsCurrencySupported(request.Currency))
        {
            throw new CustomExceptions.CurrencyDoesNotSupportedException(request.Currency);
        }
        
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

        return Task.FromResult(_mapper.Map<ReturnAccountDto>(account));
    }
}