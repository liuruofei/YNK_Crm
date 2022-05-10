using ADT.Models;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Repository.IRepository
{
   public interface IC_ProjectRepository : IBaseRepository<C_Project>
    {
        /// <summary>
        /// 保存科目单元
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveProjectUnit(ProjectUnitInput input);

        /// <summary>
        /// 保存单元考试局
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResResult SaveProjectUnitTime(ProjectUnitTimeInput input);
    }
}
