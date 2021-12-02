using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ADT.Models;
using ADT.Models.ResModel;
using ADT.Models.InputModel;

namespace ADT.Service.IService
{
    public interface IC_ClueUserService : IBaseService<C_ClueUser>
    {
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveClueUser(ClueUserInput input);


        /// <summary>
        /// ת������
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult OwinClue(ClueUserInput input);


        /// <summary>
        /// ����������¼
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        ResResult SaveClueRecord(C_ClueUser_Record record);

        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        ResResult DeleteClueUser(int ID);
    }
}
