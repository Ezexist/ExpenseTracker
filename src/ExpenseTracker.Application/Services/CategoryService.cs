
using ExpenseTracker.Application.DTOs.Categories;
using ExpenseTracker.Application.Interfaces.Repositories;
using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(
            cancellationToken);

        return categories.Select(MapToDto).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(
            id,
            cancellationToken);

        return category is null ? null : MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(
        CreateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty.");

        var exists = await _categoryRepository.ExistsAsync(
            name,
            dto.Type,
            cancellationToken: cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "A category with the same name and type already exists.");

        var category = new Category
        {
            Name = name,
            Type = dto.Type
        };

        await _categoryRepository.AddAsync(
            category,
            cancellationToken);

        return MapToDto(category);
    }

    public async Task<CategoryDto?> UpdateAsync(
        int id,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (category is null)
            return null;

        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty.");

        var exists = await _categoryRepository.ExistsAsync(
            name,
            dto.Type,
            excludeId: id,
            cancellationToken: cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "A category with the same name and type already exists.");

        category.Name = name;
        category.Type = dto.Type;

        await _categoryRepository.UpdateAsync(
            category,
            cancellationToken);

        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (category is null)
            return false;

        var hasTransactions = await _categoryRepository.HasTransactionsAsync(
            id,
            cancellationToken);

        if (hasTransactions)
            throw new InvalidOperationException(
                "Cannot delete a category that has transactions.");

        var hasBudgets = await _categoryRepository.HasBudgetsAsync(
            id,
            cancellationToken);

        if (hasBudgets)
            throw new InvalidOperationException(
                "Cannot delete a category that has budgets.");

        await _categoryRepository.DeleteAsync(
            category,
            cancellationToken);

        return true;
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type
        };
    }
}
