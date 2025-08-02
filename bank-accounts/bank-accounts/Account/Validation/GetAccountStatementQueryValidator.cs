using bank_accounts.Account.Queries;
using FluentValidation;

namespace bank_accounts.Account.Validation;

public class GetAccountStatementQueryValidator : AbstractValidator<GetAccountStatementQuery>
{
    public GetAccountStatementQueryValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.AccountId)
            .NotEmpty();
        RuleFor(x => x.From)
            .LessThan(x => x.To)
            .WithMessage("Дата начала периода должна быть меньше даты конца периода.")
            .When(x => x.To != null);
    }
}
