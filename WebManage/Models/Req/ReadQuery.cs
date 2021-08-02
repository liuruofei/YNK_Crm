using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Req
{
    /// <summary>
    /// 核对答案参数实体
    /// </summary>
    public class CheckAnswersReadQuery
    {
        /// <summary>
        /// 答题列表
        /// </summary>
        public List<CheckAnswersListReadQuery> list { get; set; }
    }
    /// <summary>
    /// 答题实体
    /// </summary>
    public class CheckAnswersListReadQuery
    {
        /// <summary>
        /// 阅读id
        /// </summary>
        public string read_id { get; set; }
        /// <summary>
        /// 阅读数据id
        /// </summary>
        public string read_main_id { get; set; }
        /// <summary>
        /// 题目id
        /// </summary>
        public string subject_id { get; set; }
        /// <summary>
        /// 答案顺序
        /// </summary>
        public string answer { get; set; }
    }
}
