using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP.Models
{
    using RAP.Helpers;
    public class Position
    {
        public EmploymentLevel Level { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { private get; set; }

        public string GetStart
        {
            get
            {
                return start.ToFormattedDate();
            }
        }

        public string GetEnd
        {
            get
            {
                return end == null ? "Present" : end.ToFormattedDate();
            }
        }
        public string JobTitle
        {
            get
            {
                var _title = "Unknown";
                switch (Level)
                {
                    case EmploymentLevel.A:
                        _title = "PostDoc";
                        break;
                    case EmploymentLevel.B:
                        _title = "Lecturer";
                        break;
                    case EmploymentLevel.C:
                        _title = "Senior Lecturer";
                        break;
                    case EmploymentLevel.D:
                        _title = "Associate Professor";
                        break;
                    case EmploymentLevel.E:
                        _title = "Professor";
                        break;
                }
                return _title;
            }
        }
    }
}
