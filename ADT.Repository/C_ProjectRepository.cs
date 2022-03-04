using ADT.Models;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Repository
{
   public class C_ProjectRepository : BaseRepository<C_Project>, IC_ProjectRepository
    {
        /// <summary>
        /// 保存科目单元
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveProjectUnit(ProjectUnitInput input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (input.SubjectId > 0&& input.ProjectId>0)
                    {
                        if (input.UnitList != null && input.UnitList.Count > 0)
                        {
                            var updateList = input.UnitList.FindAll(item => item.UnitId > 0);
                            var InsertList = input.UnitList.FindAll(item => item.UnitId < 1);
                            if (updateList != null && updateList.Count > 0)
                            {
                                db.Updateable<C_Project_Unit>(updateList).UpdateColumns(item => new { item.UnitName}).ExecuteCommand();
                            }
                            if (InsertList != null && InsertList.Count > 0)
                            {
                                InsertList.ForEach(item =>
                                {
                                    item.SubjectId = input.SubjectId;
                                    item.ProjectId = input.ProjectId;
                                    item.Sort = 0;
                                });
                                db.Insertable<C_Project_Unit>(InsertList).ExecuteCommand();
                            }
                        }
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "保存成功";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "保存失败-" + er.Message;
                }
            }
            return rsg;
        }
    }
}
