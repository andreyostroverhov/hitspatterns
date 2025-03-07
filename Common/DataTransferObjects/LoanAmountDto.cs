using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects;

public class LoanAmountDto
{
    // Cумма кредита
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    // Валюта
    [Required]
    public Currency Currency { get; set; }
}
