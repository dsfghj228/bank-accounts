using bank_accounts.Account.Commands;
using bank_accounts.Account.Dto;
using bank_accounts.Account.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace bank_accounts.Account.Controllers;
/// <summary>
/// Контроллер для управления транзакциями
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TransactionController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Выполнить транзакцию
    /// </summary>
    /// <param name="transactionDto">Данные для транзакции</param>
    /// <response code="200">Успешное выполнение транзакции</response>
    /// <response code="400">
    /// Возможные ошибки:
    /// - Некорректные данные запроса
    /// - Неверная операция перевода
    /// - Валюты не совпадают
    /// </response>
    /// <response code="404">Счет не найден</response>
    /// <response code="409">
    /// Возможные ошибки:
    /// - Аккаунт закрыт
    /// - Недостаточно средств
    /// </response>
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
}
