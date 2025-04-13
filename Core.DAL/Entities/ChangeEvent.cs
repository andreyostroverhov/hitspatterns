using Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAL.Entities
{
    public class ChangeEvent
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid BankAccId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ChangeType Type { get; set; }

        public Guid? RelatedAccountId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}
