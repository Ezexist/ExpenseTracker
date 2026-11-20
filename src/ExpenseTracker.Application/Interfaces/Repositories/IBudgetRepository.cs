using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Interfaces.Repositories;

public interface IBudgetRepository
{
    Task<IReadOnlyList<Budget>> GetAllAsync(
        int? month = null,
        int? year = null,
        CancellationToken cancellationToken = default);

    Task<Budget?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        int categoryId,
        int month,
        int year,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Budget budget,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Budget budget,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Budget budget,
        CancellationToken cancellationToken = default);
}