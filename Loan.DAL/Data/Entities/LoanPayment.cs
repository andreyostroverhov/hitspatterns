using System.ComponentModel.DataAnnotations;

namespace Loan.DAL.Data.Entities;

public class LoanPayment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid LoanId { get; set; } // Ссылка на кредит

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; } // Сумма платежа

    [Required]
    public DateTime PaymentDate { get; set; } // Дата платежа

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата создания

    // Навигационные свойства
    public Loan Loan { get; set; } // Связь с кредитом
}