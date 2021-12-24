using ADT.Models;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Repository
{
    public class C_ClueUserRepository : BaseRepository<C_ClueUser>, IC_ClueUserRepository
    {
        /// <summary>
        /// 保存线索
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveClueUser(ClueUserInput input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (input.ClueId > 0)
                    {
                        C_ClueUser model = db.Queryable<C_ClueUser>().Where(n => n.ClueId == input.ClueId).First();
                        if (!string.IsNullOrEmpty(input.CC_Uid))
                            model.CC_Uid = input.CC_Uid;
                        if (!string.IsNullOrEmpty(input.CR_Uid))
                            model.CR_Uid = input.CR_Uid;
                        model.CampusId = input.CampusId;
                        model.Birthday = input.Birthday;
                        model.ContracRate = input.ContracRate;
                        model.Elder_Email = input.Elder_Email;
                        model.Elder_Identity = input.Elder_Identity;
                        model.Elder_Name = input.Elder_Name;
                        model.Elder_Phone = input.Elder_Phone;
                        model.Elder_Email2 = input.Elder_Email2;
                        model.Elder_Identity2 = input.Elder_Identity2;
                        model.Elder_Name2 = input.Elder_Name2;
                        model.Elder_Phone2 = input.Elder_Phone2;
                        model.Follow_Date = input.Follow_Date;
                        model.Follow_Plan = input.Follow_Plan;
                        model.Grade = input.Grade;
                        model.InSchool = input.InSchool;
                        model.Is_Visit = input.Is_Visit;
                        model.More_Contacts = input.More_Contacts;
                        model.Sex = input.Sex;
                        model.Soure = input.Soure;
                        model.Student_Email = input.Student_Email;
                        model.Student_Name = input.Student_Name;
                        model.Student_Phone = input.Student_Phone;
                        model.Student_Status = input.Student_Status;
                        model.Visit_Date = input.Visit_Date;
                        model.FirstTime = input.FirstTime;
                        model.Recommend = input.Recommend;
                        model.UpdateTime = DateTime.Now;
                        model.UpdateUid = input.CreateUid;
                        model.Student_Wechat = input.Student_Wechat;
                        model.Elder_Wechat = input.Elder_Wechat;
                        var ClueResult = db.Updateable<C_ClueUser>(model).ExecuteCommand();
                        db.Deleteable<C_ClueUser_Subject>().Where(m => m.ClueId == input.ClueId).ExecuteCommand();
                        if (input.SubjectIds != null && input.SubjectIds.Count > 0)
                        {
                            List<C_ClueUser_Subject> clueCourses = new List<C_ClueUser_Subject>();
                            input.SubjectIds.ForEach(item =>
                            {
                                clueCourses.Add(new C_ClueUser_Subject() { ClueId = model.ClueId, SubjectId = item, CreateTime = DateTime.Now, CreateUid = input.CreateUid });
                            });
                            db.Insertable(clueCourses.ToArray()).ExecuteCommand();
                        }

                    }
                    else
                    {
                        C_ClueUser model = new C_ClueUser();
                        model.ClueNo = DateTime.Now.ToString("yyyyMMddHHmmss");
                        model.CC_Uid = input.CC_Uid;
                        model.CR_Uid = input.CR_Uid;
                        model.CampusId = input.CampusId;
                        model.Birthday = input.Birthday;
                        model.ContracRate = input.ContracRate;
                        model.Elder_Email = input.Elder_Email;
                        model.Elder_Identity = input.Elder_Identity;
                        model.Elder_Name = input.Elder_Name;
                        model.Elder_Phone = input.Elder_Phone;
                        model.Elder_Email2 = input.Elder_Email2;
                        model.Elder_Identity2 = input.Elder_Identity2;
                        model.Elder_Name2 = input.Elder_Name2;
                        model.Elder_Phone2 = input.Elder_Phone2;
                        model.Follow_Date = input.Follow_Date;
                        model.Follow_Plan = input.Follow_Plan;
                        model.Grade = input.Grade;
                        model.InSchool = input.InSchool;
                        model.Is_Visit = input.Is_Visit;
                        model.More_Contacts = input.More_Contacts;
                        model.Sex = input.Sex;
                        model.Soure = input.Soure;
                        model.Student_Email = input.Student_Email;
                        model.Student_Name = input.Student_Name;
                        model.Student_Phone = input.Student_Phone;
                        model.Student_Status = input.Student_Status;
                        model.Visit_Date = input.Visit_Date;
                        model.FirstTime = input.FirstTime;
                        model.Recommend = input.Recommend;
                        model.CreateTime = DateTime.Now;
                        model.CreateUid = input.CreateUid;
                        model.UpdateTime = DateTime.Now;
                        model.UpdateUid = input.CreateUid;
                        model.Student_Wechat = input.Student_Wechat;
                        model.Elder_Wechat = input.Elder_Wechat;
                        var clueId = db.Insertable<C_ClueUser>(model).ExecuteReturnIdentity();
                        if (clueId > 0 && input.SubjectIds != null && input.SubjectIds.Count > 0)
                        {
                            List<C_ClueUser_Subject> clueCourses = new List<C_ClueUser_Subject>();
                            input.SubjectIds.ForEach(item =>
                            {
                                clueCourses.Add(new C_ClueUser_Subject() { ClueId = clueId, SubjectId = item, CreateTime = DateTime.Now, CreateUid = input.CreateUid });
                            });
                            db.Insertable(clueCourses.ToArray()).ExecuteCommand();
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

        /// <summary>
        /// 转移线索
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult OwinClue(ClueUserInput input) {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    C_ClueUser model = db.Queryable<C_ClueUser>().Where(n => n.ClueId == input.ClueId).First();
                    if (string.IsNullOrEmpty(model.Owin_CC_Uid))
                    {
                        //更改原状态
                        if (!string.IsNullOrEmpty(input.Owin_CC_Uid))
                            model.Owin_CC_Uid = input.Owin_CC_Uid;
                        model.Status = 1;//转移后，逻辑删除线索
                        var ClueResult = db.Updateable<C_ClueUser>(model).ExecuteCommand();
                        db.Queryable<C_ClueUser_Subject>().Where(it => it.ClueId == input.ClueId);
                        List<C_ClueUser_Subject> oldclueCourses = db.Queryable<C_ClueUser_Subject>().Where(m => m.ClueId == input.ClueId).ToList();


                        C_ClueUser clue = new C_ClueUser();
                        clue.ClueNo = DateTime.Now.ToString("yyyyMMddHHmmss");
                        clue.CC_Uid = input.Owin_CC_Uid;
                        clue.CR_Uid = model.CR_Uid;
                        clue.CampusId = model.CampusId;
                        clue.Birthday = model.Birthday;
                        clue.ContracRate = model.ContracRate;
                        clue.Elder_Email = model.Elder_Email;
                        clue.Elder_Identity = model.Elder_Identity;
                        clue.Elder_Name = model.Elder_Name;
                        clue.Elder_Phone = model.Elder_Phone;
                        clue.Elder_Email2 = model.Elder_Email2;
                        clue.Elder_Identity2 = model.Elder_Identity2;
                        clue.Elder_Name2 = model.Elder_Name2;
                        clue.Elder_Phone2 = model.Elder_Phone2;
                        clue.Follow_Date = model.Follow_Date;
                        clue.Follow_Plan = model.Follow_Plan;
                        clue.Grade = model.Grade;
                        clue.InSchool = model.InSchool;
                        clue.Is_Visit = model.Is_Visit;
                        clue.More_Contacts = model.More_Contacts;
                        clue.Sex = model.Sex;
                        clue.Soure = model.Soure;
                        clue.Student_Email = model.Student_Email;
                        clue.Student_Name = model.Student_Name;
                        clue.Student_Phone = model.Student_Phone;
                        clue.Student_Status = model.Student_Status;
                        clue.Visit_Date = model.Visit_Date;
                        clue.FirstTime = model.FirstTime;
                        clue.Recommend = model.Recommend;
                        clue.CreateTime = DateTime.Now;
                        clue.CreateUid = input.CreateUid;
                        clue.UpdateTime = DateTime.Now;
                        clue.UpdateUid = input.CreateUid;
                        clue.Student_Wechat = model.Student_Wechat;
                        clue.Elder_Wechat = model.Elder_Wechat;
                        clue.Defualt_ClueId = input.ClueId;//原来的线索id
                        int clueNewId = db.Insertable(clue).ExecuteReturnIdentity();
                        if (oldclueCourses != null && oldclueCourses.Count > 0)
                        {
                            List<C_ClueUser_Subject> owinClueCourses = new List<C_ClueUser_Subject>();
                            oldclueCourses.ForEach(item =>
                            {
                                item.Id = 0;
                                item.ClueId = clueNewId;
                                item.CreateTime = DateTime.Now;
                                item.CreateUid = input.CreateUid;
                                owinClueCourses.Add(item);
                            });
                            db.Insertable(owinClueCourses.ToArray()).ExecuteCommand();
                        }

                    }
                    else {
                        return new ResResult(){code=0,msg="线索已转移,无法连续转移" };
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "转移成功";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "转移失败-" + er.Message;
                }
            }
            return rsg;
        }

        /// <summary>
        /// 添加追踪记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public ResResult SaveClueRecord(C_ClueUser_Record record)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    C_ClueUser clue= db.Queryable<C_ClueUser>().Where(n => n.ClueId == record.ClueId).First();
                    //添加记录
                    record.CC_Uid = clue.CC_Uid;
                    record.CreateTime = DateTime.Now;
                    var recordId= db.Insertable<C_ClueUser_Record>(record).ExecuteReturnIdentity();
                    if (recordId > 0)
                    {
                        clue.Follow_Date = record.Follow_Date;
                        clue.Follow_Plan = record.Follow_Plan;
                        clue.ContracRate = record.ContracRate;
                        clue.Is_Visit = record.Is_Visit;
                        clue.Visit_Date = record.Visit_Date;
                        db.Updateable<C_ClueUser>(clue).ExecuteCommand();
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

        /// <summary>
        /// 删除线索
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ResResult DeleteClueUser(int ID)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                   db.BeginTran();
                   var result = db.Deleteable<C_ClueUser_Subject>().Where(p => p.ClueId == ID).ExecuteCommand();
                    if (result > 0)
                    {
                        db.Deleteable<C_ClueUser>().Where(p => p.ClueId == ID).ExecuteCommand();
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "删除成功";
                }
                catch (Exception er)
                {
                    db.RollbackTran();
                    rsg.msg = er.Message;
                }
            }
            return rsg;
        }
    }
}
