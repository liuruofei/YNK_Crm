using ADT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class ContracInfmotion
    {
        /// <summary>
        /// 学生名称
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 学生编号
        /// </summary>
        public string StudentNo { get; set; }


        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContraNo { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 合同中心
        /// </summary>
        public int ContraCenterId { get; set; }

        /// <summary>
        /// 校区
        /// </summary>
        public int CampusId { get; set; }


        /// <summary>
        /// 学生手机
        /// </summary>
        public string Student_Phone { get; set; }

        /// <summary>
        /// 学生微信
        /// </summary>
        public string Student_Wechat { get; set; }

        /// <summary>
        /// 家长邮箱
        /// </summary>
        public string Elder_Email { get; set; }

        /// <summary>
        /// 家长姓名
        /// </summary>
        public string Elder_Name { get; set; }

        /// <summary>
        /// 家长手机
        /// </summary>
        public string Elder_Phone { get; set; }

        /// <summary>
        /// 家长微信
        /// </summary>
        public string Elder_Wechat { get; set; }

        /// <summary>
        /// 家长2邮箱
        /// </summary>
        public string Elder2_Email { get; set; }

        /// <summary>
        /// 家长2姓名
        /// </summary>
        public string Elder2_Name { get; set; }

        /// <summary>
        /// 家长2手机
        /// </summary>
        public string Elder2_Phone { get; set; }

        /// <summary>
        /// 家长2微信
        /// </summary>
        public string Elder2_Wechat { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }


        /// <summary>
        /// 年级
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Soure { get; set; }

        /// <summary>
        /// 学生邮箱
        /// </summary>
        public string Student_Email { get; set; }


        /// <summary>
        /// 在读学校
        /// </summary>
        public string InSchool { get; set; }



        public List<C_Contrac_Child>  Listchild { get; set; }

        /// <summary>
        /// 子类列表扩展展示
        /// </summary>
        public List<UserChildContracModel> childList { get; set; }
    }
}
