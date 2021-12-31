using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_ClueTaskModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 线索ID
        /// </summary>
        public int ClueId { get; set; }
        /// <summary>
        /// 任务时间
        /// </summary>
        public DateTime? TaskDate { get; set; }
        /// <summary>
        /// 任务内容
        /// </summary>
        public string TaskContent { get; set; }
        /// <summary>
        /// 重要等级
        /// </summary>
        public int ImportLevel { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public int TaskStutas { get; set; }

        /// <summary>
        /// 校区Id
        /// </summary>
        public int CampusId { get; set; }

        /// <summary>
        /// 顾问uid
        /// </summary>
        public string CC_Uid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        public string CreateUid { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        public string UpdateUid { get; set; }

        public string CCUserName { get; set; }

        /// <summary>
        /// 学生名称
        /// </summary>
        public string Student_Name { get; set; }
    }
}
