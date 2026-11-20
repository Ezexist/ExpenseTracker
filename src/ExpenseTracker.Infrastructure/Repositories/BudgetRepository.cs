
using ExpenseTracker.Application.Interfaces.Repositories;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly ExpenseTrackerDbContext _context;

    public BudgetRepository(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Budget>> GetAllAsync(
        int? month = null,
        int? year = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Budgets
            .AsNoTracking()
            .Include(b => b.Category)
            .AsQueryable();

        if (month.HasValue)
            query = query.Where(b => b.Month == month.Value);

        if (year.HasValue)
            query = query.Where(b => b.Year == year.Value);

        return await query
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
            .ToListAsync(cancellationToken);
    }

    public async Task<Budget?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Budgets
            .AsNoTracking()
            .Include(b => b.Category)
            .FirstOrDefaultAsync(
                b => b.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        int categoryId,
        int month,
        int year,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await _context.Budgets
            .AnyAsync(b =>
                b.CategoryId == categoryId &&
                b.Month == month &&
                b.Year == year &&
                (!excludeId.HasValue || b.Id != excludeId.Value),
                cancellationToken);
    }

    public async Task AddAsync(
        Budget budget,
        CancellationToken cancellationToken = default)
    {
        await _context.Budgets.AddAsync(budget, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Budget budget,
        CancellationToken cancellationToken = default)
    {
        _context.Entry(budget).State = EntityState.Modified;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Budget budget,
        CancellationToken cancellationToken = default)
    {
        _context.Budgets.Remove(budget);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
