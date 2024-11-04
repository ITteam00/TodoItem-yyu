using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.Strategy
{
    public interface IDateSelectionStrategy
    {
        DateTimeOffset SelectDate(Dictionary<DateTimeOffset, int> dateCounts);
    }
}
