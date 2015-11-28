namespace DLRG.OekA.Infoheft.LatexGenerator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DLRG.OekA.Infoheft.Common;

    public class CalenderBuilder
    {
        List<CalenderEntry> calenderEntries = new List<CalenderEntry>();
        private StringBuilder calender;
        public CalenderBuilder()
        {
            this.calender = new StringBuilder();
            this.calender.AppendLine(@"\renewcommand\myheadingtext{Jahresübersicht}");
            this.calender.AppendLine(@"\section*{Seminarübersicht}");
            this.calender.AppendLine(@"\small");
            this.calender.AppendLine(@"\changetext{}{}{-1cm}{-1cm}{}");
            this.calender.AppendLine(@"\begin{tabularx}{\textwidth+\marginparwidth}{lllXlr}");
            this.calender.AppendLine(@"\rowcolor{gray} \textcolor{white}{von} & \textcolor{white}{bis} & \textcolor{white}{Lg-Nr.}&\textcolor{white}{Titel}&\textcolor{white}{Meldeschluss}&\textcolor{white}{Seite} \\");
            this.calender.AppendLine(@"\endhead");
        }


        public void AddToCalender(Course course)
        {
            foreach (var courseDate in course.Dates)
            {
                
                foreach (var coursePart in courseDate.Parts)
                {
                    this.calenderEntries.Add(new CalenderEntry() { CheckinDeadline = courseDate.CheckinDeadline, CourseNumber = courseDate.CourseNo, Title = course.Title, StartDate = coursePart.Start, EndDate = coursePart.End, PageReference = course.Dates[0].CourseNo});
                }
                
            }
        }


        public string GetCalender()
        {
            var list = this.calenderEntries.OrderBy(x => x.StartDate);

            foreach (var calenderEntry in list)
            {
                this.calender.AppendLine(
                    string.Format(
                        @"{0}&{1}&{2}&{3}&{4}&~\pageref{{{5}}}\\",
                        calenderEntry.StartDate.ToString("dd.MM."),
                        calenderEntry.EndDate.ToString("dd.MM."),
                        calenderEntry.CourseNumber,
                        calenderEntry.Title,
                        calenderEntry.CheckinDeadline.ToString("dd.MM.yyyy"),
                        calenderEntry.PageReference));
            }



            this.CloseCalender();
            return this.calender.ToString();
        }

        private void CloseCalender()
        {
            this.calender.AppendLine(@"\end{tabularx}");
            this.calender.AppendLine(@"\changetext{}{}{1cm}{1cm}{}");
            this.calender.AppendLine(@"\normalsize");
            this.calender.AppendLine(@"\newpage");
        }
    }
}