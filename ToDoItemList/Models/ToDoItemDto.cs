namespace ToDoItemList.Models
{
    public class ToDoItemDto
    {
        public  required string Id { get; init; }
        public string Description { get; set; } = string.Empty;
        public bool isDone { get; set; } = false;
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.Now;
        public bool isFavorite { get; set; } = false;
        public bool isDelete { get; set; } = false;
    }
}
