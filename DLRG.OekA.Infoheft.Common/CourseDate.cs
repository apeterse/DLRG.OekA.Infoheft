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


        public string Requirements
        {
            get;

            set;

        }


        public string TargetAudience
        {
            get;
            set;

        }


        public string Price
        {
            get;
            set;
        }

        public bool Juleica
        {
            get;
            set;

        }

        public bool AP
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