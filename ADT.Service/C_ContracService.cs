using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Repository.IRepository;
using ADT.Service.IService;
using ADT.Models.ResModel;
using ADT.Models.InputModel;

namespace ADT.Service
{
    public class C_ContracService : BaseService<C_Contrac>, IC_ContracService
    {
        private IC_ContracRepository _contracRepository;
        public C_ContracService(IC_ContracRepository contracRepository) : base(contracRepository)
        {
            _contracRepository = contracRepository;
        }

        /// <summary>
        /// 添加合同
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult AddUserContrac(ContracInput input) {
            return _contracRepository.AddUserContrac(input);
        }

        /// <summary>
        /// 更新合同信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveContrc(ContracInput input) {
            return _contracRepository.SaveContrc(input);
        }

        /// <summary>
        /// 创建学生账号，同时创建抽词账号
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResResult SettingAccount(C_Contrac_User user) {
            return _contracRepository.SettingAccount(user);
        }

        /// <summary>
        /// 添加签约用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveContracUser(C_Contrac_User input) {
            return _contracRepository.SaveContracUser(input);
        }

        public ResResult SaveGiftUserTime(ContracInput input) {
            return _contracRepository.SaveGiftUserTime(input);
        }
        /// <summary>
        /// 保存项目科目
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveSubject(SubjectInput input) {
            return _contracRepository.SaveSubject(input);
        }

        /// <summary>
        /// 审核主合同
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="thought"></param>
        /// <returns></returns>
        public ResResult Audit(int Id, bool thought) {
            return _contracRepository.Audit(Id,thought);
        }

       /// <summary>
       /// 保存子合同
       /// </summary>
       /// <param name="input"></param>
       /// <returns></returns>
        public ResResult SaveChildContrac(ContracChildInput input) {
            return _contracRepository.SaveChildContrac(input);
        }

        /// <summary>
        /// 退费申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public ResResult ContrcBackCostPart(C_Contrac_Child_ConstBack input) {
            return _contracRepository.ContrcBackCostPart(input);
        }


        /// <summary>
        /// 退班申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult ContrcBackClass(C_Contrac_Child_RetrunClass input) {
            return _contracRepository.ContrcBackClass(input);
        }

        /// <summary>
        /// 转班
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult ContrcChangeClass(C_Contrac_Child_ChangeClass input) {
            return _contracRepository.ContrcChangeClass(input);
        }


        /// <summary>
        /// 子合同退费
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult ContrcBackCostPartAudit(string Contra_ChildNo, bool through, string uid)
        {
            return _contracRepository.ContrcBackCostPartAudit(Contra_ChildNo, through, uid);
        }

        /// <summary>
        /// 子合同退班
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult ContrcBackClassAudit(string Contra_ChildNo, bool through, string uid) {
            return _contracRepository.ContrcBackClassAudit(Contra_ChildNo, through, uid);
        }

        /// <summary>
        /// 子合同转班
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="through"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult ContrcChnageClassAudit(string Contra_ChildNo, bool through, string uid) {
            return _contracRepository.ContrcChnageClassAudit(Contra_ChildNo, through, uid);
        }

        /// <summary>
        /// 子合同撤销
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult ContracCancel(string Contra_ChildNo, string uid) {
            return _contracRepository.ContracCancel(Contra_ChildNo,uid);
        }

        /// <summary>
        /// 主合同删除
        /// </summary>
        /// <param name="contraNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult MasterContracCancel(string contraNo, string uid) {
            return _contracRepository.MasterContracCancel(contraNo, uid);
        }

        /// <summary>
        /// 子合同确认
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult ContracChildConfig(string Contra_ChildNo, string uid) {
            return _contracRepository.ContracChildConfig(Contra_ChildNo, uid);
        }

        /// <summary>
        /// 子合同确认变更
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult ContracChildChangeConfig(string Contra_ChildNo, string uid) {
            return _contracRepository.ContracChildChangeConfig(Contra_ChildNo, uid);
        }

        /// <summary>
        ///确认收款
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="through"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult ConfigCollection(int collectionId, bool through, string uid) {
            return _contracRepository.ConfigCollection(collectionId,through,uid);
        }

        /// <summary>
        /// 保存到款收据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveCollection(CollectionInput input) {
            return _contracRepository.SaveCollection(input);
        }

        /// <summary>
        /// 部分小时退费
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult BackPartCostByTimeId(PartBackCostInput input) {
            return _contracRepository.BackPartCostByTimeId(input);
        }
    }
}
