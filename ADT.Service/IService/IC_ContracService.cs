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
        /// �½���ͬ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult AddUserContrac(ContracInput input);


        /// <summary>
        /// ���º�ͬ��Ϣ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveContrc(ContracInput input);

        /// <summary>
        /// �½�ǩԼ�˺�
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ResResult SettingAccount(C_Contrac_User user);


        /// <summary>
        /// ���ǩԼ�û�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveContracUser(C_Contrac_User input);


        /// <summary>
        /// ���濼������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveSubject(SubjectInput input);

        /// <summary>
        /// �������ͬ
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="thought"></param>
        /// <returns></returns>
        ResResult Audit(int Id, bool thought);

        /// <summary>
        /// �����Ӻ�ͬ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveChildContrac(ContracChildInput input);


        /// <summary>
        /// �˷�����
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        ResResult ContrcBackCostPart(C_Contrac_Child_ConstBack input);


        /// <summary>
        /// �˰�����
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult ContrcBackClass(C_Contrac_Child_RetrunClass input);


        /// <summary>
        ///ת������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult ContrcChangeClass(C_Contrac_Child_ChangeClass input);


        /// <summary>
        /// �Ӻ�ͬ�˷�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult ContrcBackCostPartAudit(string Contra_ChildNo, bool through, string uid);


        /// <summary>
        /// �Ӻ�ͬ�˰�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult ContrcBackClassAudit(string Contra_ChildNo, bool through, string uid);

        /// <summary>
        /// �Ӻ�ͬת��
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="through"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult ContrcChnageClassAudit(string Contra_ChildNo, bool through, string uid);


        /// <summary>
        /// �Ӻ�ͬ����
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult ContracCancel(string Contra_ChildNo,string uid);


        /// <summary>
        /// ����ͬɾ��
        /// </summary>
        /// <param name="contraNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult MasterContracCancel(string contraNo, string uid);


        /// <summary>
        /// �Ӻ�ͬȷ��
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult ContracChildConfig(string Contra_ChildNo, string uid);

        /// <summary>
        /// ȷ���տ�
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="through"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        ResResult ConfigCollection(int collectionId, bool through, string uid);


        /// <summary>
        /// ���浽���վ�
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveCollection(CollectionInput input);
    }
}
