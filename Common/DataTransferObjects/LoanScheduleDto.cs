using Common.Enums;

namespace Common.DataTransferObjects;

public class LoanScheduleDto
{
    public Guid Id { get; set; }
    public Guid LoanId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public LoanStatus Status { get; set; } 
    public DateTime CreatedAt { get; set; }
}