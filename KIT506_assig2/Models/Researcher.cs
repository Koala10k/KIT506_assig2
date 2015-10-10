using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP.Models
{
    using RAP.Helpers;
    public class Researcher
    {
        public int Id { get; set; }
        public string GivenName { private get; set; }
        public string FamilyName { private get; set; }
        public string Title { get; set; }
        public string Unit { get; set; }
        public string Campus { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime? current_start { private get; set; }
        public DateTime? utas_start { private get; set; }
        public List<Position> AllPositions = new List<Position>();
        public List<Publication> AllPublications = new List<Publication>();

        public Position currentJob
        {
            get
            {
                return AllPositions.LastOrDefault();
            }
        }
        public Position earliestJob
        {
            get
            {
                return AllPositions.FirstOrDefault();
            }
        }
        public string CurrentJobTitle
        {
            get
            {
                return currentJob == null ? "Student" : currentJob.JobTitle;
            }
        }

        public string Name
        {
            get
            {
                return GivenName + " " + FamilyName;
            }
        }

        public string CommencedWithInstitution
        {
            get
            {
                return utas_start.ToFormattedDate();
            }
        }

        public string CommencedCurrentPosition
        {
            get
            {
                return current_start.ToFormattedDate();
            }
        }

        public string Tenure
        {
            get
            {
                var start = AllPositions.FirstOrDefault();
                if (start == null || start.start == null) return "Unknown";
                return String.Format("{0:0.00}",DateTime.Now.Subtract(start.start.Value).TotalDays / 365) + " years";
            }
        }

        public int PublicationsCount
        {
            get
            {
                return AllPublications.Count;
            }
        }




        public override string ToString()
        {
            return GivenName + " " + FamilyName;
        }

        public ObservableCollection<Position> HistoricalPositions
        {
            get
            {
                return new ObservableCollection<Position>(AllPositions);
            }
        }

        public int PublicationCount
        {
            get
            {
                return AllPublications.Count;
            }
        }

        public ObservableCollection<Publication> GetPublications
        {
            get
            {
                return new ObservableCollection<Publication>(AllPublications);
            }
        }
    }
}
