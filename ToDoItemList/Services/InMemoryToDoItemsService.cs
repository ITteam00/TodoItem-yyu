using TodoItem.core.Models;

namespace TodoItem.core.Services
{
    public class InMemoryToDoItemsService
    {
        private static readonly List<ToDoItemDto> _toDoItems = new();

        public void Create(ToDoItemDto newToDoItem)
        {
            _toDoItems.Add(newToDoItem);
        }

        public bool Delete(string id)
        {
            var itemToRemove = _toDoItems.Find(x => x.Id == id);
            if (itemToRemove != null)
            {
                _toDoItems.Remove(itemToRemove);
                return true;
            }
            else
            {
                return false;
            };
        }

        public List<ToDoItemDto> Get()
        {
            return _toDoItems;
        }

        public ToDoItemDto? Get(string id)
        {
            var s = _toDoItems.FirstOrDefault(x => x.Id == id);
            return s;
        }

        public void Update(string id, ToDoItemDto updatedToDoItem)
        {
            var index = _toDoItems.FindIndex(x => x.Id == id);
            if (index >= 0)
            {
                updatedToDoItem.CreatedTime = _toDoItems[index].CreatedTime;
                _toDoItems[index] = updatedToDoItem;
            };
        }
    }
}
