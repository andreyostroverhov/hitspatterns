using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects;

public class DeleteTariffRequest
{
    [Required]
    public Guid Id { get; set; }
}
