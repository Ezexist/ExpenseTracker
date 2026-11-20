using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Category?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string name,
        TransactionType type,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> HasTransactionsAsync(
        int categoryId,
        CancellationToken cancellationToken = default);

    Task<bool> HasBudgetsAsync(
        int categoryId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Category category,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Category category,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Category category,
        CancellationToken cancellationToken = default);
}