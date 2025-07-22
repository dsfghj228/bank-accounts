using bank_accounts.Account.Commands;
using FluentValidation;

namespace bank_accounts.Account.Validation;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.AccountType).NotEmpty();
    }
}