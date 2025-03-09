
namespace Common.Enums;

public enum LoanStatus
{
    Pending,    // Кредит ожидает одобрения
    Active,     // Кредит активен (выдан и не погашен)
    Paid,       // Кредит полностью погашен
    Overdue,    // Кредит просрочен
    Cancelled   // Кредит отменен
}
