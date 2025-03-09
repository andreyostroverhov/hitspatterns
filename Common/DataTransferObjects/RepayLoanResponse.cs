
using Common.Enums;

namespace Common.DataTransferObjects;

public class RepayLoanResponse
{
    public Guid LoanId { get; set; } // ID кредита
    public decimal RemainingAmount { get; set; } // Оставшаяся сумма к погашению
    public string Message { get; set; } // Сообщение о результате
    public Currency Currency { get; set; } //Валюта
}
