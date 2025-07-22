using AutoMapper;
using bank_accounts.Account.Dto;

namespace bank_accounts.Account;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Models.Account, CreateAccountDto>();
    }
}