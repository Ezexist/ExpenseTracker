
using ExpenseTracker.Application.DTOs.Common;
using ExpenseTracker.Application.DTOs.Transactions;
using ExpenseTracker.Application.Interfaces.Repositories;
using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;

    public TransactionService(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<TransactionDto>> GetPagedAsync(
        TransactionType? type,
        int? categoryId,
        DateOnly? fromDate,
        DateOnly? toDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            throw new ArgumentException("Page must be greater than zero.");

        if (pageSize < 1 || pageSize > 100)
            throw new ArgumentException(
                "Page size must be between 1 and 100.");

        if (fromDate.HasValue && toDate.HasValue &&
            fromDate.Value > toDate.Value)
        {
            throw new ArgumentException(
                "From date cannot be later than to date.");
        }

        var (items, totalCount) =
            await _transactionRepository.GetPagedAsync(
                type,
                categoryId,
                fromDate,
                toDate,
                page,
                pageSize,
                cancellationToken);

        return new PagedResult<TransactionDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TransactionDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(
            id,
            cancellationToken);

        return transaction is null ? null : MapToDto(transaction);
    }

    public async Task<TransactionDto> CreateAsync(
        CreateTransactionDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateTransaction(dto.Amount, dto.Type, dto.Description);

        var category = await _categoryRepository.GetByIdAsync(
            dto.CategoryId,
            cancellationToken);

        if (category is null)
            throw new InvalidOperationException("Category not found.");

        if (category.Type != dto.Type)
            throw new InvalidOperationException(
                "Transaction type must match category type.");

        var transaction = new Transaction
        {
            Amount = dto.Amount,
            Type = dto.Type,
            Description = dto.Description?.Trim(),
            Date = dto.Date,
            CreatedAt = DateTimeOffset.UtcNow,
            CategoryId = dto.CategoryId
        };

        await _transactionRepository.AddAsync(
            transaction,
            cancellationToken);

        transaction.Category = category;

        return MapToDto(transaction);
    }

    public async Task<TransactionDto?> UpdateAsync(
        int id,
        UpdateTransactionDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateTransaction(dto.Amount, dto.Type, dto.Description);

        var transaction = await _transactionRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (transaction is null)
            return null;

        var category = await _categoryRepository.GetByIdAsync(
            dto.CategoryId,
            cancellationToken);

        if (category is null)
            throw new InvalidOperationException("Category not found.");

        if (category.Type != dto.Type)
            throw new InvalidOperationException(
                "Transaction type must match category type.");

        transaction.Amount = dto.Amount;
        transaction.Type = dto.Type;
        transaction.Description = dto.Description?.Trim();
        transaction.Date = dto.Date;
        transaction.CategoryId = dto.CategoryId;

        await _transactionRepository.UpdateAsync(
            transaction,
            cancellationToken);

        transaction.Category = category;

        return MapToDto(transaction);
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (transaction is null)
            return false;

        await _transactionRepository.DeleteAsync(
            transaction,
            cancellationToken);

        return true;
    }

    private static void ValidateTransaction(
        decimal amount,
        TransactionType type,
        string? description)
    {
        if (amount <= 0)
            throw new ArgumentException(
                "Transaction amount must be greater than zero.");

        if (!Enum.IsDefined(type))
            throw new ArgumentException("Invalid transaction type.");

        if (description?.Length > 250)
            throw new ArgumentException(
                "Description cannot exceed 250 characters.");
    }

    private static TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Description = transaction.Description,
            Date = transaction.Date,
            CreatedAt = transaction.CreatedAt,
            CategoryId = transaction.CategoryId,
            CategoryName = transaction.Category.Name
        };
    }
}
