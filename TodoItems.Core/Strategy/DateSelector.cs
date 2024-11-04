using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.Strategy
{
    public class DateSelector
    {
        private readonly IDateSelectionStrategy _strategy;

        public DateSelector(string strategyType)
        {
            _strategy = GetStrategy(strategyType);
        }

        public DateTimeOffset SelectDate(Dictionary<DateTimeOffset, int> dateCounts)
        {
            return _strategy.SelectDate(dateCounts);
        }

        private IDateSelectionStrategy GetStrategy(string strategyType)
        {
            return strategyType switch
            {
                "closest" => new ClosestDateStrategy(),
                "freest" => new FreestDateStrategy(),
                _ => throw new ArgumentException("Invalid strategy type")
            };
        }
    }
}
