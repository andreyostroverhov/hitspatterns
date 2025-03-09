using Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Dtos
{
    public class StoryDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public ChangeType Type { get; set; }
    }
}
