using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects;

public class RepayLoanRequest
{
    [Required]
    public Guid LoanId { get; set; } // ID кредита

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; } // Сумма погашения
}
