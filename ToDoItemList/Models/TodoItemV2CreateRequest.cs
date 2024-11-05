using TodoItems.Core.Model;

namespace TodoItem.core.Models
{
    public class TodoItemV2CreateRequest
    {
        public string?  StrategyType { get; set; }
        public TodoItemDTO CreateTodoItemDTO { get; set; }
    }
}
