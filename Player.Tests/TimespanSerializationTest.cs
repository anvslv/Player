using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Player.Tests
{
    public class TimespanSerializationTest
    {
        [Fact]
        public void ConvertToStringConvertToTimespan_IsValid()
        {
            var time = new TimeSpan(0, 0, 15, 20);
            string ticks = time.Ticks.ToString(CultureInfo.InvariantCulture);
            var expected = TimeSpan.FromTicks(Int64.Parse(ticks));

            Assert.Equal(time, expected);
        }
    }
}
