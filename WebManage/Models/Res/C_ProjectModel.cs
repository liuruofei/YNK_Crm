using ADT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_ProjectModel
    {
        /// <summary>
        /// 考试类型id
        /// </summary>
        public int SubjectId { get; set; }
        /// <summary>
        ///考试类型名称
        /// </summary>
        public string SubjectName { get; set; }
        /// <summary>
        /// 单课时
        /// </summary>
        public float UnitCourse_Time { get; set; }


        /// <summary>
        /// 校区Id
        /// </summary>
        public int CampusId { get; set; }

        /// <summary>
        /// 等级1单价
        /// </summary>
        public decimal Lvel1Price { get; set; }

        /// <summary>
        /// 等级2单价
        /// </summary>
        public decimal Lvel2Price { get; set; }

        /// <summary>
        /// 等级3单价
        /// </summary>
        public decimal Lvel3Price { get; set; }

        /// <summary>
        /// 等级4单价
        /// </summary>
        public decimal Lvel4Price { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUid { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateUid { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// 科目列表
        /// </summary>
        public List<C_Project> ProjectList { get; set; }

        /// <summary>
        /// 校区名称
        /// </summary>
        public string CampusName { get; set; }
    }
}
