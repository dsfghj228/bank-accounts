using System.Net.Http.Json;
using bank_accounts.Account.Data;
using bank_accounts.Account.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace bank_accounts.IntegrationTests;

public class AccountIntegrationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    
    private readonly CustomWebApplicationFactory _factory = factory;
    
    [Fact]
    public async Task FiftyParallelTransfers_ShouldKeepTotalBalance_AndAllowConflicts()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BankAccountsDbContext>();

        var accounts = await dbContext.Accounts.Where(a => a.ClosedAt == null && a.Balance > 50).Take(2).ToListAsync();
        var fromAccount = accounts[0];
        var toAccount = accounts[1];

        decimal initialTotalBalance = await dbContext.Accounts.SumAsync(a => a.Balance);

        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 50; i++)
        {
            var dto = new CreateTransactionDto
            {
                AccountId = fromAccount.Id,
                CounterpartyId = toAccount.Id,
                Amount = 1,
                Currency = fromAccount.Currency,
                Description = $"Transfer {i}"
            };
            tasks.Add(_client.PostAsJsonAsync("/transaction", dto));
        }

        var responses = await Task.WhenAll(tasks);

        // Проверяем, что хотя бы один запрос успешен
        Assert.Contains(responses, r => r.IsSuccessStatusCode);

        // Можно вывести ошибки для диагностики, если нужно
        foreach (var response in responses)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    // 409 - разрешаем
                    continue;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!content.Contains("transient failure", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.False(true, $"Unexpected 400 failure without transient failure message: {content}");
                    }
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Assert.False(true, $"Unexpected failure: {(int)response.StatusCode} - {content}");
                }
            }
        }

        decimal finalTotalBalance = await dbContext.Accounts.SumAsync(a => a.Balance);
        Assert.Equal(initialTotalBalance, finalTotalBalance);
    }

    
    [Fact]
    public async Task UpdateAccount_ShouldThrowConcurrencyException()
    {
        using var scope1 = _factory.Services.CreateScope();
        using var scope2 = _factory.Services.CreateScope();

        var db1 = scope1.ServiceProvider.GetRequiredService<BankAccountsDbContext>();
        var db2 = scope2.ServiceProvider.GetRequiredService<BankAccountsDbContext>();

        var accountId = await db1.Accounts.Select(a => a.Id).FirstAsync();

        var acc1 = await db1.Accounts.FirstAsync(a => a.Id == accountId);
        var acc2 = await db2.Accounts.FirstAsync(a => a.Id == accountId);

        acc1.Balance += 10;
        acc2.Balance += 20;

        await db1.SaveChangesAsync();

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
        {
            await db2.SaveChangesAsync();
        });
    }
}