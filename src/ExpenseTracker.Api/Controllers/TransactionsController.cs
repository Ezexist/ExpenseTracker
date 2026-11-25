
using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] TransactionType? type,
        [FromQuery] int? categoryId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _transactionService.GetPagedAsync(
            type,
            categoryId,
            fromDate,
            toDate,
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var transaction = await _transactionService.GetByIdAsync(
            id,
            cancellationToken);

        if (transaction is null)
            return NotFound();

        return Ok(transaction);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTransactionDto dto,
        CancellationToken cancellationToken)
    {
        var transaction = await _transactionService.CreateAsync(
            dto,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = transaction.Id },
            transaction);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateTransactionDto dto,
        CancellationToken cancellationToken)
    {
        var transaction = await _transactionService.UpdateAsync(
            id,
            dto,
            cancellationToken);

        if (transaction is null)
            return NotFound();

        return Ok(transaction);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _transactionService.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
