using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Models.ResModel;
using ADT.Models.InputModel;

namespace ADT.Service.IService
{
    public interface IC_ContracService : IBaseService<C_Contrac>
    {
        /// <summary>
        /// 新建合同
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult AddUserContrac(ContracInput input);


        /// <summary>
        /// 更新合同信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveContrc(ContracInput input);

        /// <summary>
        /// 新建签约账号
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


        /// <summary>
        /// 保存考试类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveSubject(SubjectInput input);

        /// <summary>
        /// 审核主合同
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="thought"></param>
        /// <returns></returns>
        ResResult Audit(int Id, bool thought);

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


        /// <summary>
        ///转班申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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
        /// 子合同撤销
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult ContracCancel(string Contra_ChildNo,string uid);


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
        /// 确认收款
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
    }
}
