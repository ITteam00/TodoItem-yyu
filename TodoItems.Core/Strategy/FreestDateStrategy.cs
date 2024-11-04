using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.Strategy
{
    public class FreestDateStrategy : IDateSelectionStrategy
    {
        public DateTimeOffset SelectDate(Dictionary<DateTimeOffset, int> dateCounts)
        {
            return dateCounts
                .Where(kvp => kvp.Value < 8)
                .OrderBy(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key)
                .FirstOrDefault().Key;
        }
    }
}
