
using ExpenseTracker.Application.DTOs.Budgets;
using ExpenseTracker.Application.Interfaces.Repositories;
using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;

    public BudgetService(
        IBudgetRepository budgetRepository,
        ICategoryRepository categoryRepository,
        ITransactionRepository transactionRepository)
    {
        _budgetRepository = budgetRepository;
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<IReadOnlyList<BudgetDto>> GetAllAsync(
        int? month = null,
        int? year = null,
        CancellationToken cancellationToken = default)
    {
        ValidatePeriod(month, year);

        var budgets = await _budgetRepository.GetAllAsync(
            month,
            year,
            cancellationToken);

        var result = new List<BudgetDto>();

        foreach (var budget in budgets)
        {
            var spent = await _transactionRepository
                .GetTotalByCategoryAndPeriodAsync(
                    budget.CategoryId,
                    budget.Month,
                    budget.Year,
                    cancellationToken);

            result.Add(MapToDto(budget, spent));
        }

        return result;
    }

    public async Task<BudgetDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var budget = await _budgetRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (budget is null)
            return null;

        var spent = await _transactionRepository
            .GetTotalByCategoryAndPeriodAsync(
                budget.CategoryId,
                budget.Month,
                budget.Year,
                cancellationToken);

        return MapToDto(budget, spent);
    }

    public async Task<BudgetDto> CreateAsync(
        CreateBudgetDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateBudget(dto.Amount, dto.Month, dto.Year);

        var category = await GetExpenseCategoryAsync(
            dto.CategoryId,
            cancellationToken);

        var exists = await _budgetRepository.ExistsAsync(
            dto.CategoryId,
            dto.Month,
            dto.Year,
            cancellationToken: cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "A budget for this category and period already exists.");

        var budget = new Budget
        {
            Amount = dto.Amount,
            Month = dto.Month,
            Year = dto.Year,
            CategoryId = dto.CategoryId
        };

        await _budgetRepository.AddAsync(
          budget,
           cancellationToken);

        budget.Category = category;

        var spent = await _transactionRepository
            .GetTotalByCategoryAndPeriodAsync(
                budget.CategoryId,
                budget.Month,
                budget.Year,
                cancellationToken);

        return MapToDto(budget, spent);
    }

    public async Task<BudgetDto?> UpdateAsync(
        int id,
        UpdateBudgetDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateBudget(dto.Amount, dto.Month, dto.Year);

        var budget = await _budgetRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (budget is null)
            return null;

        var category = await GetExpenseCategoryAsync(
            dto.CategoryId,
            cancellationToken);

        var exists = await _budgetRepository.ExistsAsync(
            dto.CategoryId,
            dto.Month,
            dto.Year,
            excludeId: id,
            cancellationToken: cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "A budget for this category and period already exists.");

        budget.Amount = dto.Amount;
        budget.Month = dto.Month;
        budget.Year = dto.Year;
        budget.CategoryId = dto.CategoryId;

        await _budgetRepository.UpdateAsync(
            budget,
            cancellationToken);

        budget.Category = category;

        var spent = await _transactionRepository
            .GetTotalByCategoryAndPeriodAsync(
                budget.CategoryId,
                budget.Month,
                budget.Year,
                cancellationToken);

        return MapToDto(budget, spent);
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var budget = await _budgetRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (budget is null)
            return false;

        await _budgetRepository.DeleteAsync(
            budget,
            cancellationToken);

        return true;
    }

    private async Task<Category> GetExpenseCategoryAsync(
        int categoryId,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(
            categoryId,
            cancellationToken);

        if (category is null)
            throw new InvalidOperationException("Category not found.");

        if (category.Type != TransactionType.Expense)
            throw new InvalidOperationException(
                "Budgets can only be assigned to expense categories.");

        return category;
    }

    private static void ValidateBudget(
        decimal amount,
        int month,
        int year)
    {
        if (amount <= 0)
            throw new ArgumentException(
                "Budget amount must be greater than zero.");

        if (month < 1 || month > 12)
            throw new ArgumentException(
                "Month must be between 1 and 12.");

        if (year < 1 || year > 9999)
            throw new ArgumentException(
                "Year must be between 1 and 9999.");
    }

    private static void ValidatePeriod(int? month, int? year)
    {
        if (month.HasValue && (month < 1 || month > 12))
            throw new ArgumentException(
                "Month must be between 1 and 12.");

        if (year.HasValue && (year < 1 || year > 9999))
            throw new ArgumentException(
                "Year must be between 1 and 9999.");
    }

    private static BudgetDto MapToDto(
        Budget budget,
        decimal spent)
    {
        return new BudgetDto
        {
            Id = budget.Id,
            Amount = budget.Amount,
            Month = budget.Month,
            Year = budget.Year,
            CategoryId = budget.CategoryId,
            CategoryName = budget.Category.Name,
            Spent = spent
        };
    }
}
