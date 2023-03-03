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

        /// <summary>
        /// 有效课时
        /// </summary>
        public float totalValiteTime { get; set; }

        /// <summary>
        /// 无效课时
        /// </summary>
        public float totalUnValiteTime { get; set; }

        /// <summary>
        /// 迟到扣除课时
        /// </summary>
        public float totalDeductTime { get; set; }
    }

}
