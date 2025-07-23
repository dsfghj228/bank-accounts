using MediatR;

namespace bank_accounts.Account.Queries;

public record GetUserAccountsQuery : IRequest<IList<Models.Account>>
{
    public Guid OwnerId { get; init; }
}