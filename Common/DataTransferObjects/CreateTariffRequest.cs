
using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects;

public class CreateTariffRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal InterestRate { get; set; }
}
