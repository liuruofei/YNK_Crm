using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Req
{
    /// <summary>
    /// 核对答案参数实体
    /// </summary>
    public class CheckAnswersQuery
    {
        /// <summary>
        /// 答题列表
        /// </summary>
        public List<CheckAnswersListQuery> list { get; set; }
    }
    /// <summary>
    /// 答题实体
    /// </summary>
    public class CheckAnswersListQuery
    {
        /// <summary>
        /// 听力id
        /// </summary>
        public string hearing_id { get; set; }
        /// <summary>
        /// 听力数据id
        /// </summary>
        public string hearing_main_id { get; set; }
        /// <summary>
        /// 题目id
        /// </summary>
        public string subject_id { get; set; }
        /// <summary>
        /// 答案选项
        /// </summary>
        public string answer { get; set; }
    }
}
