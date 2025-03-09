using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects;

public class TakeLoanRequest
{
    [Required]
    public Guid AccountId { get; set; } // ID счета

    [Required]
    public Guid TariffId { get; set; } // ID тарифа

    [Required]
    public LoanAmountDto Amount { get; set; } // Сумма кредита
    [Required]
    public int NumberOfMonths { get; set; } // Кол-во месяцев

}
