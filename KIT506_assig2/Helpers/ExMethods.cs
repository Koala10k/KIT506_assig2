using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP.Helpers
{
    static class ExMethods
    {
        public static string ToFormattedDate(this DateTime? datetime)
        {
            if (datetime == null)
                return "Unknown";
            else
            {
                return datetime.Value.ToString("dd/MM/yyyy");
            }
        }
    }
}
