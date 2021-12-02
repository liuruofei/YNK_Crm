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
        /// 保存线索
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveClueUser(ClueUserInput input);


        /// <summary>
        /// 转移线索
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult OwinClue(ClueUserInput input);


        /// <summary>
        /// 保存线索记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        ResResult SaveClueRecord(C_ClueUser_Record record);

        /// <summary>
        /// 删除线索
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        ResResult DeleteClueUser(int ID);
    }
}
