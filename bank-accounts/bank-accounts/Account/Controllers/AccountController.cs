using bank_accounts.Account.Commands;
using bank_accounts.Account.Dto;
using bank_accounts.Account.Queries;
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

    [HttpGet("/accounts")]
    public async Task<IActionResult> GetUserAccounts(Guid ownerId)
    {
        var query = new GetUserAccountsQuery
        {
            OwnerId = ownerId
        };
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPatch("/account/{accountId}/interest-rate")]
    public async Task<IActionResult> ChangeInterestRate(Guid accountId, decimal interestRate)
    {
        var command = new ChangeInterestRateCommand
        {
            AccountId = accountId,
            InterestRate = interestRate
        };
        
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
