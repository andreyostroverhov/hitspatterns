using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Loan.DAL.Data.Entities;

public class LoanSchedule
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid LoanId { get; set; } // Ссылка на кредит

    [Required]
    public DateTime PaymentDate { get; set; } // Дата платежа

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; } // Сумма платежа

    [Required]
    [MaxLength(50)]
    public LoanStatus Status { get; set; } // Статус платежа

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата создания

    // Навигационные свойства
    public Loan Loan { get; set; } // Связь с кредитом
}