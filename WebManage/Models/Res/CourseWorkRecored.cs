using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class CourseWorkRecored
    {

        public int Id { get; set; }
        public string Msg { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string CreateUid { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string CreateUserName { get; set; }
    }
}
