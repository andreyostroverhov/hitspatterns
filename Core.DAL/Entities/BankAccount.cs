using Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAL.Entities
{
    public class BankAccount
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ClientId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public BankAccStatus Status { get; set; } = BankAccStatus.Open;
    }
}
