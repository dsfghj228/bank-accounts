using bank_accounts.Account.Commands;
using bank_accounts.Account.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace bank_accounts.Account.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public TransactionController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
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
        var transaction = await _mediator.Send(command);
        return Ok(transaction);
    }
}