using AutoMapper;
using bank_accounts.Account.Dto;
using bank_accounts.Account.Interfaces;
using bank_accounts.Account.Queries;
using MediatR;

namespace bank_accounts.Account.Handlers.Queries;

public class GetAccountStatementQueryHandler : IRequestHandler<GetAccountStatementQuery, IList<ReturnTransactionDto>>
{
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;
    
    public GetAccountStatementQueryHandler(IAccountService accountService, IMapper mapper)
    {
        _accountService = accountService;
        _mapper = mapper;
    }
    
    public Task<IList<ReturnTransactionDto>> Handle(GetAccountStatementQuery request, CancellationToken cancellationToken)
    {
        var transactions = _accountService.GetAccountTransactions(request.AccountId, request.From, request.To);
        
        return Task.FromResult(_mapper.Map<IList<ReturnTransactionDto>>(transactions));
    }
}