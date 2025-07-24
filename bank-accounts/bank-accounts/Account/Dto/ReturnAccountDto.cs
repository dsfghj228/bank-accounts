using bank_accounts.Account.Enums;

namespace bank_accounts.Account.Dto;

public class ReturnAccountDto
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; init; }
    public AccountType AccountType { get; init; }
    public string Currency { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal? InterestRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public bool IsClosed => ClosedAt.HasValue;
}