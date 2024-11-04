using MongoDB.Driver;
using System;
using System.Xml.Serialization;
using TodoItems.Core;
using TodoItems.Core.Model;
using TodoItems.Core.Services;
using Moq;
using Xunit;
using Microsoft.VisualBasic;
using TodoItem.Infrastructure;
using TodoItems.Core.Validator;

namespace TodoItems.Test;

public class TodoItemServiceTest
{
    private readonly ToDoItemsService _toDoService;
    private readonly TodoItemsRepository _todoItemsRepository;
    private readonly TodoItemValidator _todoItemValidator;
    private readonly Mock<ITodosRepository> _mockRepository;

    public TodoItemServiceTest()
    {
        _mockRepository = new Mock<ITodosRepository>();
        _toDoService = new ToDoItemsService(_mockRepository.Object);
        _todoItemValidator = new TodoItemValidator();
    }

    [Fact]
    public async Task Should_throw_Exception_when_call_Update()
    {
        List<DateTimeOffset> dateTimes = new List<DateTimeOffset>
        {
            new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(-2)),
            new DateTimeOffset(DateTimeOffset.Now.Date),
            new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(-4)),
            new DateTimeOffset(DateTimeOffset.Now.Date),
            new DateTimeOffset(DateTimeOffset.Now.Date),
            new DateTimeOffset(DateTimeOffset.Now.Date)
        };
        var updatedToDoItem = new TodoItemDTO
        {
            Description = "Test Description",
            IsDone = false,
            IsFavorite = false,
            CreatedTime = DateTime.UtcNow,
            ModificationDateTimes = dateTimes,
            Id = "1",
        };
        var exception =
            await Assert.ThrowsAsync<TooManyEntriesException>(() =>
                _toDoService.UpdateAsync(updatedToDoItem.Id, updatedToDoItem));
        Assert.Equal(exception.Message, "too many");
    }


    [Fact]
    public async Task UpdateAsync_ShouldUpdateItemInCollection()
    {
        // Arrange
        var id = "test-id";
        List<DateTimeOffset> dateTimes = new List<DateTimeOffset>
        {
            new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(-4)),
            new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(-3)),
            new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(-5)),
            new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(-2)),
        };
        var updatedToDoItem = new TodoItemDTO
        {
            Description = "Test Description",
            IsDone = false,
            IsFavorite = false,
            CreatedTime = DateTime.UtcNow,
            ModificationDateTimes = dateTimes,
            Id = "1",
        };

        _mockRepository.Setup(r => r.UpdateAsync(updatedToDoItem.Id, updatedToDoItem))
            .Returns(Task.CompletedTask);

        await _toDoService.UpdateAsync(updatedToDoItem.Id, updatedToDoItem);

        Assert.Equal(4, updatedToDoItem.ModificationDateTimes.Count);
    }

    [Fact]
    public async Task CreateAsync_WithDueDate_ThrowsExceptionIfTooManyItems()
    {
        var id = "test-id";
        var createTodoItem = new TodoItemDTO { Id = id, DueDate = DateTimeOffset.Now };
        _mockRepository.Setup(repo => repo.GetItemsByDueDate(createTodoItem.DueDate))
            .ReturnsAsync(new List<TodoItemDTO>
            {
                new TodoItemDTO() { Id = id },
                new TodoItemDTO() { Id = id },
                new TodoItemDTO() { Id = id },
                new TodoItemDTO() { Id = id },
                new TodoItemDTO() { Id = id },
                new TodoItemDTO() { Id = id },
                new TodoItemDTO() { Id = id },
                new TodoItemDTO() { Id = id },
                new TodoItemDTO() { Id = id }
            });

        await Assert.ThrowsAsync<Exception>(() => _toDoService.CreateAsync(createTodoItem, "type"));
    }

    [Fact]
    public async Task CreateAsync_WithDueDate_CreatesItemIfNotTooManyItems()
    {
        var id = "test-id";
        var createTodoItem = new TodoItemDTO { Id = id, DueDate = DateTimeOffset.Now };
        _mockRepository.Setup(repo => repo.GetItemsByDueDate(createTodoItem.DueDate))
            .ReturnsAsync(new List<TodoItemDTO> { new TodoItemDTO() { Id = id }, new TodoItemDTO() { Id = id } });

        // Act
        await _toDoService.CreateAsync(createTodoItem, "type");

        // Assert
        _mockRepository.Verify(repo => repo.CreateAsync(createTodoItem), Times.Once);
    }
}