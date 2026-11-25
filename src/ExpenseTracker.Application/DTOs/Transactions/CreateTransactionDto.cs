
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs.Transactions;

public class CreateTransactionDto
{
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string? Description { get; set; }
    public DateOnly Date { get; set; }
    public int CategoryId { get; set; }
}
