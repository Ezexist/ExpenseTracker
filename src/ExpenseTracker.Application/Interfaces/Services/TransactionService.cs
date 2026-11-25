
using ExpenseTracker.Application.DTOs.Common;
using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Interfaces.Services;

public interface ITransactionService
{
    Task<PagedResult<TransactionDto>> GetPagedAsync(
        TransactionType? type,
        int? categoryId,
        DateOnly? fromDate,
        DateOnly? toDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<TransactionDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<TransactionDto> CreateAsync(
        CreateTransactionDto dto,
        CancellationToken cancellationToken = default);

    Task<TransactionDto?> UpdateAsync(
        int id,
        UpdateTransactionDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}
