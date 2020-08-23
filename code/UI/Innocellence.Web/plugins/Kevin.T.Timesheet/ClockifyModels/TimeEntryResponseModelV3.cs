using System.Collections.Generic;

namespace Kevin.T.Clockify.Data.Models
{
    public class TimeEntryResponseModelV3
    {

        public List<TimeEntryModelV3> groupOne { get; set; }

    }


    public class TimeEntryModelV3
    {
        /// <summary>
        ///  user gid
        /// </summary>
        public string _id { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 员工姓名 小写
        /// </summary>
        public string nameLowerCase { get; set; }

        public List<ProjectChild> children { get; set; }
    }

    public class ProjectChild
    {
        /// <summary>
        /// 项目GID
        /// </summary>
        public string _id { get; set; }

        public string clientName { get; set; }
        public long duration { get; set; }
        public string name { get; set; }
        public string nameLowerCase { get; set; }
        public string color { get; set; }

        public List<TaskChild> children { get; set; }
    }

    public class TaskChild
    {
        /// <summary>
        /// Task GID
        /// </summary>
        public string _id { get; set; }

        public string clientName { get; set; }
        public long duration { get; set; }
        public string name { get; set; }
        public string nameLowerCase { get; set; }
        public string projectColor { get; set; }
        public string projectName { get; set; }
    }
}
