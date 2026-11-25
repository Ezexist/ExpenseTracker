
using ExpenseTracker.Application.DTOs.Budgets;

namespace ExpenseTracker.Application.Interfaces.Services;

public interface IBudgetService
{
    Task<IReadOnlyList<BudgetDto>> GetAllAsync(
        int? month = null,
        int? year = null,
        CancellationToken cancellationToken = default);

    Task<BudgetDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<BudgetDto> CreateAsync(
        CreateBudgetDto dto,
        CancellationToken cancellationToken = default);

    Task<BudgetDto?> UpdateAsync(
        int id,
        UpdateBudgetDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}
