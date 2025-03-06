using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects;

public class TakeLoanRequest
{
    [Required]
    public Guid ClientId { get; set; } // ID клиента

    [Required]
    public Guid AccountId { get; set; } // ID счета

    [Required]
    public Guid TariffId { get; set; } // ID тарифа

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; } // Сумма кредита
}
