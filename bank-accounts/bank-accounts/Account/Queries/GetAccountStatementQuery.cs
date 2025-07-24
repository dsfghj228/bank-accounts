using bank_accounts.Account.Dto;
using MediatR;

namespace bank_accounts.Account.Queries;

public class GetAccountStatementQuery : IRequest<IList<ReturnTransactionDto>>
{
    public Guid AccountId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}