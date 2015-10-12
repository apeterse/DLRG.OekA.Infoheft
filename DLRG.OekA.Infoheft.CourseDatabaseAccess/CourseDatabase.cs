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
            
            command.CommandText = @"SELECT t.id, t.veranstalter, f.fachbereich ressort, t.beginn start_t, t.ende ende_t, t.ort ort_t,l.inhalt,l.untertitel, l.voraussetzung,
                 l.start start_l, l.ende ende_l, l.lehrgang_nr,l.titel,l.ort ort_l, t.meldeschluss meldeschluss_t, l.meldeschluss meldeschluss_l ,
                 l.zielgruppe, l.ziel, l.leiter, l.fachbereich 
                 FROM `tbl_lehrgang` l
                 join `tbl_fachbereich` f on l.fachbereich = f.id
                 left outer join  `tbl_termine` t on t.lehrgang = l.id 
                where t.beginn > @startdate and t.beginn < @enddate
                order by lehrgang_nr";
            command.Parameters.AddWithValue("@startdate", startDate);
            command.Parameters.AddWithValue("@enddate", endDate);

            MySqlDataReader Reader;
            connection.Open();
            Reader = command.ExecuteReader();
            Course course = new Course();
            string courseNo = string.Empty;

            while (Reader.Read())
            {
                if (Reader.GetString("lehrgang_nr") != courseNo)
                {
                    if (courseNo != String.Empty)
                    {
                        result.Add(course);
                    }
                    
                    course = new Course();
                    course.Id = Reader.GetInt32("id");
                    GetHost(Reader, course);
                    course.Department = Reader.GetString("ressort");
                    course.Title = Reader.GetString("titel");
                    course.Subtitle = Reader.GetString("untertitel");
                    course.Description = Reader.GetString("inhalt");
                    course.Category = (CourseCategory)Reader.GetInt32("fachbereich");
                    courseNo = Reader.GetString("lehrgang_nr");
                }
                var courseDate = new CourseDate();
                courseDate.CourseNo = Reader.GetString("lehrgang_nr");
                
                courseDate.CheckinDeadline = Reader.GetDateTime("meldeschluss_l");
                courseDate.Requirements = Reader.GetString("voraussetzung");
                courseDate.Supervisor = Reader.GetString("Leiter");
                courseDate.CheckinDeadline = Reader.GetDateTime("meldeschluss_l");
                courseDate.TargetAudience = Reader.GetString("zielgruppe");
                courseDate.AP = Reader.GetString("ziel").Contains("AP Fortbildung: Ja");
                courseDate.Juleica = Reader.GetString("ziel").Contains("JuLeiCa Fortbildung: Ja");
                courseDate.Parts.Add(new CoursePart() {Start = Reader.GetDateTime("start_t"), End = Reader.GetDateTime("ende_t"), Location =  Reader.GetString("ort_t") });
                

                

                var priceCommand = new MySqlCommand(@"select * from  `tbl_lehrgang_additional` 
                                                        where lehrgang_id = @lehrgangid and typ = 'kosten' and titel is not null",priceConnection);
                priceCommand.Parameters.AddWithValue("@lehrgangid", course.Id);
                priceConnection.Open();
                var priceReader = priceCommand.ExecuteReader();
                StringBuilder sb = new StringBuilder();
                while (priceReader.Read())
                {
                    sb.Append(priceReader.GetString("titel") + ": " + priceReader.GetString("option") + " € ");
                }
                priceConnection.Close();
                courseDate.Price = sb.ToString();
                course.Dates.Add(courseDate);

                // Kosten muss ich noch extra ermitteln










                    Console.WriteLine(Reader[0]);
            }
            result.Add(course);
            
            
            return result;
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
