namespace DLRG.OekA.Infoheft.LatexGenerator
{
    using System.Text;

    using DLRG.OekA.Infoheft.Common;

    public class TocBuilder
    {
        private StringBuilder tableOfContents;
        public TocBuilder()
        {
            this.tableOfContents = new StringBuilder();
            this.tableOfContents.AppendLine(@"\renewcommand\myheadingtext{Inhaltsverzeichnis}");
            this.tableOfContents.AppendLine(@"\section*{Seminar�bersicht}");
            this.tableOfContents.AppendLine(@"\begin{tabularx}{\textwidth+\marginparwidth}{rXl}");
        }

        public void AddToToc(Course course)
        {
            var multiCouseMarker = course.Dates.Count > 1 ? "*" : "";

            this.tableOfContents.AppendLine(@"~\pageref{" + course.Dates[0].CourseNo + @"} \hspace{1em} " + course.Title + @" \hfill " + course.Dates[0].CourseNo + multiCouseMarker + @" \\");
        }

        public void AddChapterToToc(Course course)
        {
            this.tableOfContents.AppendLine(@"\mygreybox{" + course.Department + @"} \\");
        }



        public string GetToc()
        {
            this.CloseToc();
            return this.tableOfContents.ToString();
        }

        private void CloseToc()
        {
            this.tableOfContents.AppendLine(@"\end{tabularx}");
        }
    }
}