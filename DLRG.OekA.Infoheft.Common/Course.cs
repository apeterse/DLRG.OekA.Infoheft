namespace DLRG.OekA.Infoheft.Common
{
    using System.Collections.Generic;

    public class Course
    {
        public Course()
        {
            this.Dates = new List<CourseDate>();
        }

        public List<CourseDate> Dates
        {
            get; set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Subtitle
        {
            get;
            set;

        }

        public string Department
        {
            get;
            set;
        }

        public CourseCategory Category
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;

        }

        public string Host
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
    }
}