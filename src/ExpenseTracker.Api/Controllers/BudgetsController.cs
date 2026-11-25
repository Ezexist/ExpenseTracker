
using ExpenseTracker.Application.DTOs.Budgets;
using ExpenseTracker.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? month,
        [FromQuery] int? year,
        CancellationToken cancellationToken)
    {
        var budgets = await _budgetService.GetAllAsync(
            month,
            year,
            cancellationToken);

        return Ok(budgets);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var budget = await _budgetService.GetByIdAsync(
            id,
            cancellationToken);

        if (budget is null)
            return NotFound();

        return Ok(budget);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBudgetDto dto,
        CancellationToken cancellationToken)
    {
        var budget = await _budgetService.CreateAsync(
            dto,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = budget.Id },
            budget);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateBudgetDto dto,
        CancellationToken cancellationToken)
    {
        var budget = await _budgetService.UpdateAsync(
            id,
            dto,
            cancellationToken);

        if (budget is null)
            return NotFound();

        return Ok(budget);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _budgetService.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
