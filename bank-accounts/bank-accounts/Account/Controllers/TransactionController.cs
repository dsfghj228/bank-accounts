using bank_accounts.Account.Commands;
using bank_accounts.Account.Dto;
using bank_accounts.Account.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace bank_accounts.Account.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionController(IMediator mediator) : ControllerBase
{
    [HttpPost("/transaction")]
    public async Task<IActionResult> RegisterTransaction([FromBody] CreateTransactionDto transactionDto)
    {
        var command = new RegisterAccountTransactionCommand
        {
            AccountId = transactionDto.AccountId,
            CounterpartyId = transactionDto.CounterpartyId,
            Amount = transactionDto.Amount,
            Currency = transactionDto.Currency,
            Description = transactionDto.Description
        };
        var transaction = await mediator.Send(command);
        return Ok(transaction);
    }
    
    [HttpGet("/transaction/{accountId}")]
    public async Task<IActionResult> GetAccountStatement(Guid accountId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var query = new GetAccountStatementQuery
        {
            AccountId = accountId,
            From = from,
            To = to
        };
        var transactions = await mediator.Send(query);
        return Ok(transactions);
    }
}
