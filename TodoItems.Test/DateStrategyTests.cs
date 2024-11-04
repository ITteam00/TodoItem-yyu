using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Strategy;

namespace TodoItems.Test
{
    public class DateStrategyTests
    {
        [Fact]
        public void SelectDate_ShouldReturnClosestDate_WhenMultipleDatesAreValid()
        {
            var strategy = new ClosestDateStrategy();
            var today = new DateTimeOffset(DateTimeOffset.Now.DateTime);
            var dateCounts = new Dictionary<DateTimeOffset, int>
            {
                { today.AddDays(1), 3 },
                { today.AddDays(2), 7 },
                { today.AddDays(3), 10 },
                { today.AddDays(4), 5 },
            };

            var result = strategy.SelectDate(dateCounts);

            Assert.Equal(today.AddDays(1), result);
        }

        [Fact]
        public void SelectDate_ShouldReturnFreestDate_WhenMultipleDatesAreValid()
        {
            var strategy = new FreestDateStrategy();
            var today = new DateTimeOffset(2024, 11, 1, 0, 0, 0, TimeSpan.Zero);
            var dateCounts = new Dictionary<DateTimeOffset, int>
            {
                { today.AddDays(1), 3 },
                { today.AddDays(2), 1 },
                { today.AddDays(3), 10 },
                { today.AddDays(4), 5 },
            };

            var result = strategy.SelectDate(dateCounts);

            Assert.Equal(today.AddDays(2), result);
        }
    }
}