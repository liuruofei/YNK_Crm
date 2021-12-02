using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Models.ResModel
{
    public class ResResult
    {
        public string msg { get; set;}

        public int code { get; set; } = 0;

        public dynamic data { get; set; }

        public totalRow totalRow { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int count { get; set; }
    }
    public class totalRow {
        /// <summary>
        /// 总课时
        /// </summary>
        public float totalCourseTime { get; set; }
    }

}
