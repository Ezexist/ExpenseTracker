
using ExpenseTracker.Application.Interfaces.Repositories;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ExpenseTrackerDbContext _context;

    public CategoryRepository(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        string name,
        TransactionType type,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AnyAsync(c =>
                c.Name == name &&
                c.Type == type &&
                (!excludeId.HasValue || c.Id != excludeId.Value),
                cancellationToken);
    }

    public async Task<bool> HasTransactionsAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AnyAsync(t => t.CategoryId == categoryId, cancellationToken);
    }

    public async Task<bool> HasBudgetsAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Budgets
            .AnyAsync(b => b.CategoryId == categoryId, cancellationToken);
    }

    public async Task AddAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
