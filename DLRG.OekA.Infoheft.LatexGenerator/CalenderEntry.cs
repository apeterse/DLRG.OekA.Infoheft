namespace DLRG.OekA.Infoheft.LatexGenerator
{
    using System;

    public class CalenderEntry
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string CourseNumber { get; set; }

        public string Title { get; set; }

        public DateTime CheckinDeadline { get; set; }
    }
}