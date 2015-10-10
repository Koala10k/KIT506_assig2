using MySql.Data.MySqlClient;
using RAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP.DB
{
    public class ERDAdapter
    {
        private static ERDAdapter instance;
        private MySqlConnection conn;

        private ERDAdapter()
        {
            conn = DAO.GetConnection();
        }

        public static ERDAdapter getInstance()
        {
            if (instance == null)
                instance = new ERDAdapter();
            return instance;
        }

        public List<Researcher> FetchBasicResearcherDetails(string enumLevel = "", string Name = null)
        {
            List<Researcher> researchers = new List<Researcher>();

            MySqlDataReader rdr = null;

            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("select id, given_name, family_name, type from researcher", conn);
                if (enumLevel == null)
                {
                    cmd = new MySqlCommand("select id, given_name, family_name, type from researcher where level is null", conn);
                }
                if ("" != enumLevel && enumLevel != null)
                {
                    cmd = new MySqlCommand("select id, given_name, family_name, type from researcher where level=?level", conn);
                    cmd.Parameters.AddWithValue("level", enumLevel);
                }

                if (!String.IsNullOrEmpty(Name))
                {
                    cmd = new MySqlCommand("select id, given_name, family_name, type from researcher where given_name like @gn or family_name like @fn", conn);
                    cmd.Parameters.AddWithValue("gn", "%" + Name + "%");
                    cmd.Parameters.AddWithValue("fn", "%" + Name + "%");
                }

                if ("" != enumLevel && !String.IsNullOrEmpty(Name))
                {
                    cmd = new MySqlCommand("select id, given_name, family_name, type from researcher where level=?level and (given_name like @gn or family_name like @fn)", conn);
                    cmd.Parameters.AddWithValue("level", enumLevel);
                    cmd.Parameters.AddWithValue("gn", "%" + Name + "%");
                    cmd.Parameters.AddWithValue("fn", "%" + Name + "%");
                }
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    if (rdr.GetString("type") == "Staff")
                    {
                        researchers.Add(new Staff { Id = rdr.GetInt32(0), GivenName = rdr.GetString(1), FamilyName = rdr.GetString(2) });
                    }
                    else
                    {
                        researchers.Add(new Student { Id = rdr.GetInt32(0), GivenName = rdr.GetString(1), FamilyName = rdr.GetString(2) });
                    }
                }
            }
            catch (MySqlException e)
            {
                throw new Exception("loading staff", e);
            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return researchers;
        }

        public Researcher FetchFullResearcherDetails(Researcher researcher)
        {
            MySqlDataReader rdr = null;
            bool isEmpty = true;
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("select * from researcher where id=?id", conn);
                cmd.Parameters.AddWithValue("id", researcher.Id);

                using (rdr = cmd.ExecuteReader())
                {

                    if (rdr.Read())
                    {
                        if (researcher is Student)
                        {
                            var student = researcher as Student;
                            student.Degree = rdr.GetString("degree");
                            student.Supervisor = new Staff { Id = rdr.GetInt32("supervisor_id") };
                        }
                        else
                        {
                            var staff = researcher as Staff;
                            staff.currentLevel = (EmploymentLevel)Enum.Parse(typeof(EmploymentLevel), rdr.GetString("level"));
                        }
                        PopulateResearcher(rdr, researcher);
                        isEmpty = false;
                    }
                }

                if (isEmpty) return null;

                if (researcher is Student)
                {
                    var student = researcher as Student;
                    cmd = new MySqlCommand("select * from researcher where id=?id", conn);
                    cmd.Parameters.AddWithValue("id", student.Supervisor.Id);
                    using (rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            PopulateStaff(rdr, student.Supervisor);
                        }
                    }
                }
                else
                {
                    var staff = researcher as Staff;
                    cmd = new MySqlCommand("select * from researcher where supervisor_id=?id", conn);
                    cmd.Parameters.AddWithValue("id", staff.Id);
                    using (rdr = cmd.ExecuteReader())
                    {
                        staff.Supervisions.Clear();
                        while (rdr.Read())
                        {
                            var student = new Student();
                            PopulateStudent(rdr, student);
                            student.Supervisor = staff;
                            staff.Supervisions.Add(student);
                        }
                    }
                }


                cmd = new MySqlCommand("select * from position where id=?id order by start", conn);
                cmd.Parameters.AddWithValue("id", researcher.Id);
                using (rdr = cmd.ExecuteReader())
                {

                    researcher.AllPositions.Clear();
                    while (rdr.Read())
                    {
                        researcher.AllPositions.Add(new Position
                        {
                            Level = (EmploymentLevel)Enum.Parse(typeof(EmploymentLevel), rdr.GetString("level")),
                            start = rdr.IsDBNull(rdr.GetOrdinal("start")) ? (DateTime?)null : rdr.GetDateTime("start"),
                            end = rdr.IsDBNull(rdr.GetOrdinal("end")) ? (DateTime?)null : rdr.GetDateTime("end")
                        });
                    }
                }

                cmd = new MySqlCommand("select p.* from publication p join researcher_publication rp on p.doi=rp.doi where rp.researcher_id=?id order by year desc, title", conn);
                cmd.Parameters.AddWithValue("id", researcher.Id);
                using (rdr = cmd.ExecuteReader())
                {
                    researcher.AllPublications.Clear();
                    while (rdr.Read())
                    {
                        var id = rdr.GetString(4);
                        researcher.AllPublications.Add(new Publication
                        {
                            DOI = rdr.GetString(0),
                            Title = rdr.GetString(1),
                            Authors = rdr.GetString(2),
                            Year = rdr.GetString(3),
                            Type = (OutputType)Enum.Parse(typeof(OutputType), rdr.GetString(4)),
                            CiteAs = rdr.GetString(5),
                            Available = rdr.GetDateTime(6),
                            Age = DateTime.Now.Subtract(rdr.GetDateTime(6)).Days.ToString() + " Days"

                        });




                    }
                }
            }
            catch (MySqlException e)
            {
                throw new Exception("loading staff", e);
            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return researcher;
        }

        private void PopulateStudent(MySqlDataReader rdr, Student student)
        {
            PopulateResearcher(rdr, student);
            student.Degree = rdr.GetString("degree");
        }

        private void PopulateStaff(MySqlDataReader rdr, Staff staff)
        {
            PopulateResearcher(rdr, staff);
            staff.currentLevel = (EmploymentLevel)Enum.Parse(typeof(EmploymentLevel), rdr.GetString("level"));
        }

        private void PopulateResearcher(MySqlDataReader rdr, Researcher researcher)
        {
            researcher.Id = rdr.GetInt32("id");
            researcher.GivenName = rdr.GetString("given_name");
            researcher.FamilyName = rdr.GetString("family_name");
            researcher.Title = rdr.GetString("title");
            researcher.Unit = rdr.GetString("unit");
            researcher.Campus = rdr.GetString("campus");
            researcher.PhotoUrl = rdr.GetString("photo");
            researcher.Email = rdr.GetString("email");
            researcher.utas_start = rdr.GetDateTime("utas_start");
            researcher.current_start = rdr.GetDateTime("current_start");
        }

        public Researcher CompleteResearcherDetails(Researcher r)
        {
            return null;
        }

        public IEnumerable<Publication> FetchBasicPublicationDetails(Researcher r)
        {
            var publications = new List<Publication>();
            MySqlDataReader rdr = null;
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select p.* from publication p join researcher_publication rp on p.doi=rp.doi where rp.researcher_id=?id order by year desc, title", conn);
                cmd.Parameters.AddWithValue("id", r.Id);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    publications.Add(new Publication
                        {
                            DOI = rdr.GetString(0),
                            Title = rdr.GetString(1),
                            Authors = rdr.GetString(2),
                            Year = rdr.GetString(3),
                            Type = (OutputType)Enum.Parse(typeof(OutputType), rdr.GetString(4)),
                            CiteAs = rdr.GetString(5),
                            Available = rdr.GetDateTime(6)
                        });
                }
            }
            catch (MySqlException e)
            {
                throw new Exception("loading staff", e);
            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return publications;
        }

        public Publication CompletePublicationDetails(Publication p)
        {
            return null;
        }

        public Dictionary<Publication, int> FetchPublicationCounts(DateTime from, DateTime to)
        {
            return null;
        }

        internal List<Staff> FetchStaffPerfList()
        {
            List<Staff> staffList = new List<Staff>();

            MySqlDataReader rdr = null;

            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("select id, given_name, family_name, level from researcher where type=?type", conn);
                cmd.Parameters.AddWithValue("type", "Staff");
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    staffList.Add(new Staff { Id = rdr.GetInt32(0), GivenName = rdr.GetString(1), FamilyName = rdr.GetString(2), currentLevel = (EmploymentLevel)Enum.Parse(typeof(EmploymentLevel), rdr.GetString("level")) });
                }
            }
            catch (MySqlException e)
            {
                throw new Exception("loading staff", e);
            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }

            foreach (Staff s in staffList)
            {
                s.AllPublications = FetchBasicPublicationDetails(s).ToList();
            }

            return staffList;
        }
    }
}
