using Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Dtos
{
    public class Transaction
    {
        public ChangeType Type { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public Guid? RelatedAccountId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}