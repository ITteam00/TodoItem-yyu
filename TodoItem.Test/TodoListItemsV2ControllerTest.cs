using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MongoDB.Driver;
using Moq;
using System.Net;
using System.Text.Json;
using System.Text;
using TodoItem.core.Controllers;
using TodoItem.core.Models;
using TodoItems.Core;
using TodoItems.Core.Model;
using TodoItems.Core.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoItem.Infrastructure.Model;

namespace TodoItem.Test
{
    public class TodoListItemsV2ControllerTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private IMongoCollection<TodoItemMongoDTO> _mongoCollection;

        public TodoListItemsV2ControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

            var mongoClient = new MongoClient("mongodb://localhost:27081");
            var mongoDatabase = mongoClient.GetDatabase("TodoItem");
            _mongoCollection = mongoDatabase.GetCollection<TodoItemMongoDTO>("todos");
        }

        public async Task InitializeAsync()
        {
            await _mongoCollection.DeleteManyAsync(FilterDefinition<TodoItemMongoDTO>.Empty);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task Should_Update_Existing_Todo_Item()
        {
            // Arrange
            var existingItem = new TodoItemMongoDTO()
            {
                Id = "existing-id",
                Description = "Original Todo",
                IsDone = false,
                CreatedTime = DateTimeOffset.Now,
                ModificationDateTimes = new List<DateTimeOffset>()
            };
            await _mongoCollection.InsertOneAsync(existingItem);

            var updateDto = new TodoItemDTO
            {
                Id = "existing-id",
                Description = "Updated Todo",
                IsDone = true,
                CreatedTime = DateTimeOffset.Now,
                ModificationDateTimes = new List<DateTimeOffset> { DateTimeOffset.Now }
            };

            // Act
            var response = await _client.PutAsync(
                $"/api/v2/TodoListItemsV2/{existingItem.Id}?strategyType=someStrategy",
                new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var returnedItem = JsonSerializer.Deserialize<TodoItemDTO>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(returnedItem);
            Assert.Equal("Updated Todo", returnedItem.Description);
            Assert.True(returnedItem.IsDone);
        }

        [Fact]
        public async Task Should_Create_New_Todo_Item_When_Not_Exists()
        {
            // Arrange
            var newItemDto = new TodoItemDTO
            {
                Id = "new-id",
                Description = "New Todo",
                IsDone = false,
                CreatedTime = DateTimeOffset.Now,
                ModificationDateTimes = new List<DateTimeOffset>()
            };

            // Act
            var response = await _client.PutAsync(
                $"/api/v2/TodoListItemsV2/{newItemDto.Id}?strategyType=freest",
                new StringContent(JsonSerializer.Serialize(newItemDto), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var returnedItem = JsonSerializer.Deserialize<TodoItemDTO>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(returnedItem);
            Assert.Equal("New Todo", returnedItem.Description);
            Assert.False(returnedItem.IsDone);

            // Verify item was actually created in database
            var dbItem = await _mongoCollection.Find(x => x.Id == "new-id").FirstOrDefaultAsync();
            Assert.NotNull(dbItem);
            Assert.Equal("New Todo", dbItem.Description);
        }

        [Fact]
        public async Task Should_Return_BadRequest_When_Ids_Dont_Match()
        {
            // Arrange
            var todoItemDto = new TodoItemDTO
            {
                Id = "different-id",
                Description = "Test Todo",
                IsDone = false,
                CreatedTime = DateTimeOffset.Now,
                ModificationDateTimes = new List<DateTimeOffset>()
            };

            // Act
            var response = await _client.PutAsync(
                "/api/v2/TodoListItemsV2/url-id?strategyType=someStrategy",
                new StringContent(JsonSerializer.Serialize(todoItemDto), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("ToDo Item ID in url must be equal to request body", content);
        }

        [Fact]
        public async Task Should_Return_BadRequest_When_TooManyEntries()
        {
            // Arrange
            var existingItem = new TodoItemMongoDTO()
            {
                Id = "existing-id",
                Description = "Original Todo",
                IsDone = false,
                CreatedTime = DateTimeOffset.Now,
                ModificationDateTimes = new List<DateTimeOffset>()
            };
            await _mongoCollection.InsertOneAsync(existingItem);

            var updateDto = new TodoItemDTO
            {
                Id = "existing-id",
                Description = "Updated Todo",
                IsDone = true,
                CreatedTime = DateTimeOffset.Now,
                ModificationDateTimes = new List<DateTimeOffset>
                {
                    DateTimeOffset.Now,
                    DateTimeOffset.Now.AddMinutes(1),
                    DateTimeOffset.Now.AddMinutes(2)
                }
            };

            // Act
            var response = await _client.PutAsync(
                $"/api/v2/TodoListItemsV2/{existingItem.Id}?strategyType=someStrategy",
                new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}