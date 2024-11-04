using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoItem.core.Controllers;
using TodoItem.core.Models;
using TodoItems.Core.Model;
using TodoItems.Core.Services;

namespace TodoItem.Test
{
    public class TodoListItemsV2ControllerTest : IDisposable
    {
        private readonly Mock<ITodosRepository> _todosRepositoryMock;
        private readonly TodoItemsService _todoItemsService;
        private readonly Mock<ILogger<ToDoItemsController>> _loggerMock;
        private readonly TodoListItemsV2Controller _controller;

        public TodoListItemsV2ControllerTest()
        {
            _todosRepositoryMock = new Mock<ITodosRepository>();
            _todoItemsService = new TodoItemsService(_todosRepositoryMock.Object);
            _loggerMock = new Mock<ILogger<ToDoItemsController>>();
            _controller = new TodoListItemsV2Controller(
                _todoItemsService,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task PostAsync_WithValidRequest_WithoutDueDate_ShouldReturnCreatedResult()
        {
            // Arrange
            var todoItemDto = new TodoItemDTO
            {
                Id = "test-id",
                Description = "Test Todo",
                IsDone = false,
                CreatedTime = DateTimeOffset.Now,
                IsFavorite = false,
                ModificationDateTimes = new List<DateTimeOffset>(),
            };

            var createRequest = new TodoItemV2CreateRequest
            {
                CreateTodoItemDTO = todoItemDto,
                StrategyType = "freest"
            };

            // Mock repository responses
            _todosRepositoryMock
                .Setup(x => x.GetNextFiveDaysItems(It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(new List<TodoItemDTO>());

            _todosRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<TodoItemDTO>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostAsync(createRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(201, createdResult.StatusCode);

            _todosRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<TodoItemDTO>()), Times.Once);
        }

        [Fact]
        public async Task PostAsync_WithValidRequest_WithDueDate_ShouldReturnCreatedResult()
        {
            // Arrange
            var dueDate = DateTimeOffset.Now.AddDays(1);
            var todoItemDto = new TodoItemDTO
            {
                Id = "test-id",
                Description = "Test Todo",
                IsDone = false,
                CreatedTime = DateTimeOffset.Now,
                IsFavorite = false,
                ModificationDateTimes = new List<DateTimeOffset>(),
                DueDate = dueDate
            };

            var createRequest = new TodoItemV2CreateRequest
            {
                CreateTodoItemDTO = todoItemDto,
                StrategyType = "someStrategy"
            };

            // Mock repository to return less than 8 items for the due date
            _todosRepositoryMock
                .Setup(x => x.GetItemsByDueDate(dueDate))
                .ReturnsAsync(new List<TodoItemDTO>());

            _todosRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<TodoItemDTO>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostAsync(createRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(201, createdResult.StatusCode);

            _todosRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<TodoItemDTO>()), Times.Once);
        }

        [Fact]
        public async Task PostAsync_WithDueDate_WhenTooManyItems_ShouldReturnInternalServerError()
        {
            // Arrange
            var dueDate = DateTimeOffset.Now.AddDays(1);
            var todoItemDto = new TodoItemDTO
            {
                Id = "test-id",
                Description = "Test Todo",
                DueDate = dueDate,
                ModificationDateTimes = new List<DateTimeOffset>()
            };

            var createRequest = new TodoItemV2CreateRequest
            {
                CreateTodoItemDTO = todoItemDto,
                StrategyType = "someStrategy"
            };

            _todosRepositoryMock
                .Setup(x => x.GetItemsByDueDate(dueDate))
                .ReturnsAsync(Enumerable.Range(1, 9).Select(i => new TodoItemDTO() { Id = "" }).ToList());
            var str = "";
            try
            {
                var result = await _controller.PostAsync(createRequest);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                str = e.Message;
            }

            Assert.Equal("too many", str);
        }

        public void Dispose()
        {
        }
    }
}