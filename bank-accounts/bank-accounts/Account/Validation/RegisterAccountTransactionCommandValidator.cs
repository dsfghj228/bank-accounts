using bank_accounts.Account.Commands;
using FluentValidation;

namespace bank_accounts.Account.Validation;

public class RegisterAccountTransactionCommandValidator : AbstractValidator<RegisterAccountTransactionCommand>
{
    public RegisterAccountTransactionCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .Must(id => id != Guid.Empty)
            .WithMessage("AccountId не может быть пустым GUID.");
        RuleFor(x => x.CounterpartyId)
            .NotEmpty()
            .Must(id => id != Guid.Empty)
            .WithMessage("CounterpartyId не может быть пустым GUID.");
        RuleFor(x => x.Amount)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Amount должен быть больше нуля.");
        RuleFor(x => x.Currency)
            .NotEmpty();
    }
}