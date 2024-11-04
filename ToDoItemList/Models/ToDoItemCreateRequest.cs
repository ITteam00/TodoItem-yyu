namespace TodoItem.core.Models
{
    public class ToDoItemCreateRequest
    {
        public string Description { get; set; } = string.Empty;
        public bool isDone { get; set; }
        public bool isFavorite { get; set; }
    }
}
