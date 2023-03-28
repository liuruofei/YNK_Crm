using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class C_UserCourseTimeModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 子合同编码
        /// </summary>
        public string Contra_ChildNo { get; set; }
        public int StudentUid { get; set; }
        public int SubjectId { get; set; }

        /// <summary>
        /// 科目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
        public int ClassId { get; set; }
        public float Course_Time { get; set; }
        public float Course_UseTime { get; set; }
        public float Class_Course_Time { get; set; }
        public float Class_Course_UseTime { get; set; }

        //实际使用1对1课时
        public float ShjiUseTime { get; set; }

        //实际使用班课时
        public float ShjiClassUseTime { get; set; }
        /// <summary>
        /// 赠送课使用
        /// </summary>
        public float UsePresendTime { get; set; }




        /// <summary>
        /// 班级名称
        /// </summary>
        public string Class_Name { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        ///分类名称
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// 是否是试听
        /// </summary>
        public int IsLinsting { get; set; }

        /// <summary>
        /// 是否赠送课时
        /// </summary>
        public int IsPresent { get; set; }

        /// <summary>
        /// 等级1价格
        /// </summary>
        public decimal Lvel1Price { get; set; }

        /// <summary>
        /// 等级2价格
        /// </summary>
        public decimal Lvel2Price { get; set; }

        /// <summary>
        /// 等级3价格
        /// </summary>
        public decimal Lvel3Price { get; set; }

        /// <summary>
        /// 等级4价格
        /// </summary>
        public decimal Lvel4Price { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}
