namespace DLRG.OekA.Infoheft.LatexGenerator
{
    using System;
    using System.Text;

    using DLRG.OekA.Infoheft.Common;

    public class TocBuilder
    {
        private StringBuilder tableOfContents;
        public TocBuilder()
        {
            this.tableOfContents = new StringBuilder();
            this.tableOfContents.AppendLine(@"\renewcommand\myheadingtext{Inhaltsverzeichnis}");
            this.tableOfContents.AppendLine(@"\section*{Seminarübersicht}");
            this.tableOfContents.AppendLine(@"\begin{tabularx}{\textwidth+\marginparwidth}{rXl}");
        }

        public void AddToToc(Course course)
        {
            var multiCouseMarker = course.Dates.Count > 1 ? "*" : "";

            this.tableOfContents.AppendLine(@"~\pageref{" + course.Dates[0].CourseNo + @"} \hspace{1em}& " + course.Title + @" \hfill &" + course.Dates[0].CourseNo + multiCouseMarker + @" \\");
            
        }

        public void AddChapterToToc(Course course)
        {
            
            this.tableOfContents.AppendLine(String.Format(@"\rowcolor{{gray}} \hspace{{1em}}& {0} \hfill &  \\",course.Department));
        }
        
        public string GetToc()
        {
            this.CloseToc();
            return this.tableOfContents.ToString();
        }

        private void CloseToc()
        {
            this.tableOfContents.AppendLine(@"\end{tabularx}");
            this.tableOfContents.AppendLine("*zu diesem Lehrgang gibt es weitere Termine, s. Lehrgangsseite");
            this.tableOfContents.AppendLine(@"\newpage");
        }
    }
}