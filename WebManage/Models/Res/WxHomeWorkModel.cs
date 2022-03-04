using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class WxHomeWorkModel
    {
        /// <summary>
        /// 排课标题
        /// </summary>
        public string Work_Title { get; set; }

        /// <summary>
        /// 课程评语
        /// </summary>
        public string Comment { get; set; }


        /// <summary>
        /// 家庭作业
        /// </summary>
        public string HomeWorkComent { get; set; }
    }
}
