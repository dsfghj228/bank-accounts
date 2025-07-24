using bank_accounts.Account.Dto;
using bank_accounts.Account.Enums;
using bank_accounts.Account.Models;
using MediatR;

namespace bank_accounts.Account.Commands;

public class RegisterAccountTransactionCommand : IRequest<ReturnTransactionDto>
{
    public Guid AccountId { get; set; }
    public Guid CounterpartyId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public TransactionType TransactionType { get; set; }
    public string Description { get; set; } = string.Empty;
}