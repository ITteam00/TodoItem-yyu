using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoItem.core.Models
{
    [BsonIgnoreExtraElements]
    public class ToDoItemMongoDTO
    {
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Description { get; set; } = string.Empty;
        public bool isDone { get; set; }
        public bool isFavorite { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset CreatedTime { get; set; }
    }
}
