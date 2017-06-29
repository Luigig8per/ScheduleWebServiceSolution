using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class Utilities
    {
        public string convertToEastern(string originalTime)
        {
            DateTime dt = DateTime.ParseExact(originalTime, "yy-MM-dd'T'HH:mm:ssK",
                                  CultureInfo.InvariantCulture,
                                  DateTimeStyles.AdjustToUniversal);

            TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                                 "Eastern Standard Time");

            DateTime easternDateTime = TimeZoneInfo.ConvertTimeFromUtc(dt,
                                                                       easternTimeZone);

            return easternDateTime.ToString();
        }
    }
}
