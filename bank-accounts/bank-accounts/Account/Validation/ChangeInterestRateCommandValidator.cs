using bank_accounts.Account.Commands;
using FluentValidation;

namespace bank_accounts.Account.Validation;

public class ChangeInterestRateCommandValidator : AbstractValidator<ChangeInterestRateCommand>
{
    public ChangeInterestRateCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.AccountId).NotEmpty();
    }
}
