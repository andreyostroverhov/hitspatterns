using Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Dtos
{
    public class StoryDto
    {
        public Guid Id { get; set; }
        public Guid BankAccId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ChangeType Type { get; set; }
        public Guid? RelatedAccountId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}