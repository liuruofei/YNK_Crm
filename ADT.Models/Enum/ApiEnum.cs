using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ADT.Models.Enum
{
    public enum Student_Status
    {
        [Description("未上门")]
        NoVisit=0,

        [Description("已上门")]
        Visit =1,

        [Description("登记汇款")]
        Remittance =2
    }

    public enum ContracRate {
        [Description("小")]
        Min = 0,

        [Description("一般")]
        SameSize = 0,

        [Description("大")]
        Max= 2,

    }

    public enum Constra_Status {
        [Description("待确认")]
        ConfirmationIng =1,

        [Description("校长已审批")]
        Schoolmaster = 2,

        [Description("校长已驳回")]
        SchoolmasterReject = 3,

        [Description("总监已审批")]
        Master=4,

        [Description("总监已驳回")]
        MasterReject = 5,

        [Description("已确认")]
        Confirmationed = 6,

        [Description("退费确认中")]
        BackPay = 7,

        [Description("已退费")]
        BackPayOk = 8,


        [Description("已完成")]
        Finsh = 9,
    }

    public enum Constra_Pay_Status {
        /// <summary>
        /// 未付款
        /// </summary>
        [Description("未付款")]
        NoPay,

        /// <summary>
        /// 部分付款
        /// </summary>
        [Description("部分付款")]
        PartPay,

        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款完成")]
        PayOk,

        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        PayBack,
    }

    public enum ConstraChild_Status
    {
        /// <summary>
        /// 待确认
        /// </summary>
        [Description("待确认")]
        Created,
        /// <summary>
        /// 退费申请中
        /// </summary>
        [Description("退费申请中")]
        BackPay,
        /// <summary>
        /// 退班申请中
        /// </summary>
        [Description("退班申请中")]
        RetrunClass,
        /// <summary>
        /// 转班申请中
        /// </summary>
        [Description("转班申请中")]
        ChangeClass,
        /// <summary>
        /// 变更申请中
        /// </summary>
        [Description("变更申请中")]
        Change,
        /// <summary>
        ///已确认退班
        /// </summary>
        [Description("已确认退班")]
        RetrunClassOk,
        /// <summary>
        /// 已确认退费
        /// </summary>
        [Description("已确认退费")]
        BackPayOk,
        /// <summary>
        /// 已确认转班
        /// </summary>
        [Description("已确认转班")]
        ChangeClassOk,
        /// <summary>
        /// 已确认变更
        /// </summary>
        [Description("已确认变更")]
        ChangeOk,
        /// <summary>
        /// 已确认
        /// </summary>
        [Description("已确认")]
        Confirmationed,

        /// <summary>
        /// 退费申请驳回
        /// </summary>
        [Description("退费申请驳回")]
        BackPayReject,


        /// <summary>
        /// 退班申请驳回
        /// </summary>
        [Description("退班申请驳回")]
        RetrunClassReject,


        /// <summary>
        /// 转班申请驳回
        /// </summary>
        [Description("转班申请驳回")]
        ChangeClassReject,

        /// <summary>
        /// 合同已撤销
        /// </summary>
        [Description("合同已撤销")]
        CanCel,

        /// <summary>
        /// 已驳回
        /// </summary>
        [Description("已驳回")]
        ConfirmationedReject,
    }


    public enum ConstraChild_Pay_Stutas {

        /// <summary>
        /// 未付款
        /// </summary>
        [Description("未付款")]
        NoPay,

        /// <summary>
        /// 部分付款
        /// </summary>
        [Description("部分付款")]
        PartPay,

        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        PayOk,

        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        PayBack,
    }

    public enum Contra_Property {
        [Description("新签")]
        Create =0,
        [Description("续费")]
        Renew = 1,
    }

    public enum Class_Type
    {
        [Description("精品班")]
        Boutique =1,

        [Description("定制班")]
        Customized = 2,

        [Description("常规班")]
        Routine = 3,

        [Description("冲刺班")]
        Sprint = 4,
    }

    public enum Study_Mode {
        [Description("一对一")]
        OneOfOne=1,

        [Description("小班")]
        SmallClass=2,

        [Description("休息")]
        PayMaterial=3,

        [Description("试听")]
        TestGroup=4,
    }
}
