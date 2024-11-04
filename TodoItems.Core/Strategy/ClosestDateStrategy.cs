using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.Strategy
{
    public class ClosestDateStrategy : IDateSelectionStrategy
    {
        public DateTimeOffset SelectDate(Dictionary<DateTimeOffset, int> dateCounts)
        {
            DateTimeOffset today = DateTimeOffset.Now;
            return dateCounts
                .Where(kvp => kvp.Value < 8)
                .OrderBy(kvp => Math.Abs((kvp.Key - today).TotalDays))
                .FirstOrDefault().Key;
        }
    }
}
