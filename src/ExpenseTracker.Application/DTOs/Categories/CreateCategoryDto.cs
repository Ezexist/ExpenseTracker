
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Categories;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
}
