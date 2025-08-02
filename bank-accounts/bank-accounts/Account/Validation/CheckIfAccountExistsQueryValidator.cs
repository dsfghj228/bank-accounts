using bank_accounts.Account.Queries;
using FluentValidation;

namespace bank_accounts.Account.Validation;

public class CheckIfAccountExistsQueryValidator : AbstractValidator<CheckIfAccountsExistsQuery>
{
    public CheckIfAccountExistsQueryValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.AccountId).NotEmpty();
    }
}
