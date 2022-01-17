using ADT.Models;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Repository.IRepository
{
    public interface IC_ClueUserRepository : IBaseRepository<C_ClueUser>
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
        /// 保存线索跟踪记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        ResResult SaveClueRecord(ClueRecordInput record);


        /// <summary>
        /// 删除线程
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        ResResult DeleteClueUser(int ID);
    }
}
