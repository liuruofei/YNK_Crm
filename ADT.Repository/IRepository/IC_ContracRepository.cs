using ADT.Models;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Repository.IRepository
{
    public interface IC_ContracRepository : IBaseRepository<C_Contrac>
    {
        /// <summary>
        /// 新增合同
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        ResResult AddUserContrac(ContracInput input);

        /// <summary>
        /// 更新主合同信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveContrc(ContracInput input);

        /// <summary>
        /// 创建账号
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ResResult SettingAccount(C_Contrac_User user);

        /// <summary>
        /// 添加签约用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveContracUser(C_Contrac_User input);


        ResResult SaveGiftUserTime(ContracInput input);

        /// <summary>
        /// 审核主合同
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="thought"></param>
        /// <returns></returns>
        ResResult Audit(int Id, bool thought);


        /// <summary>
        /// 保存考试类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveSubject(SubjectInput input);


        /// <summary>
        /// 保存子合同
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveChildContrac(ContracChildInput input);

        /// <summary>
        /// 退费申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        ResResult ContrcBackCostPart(C_Contrac_Child_ConstBack input);


        /// <summary>
        /// 退班申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult ContrcBackClass(C_Contrac_Child_RetrunClass input);


        ResResult ContrcChangeClass(C_Contrac_Child_ChangeClass input);

        /// <summary>
        /// 子合同退费
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult ContrcBackCostPartAudit(string Contra_ChildNo, bool through, string uid);

        /// <summary>
        /// 子合同退班
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult ContrcBackClassAudit(string Contra_ChildNo, bool through, string uid);

        /// <summary>
        /// 子合同转班
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="through"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult ContrcChnageClassAudit(string Contra_ChildNo, bool through, string uid);


        /// <summary>
        /// 子合撤销
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult ContracCancel(string Contra_ChildNo, string uid);

        /// <summary>
        /// 主合同删除
        /// </summary>
        /// <param name="contraNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult MasterContracCancel(string contraNo, string uid);

        /// <summary>
        /// 子合同确认
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult ContracChildConfig(string Contra_ChildNo, string uid);


        /// <summary>
        /// 子合同确认变更
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult ContracChildChangeConfig(string Contra_ChildNo, string uid);


        /// <summary>
        ///确认收款
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="through"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult ConfigCollection(int collectionId, bool through, string uid);

        /// <summary>
        /// 保存到款收据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveCollection(CollectionInput input);

        /// <summary>
        /// 部分小时退费
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult BackPartCostByTimeId(PartBackCostInput input);
    }
}
