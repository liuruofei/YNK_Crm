using ADT.Common;
using ADT.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models.Res
{
    public class UserContracModel
    {
        public int ContracId { get; set; }
        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContraNo { get; set; }
        /// <summary>
        /// 学生uid
        /// </summary>
        public int StudentUid { get; set; }
        /// <summary>
        /// 合同中心
        /// </summary>
        public string ContraCenter { get; set; }
        /// <summary>
        /// 线索id
        /// </summary>
        public int ClueId { get; set; }
        /// <summary>
        /// 顾问uid
        /// </summary>
        public string CC_Uid { get; set; }
        public int Constra_Status { get; set; }
        /// <summary>
        /// 校区id
        /// </summary>
        public int CampusId { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal Total_Amount { get; set; }
        /// <summary>
        ///已付金额
        /// </summary>
        public decimal Pay_Amount { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Start_Time { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End_Time { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 学员姓名
        /// </summary>
        public string Student_Name { get; set; }


        /// <summary>
        /// 学员余额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 签约人
        /// </summary>
        public string CCUserName { get; set; }

        /// <summary>
        /// 校区名称
        /// </summary>
        public string CampusName { get; set; }

        public string StudyModes { get; set; }

        public string StudyModesName { get {
                var currtName = "";
                if (!string.IsNullOrEmpty(StudyModes))
                {
                    string[] modeGroup = StudyModes.Split(",");
                    foreach (var item in modeGroup) {
                        if (int.Parse(item) == 1) 
                            currtName += "一对一,";
                        if (int.Parse(item) ==2)
                            currtName += "小班,";
                        if (int.Parse(item) ==3)
                            currtName += "付费教材,";
                        if (int.Parse(item) == 4)
                            currtName += "考团,";
                    }
                }
                return currtName;
            } }

        /// <summary>
        /// 合同状态名称
        /// </summary>
        public string Constra_Status_Name
        {
            get {
                return EnumHelper.GetDescription<Constra_Status>(Constra_Status);
            }
        }
    }
}
