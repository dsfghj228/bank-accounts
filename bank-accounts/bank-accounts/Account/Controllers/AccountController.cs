using bank_accounts.Account.Commands;
using bank_accounts.Account.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace bank_accounts.Account.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/account")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto accountDto)
    {
        var account = new CreateAccountCommand
        {
            OwnerId = accountDto.OwnerId,
            AccountType = accountDto.AccountType,
            Currency = accountDto.Currency,
            Balance = accountDto.Balance,
            InterestRate = accountDto.InterestRate
            
        };
        
        var result = await _mediator.Send(account);
            
        return Ok(result);
        
    }

    [HttpDelete("/account/{accountId}")]
    public async Task<IActionResult> CloseAccount(Guid accountId)
    {
        var command = new CloseAccountCommand
        {
            AccountId = accountId
        };
        
        var result = await _mediator.Send(command);
        
        return Ok(result);
        
    }
}