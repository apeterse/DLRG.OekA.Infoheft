namespace DLRG.OekA.Infoheft.Common
{
    using System.Collections.Generic;

    public class Course
    {
        public Course()
        {
            this.Dates = new List<CourseDate>();
        }
        public int Id { get; set; }

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

        public string Host { get; set; }
    }
}