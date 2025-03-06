using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Loan.DAL.Data.Entities;

public class Loan
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ClientId { get; set; } // ID клиента (ссылка на сервис пользователей)

    [Required]
    public Guid AccountId { get; set; } // ID счета (ссылка на сервис ядра)

    [Required]
    public Guid TariffId { get; set; } // ID тарифа (ссылка на Tariff)

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; } // Сумма кредита

    [Required]
    [Range(0, double.MaxValue)]
    public decimal RemainingAmount { get; set; } // Оставшаяся сумма к погашению

    [Required]
    public DateTime StartDate { get; set; } // Дата начала кредита

    public DateTime? EndDate { get; set; } // Дата окончания кредита (если известна)

    [Required]
    [MaxLength(50)]
    public LoanStatus Status { get; set; } // Статус кредита 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата создания

    // Навигационные свойства
    public Tariff Tariff { get; set; } // Связь с тарифом
}