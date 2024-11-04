using TodoItems.Core.Model;

namespace TodoItems.Core.Services
{
    public interface ITodosRepository
    {
        Task<List<TodoItemDTO>> GetItemsByDueDate(DateTimeOffset? dueDate);
        Task UpdateAsync(string s, TodoItemDTO item);
        Task CreateAsync(TodoItemDTO newTodoItem);
        Task<List<TodoItemDTO>> GetNextFiveDaysItems(DateTimeOffset date);
    }
}