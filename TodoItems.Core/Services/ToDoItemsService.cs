using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TodoItems.Core.Model;
using DnsClient.Internal;
using TodoItems.Core.Strategy;
using TodoItems.Core.Validator;

namespace TodoItems.Core.Services
{
    public class ToDoItemsService
    {
        private readonly ITodosRepository _todosRepository;
        private readonly TodoItemValidator _todoItemValidator;

        public ToDoItemsService(ITodosRepository todosRepository)
        {
            _todosRepository = todosRepository;
            _todoItemValidator = new TodoItemValidator();
        }


        public async Task UpdateAsync(string id, TodoItemDTO updatedTodoItem)
        {
            var modificationCount = _todoItemValidator.ModificationCount(updatedTodoItem.ModificationDateTimes);
            if (modificationCount >= 3)
            {
                throw new TooManyEntriesException("too many");
            }

            await _todosRepository.UpdateAsync(id, updatedTodoItem);
        }


        public async Task CreateAsync(TodoItemDTO createTodoItem, string type)
        {
            if (createTodoItem.DueDate != null)
            {
                var dueDateItems = await _todosRepository.GetItemsByDueDate(createTodoItem.DueDate);
                if (dueDateItems.Count > 8)
                {
                    throw new Exception("too many");
                }

                await _todosRepository.CreateAsync(createTodoItem);
            }
            else
            {
                var dateSelector = new DateSelector(type);
                var nextFiveDaysItems = _todosRepository.GetNextFiveDaysItems(DateTimeOffset.Now);
                var countDueDates = _todoItemValidator.CountDueDates(await nextFiveDaysItems);
                createTodoItem.DueDate = dateSelector.SelectDate(countDueDates);
                await _todosRepository.CreateAsync(createTodoItem);
            }
        }
    }

    public class TooManyEntriesException : Exception
    {
        public TooManyEntriesException(string message) : base(message)
        {
        }
    }
}