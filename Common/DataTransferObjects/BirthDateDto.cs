using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Common.DataTransferObjects;

public class BirthDateDto
{
    [Range(typeof(DateTime), "01/01/1900", "01/01/2023")]
    public DateTime? Value { get; set; }
}
