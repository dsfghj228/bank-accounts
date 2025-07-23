using MediatR;

namespace bank_accounts.Account.Commands;

public record CloseAccountCommand : IRequest<Models.Account>
{
    public Guid AccountId { get; init; }
}