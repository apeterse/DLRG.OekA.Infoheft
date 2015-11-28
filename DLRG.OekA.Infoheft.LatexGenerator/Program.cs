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

            
            LatexBuilder latexBuilder = new LatexBuilder();

            CourseDatabase db = new CourseDatabase();
            List<Course> lehrgangsList = db.GetCourseList(Properties.Settings.Default.StartDate, Properties.Settings.Default.StartDate.AddYears(1));

            var latexData = latexBuilder.GetLatexCode(lehrgangsList);

            var utf8WithoutBom = new System.Text.UTF8Encoding(false);
            using (StreamWriter sw = new StreamWriter(Path.Combine(Properties.Settings.Default.ExportPath,
                $"Infoheft{Properties.Settings.Default.StartDate.ToString("YYYY")}.tex"), false, utf8WithoutBom))
            {
                sw.Write(latexData.MainDocument);
                sw.Close();
            }

            
            using (StreamWriter sw = new StreamWriter(Path.Combine(Properties.Settings.Default.ExportPath, "Inhaltsverzeichnis.tex"), false, utf8WithoutBom))
            {
                sw.Write(latexData.TableOfContents);
                sw.Close();
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(Properties.Settings.Default.ExportPath, "Jahresuebersicht.tex"), false, utf8WithoutBom))
            {
                sw.Write(latexData.Calender);
                sw.Close();
            }
        }

        
    }
}
