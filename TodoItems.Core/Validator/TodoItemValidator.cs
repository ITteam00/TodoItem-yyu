using TodoItems.Core.Model;

namespace TodoItems.Core.Validator
{
    public class TodoItemValidator
    {
        private const int MaximumModificationNumber = 8;

        public int ModificationCount(List<DateTimeOffset> modificationDateTimes)
        {
            var count = modificationDateTimes.Count(d => d.Date == DateTimeOffset.UtcNow.Date);
            return count;
        }

        public Dictionary<DateTimeOffset, int> CountDueDates(List<TodoItemDTO> todoItems)
        {
            DateTimeOffset currentDate = DateTimeOffset.Now.Date;

            var dueDateCounts = new Dictionary<DateTimeOffset, int>();
            for (int i = 1; i <= 5; i++)
            {
                dueDateCounts[currentDate.AddDays(i)] = 0;
            }

            if (todoItems == null)
            {
                return dueDateCounts;
            }

            var countedDueDates = todoItems
                .Where(item => item.DueDate.HasValue)
                .GroupBy(item => item.DueDate.Value.Date)
                .ToDictionary(group => group.Key, group => group.Count());

            foreach (var kvp in countedDueDates)
            {
                if (dueDateCounts.ContainsKey(kvp.Key))
                {
                    dueDateCounts[kvp.Key] = kvp.Value;
                }
            }

            return dueDateCounts;
        }

        public bool CanCreate(TodoItemDTO updatedTodoItem)
        {
            var ModifiedTimes = updatedTodoItem.ModificationDateTimes;
            ModifiedTimes = ModifiedTimes.Where(t => IsTodady(t)).ToList();
            if (ModifiedTimes.Count >= MaximumModificationNumber)
            {
                return false;
            }

            return true;
        }

        public bool IsTodady(DateTimeOffset dateTime)
        {
            var toady = DateTimeOffset.Now.Date;
            return dateTime.Date == toady;
        }
    }
}