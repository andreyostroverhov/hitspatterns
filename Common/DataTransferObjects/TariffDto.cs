
namespace Common.DataTransferObjects;

public class TariffDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal InterestRate { get; set; }
    public DateTime CreatedAt { get; set; }
}
