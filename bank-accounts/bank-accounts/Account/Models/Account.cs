using bank_accounts.Account.Enums;

namespace bank_accounts.Account.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public AccountType AccountType { get; set; }
        public Currency Currency { get; set; }
        public decimal Balance { get; set; }
        public decimal? InterestRate { get; set; } // процентная ставка
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public bool IsClosed => ClosedAt.HasValue;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}