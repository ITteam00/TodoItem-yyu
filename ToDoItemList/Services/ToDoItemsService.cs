using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToDoItemList.Models;

namespace ToDoItemList.Services
{
    public class ToDoItemsService : IToDoItemsService
    {
        private readonly IMongoCollection<ToDoItemMongoDTO> _ToDoItemsCollection;
        private readonly ILogger<ToDoItemsService> _Logger;
        public ToDoItemsService(IOptions<ToDoItemDatabaseSettings> ToDoItemStoreDatabaseSettings, ILogger<ToDoItemsService> logger)
        {
            var mongoClient = new MongoClient(
                ToDoItemStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                ToDoItemStoreDatabaseSettings.Value.DatabaseName);

            _ToDoItemsCollection = mongoDatabase.GetCollection<ToDoItemMongoDTO>(
                ToDoItemStoreDatabaseSettings.Value.CollectionName);
            _Logger = logger;
        }
        public async Task CreateAsync(ToDoItemDto newToDoItem)
        {
            var item = new ToDoItemMongoDTO
            {
                Id = newToDoItem.Id,
                Description = newToDoItem.Description,
                isDone = newToDoItem.isDone,
                isFavorite = newToDoItem.isFavorite,
                CreatedTime = newToDoItem.CreatedTime,
            };
            _Logger.LogInformation($"Inserting new todo item to DB {newToDoItem.Id}");

            await _ToDoItemsCollection.InsertOneAsync(item); ;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var res = await _ToDoItemsCollection.DeleteOneAsync(x => x.Id == id);
            return res.DeletedCount > 0;
        }

        public async Task<List<ToDoItemDto>> GetAsync()
        {
            var toDoItemDtos = new List<ToDoItemDto>();
            var toDoItems = await _ToDoItemsCollection.Find(_ => true).ToListAsync();
            if (toDoItems is null)
            {
                return toDoItemDtos;
            }
            for (var i = 0; i < toDoItems.Count; i++)
            {
                toDoItemDtos.Add(new ToDoItemDto
                {
                    Id = toDoItems[i].Id,
                    Description = toDoItems[i].Description,
                    isDone = toDoItems[i].isDone,
                    CreatedTime = toDoItems[i].CreatedTime,
                    isFavorite = toDoItems[i].isFavorite
                });
            }
            return toDoItemDtos;
        }

        public async Task<ToDoItemDto?> GetAsync(string id)
        {
            var toDoItem = await _ToDoItemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (toDoItem is null) { return null; }
            var toDoItemDto = new ToDoItemDto
            {
                Id = toDoItem.Id,
                Description = toDoItem.Description,
                isDone = toDoItem.isDone,
                isFavorite = toDoItem.isFavorite,
                CreatedTime = toDoItem.CreatedTime,
            };
            return toDoItemDto;
        }

        public async Task UpdateAsync(string id, ToDoItemDto updatedToDoItem)
        {
            var item = new ToDoItemMongoDTO
            {
                Id = id,
                Description = updatedToDoItem.Description,
                isDone = updatedToDoItem.isDone,
                isFavorite = updatedToDoItem.isFavorite,
                CreatedTime = updatedToDoItem.CreatedTime,
            };
            await _ToDoItemsCollection.ReplaceOneAsync(x => x.Id == id, item);
        }
    }
}
