namespace DLRG.OekA.Infoheft.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using log4net;

    public class InfoHeftFileAccess
    {
        private static readonly ILog log = LogManager.GetLogger("InfoheftFileAccess");

        public Course LoadLehrgang(string filename)
        {
            Course course = null;
            try
            {
                using (StreamReader sr = new StreamReader(filename, Encoding.UTF8))
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(Course));
                    course = (Course)mySerializer.Deserialize(sr);
                    InfoHeftRules rules = new InfoHeftRules();
                    course.Description = rules.EnsureThatAnredeIsUpercase(course.Description);
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                log.Error(filename + Environment.NewLine + e);
            }

            return course;
        }

        public List<Course> GetLehrgangsList(string pathname)
        {
            string[] filenames = Directory.GetFiles(pathname, "*.xml");
            List<Course> courses = new List<Course>();
            foreach (var filename in filenames)
            {
                if (!filename.ToLower().Contains("kalender"))
                {
                    var course = this.LoadLehrgang(filename);
                    if (course != null)
                    {
                        courses.Add(course);
                    }

                }
            }
            return courses;
        }
    }
}
