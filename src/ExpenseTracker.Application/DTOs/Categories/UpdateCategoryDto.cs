
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Categories;

public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
}
