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

    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            LatexBuilder latexBuilder = new LatexBuilder();
            latexBuilder.AddLatexHeader(sb);

            CourseDatabase db = new CourseDatabase();

            List<Course> lehrgangsList = db.GetCourseList(new DateTime(2016, 01, 01), new DateTime(2017, 01, 01));

            //LatexBuilder.AddMainContentTable(sb);
            //LatexBuilder.AddEditorial(sb);

            latexBuilder.AddAllCourses(sb, lehrgangsList);
            //LatexBuilder.AddConditionPage(sb);
            //LatexBuilder.AddSpeakerList(sb);
            //LatexBuilder.AddContactList(sb);

            latexBuilder.AddLatexFooter(sb);
            using (StreamWriter sw = new StreamWriter(Path.Combine(Properties.Settings.Default.ExportPath, "teste.tex"), false, Encoding.UTF8))
            {
                sw.Write(sb.ToString());
                sw.Close();
            }
            
        }
    }
}
