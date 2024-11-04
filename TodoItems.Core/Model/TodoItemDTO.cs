using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.Model
{
    public class TodoItemDTO
    {
        public required string Id { get; init; }
        public string Description { get; set; } = string.Empty;
        public bool IsDone { get; set; } = false;
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.Now;
        public bool IsFavorite { get; set; } = false;
        public List<DateTimeOffset> ModificationDateTimes { get; set; }
        public DateTimeOffset? DueDate { get; set; }
    }
}