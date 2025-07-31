using bank_accounts.Account.Enums;

namespace bank_accounts.Account.Dto;

public class RegisterIncomingOutgoingTransactionDto
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public TransactionType TransactionType { get; set; }
    public string Description { get; set; } = string.Empty;
}
