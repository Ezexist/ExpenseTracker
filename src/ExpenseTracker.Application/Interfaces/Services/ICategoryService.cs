
using ExpenseTracker.Application.DTOs.Categories;

namespace ExpenseTracker.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<CategoryDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<CategoryDto> CreateAsync(
        CreateCategoryDto dto,
        CancellationToken cancellationToken = default);

    Task<CategoryDto?> UpdateAsync(
        int id,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}
