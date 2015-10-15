namespace DLRG.OekA.Infoheft.Common
{
    using System;
    using System.Collections.Generic;

    public class CourseDate
    {
        public CourseDate()
        {
            this.Parts = new List<CoursePart>();
        }

        public string Note { get; set; }

        public string CourseNo
        {
            get;
            set;
        }

        public string Supervisor
        {
            get;
            set;

        }

        public DateTime CheckinDeadline
        {
            get;
            set;

        }

        public List<CoursePart> Parts
        {
            get;
            set;

        }

        public int Id { get; set; }
    }
}