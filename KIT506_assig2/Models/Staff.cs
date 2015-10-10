using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP.Models
{
    public class Staff : Researcher
    {
        public List<Student> Supervisions = new List<Student>();
        public EmploymentLevel currentLevel { get; set; }
        public float ThreeYearAvg
        {
            get
            {
                int threeYearsAgo = DateTime.Now.Year - 3;
                return AllPublications.Where(x => Int32.Parse(x.Year) >= threeYearsAgo).Count()/3f;
            }
        }

        public float Performance
        {
            get
            {
                float expectedNum = 0;
                switch (currentLevel)
                {
                    case EmploymentLevel.A:
                        expectedNum = 0.5f;
                        break;
                    case EmploymentLevel.B:
                        expectedNum = 1f;
                        break;
                    case EmploymentLevel.C:
                        expectedNum = 2f;
                        break;
                    case EmploymentLevel.D:
                        expectedNum = 3.2f;
                        break;
                    case EmploymentLevel.E:
                        expectedNum = 4f;
                        break;
                }
                int threeYearsAgo = DateTime.Now.Year - 3;
                int realNum = AllPublications.Where(x => Int32.Parse(x.Year) >= threeYearsAgo).Count();

                return realNum / expectedNum;
            }
        }

        public string GetPerformance
        {
            get
            {
                return Performance * 100 + "%";
            }
        }

        public string GetThreeYearAvg
        {
            get
            {
                return String.Format("{0:0.00}", ThreeYearAvg);
            }
        }

        public int SupervisionsCount
        {
            get
            {
                return Supervisions.Count;
            }
        }

        public string GetPerformanceDesc
        {
            get
            {
                if (Performance <= 0.7)
                {
                    return "Poor";
                }
                else if (Performance < 1.1)
                {
                    return "Below Expectations";
                }
                else if (Performance < 2)
                {
                    return "Meeting Minimum";
                }
                else
                {
                    return "Star Performers";
                }
            }
        }
    }
}
