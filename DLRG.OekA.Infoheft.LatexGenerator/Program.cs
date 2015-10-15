using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLRG.OekA.Infoheft.LatexGenerator
{
    using System.IO;

    using DLRG.OekA.Infoheft.Common;
    using DLRG.OekA.Infoheft.CourseDatabaseAccess;

    using log4net;
    using log4net.Config;

    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {

            XmlConfigurator.Configure();
            log.Info("start");

            StringBuilder sb = new StringBuilder();
            LatexBuilder latexBuilder = new LatexBuilder();
            latexBuilder.AddLatexHeader(sb);

            CourseDatabase db = new CourseDatabase();

            List<Course> lehrgangsList = db.GetCourseList(new DateTime(2016, 01, 01), new DateTime(2017, 01, 01));

            // hier die dubletten ermitteln

            // dann die Lehrgänge in eine neue liste schreiben


            latexBuilder.AddAllCourses(sb, lehrgangsList);
            
            latexBuilder.AddLatexFooter(sb);

            var utf8WithoutBom = new System.Text.UTF8Encoding(false);
            using (StreamWriter sw = new StreamWriter(Path.Combine(Properties.Settings.Default.ExportPath, "teste.tex"), false, utf8WithoutBom))
            {
                sw.Write(sb.ToString());
                sw.Close();
            }
        }
    }
}
