using Common.Enums;

namespace Loan.DAL.Data.Entities;

class LoanAmount
{
    public required decimal Amount { get; set; }
    public required Currency Currency { get; set; }
}
