using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLRG.OekA.Infoheft.CourseDatabaseAccess
{
    using DLRG.OekA.Infoheft.Common;

    using log4net;

    using MySql.Data.MySqlClient;

    public class CourseDatabase
    {
        private static readonly ILog log = LogManager.GetLogger("CourseDatabase");

        public List<Course> GetCourseList(DateTime startDate, DateTime endDate)
        {
            var result = new List<Course>();

            MySqlConnection connection = new MySqlConnection(Properties.Settings.Default.CourseDbConnectionString);

            MySqlConnection priceConnection = new MySqlConnection(Properties.Settings.Default.CourseDbConnectionString);

            MySqlCommand command = connection.CreateCommand();
            
            command.CommandText = @"SELECT l.id, t.veranstalter, f.fachbereich ressort, t.beginn start_t, t.ende ende_t, t.ort ort_t,l.inhalt,l.untertitel, l.voraussetzung,
                 l.start start_l, l.ende ende_l, l.lehrgang_nr,l.titel,l.ort ort_l, t.meldeschluss meldeschluss_t, l.meldeschluss meldeschluss_l ,
                 l.zielgruppe, l.ziel, l.leiter, l.fachbereich 
                 FROM `tbl_lehrgang` l
                 join `tbl_fachbereich` f on l.fachbereich = f.id
                 left outer join  `tbl_termine` t on t.lehrgang = l.id 
                where t.beginn > @startdate and t.beginn < @enddate
                order by titel,untertitel,inhalt,lehrgang_nr";
            command.Parameters.AddWithValue("@startdate", startDate);
            command.Parameters.AddWithValue("@enddate", endDate);

            MySqlDataReader Reader;
            connection.Open();
            Reader = command.ExecuteReader();
            Course course = new Course();
            CourseDate courseDate= new CourseDate();
            string courseNo = string.Empty;
            string TitleSubtitleDesc = string.Empty;

            while (Reader.Read())
            {
                if (IsNewCourseDate(Reader, courseNo))
                {
                    if (IsNewCourse(Reader, TitleSubtitleDesc))
                    {
                        // general information
                        course = new Course();
                        GetHost(Reader, course);
                        course.Department = Reader.GetString("ressort");
                        course.Title = Reader.GetString("titel");
                        course.Subtitle = Reader.GetString("untertitel");
                        course.Description = Reader.GetString("inhalt");
                        course.Category = (CourseCategory)Reader.GetInt32("fachbereich");
                        course.Requirements = Reader.GetString("voraussetzung");
                        course.TargetAudience = Reader.GetString("zielgruppe");
                        course.AP = Reader.GetString("ziel").Contains("AP Fortbildung: Ja");
                        course.Juleica = Reader.GetString("ziel").Contains("JuLeiCa Fortbildung: Ja");
                        var sb = GetPriceData(priceConnection, Reader.GetInt32("id"));
                        course.Price = sb.ToString();
                        result.Add(course);
                        TitleSubtitleDesc = course.Title + course.Subtitle + course.Description;
                    }

                    courseDate = new CourseDate();
                    courseDate.CourseNo = Reader.GetString("lehrgang_nr");

                    courseDate.CheckinDeadline = Reader.GetDateTime("meldeschluss_l");
                    courseDate.Supervisor = Reader.GetString("Leiter");
                    courseDate.CheckinDeadline = Reader.GetDateTime("meldeschluss_l");
                    courseDate.Id = Reader.GetInt32("id");
                    course.Dates.Add(courseDate);
                    courseNo = courseDate.CourseNo;
                    
                    log.DebugFormat("Lehrgang {0} verarbeitet", courseDate.Id);
                }

                courseDate.Parts.Add(new CoursePart() {Start = Reader.GetDateTime("start_t"), End = Reader.GetDateTime("ende_t"), Location =  Reader.GetString("ort_t") });
            }
            course.Dates.Add(courseDate);
            result.Add(course);
            
            return result;
        }

        private bool IsNewCourseDate(MySqlDataReader reader, string courseNo)
        {
            return reader.GetString("titel") != courseNo;
        }

        private static bool IsNewCourse(MySqlDataReader Reader, string TitleSubtitleDesc)
        {
            return GetTitleSubtitleDescFormReader(Reader) != TitleSubtitleDesc;
        }

        private static string GetTitleSubtitleDescFormReader(MySqlDataReader reader)
        {
            return reader.GetString("titel")+ reader.GetString("untertitel") + reader.GetString("inhalt");
        }

        private static StringBuilder GetPriceData(MySqlConnection priceConnection, int courseId)
        {
            var priceCommand = new MySqlCommand(
                @"select * from  `tbl_lehrgang_additional` 
                                                        where lehrgang_id = @lehrgangid and typ = 'kosten' and titel is not null",
                priceConnection);
            priceCommand.Parameters.AddWithValue("@lehrgangid", courseId);
            priceConnection.Open();
            var priceReader = priceCommand.ExecuteReader();
            StringBuilder sb = new StringBuilder();
            while (priceReader.Read())
            {
                string price = priceReader.GetString("option");
                string[] priceparts = price.Split('.');

                sb.AppendLine(priceReader.GetString("titel") + ": " + priceparts[0] + " € ");
            }
            priceConnection.Close();
            return sb;
        }

        private static void GetHost(MySqlDataReader Reader, Course course)
        {
            int host = Reader.GetInt32("veranstalter");
            if (host == 1)
            {
                course.Host = "Stammverband";
            }
            course.Host = "Jugend";
        }
    }
}
