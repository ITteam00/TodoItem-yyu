using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Model;
using TodoItems.Core.Validator;

namespace TodoItems.Test
{
    public class TodoItemValidatorTest
    {
        private readonly TodoItemValidator _todoItemValidator;

        public TodoItemValidatorTest()
        {
            _todoItemValidator = new TodoItemValidator();
        }

        [Fact]
        public void Should_return_modificationCount_when_call_ModificationCount()
        {
            List<DateTimeOffset> dateTimes = new List<DateTimeOffset>
            {
                new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(-3)),
                new DateTimeOffset(DateTimeOffset.Now.Date.AddHours(3)),
                new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(-4)),
                new DateTimeOffset(DateTimeOffset.Now.Date.AddHours(4)),
                new DateTimeOffset(DateTimeOffset.Now.Date.AddHours(5)),
                new DateTimeOffset(DateTimeOffset.Now.Date.AddHours(6))
            };

            Assert.Equal(4, _todoItemValidator.ModificationCount(dateTimes));
        }


        [Fact]
        public void CountDueDates_ShouldReturnInitializedDictionary_WhenTodoItemsIsNull()
        {
            DateTimeOffset currentDate = DateTime.Now.Date;
            var expected = Enumerable.Range(1, 5)
                .ToDictionary(i => currentDate.AddDays(i), i => 0);

            var result = _todoItemValidator.CountDueDates(null);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CountDueDates_ShouldReturnCorrectCounts()
        {
            DateTimeOffset currentDate = DateTime.Now.Date;
            var todoItems = new List<TodoItemDTO>
            {
                new TodoItemDTO
                {
                    Id = "1", Description = "Task 1", DueDate = new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(1))
                },
                new TodoItemDTO
                {
                    Id = "2", Description = "Task 2", DueDate = new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(1))
                },
                new TodoItemDTO
                {
                    Id = "3", Description = "Task 3", DueDate = new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(2))
                }
            };

            var expected = Enumerable.Range(1, 5)
                .ToDictionary(i => currentDate.AddDays(i), i => 0);
            expected[new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(1))] = 2;
            expected[new DateTimeOffset(DateTimeOffset.Now.Date.AddDays(2))] = 1;

            // Act
            var result = _todoItemValidator.CountDueDates(todoItems);

            // Assert
            Assert.Equal(expected, result);
        }


        [Fact]
        public void IsTodady_ShouldReturnTrue_WhenDateIsToday()
        {
            var today = DateTimeOffset.Now;
            var result = _todoItemValidator.IsTodady(today);
            Assert.True(result);
        }

        [Fact]
        public void IsTodady_ShouldReturnFalse_WhenDateIsNotToday()
        {
            var notToday = DateTimeOffset.Now.AddDays(-1);
            var result = _todoItemValidator.IsTodady(notToday);
            Assert.False(result);
        }

        [Fact]
        public void CanModify_ShouldReturnFalse_WhenModificationLimitExceeded()
        {
            // Arrange
            var today = DateTimeOffset.Now.Date;
            var updatedToDoItem = new TodoItemDTO
            {
                Id = "test-id",
                ModificationDateTimes = new List<DateTimeOffset>
                {
                    today.AddHours(1),
                    today.AddHours(2),
                    today.AddHours(3),
                    today.AddHours(1),
                    today.AddHours(1),
                    today.AddHours(2),
                    today.AddHours(3),
                    today.AddHours(1),
                    today.AddHours(2),
                    today.AddHours(3),
                }
            };

            // Act
            var result = _todoItemValidator.CanCreate(updatedToDoItem);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanModify_ShouldReturnTrue_WhenModificationLimitNotExceeded()
        {
            var today = DateTimeOffset.Now.Date;
            var updatedToDoItem = new TodoItemDTO
            {
                Id = "test-id",
                ModificationDateTimes = new List<DateTimeOffset>
                {
                    today.AddHours(1),
                    today.AddHours(2)
                }
            };

            // Act
            var result = _todoItemValidator.CanCreate(updatedToDoItem);

            // Assert
            Assert.True(result);
        }
    }
}