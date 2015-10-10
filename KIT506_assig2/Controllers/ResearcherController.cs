using RAP.DB;
using RAP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP.Controllers
{
    public class ReseacherController : BaseController
    {

        public ReseacherController() :base()
        {

        }

        public ObservableCollection<Researcher> LoadReseachers(string level, string name){
            if (level == "Student") level = null;
            if (level == "All") level = "";
            if (name == "*") name = null;
            var researcherList = adapter.FetchBasicResearcherDetails(level, name);
            return new ObservableCollection<Researcher>(researcherList);
        }

        public Researcher LoadReseacherDetails(Researcher researcher){
            if (researcher == null) return null;
            return adapter.FetchFullResearcherDetails(researcher);
        }

        public ObservableCollection<Student> GetStaffSupervisions(Researcher researcher)
        {
            if (researcher is Staff)
            {
                var staff = researcher as Staff;
                return new ObservableCollection<Student>(staff.Supervisions);
            }
            return new ObservableCollection<Student>();
        }

        public ObservableCollection<dynamic> GetCumulativeCount(Researcher researcher)
        {
            if (researcher == null) return new ObservableCollection<dynamic>();
            var cumulativeCount = researcher.AllPublications.GroupBy(x => x.Year).Select(x => new { year = x.FirstOrDefault().Year, count = x.Count() }).OrderBy(x=>x.year);
            return new ObservableCollection<dynamic>(cumulativeCount);
        }

        public ObservableCollection<Staff> GetPerformanceDesc()
        {
            var staffLst = adapter.FetchStaffPerfList();
            return new ObservableCollection<Staff>(staffLst);
        }
    }
}
