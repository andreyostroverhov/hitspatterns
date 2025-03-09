using System.ComponentModel.DataAnnotations;

namespace Loan.DAL.Data.Entities;

public class Tariff
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } // Название тарифа

    [Required]
    [Range(0, 100)]
    public decimal InterestRate { get; set; } // Процентная ставка

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата создания
}