using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetPagedAsync(
        TransactionType? type,
        int? categoryId,
        DateOnly? fromDate,
        DateOnly? toDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalByCategoryAndPeriodAsync(
        int categoryId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    Task<(decimal Income, decimal Expense)> GetMonthlyTotalsAsync(
        int month,
        int year,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);
}