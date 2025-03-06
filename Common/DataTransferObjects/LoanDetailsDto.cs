using Common.Enums;

namespace Common.DataTransferObjects;

public class LoanDetailsDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid AccountId { get; set; }
    public Guid TariffId { get; set; }
    public decimal Amount { get; set; }
    public decimal RemainingAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public LoanStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    // Дополнительные данные
    public TariffDto Tariff { get; set; } // Информация о тарифе
    public List<LoanPaymentDto> Payments { get; set; } // Список платежей
    public List<LoanScheduleDto> Schedule { get; set; } // График платежей
}
