using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoItem.Infrastructure.Model;
using TodoItems.Core.Model;
using TodoItems.Core.Services;

namespace TodoItem.Infrastructure
{
    public class TodoItemsRepository : ITodosRepository
    {
        public readonly IMongoCollection<TodoItemMongoDTO> ToDoItemsCollection;
        private readonly ILogger<ToDoItemsService> _Logger;

        public TodoItemsRepository(IOptions<ToDoItemDatabaseSettings> ToDoItemStoreDatabaseSettings,
            ILogger<ToDoItemsService> logger)
        {
            var mongoClient = new MongoClient(
                ToDoItemStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                ToDoItemStoreDatabaseSettings.Value.DatabaseName);

            ToDoItemsCollection = mongoDatabase.GetCollection<TodoItemMongoDTO>(
                ToDoItemStoreDatabaseSettings.Value.CollectionName);
            _Logger = logger;
        }

        public async Task<List<TodoItemDTO>> GetItemsByDueDate(DateTimeOffset? dueDate)
        {
            var filter = Builders<TodoItemMongoDTO>.Filter.Eq(item => item.DueDate, dueDate);
            var todoItems = await ToDoItemsCollection.Find(filter).ToListAsync();

            return todoItems.Select(item => new TodoItemDTO
            {
                Id = item.Id,
                Description = item.Description,
                IsDone = item.IsDone,
                IsFavorite = item.IsFavorite,
                CreatedTime = item.CreatedTime,
                ModificationDateTimes = item.ModificationDateTimes,
                DueDate = item.DueDate
            }).ToList();
        }


        public async Task UpdateAsync(string id, TodoItemDTO updatedTodoItem)
        {
            
            var list = updatedTodoItem.ModificationDateTimes;
            list.Add(DateTimeOffset.Now);
            var item = new TodoItemMongoDTO
            {
                Id = id,
                Description = updatedTodoItem.Description,
                IsDone = updatedTodoItem.IsDone,
                IsFavorite = updatedTodoItem.IsFavorite,
                DueDate = updatedTodoItem.DueDate,
                ModificationDateTimes = list
            };
            await ToDoItemsCollection.ReplaceOneAsync(x => x.Id == id, item);
        }

        public async Task CreateAsync(TodoItemDTO newTodoItem)
        {
            var item = new TodoItemMongoDTO
            {
                Id = newTodoItem.Id,
                Description = newTodoItem.Description,
                IsDone = newTodoItem.IsDone,
                IsFavorite = newTodoItem.IsFavorite,
                CreatedTime = newTodoItem.CreatedTime,
                ModificationDateTimes = newTodoItem.ModificationDateTimes,
                DueDate = newTodoItem.DueDate
            };
            _Logger.LogInformation($"Inserting new todo item to DB {newTodoItem.Id}");

            await ToDoItemsCollection.InsertOneAsync(item);
            ;
        }

        public async Task<List<TodoItemDTO>> GetNextFiveDaysItems(DateTimeOffset date)
        {
            
            DateTimeOffset endDate = date.AddDays(5);

            var filter = Builders<TodoItemMongoDTO>.Filter.And(
                Builders<TodoItemMongoDTO>.Filter.Gte(item => item.DueDate, date),
                Builders<TodoItemMongoDTO>.Filter.Lt(item => item.DueDate, endDate)
            );

            var items = await ToDoItemsCollection.Find(filter).ToListAsync();

            return items.Select(item => new TodoItemDTO
            {
                Id = item.Id,
                IsDone = item.IsDone,
                IsFavorite = item.IsFavorite,
                Description = item.Description,
                ModificationDateTimes = item.ModificationDateTimes,
                DueDate = item.DueDate

            }).ToList();
        }
    }
}