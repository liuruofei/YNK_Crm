using ADT.Models;
using ADT.Models.Enum;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace ADT.Repository
{
    public class C_ContracRepository : BaseRepository<C_Contrac>, IC_ContracRepository
    {

        /// <summary>
        /// 更新子合同
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveChildContrac(ContracChildInput input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    C_Contrac contrac = db.Queryable<C_Contrac>().Where(it => it.ContraNo.Equals(input.ContraNo)).First();
                    if (!string.IsNullOrEmpty(input.Contra_ChildNo))
                    {
                        var model = db.Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(input.Contra_ChildNo)).First();
                        if (contrac == null)
                        {
                            contrac = db.Queryable<C_Contrac>().Where(it => it.ContraNo.Equals(model.ContraNo)).First();
                        }
                        model.ClassId = input.ClassId;
                        model.ContraRate = input.ContraRate;
                        model.IsPreferential = input.IsPreferential;
                        model.Contra_Property = input.Contra_Property;
                        model.Course_Time = 0;
                        if (input.Child_Detail != null && input.Child_Detail.Count > 0) {
                            input.Child_Detail.ForEach(item =>
                            {
                                model.Course_Time += item.Course_Time;
                            });
                        }
                        if (model.ClassId > 0)
                        {
                            model.Class_Course_Time = input.Class_Course_Time;
                        }
                        model.Original_Amount = input.Original_Amount;
                        model.Saler_Amount = Math.Round(input.Original_Amount * (input.ContraRate / 10), 2, MidpointRounding.AwayFromZero);
                        model.Discount_Amount = input.Original_Amount - input.Saler_Amount;
                        model.Added_Amount = input.Added_Amount;
                        model.Cycle = input.Cycle;
                        model.StudyStatus = input.StudyStatus;
                        model.StartTime = input.StartTime;
                        model.StudyMode = input.StudyMode;
                        model.SignIn_Data = DateTime.Now;
                        model.Remarks = input.Remarks;
                        model.ArrearageStatus = input.ArrearageStatus;
                        model.PresentTime = input.PresentTime;
                        model.UpdateTime = DateTime.Now;
                        model.Contrac_Child_Status = (int)ConstraChild_Status.Change;// 子合同变更状态
                        var contracChildId = db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                        if (contracChildId > 0)
                        {
                            List<C_Contrac_Child> listchild = db.Queryable<C_Contrac_Child>().Where(it => it.ContraNo.Equals(model.ContraNo)).ToList();
                            if (contrac != null && listchild != null)
                            {
                                contrac.Total_Amount = 0;
                                listchild.ForEach(item =>
                                {
                                    item.ContraRate = item.ContraRate < 1 ? 10 : item.ContraRate;
                                    contrac.Total_Amount += Math.Round(item.Original_Amount * (item.ContraRate / 10), 2, MidpointRounding.AwayFromZero);
                                    //减去额外优惠金额
                                    contrac.Total_Amount = contrac.Total_Amount - item.Added_Amount;
                                });
                            }

                            //重新更新总合同金额
                            db.Updateable<C_Contrac>(contrac).ExecuteCommand();
                            //保存当前子合同的明细
                            if (input.Child_Detail != null&& input.Child_Detail.Count>0)
                            {
                                List<C_Contrac_Child_Detail> addDetail = new List<C_Contrac_Child_Detail>();
                                List<C_Contrac_Child_Detail> updateDetail = new List<C_Contrac_Child_Detail>();
                                var updateList = db.Queryable<C_Contrac_Child_Detail>().Where(it => it.Contra_ChildNo == input.Contra_ChildNo).ToList();
                                if (input.StudyMode == (int)Study_Mode.OneOfOne && input.Child_Detail != null && input.Child_Detail.Count > 0)
                                {
                                    if (model.Pay_Stutas == (int)ConstraChild_Pay_Stutas.PayOk)
                                    {

                                        decimal detailSum = 0;
                                        input.Child_Detail.ForEach(con => {
                                            detailSum += con.Price;
                                        });
                                        //金额相同(只改动了科目)允许改动
                                        if ((detailSum*(model.ContraRate/10))!=model.Pay_Amount + model.Added_Amount)
                                        {
                                            rsg.code = 300;
                                            rsg.msg = "该合同为已支付合同,子合同项目最终价格合计已超过已支付价格,无法完成更改";
                                            return rsg;
                                        }
                                       var useCourTimeList= db.Queryable<C_User_CourseTime>().Where(con => con.Contra_ChildNo == model.Contra_ChildNo &&con.StudentUid==model.StudentUid).ToList();
                                        updateList.ForEach(con =>
                                        {
                                            var inputDeatail = input.Child_Detail.Find(cv => cv.Id == con.Id);
                                            //更改科目
                                            if (inputDeatail != null &&(con.SubjectId== inputDeatail.SubjectId||con.ProjectId != inputDeatail.ProjectId)) {
                                                db.Updateable<C_User_CourseTime>().SetColumns(vm => new C_User_CourseTime {SubjectId=inputDeatail.SubjectId,ProjectId= inputDeatail.ProjectId })
                                               .Where(vm => vm.Contra_ChildNo == model.Contra_ChildNo && vm.StudentUid == model.StudentUid && vm.SubjectId == con.SubjectId&&vm.ProjectId== con.ProjectId).ExecuteCommand();
                                            }
                                        });

                                    }
                                    input.Child_Detail.ForEach(item =>
                                    {
                                        item.Contra_ChildNo = model.Contra_ChildNo;
                                        item.ContraNo = model.ContraNo;
                                        item.StudentUid = model.StudentUid;
                                        item.UpdateTime = DateTime.Now;
                                        item.UpdateUid = contrac.CreateUid;
                                        if (updateList.Find(iv => iv.Id == item.Id) != null)
                                        {
                                            updateDetail.Add(item);
                                        }
                                        else
                                        {
                                            item.CreateTime = DateTime.Now;
                                            item.CreateUid = contrac.CreateUid;
                                            addDetail.Add(item);
                                        }
                                    });
                                    //新增明细
                                    if (addDetail.Count > 0)
                                        db.Insertable<C_Contrac_Child_Detail>(addDetail).ExecuteCommand();
                                    //修改明细
                                    if (updateDetail.Count > 0)
                                        db.Updateable<C_Contrac_Child_Detail>(updateDetail).ExecuteCommand();
                                }
                            }
                        }

                    }
                    else
                    {
                        C_Contrac_Child child = new C_Contrac_Child();
                        var maxchild = db.Queryable<C_Contrac_Child>().Where(it => it.ContraNo.Equals(contrac.ContraNo)).OrderBy(it => it.CreateTime, OrderByType.Desc).First();
                        var number = maxchild.Contra_ChildNo.Substring(maxchild.Contra_ChildNo.LastIndexOf("_") + 1, maxchild.Contra_ChildNo.Length - (maxchild.Contra_ChildNo.LastIndexOf("_") + 1));
                        child.Contra_ChildNo = contrac.ContraNo + "_" + (int.Parse(number) + 1);
                        child.ContraNo = contrac.ContraNo;
                        child.Contrac_Child_Status = (int)ConstraChild_Status.Created;
                        child.Pay_Stutas = (int)ConstraChild_Pay_Stutas.NoPay;
                        child.ContraRate = input.ContraRate;
                        child.Contra_Property = input.Contra_Property;
                        input.Child_Detail.ForEach(item =>
                        {
                            child.Course_Time += item.Course_Time;
                        });
                        child.ClassId = input.ClassId;
                        if (input.ClassId > 0)
                        {
                            child.Class_Course_Time = input.Class_Course_Time;
                        }
                        child.Cycle = input.Cycle;
                        child.Discount_Amount = input.Discount_Amount;
                        child.IsPreferential = input.IsPreferential;
                        child.Original_Amount = input.Original_Amount;
                        child.Pay_Amount = 0;
                        child.Saler_Amount = input.Saler_Amount;
                        child.Added_Amount = input.Added_Amount;
                        child.IsUseAddedAmount = 0;
                        child.SignIn_Data = DateTime.Now;
                        child.StartTime = input.StartTime;
                        child.StudentUid = contrac.StudentUid;
                        child.StudyMode = input.StudyMode;
                        child.StudyStatus = input.StudyStatus;
                        child.Status = 0;
                        child.CreateUid = contrac.CreateUid;
                        child.CreateTime = DateTime.Now;
                        child.UpdateTime = DateTime.Now;
                        child.CC_Uid = contrac.CC_Uid;
                        child.UpdateUid = contrac.CreateUid;
                        child.CampusId = contrac.CampusId;
                        child.PresentTime = input.PresentTime;
                        var contracChildId = db.Insertable<C_Contrac_Child>(child).ExecuteCommand();
                        if (contracChildId > 0)
                        {
                            List<C_Contrac_Child> listchild = db.Queryable<C_Contrac_Child>().Where(it => it.ContraNo.Equals(contrac.ContraNo)).ToList();
                            if (contrac != null && listchild != null)
                            {
                                contrac.Total_Amount = 0;
                                listchild.ForEach(item =>
                                {
                                    item.ContraRate = item.ContraRate < 1 ? 10 : item.ContraRate;
                                    contrac.Total_Amount += Math.Round(item.Original_Amount * (item.ContraRate / 10), 2, MidpointRounding.AwayFromZero);
                                    //减去额外优惠金额
                                    contrac.Total_Amount = contrac.Total_Amount - item.Added_Amount;
                                });
                            }
                            //重新更新总合同金额
                            db.Updateable<C_Contrac>(contrac).ExecuteCommand();
                            //保存当前子合同的明细
                            if (input.Child_Detail != null && input.StudyMode == (int)Study_Mode.OneOfOne && input.Child_Detail != null && input.Child_Detail.Count > 0)
                            {
                                input.Child_Detail.ForEach(item =>
                                {
                                    item.Contra_ChildNo = child.Contra_ChildNo;
                                    item.ContraNo = child.ContraNo;
                                    item.StudentUid = contrac.StudentUid;
                                    item.CreateTime = DateTime.Now;
                                    item.CreateUid = contrac.CreateUid;
                                    item.UpdateTime = DateTime.Now;
                                    item.UpdateUid = contrac.CreateUid;
                                });
                                //重新批量插入明细
                                db.Insertable<C_Contrac_Child_Detail>(input.Child_Detail).ExecuteCommand();
                            }
                        }

                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "保存子合同成功";
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
        /// 添加主合同
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult AddUserContrac(ContracInput input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (input.ClueId > 0 && input.StudentUid < 1)
                    {
                        //通过线索创建签约用户
                        C_ClueUser clue = db.Queryable<C_ClueUser>().Where(n => n.ClueId == input.ClueId).First();

                        C_Contrac_User User = db.Queryable<C_Contrac_User>().Where(n => n.Student_Name == clue.Student_Name).First();
                        int studentId = 0;
                        if (User != null)
                        {
                            studentId = User.StudentUid;
                        }
                        else
                        {
                            User = new C_Contrac_User();
                            User.Student_No = input.Student_No;
                            User.Student_Name = input.Student_Name;
                            User.CampusId = input.CampusId;
                            User.Student_Phone = input.Student_Phone;
                            User.Birthday = input.Birthday;
                            User.Student_Wechat = input.Student_Wechat;
                            User.CC_Uid = clue.CC_Uid;
                            User.ContactFamily = clue.Elder_Name + clue.Elder_Phone;
                            User.Sex = input.Sex;
                            User.InSchool = input.InSchool;
                            User.Elder_Name = input.Elder_Name;
                            User.Elder_Phone = input.Elder_Phone;
                            User.Elder_Wechat = input.Elder_Wechat;
                            User.Elder2_Name = input.Elder2_Name;
                            User.Elder2_Phone = input.Elder2_Phone;
                            User.Elder2_Wechat = input.Elder2_Wechat;
                            User.InSchool = input.InSchool;
                            User.Grade = input.Grade;
                            User.Amount = 0;     //由财务确认到帐号更新余额
                            User.Course_Time = 0; //课时(如果只到帐部分,课时由顾问手动更改，如果全部到账，财务确认后就按合同课时)
                            User.CreateTime = DateTime.Now;
                            User.CreateUid = input.CreateUid;
                            User.UpdateTime = DateTime.Now;
                            User.UpdateUid = input.CreateUid;
                            studentId = db.Insertable<C_Contrac_User>(User).ExecuteReturnIdentity();
                        }
                        if (studentId > 0)
                        {
                            //创建合同
                            C_Contrac contrac = new C_Contrac();
                            contrac.CampusId = input.CampusId;
                            if (input.ClueId > 0)
                            {
                                contrac.ClueId = input.ClueId;
                                //更新线索状态
                                db.Updateable<C_ClueUser>().SetColumns(item =>
                                    new C_ClueUser { Status=1, UpdateTime = DateTime.Now, UpdateUid = input.CreateUid }).Where(item => item.ClueId == input.ClueId).ExecuteCommand();

                            }
                            contrac.Constra_Status = (int)Constra_Status.ConfirmationIng;
                            contrac.Pay_Status = (int)Constra_Pay_Status.NoPay;
                            contrac.ContraCenterId = input.ContraCenterId;
                            contrac.StudentUid = studentId;
                            contrac.CC_Uid = clue.CC_Uid;
                            contrac.ContraNo = input.ContraNo;
                            input.childList.ForEach(item =>
                            {
                                item.ContraRate = item.ContraRate < 1 ? 10 : item.ContraRate;
                                contrac.Total_Amount += Math.Round(item.Original_Amount * (item.ContraRate / 10), 2, MidpointRounding.AwayFromZero);
                                //减去额外优惠金额
                                contrac.Total_Amount = contrac.Total_Amount - item.Added_Amount;
                            });
                            contrac.Start_Time = input.Start_Time;
                            contrac.End_Time = input.End_Time;
                            contrac.CreateTime = DateTime.Now;
                            contrac.CreateUid = input.CreateUid;
                            var contracId = db.Insertable<C_Contrac>(contrac).ExecuteReturnIdentity();
                            if (contracId > 0 && input.childList != null)
                            {
                                input.childList.ForEach(childInput =>
                                {
                                    C_Contrac_Child child = new C_Contrac_Child();
                                    child.Pay_Stutas = (int)ConstraChild_Pay_Stutas.NoPay;
                                    child.CC_Uid = contrac.CC_Uid;
                                    child.ClassId = childInput.ClassId;
                                    child.StudentUid = studentId;
                                    child.Contrac_Child_Status = (int)ConstraChild_Status.Created;
                                    child.ContraNo = contrac.ContraNo;
                                    child.ContraRate = childInput.ContraRate;
                                    child.Contra_ChildNo = childInput.Contra_ChildNo;
                                    child.IsPreferential = childInput.IsPreferential;
                                    child.Contra_Property = (int)Contra_Property.Create;
                                    if (childInput.Child_Detail != null && childInput.Child_Detail.Count > 0)
                                    {
                                        childInput.Child_Detail.ForEach(item =>
                                        {
                                            child.Course_Time += item.Course_Time;
                                        });
                                    }
                                    child.Class_Course_Time = childInput.Class_Course_Time;
                                    child.Original_Amount = childInput.Original_Amount;
                                    child.Saler_Amount = Math.Round(childInput.Original_Amount * (childInput.ContraRate / 10), 2, MidpointRounding.AwayFromZero);
                                    child.Discount_Amount = childInput.Original_Amount - childInput.Saler_Amount;
                                    child.Added_Amount = childInput.Added_Amount;
                                    child.IsUseAddedAmount = 0;
                                    child.Cycle = childInput.Cycle;
                                    child.StudyStatus = childInput.StudyStatus;
                                    child.StartTime = childInput.StartTime;
                                    child.StudyMode = childInput.StudyMode;
                                    child.SignIn_Data = DateTime.Now;
                                    child.Remarks = childInput.Remarks;
                                    child.ArrearageStatus = childInput.ArrearageStatus;
                                    child.PresentTime = childInput.PresentTime;
                                    child.CreateUid = input.CreateUid;
                                    child.CreateTime = DateTime.Now;
                                    child.UpdateUid = input.CreateUid;
                                    child.UpdateTime = DateTime.Now;
                                    child.CampusId = input.CampusId;
                                    var contracChildId = db.Insertable<C_Contrac_Child>(child).ExecuteReturnIdentity();
                                    if (contracChildId > 0 && childInput.Child_Detail != null && childInput.Child_Detail.Count > 0)
                                    {
                                        childInput.Child_Detail.ForEach(item =>
                                        {
                                            item.ContraNo = contrac.ContraNo;
                                            item.Contra_ChildNo = child.Contra_ChildNo;
                                            item.StudentUid = studentId;
                                            item.CreateUid = input.CreateUid;
                                            item.UpdateUid = input.CreateUid;
                                            item.CreateTime = DateTime.Now;
                                            item.UpdateTime = DateTime.Now;
                                        });
                                        db.Insertable(childInput.Child_Detail).ExecuteCommand();
                                    }
                                });
                            }
                        }
                    }
                    else
                    {
                        C_Contrac_User user = db.Queryable<C_Contrac_User>().Where(n => n.StudentUid == input.StudentUid).First();
                        //已有学员创建合同
                        C_Contrac contrac = new C_Contrac();
                        contrac.CampusId = input.CampusId;
                        if (input.ClueId > 0) {
                            contrac.ClueId = input.ClueId;
                            //更新线索状态
                            db.Updateable<C_ClueUser>().SetColumns(item =>
                                new C_ClueUser { Status = 1, UpdateTime = DateTime.Now, UpdateUid = input.CreateUid }).Where(item => item.ClueId == input.ClueId).ExecuteCommand();
                        }
                        contrac.Constra_Status = (int)Constra_Status.ConfirmationIng;
                        contrac.Pay_Status = (int)Constra_Pay_Status.NoPay;
                        contrac.ContraCenterId = input.ContraCenterId;
                        contrac.StudentUid = input.StudentUid;
                        contrac.CC_Uid = user.CC_Uid;
                        contrac.ContraNo = input.ContraNo;
                        input.childList.ForEach(item =>
                        {
                            item.ContraRate = item.ContraRate < 1 ? 10 : item.ContraRate;
                            contrac.Total_Amount += Math.Round(item.Original_Amount * (item.ContraRate / 10), 2, MidpointRounding.AwayFromZero);
                            //减去额外优惠金额
                            contrac.Total_Amount = contrac.Total_Amount - item.Added_Amount;
                        });
                        contrac.Start_Time = input.Start_Time;
                        contrac.End_Time = input.End_Time;
                        contrac.CreateTime = DateTime.Now;
                        contrac.CreateUid = input.CreateUid;
                        var contracId = db.Insertable<C_Contrac>(contrac).ExecuteReturnIdentity();
                        if (contracId > 0 && input.childList != null)
                        {
                            input.childList.ForEach(childInput =>
                            {
                                C_Contrac_Child child = new C_Contrac_Child();
                                child.Pay_Stutas = (int)ConstraChild_Pay_Stutas.NoPay;
                                child.CC_Uid = contrac.CC_Uid;
                                child.ClassId = childInput.ClassId;
                                child.StudentUid = input.StudentUid;
                                child.Contrac_Child_Status = (int)ConstraChild_Status.Created;
                                child.ContraNo = contrac.ContraNo;
                                child.ContraRate = childInput.ContraRate;
                                child.IsPreferential = childInput.IsPreferential;
                                child.Contra_ChildNo = childInput.Contra_ChildNo;
                                child.Contra_Property = (int)Contra_Property.Renew;//续费
                                if (childInput.Child_Detail != null && childInput.Child_Detail.Count > 0)
                                {
                                    childInput.Child_Detail.ForEach(item =>
                                    {
                                        child.Course_Time += item.Course_Time;
                                    });
                                }
                                child.Class_Course_Time = childInput.Class_Course_Time;
                                child.Original_Amount = childInput.Original_Amount;
                                child.Saler_Amount = Math.Round(childInput.Original_Amount * (childInput.ContraRate / 10), 2, MidpointRounding.AwayFromZero);
                                child.Discount_Amount = childInput.Original_Amount - childInput.Saler_Amount;
                                child.Added_Amount = childInput.Added_Amount;
                                child.IsUseAddedAmount = 0;
                                child.Cycle = childInput.Cycle;
                                child.StudyStatus = childInput.StudyStatus;
                                child.StartTime = childInput.StartTime;
                                child.StudyMode = childInput.StudyMode;
                                child.SignIn_Data = DateTime.Now;
                                child.PresentTime = childInput.PresentTime;
                                child.Remarks = childInput.Remarks;
                                child.ArrearageStatus = childInput.ArrearageStatus;
                                child.CreateUid = input.CreateUid;
                                child.CreateTime = DateTime.Now;
                                child.UpdateUid = input.CreateUid;
                                child.UpdateTime = DateTime.Now;
                                child.CampusId = input.CampusId;
                                var contracChildId = db.Insertable<C_Contrac_Child>(child).ExecuteReturnIdentity();
                                if (contracChildId > 0 && childInput.Child_Detail != null && childInput.Child_Detail.Count > 0)
                                {
                                    childInput.Child_Detail.ForEach(item =>
                                    {
                                        item.ContraNo = contrac.ContraNo;
                                        item.Contra_ChildNo = child.Contra_ChildNo;
                                        item.StudentUid = input.StudentUid;
                                        item.CreateUid = input.CreateUid;
                                        item.UpdateUid = input.CreateUid;
                                        item.CreateTime = DateTime.Now;
                                        item.UpdateTime = DateTime.Now;
                                    });
                                    db.Insertable<C_Contrac_Child_Detail>(childInput.Child_Detail).ExecuteCommand();
                                }
                            });
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
        /// 更新主合同
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveContrc(ContracInput input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    C_Contrac contrac = db.Queryable<C_Contrac>().Where(it => it.ContraNo.Equals(input.ContraNo)).First();
                    if (!string.IsNullOrEmpty(input.ContraNo))
                    {
                        contrac.ContraCenterId = input.ContraCenterId;
                        contrac.CampusId = input.CampusId;
                    }
                    contrac.UpdateTime = DateTime.Now;
                    contrac.UpdateUid = input.CreateUid;
                    db.Updateable<C_Contrac>(contrac).ExecuteCommand();
                    C_Contrac_User user = db.Queryable<C_Contrac_User>().Where(c => c.StudentUid == contrac.StudentUid).First();
                    if (user != null) {
                        user.Student_Name = input.Student_Name;
                        user.Student_Phone = input.Student_Phone;
                        user.Student_Wechat = input.Student_Wechat;
                        user.InSchool = input.InSchool;
                        user.Sex = input.Sex;
                        user.CampusId = input.CampusId;
                        user.Birthday = input.Birthday;
                        user.Elder_Name = input.Elder_Name;
                        user.Elder_Phone = input.Elder_Phone;
                        user.Elder_Wechat = input.Elder_Wechat;
                        user.Grade = input.Grade;
                        user.UpdateTime = DateTime.Now;
                        user.UpdateUid = input.CreateUid;
                        db.Updateable<C_Contrac_User>(user).ExecuteCommand();
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "更新合同成功";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "更新失败-" + er.Message;
                }
            }
            return rsg;
        }

        /// <summary>
        /// 添加签约用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveContracUser(C_Contrac_User input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                int result = 0;
                try
                {
                    if (input.StudentUid > 0)
                    {
                        var info = db.Queryable<C_Contrac_User>().Where(n => n.StudentUid == input.StudentUid).First();
                        info.UpdateTime = DateTime.Now;
                        info.UpdateUid = input.CreateUid;
                        info.Sex = input.Sex;
                        info.Student_Name = input.Student_Name;
                        info.Student_Phone = input.Student_Phone;
                        info.CampusId = input.CampusId;
                        info.ContactFamily = input.ContactFamily;
                        info.Grade = input.Grade;
                        info.InSchool = input.InSchool;
                        info.Remarks = input.Remarks;
                        info.Elder_Name = input.Elder_Name;
                        info.Elder_Phone = input.Elder_Phone;
                        info.Elder_Wechat = input.Elder_Wechat;
                        info.Elder2_Name = input.Elder2_Name;
                        info.Elder2_Phone = input.Elder2_Phone;
                        info.Elder2_Wechat = input.Elder2_Wechat;
                        result = db.Updateable(info).ExecuteCommand();

                    }
                    else
                    {
                        input.CreateTime = DateTime.Now;
                        input.UpdateUid = input.CreateUid;
                        input.UpdateTime = DateTime.Now;
                        result = db.Insertable(input).ExecuteCommand();
                    }
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "保存成功";
                    }
                }
                catch (Exception er)
                {
                    rsg.msg = "保存失败-" + er.Message;
                }
            }
            return rsg;
        }

        /// <summary>
        /// 创建账号
        /// </summary>
        /// <returns></returns>
        public ResResult SettingAccount(C_Contrac_User user)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    var prm = db.Ado.GetParameters(new
                    {
                        StudentUid = user.StudentUid,
                        Student_Account = user.Student_Account,
                        Student_Pwd = user.Student_Pwd,
                        ReSult = ""
                    });
                    var studentUid = new SugarParameter("@StudentUid", user.StudentUid);
                    var student_Account = new SugarParameter("@Student_Account", user.Student_Account);
                    var student_Pwd = new SugarParameter("@Student_Pwd", user.Student_Pwd);
                    var outResult = new SugarParameter("@ReSult", null, true);
                    db.Ado.UseStoredProcedure().GetDataTable("dbo.Pro_SetAccount", studentUid, student_Account, student_Pwd, outResult);
                    if (outResult.Value.ToString().Contains("SUCCESS"))
                    {
                        rsg.code = 200;
                        rsg.msg = "保存成功";
                    }
                    else
                    {
                        rsg.code = 0;
                        rsg.msg = outResult.Value.ToString();
                    }
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
        /// 审核主合同
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="thought"></param>
        /// <returns></returns>
        public ResResult Audit(int Id, bool thought) {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    var contrca = db.Queryable<C_Contrac>().Where(it => it.ContracId == Id).First();
                    List<C_Contrac_Child> listchild = db.Queryable<C_Contrac_Child>().Where(c => c.ContraNo == contrca.ContraNo).ToList();
                    if (contrca.Constra_Status == (int)Constra_Status.ConfirmationIng)
                    {
                        int constraStatus = 0;
                        if (thought)
                        {
                            constraStatus = (int)Constra_Status.Schoolmaster;
                            int childconstraStatus=(int)ConstraChild_Status.Confirmationed;
                            db.Updateable<C_Contrac_Child>(listchild).SetColumns(it => new C_Contrac_Child { Contrac_Child_Status = childconstraStatus }).ExecuteCommand();
                        }
                        else
                        {
                            constraStatus = (int)Constra_Status.SchoolmasterReject;
                            int childconstraStatus = (int)ConstraChild_Status.ConfirmationedReject;
                            db.Updateable<C_Contrac_Child>(listchild).SetColumns(it => new C_Contrac_Child { Contrac_Child_Status = childconstraStatus }).ExecuteCommand();
                        }
                        db.Updateable<C_Contrac>().SetColumns(it => new C_Contrac { Constra_Status = constraStatus })
                        .Where(it => it.ContracId == Id).ExecuteCommand();
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "审核成功";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "审核失败-" + er.Message;
                }
            }
            return rsg;
        }

        /// <summary>
        /// 保存科目
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveSubject(SubjectInput input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (input.SubjectId > 0)
                    {
                        C_Subject sub = db.Queryable<C_Subject>().Where(f => f.SubjectId == input.SubjectId).First();
                        sub.SubjectName = input.SubjectName;
                        sub.Lvel1Price = input.Lvel1Price;
                        sub.Lvel2Price = input.Lvel2Price;
                        sub.Lvel3Price = input.Lvel3Price;
                        sub.Lvel4Price = input.Lvel4Price;
                        sub.UnitCourse_Time = input.UnitCourse_Time;
                        sub.Description = input.Description;
                        sub.UpdateTime = DateTime.Now;
                        sub.UpdateUid = input.CreateUid;
                        sub.CampusId = input.CampusId;
                        var result = db.Updateable<C_Subject>(sub).ExecuteCommand();
                        if (input.ProjectList != null && input.ProjectList.Count > 0)
                        {
                            var updateList = input.ProjectList.FindAll(item => item.ProjectId > 0);
                            var InsertList = input.ProjectList.FindAll(item => item.ProjectId < 1);
                            if (updateList != null && updateList.Count > 0)
                            {
                                db.Updateable<C_Project>(updateList).UpdateColumns(item => new { item.ProjectName }).ExecuteCommand();
                            }
                            if (InsertList != null && InsertList.Count > 0)
                            {
                                InsertList.ForEach(item =>
                                {
                                    item.SubjectId = sub.SubjectId;
                                    item.CreateUid = input.CreateUid;
                                    item.UpdateTime = DateTime.Now;
                                    item.CreateTime = DateTime.Now;
                                    item.UpdateUid = input.CreateUid;
                                });
                                db.Insertable<C_Project>(InsertList).ExecuteCommand();
                            }
                        }
                    }
                    else
                    {
                        C_Subject sub = new C_Subject();
                        sub.SubjectName = input.SubjectName;
                        sub.Lvel1Price = input.Lvel1Price;
                        sub.Lvel2Price = input.Lvel2Price;
                        sub.Lvel3Price = input.Lvel3Price;
                        sub.Lvel4Price = input.Lvel4Price;
                        sub.UnitCourse_Time = input.UnitCourse_Time;
                        sub.Description = input.Description;
                        sub.UpdateTime = DateTime.Now;
                        sub.CreateTime = DateTime.Now;
                        sub.CampusId = input.CampusId;
                        var result = db.Insertable<C_Subject>(sub).ExecuteReturnIdentity();
                        if (result > 0 && input.ProjectList != null && input.ProjectList.Count > 0)
                        {
                            input.ProjectList.ForEach(item =>
                            {
                                item.CreateUid = input.CreateUid;
                                item.UpdateTime = DateTime.Now;
                                item.CreateTime = DateTime.Now;
                                item.UpdateUid = input.CreateUid;
                                item.SubjectId = result;
                            });
                            db.Insertable<C_Project>(input.ProjectList).ExecuteCommand();
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
        /// 退费申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult ContrcBackCostPart(C_Contrac_Child_ConstBack input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (!string.IsNullOrEmpty(input.Contra_ChildNo))
                    {
                        var inputApply = db.Queryable<C_Contrac_Child_ConstBack>().Where(ite => ite.Contra_ChildNo.Equals(input.Contra_ChildNo)).First();
                        var model = db.Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(input.Contra_ChildNo)).First();
                        if (inputApply != null)
                        {
                            db.Updateable<C_Contrac_Child_ConstBack>(input).ExecuteCommand();
                        }
                        else
                        {
                            input.StudentUid = model.StudentUid;
                            db.Insertable<C_Contrac_Child_ConstBack>(input).ExecuteCommand();
                        }
                        model.UpdateTime = DateTime.Now;
                        model.Contrac_Child_Status = (int)ConstraChild_Status.BackPay;// 子合同变更状态
                        db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                        rsg.code = 200;
                        rsg.msg = "已经申请合同退款";
                    }
                    else
                    {
                        rsg.code = 300;
                        rsg.msg = "缺少合同编码参数";
                    }
                    db.CommitTran();
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "申请合同退款失败-" + er.Message;
                }
            }
            return rsg;
        }


        /// <summary>
        /// 退班申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult ContrcBackClass(C_Contrac_Child_RetrunClass input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (!string.IsNullOrEmpty(input.Contra_ChildNo))
                    {
                        var inputApply = db.Queryable<C_Contrac_Child_RetrunClass>().Where(ite => ite.Contra_ChildNo.Equals(input.Contra_ChildNo)).First();
                        var model = db.Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(input.Contra_ChildNo)).First();
                        if (inputApply != null)
                        {
                            db.Updateable<C_Contrac_Child_RetrunClass>(input).ExecuteCommand();
                        }
                        else
                        {
                            input.StudentUid = model.StudentUid;
                            db.Insertable<C_Contrac_Child_RetrunClass>(input).ExecuteCommand();
                        }
                        model.UpdateTime = DateTime.Now;
                        model.Contrac_Child_Status = (int)ConstraChild_Status.RetrunClass;// 子合同变更状态
                        db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                        rsg.code = 200;
                        rsg.msg = "已经申请退班";
                    }
                    else
                    {
                        rsg.code = 300;
                        rsg.msg = "缺少合同编码参数";
                    }
                    db.CommitTran();
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "申请退班失败-" + er.Message;
                }
            }
            return rsg;
        }


        /// <summary>
        /// 转班申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult ContrcChangeClass(C_Contrac_Child_ChangeClass input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (!string.IsNullOrEmpty(input.Contra_ChildNo))
                    {
                        var inputApply = db.Queryable<C_Contrac_Child_ChangeClass>().Where(ite => ite.Contra_ChildNo.Equals(input.Contra_ChildNo)).First();
                        var model = db.Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(input.Contra_ChildNo)).First();
                        if (inputApply != null)
                        {
                            inputApply.StartTime = input.StartTime;
                            inputApply.ChangeClassId = input.ChangeClassId;
                            inputApply.Class_Course_Time = input.Class_Course_Time;
                            inputApply.ContraRate = input.ContraRate;
                            inputApply.Contra_Property = input.Contra_Property;
                            inputApply.Cycle = input.Cycle;
                            inputApply.Discount_Amount = input.Discount_Amount;
                            inputApply.IsPreferential = input.IsPreferential;
                            inputApply.OldClassId = input.OldClassId;
                            inputApply.Original_Amount = input.Original_Amount;
                            inputApply.Remarks = input.Remarks;
                            inputApply.Saler_Amount = input.Saler_Amount;
                            inputApply.StudyMode = input.StudyMode;
                            inputApply.StudyStatus = input.StudyStatus;
                            db.Updateable<C_Contrac_Child_ChangeClass>(inputApply).ExecuteCommand();
                        }
                        else
                        {
                            input.StudentUid = model.StudentUid;
                            input.CreateTime = DateTime.Now;
                            input.CreateUid = model.CreateUid;
                            db.Insertable<C_Contrac_Child_ChangeClass>(input).ExecuteCommand();
                        }
                        model.UpdateTime = DateTime.Now;
                        model.Contrac_Child_Status = (int)ConstraChild_Status.ChangeClass;// 子合同变更状态
                        db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                        rsg.code = 200;
                        rsg.msg = "已经申请转班";
                    }
                    else
                    {
                        rsg.code = 300;
                        rsg.msg = "缺少合同编码参数";
                    }
                    db.CommitTran();
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "申请转班失败-" + er.Message;
                }
            }
            return rsg;
        }


        /// <summary>
        /// 子合同退费审核
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult ContrcBackCostPartAudit(string Contra_ChildNo, bool through, string uid)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (!string.IsNullOrEmpty(Contra_ChildNo))
                    {
                        var inputApply = db.Queryable<C_Contrac_Child_ConstBack>().Where(ite => ite.Contra_ChildNo.Equals(Contra_ChildNo)).First();
                        var model = db.Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(Contra_ChildNo)).First();
                        var contrac = db.Queryable<C_Contrac>().Where(it => it.ContraNo.Equals(model.ContraNo)).First();
                        C_Contrac_User user = db.Queryable<C_Contrac_User>().Where(iv => iv.StudentUid == contrac.StudentUid).First();
                        if (model.Contrac_Child_Status == (int)ConstraChild_Status.BackPayOk)
                        {
                            rsg.msg = "该合同已经退费到用户余额！无法重复操作";
                            rsg.code = 300;
                            return rsg;
                        }
                        if (model.Pay_Amount > 0)
                        {
                            if (!through)
                            {
                                //更新子合同状态
                                model.UpdateTime = DateTime.Now;
                                model.UpdateUid = uid;
                                model.Contrac_Child_Status = (int)ConstraChild_Status.BackPayReject;// 子合同变更状态
                                db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                                rsg.msg = "退费申请已驳回";
                                rsg.code = 200;
                                return rsg;
                            }
                            else
                            {
                                //查询用户已用课时
                                List<UserCourseTimeModel> userCouses = db.Queryable<C_User_CourseTime, C_Class, C_Subject>((cou, c, sub) => new Object[] {
                                JoinType.Left,cou.ClassId==c.ClassId,JoinType.Left,cou.SubjectId==sub.SubjectId
                            }).Where(cou => cou.StudentUid == contrac.StudentUid && cou.Contra_ChildNo.Equals(model.Contra_ChildNo))
                                .Select<UserCourseTimeModel>((cou, c, sub) => new UserCourseTimeModel
                                {
                                    ClassId = cou.ClassId,
                                    ClassPrice = c.Price,
                                    Class_Course_Time = cou.Class_Course_Time,
                                    Class_Course_UseTime = cou.Class_Course_UseTime
                                ,
                                    Contra_ChildNo = cou.Contra_ChildNo,
                                    Course_Time = cou.Course_Time,
                                    Course_UseTime = cou.Course_UseTime,
                                    Level = cou.Level,
                                    Lvel1Price = sub.Lvel1Price,
                                    Lvel2Price = sub.Lvel2Price,
                                    Lvel3Price = sub.Lvel3Price,
                                    Lvel4Price = sub.Lvel4Price,
                                    StudentUid = cou.StudentUid,
                                    SubjectId = cou.SubjectId
                                }).ToList();
                                //计算已用课时
                                decimal UnUsePrice = 0;
                                float UnUseCourseTime = 0;
                                float UnUseClassTime = 0;
                                if (userCouses != null && userCouses.Count > 0)
                                {
                                    userCouses.ForEach(iv =>
                                    {
                                        if (iv.ClassId > 0)
                                        {
                                            UnUseClassTime += iv.Class_Course_Time - iv.Class_Course_UseTime;
                                            var unitPrice = float.Parse(iv.ClassPrice.ToString()) / iv.Class_Course_Time;
                                            UnUsePrice += Convert.ToDecimal((iv.Class_Course_Time - iv.Class_Course_UseTime)*unitPrice) ;
                                        }
                                        else
                                        {
                                            UnUseCourseTime += iv.Course_Time - iv.Course_UseTime;
                                            switch (iv.Level)
                                            {
                                                case 1:
                                                    UnUsePrice += iv.Lvel1Price * Convert.ToDecimal(iv.Course_Time - iv.Course_UseTime)*(model.ContraRate/10);
                                                    break;
                                                case 2:
                                                    UnUsePrice += iv.Lvel2Price * Convert.ToDecimal(iv.Course_Time - iv.Course_UseTime) * (model.ContraRate / 10);
                                                    break;
                                                case 3:
                                                    UnUsePrice += iv.Lvel3Price * Convert.ToDecimal(iv.Course_Time - iv.Course_UseTime) * (model.ContraRate / 10);
                                                    break;
                                                case 4:
                                                    UnUsePrice += iv.Lvel4Price * Convert.ToDecimal(iv.Course_Time - iv.Course_UseTime) * (model.ContraRate / 10);
                                                    break;
                                            }
                                        }
                                    });
                                }
                                //按原价退款
                                if (inputApply.IsOrg)
                                {
                                    UnUsePrice = UnUsePrice - model.Added_Amount;//未用课时兑换成金额后扣除子合同额外优惠金额
                                    user.Amount = user.Amount + UnUsePrice;//用户总金额=(用户账户余额+子合同未用课时金额)
                                }
                                //按输入金额退款
                                if (inputApply.BackCost > 0 && inputApply.IsOrg == false)
                                {
                                    user.Amount = user.Amount + inputApply.BackCost;//用户总金额=(用户账户余额+协商金额+未用课时金额)
                                }
                                //更新用户基础数据
                                db.Updateable<C_Contrac_User>(user).ExecuteCommand();

                                int childCount = db.Queryable<C_Contrac_Child>().Where(it => it.ContraNo == model.ContraNo).Count();
                                if (childCount == 1)
                                {
                                    contrac.Constra_Status = (int)Constra_Status.BackPayOk;//退费成功
                                }
                                else if (childCount > 1) {
                                    //更新合同总金额
                                    contrac.Total_Amount = contrac.Total_Amount - model.Saler_Amount;
                                }
                                db.Updateable<C_Contrac>(contrac).ExecuteCommand();

                                //重新计算出已使用的课时
                                var useWorkList = db.Queryable<C_Course_Work>().Where(it => it.Contra_ChildNo == model.Contra_ChildNo && it.StudentUid == model.StudentUid&&it.Work_Stutas==1).ToList();
                                var UserCourseTimeList = db.Queryable<C_User_CourseTime>().Where(it => it.Contra_ChildNo == model.Contra_ChildNo && it.StudentUid == model.StudentUid).ToList();
                                if (UserCourseTimeList != null && UserCourseTimeList.Count > 0) {
                                    UserCourseTimeList.ForEach(con =>
                                    {
                                        con.Class_Course_Time = 0;
                                        con.Course_UseTime = 0;
                                        var findWork = useWorkList.FindAll(it => it.SubjectId == con.SubjectId && it.Contra_ChildNo == con.Contra_ChildNo && it.ProjectId == con.ProjectId);
                                        if (findWork != null && findWork.Count > 0) {
                                            findWork.ForEach(work =>
                                            {
                                                con.Course_UseTime+= work.CourseTime;
                                            });
                                        }
                                        con.Course_Time = con.Course_UseTime;
                                    });
                                    //清零课时
                                    db.Updateable<C_User_CourseTime>(UserCourseTimeList).ExecuteCommand();
                                }

                                //清除后面的课时
                                if (model.StudyMode == 1) {
                                    db.Deleteable<C_Course_Work>().Where(con=>con.Work_Stutas==0&&con.StudentUid==con.StudentUid&&con.Contra_ChildNo==model.Contra_ChildNo).ExecuteCommand();
                                }

                                //更新子合同状态
                                model.UpdateTime = DateTime.Now;
                                model.Contrac_Child_Status = (int)ConstraChild_Status.BackPayOk;// 子合同变更状态
                                model.Pay_Stutas = (int)ConstraChild_Pay_Stutas.PayBack;
                                db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                            }
                        }
                        else
                        {
                            rsg.msg = "该合同付款费为0,无法退款";
                            rsg.code = 300;
                            return rsg;
                        }
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "合同费用已退到用户余额";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "操作退款失败-" + er.Message;
                }
            }
            return rsg;
        }


        /// <summary>
        /// 子合同退班
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult ContrcBackClassAudit(string Contra_ChildNo, bool through, string uid)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();

                    if (!string.IsNullOrEmpty(Contra_ChildNo))
                    {
                        var inputApply = db.Queryable<C_Contrac_Child_RetrunClass>().Where(ite => ite.Contra_ChildNo.Equals(Contra_ChildNo)).First();
                        var model = db.Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(Contra_ChildNo)).First();
                        var contrac = db.Queryable<C_Contrac>().Where(it => it.ContraNo.Equals(model.ContraNo)).First();
                        C_Contrac_User user = db.Queryable<C_Contrac_User>().Where(iv => iv.StudentUid == contrac.StudentUid).First();
                        if (model.ClassId > 0)
                        {
                            if (model.Contrac_Child_Status == (int)ConstraChild_Status.RetrunClassOk)
                            {
                                rsg.msg = "该合同已经退班！无法重复操作";
                                rsg.code = 300;
                                return rsg;
                            }
                            if (model.Contrac_Child_Status != (int)ConstraChild_Status.RetrunClass)
                            {
                                rsg.msg = "该合同未申请退班,无法操作";
                                rsg.code = 300;
                                return rsg;
                            }
                            if (!through)
                            {
                                //更新子合同状态
                                model.UpdateTime = DateTime.Now;
                                model.UpdateUid = uid;
                                model.Contrac_Child_Status = (int)ConstraChild_Status.RetrunClassReject;// 子合同变更状态
                                db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                                rsg.msg = "退班申请已驳回";
                                rsg.code = 200;
                                return rsg;
                            }
                            //查询用户已用课时
                            List<UserCourseTimeModel> userCouses = db.Queryable<C_User_CourseTime, C_Class, C_Subject>((cou, c, sub) => new Object[] {
                                JoinType.Left,cou.ClassId==c.ClassId,JoinType.Left,cou.SubjectId==sub.SubjectId
                            }).Where(cou => cou.StudentUid == contrac.StudentUid && cou.Contra_ChildNo.Equals(model.Contra_ChildNo))
                            .Select<UserCourseTimeModel>((cou, c, sub) => new UserCourseTimeModel
                            {
                                ClassId = cou.ClassId,
                                ClassPrice = c.Price,
                                Class_Course_Time = cou.Class_Course_Time,
                                Class_Course_UseTime = cou.Class_Course_UseTime,
                                Contra_ChildNo = cou.Contra_ChildNo,
                                Course_Time = cou.Course_Time,
                                Course_UseTime = cou.Course_UseTime,
                                Level = cou.Level,
                                Lvel1Price = sub.Lvel1Price,
                                Lvel2Price = sub.Lvel2Price,
                                Lvel3Price = sub.Lvel3Price,
                                Lvel4Price = sub.Lvel4Price,
                                StudentUid = cou.StudentUid,
                                SubjectId = cou.SubjectId
                            }).ToList();
                            //计算未用课时
                            decimal UnUsePrice = 0;
                            float unUseClassTime = 0;
                            if (userCouses != null && userCouses.Count > 0)
                            {
                                userCouses.ForEach(iv =>
                                {
                                    if (iv.ClassId > 0)
                                    {
                                        if (iv.ClassPrice == decimal.Parse("0.01") || iv.ClassPrice == 0)
                                        {
                                            UnUsePrice = 0;
                                        }
                                        else {
                                            unUseClassTime += (iv.Course_Time - iv.Class_Course_UseTime);
                                            var unitPrice = float.Parse(iv.ClassPrice.ToString()) / iv.Class_Course_Time;
                                            UnUsePrice += Convert.ToDecimal((iv.Class_Course_Time - iv.Class_Course_UseTime) * unitPrice);
                                        }
                                    }
                                });
                            }
                            //按原价退款
                            if (inputApply.IsOrg)
                            {
                                UnUsePrice = UnUsePrice - model.Added_Amount; //退款金额-额外优惠金额
                                user.Amount = user.Amount + UnUsePrice;//用户总金额=(用户账户余额+子合同未用课时金额之和)
                                if (user.Amount < 0) {
                                    user.Amount = 0;
                                }
                            }
                            //按输入金额退款
                            if (inputApply.BackCost > 0 && inputApply.IsOrg == false)
                            {
                                user.Amount = user.Amount + inputApply.BackCost;//用户总金额=(用户账户余额+直接输入金额)
                            }
                            //更新用户基础数据
                            db.Updateable<C_Contrac_User>(user).ExecuteCommand();

                            int childCount = db.Queryable<C_Contrac_Child>().Where(it => it.ContraNo == model.ContraNo).Count();
                            if (childCount == 1)
                            {
                                contrac.Constra_Status = (int)Constra_Status.BackPayOk;//退费成功
                            }
                            else if (childCount > 1) {
                                //更新合同总金额
                                contrac.Total_Amount = contrac.Total_Amount - model.Saler_Amount;
                            }
                            db.Updateable<C_Contrac>(contrac).ExecuteCommand();

                            //清零课时
                            db.Updateable<C_User_CourseTime>().SetColumns(it => new C_User_CourseTime {Class_Course_Time = 0, Class_Course_UseTime = 0 }).Where(it => it.Contra_ChildNo == model.Contra_ChildNo && it.StudentUid == model.StudentUid).ExecuteCommand();

                            //更新子合同状态
                            model.UpdateTime = DateTime.Now;
                            model.Contrac_Child_Status = (int)ConstraChild_Status.RetrunClassOk;// 子合同变更状态
                            db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                        }
                        else
                        {
                            rsg.msg = "该合同不包含小班项";
                            rsg.code = 300;
                            return rsg;
                        }
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "退班操作成功";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "退班操作失败-" + er.Message;
                }
            }
            return rsg;
        }

        /// <summary>
        /// 子合同转班审核
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="through"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult ContrcChnageClassAudit(string Contra_ChildNo, bool through, string uid)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();

                    if (!string.IsNullOrEmpty(Contra_ChildNo))
                    {
                        var inputApply = db.Queryable<C_Contrac_Child_ChangeClass>().Where(ite => ite.Contra_ChildNo.Equals(Contra_ChildNo)).First();
                        var model = db.Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(Contra_ChildNo)).First();
                        var contrac = db.Queryable<C_Contrac>().Where(it => it.ContraNo.Equals(model.ContraNo)).First();
                        C_Contrac_User user = db.Queryable<C_Contrac_User>().Where(iv => iv.StudentUid == contrac.StudentUid).First();
                        if (model.ClassId > 0)
                        {
                            if (model.Contrac_Child_Status != (int)ConstraChild_Status.ChangeClass)
                            {
                                rsg.msg = "该合同未申请退班,无法操作";
                                rsg.code = 300;
                                return rsg;
                            }
                            if (!through)
                            {
                                //更新子合同状态
                                model.UpdateTime = DateTime.Now;
                                model.UpdateUid = uid;
                                model.Contrac_Child_Status = (int)ConstraChild_Status.ChangeClassReject;// 子合同变更状态
                                db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                                rsg.msg = "转班申请已驳回";
                                rsg.code = 200;
                                return rsg;
                            }
                            //查询用户已用课时
                            List<UserCourseTimeModel> userCouses = db.Queryable<C_User_CourseTime, C_Class, C_Subject>((cou, c, sub) => new Object[] {
                                JoinType.Left,cou.ClassId==c.ClassId,JoinType.Left,cou.SubjectId==sub.SubjectId
                            }).Where(cou => cou.StudentUid == contrac.StudentUid && cou.Contra_ChildNo.Equals(model.Contra_ChildNo))
                            .Select<UserCourseTimeModel>((cou, c, sub) => new UserCourseTimeModel
                            {
                                ClassId = cou.ClassId,
                                ClassPrice = c.Price,
                                Class_Course_Time = cou.Class_Course_Time,
                                Class_Course_UseTime = cou.Class_Course_UseTime,
                                Contra_ChildNo = cou.Contra_ChildNo,
                                Course_Time = cou.Course_Time,
                                Course_UseTime = cou.Course_UseTime,
                                Level = cou.Level,
                                Lvel1Price = sub.Lvel1Price,
                                Lvel2Price = sub.Lvel2Price,
                                Lvel3Price = sub.Lvel3Price,
                                Lvel4Price = sub.Lvel4Price,
                                StudentUid = cou.StudentUid,
                                SubjectId = cou.SubjectId
                            }).ToList();
                            //计算未用课时
                            decimal UnUsePrice = 0;
                            float unUseClassTime = 0;
                            if (userCouses != null && userCouses.Count > 0)
                            {
                                userCouses.ForEach(iv =>
                                {
                                    if (iv.ClassId > 0)
                                    {
                                        unUseClassTime += (iv.Class_Course_Time - iv.Class_Course_UseTime);
                                        var unitPrice = float.Parse(iv.ClassPrice.ToString()) / iv.Class_Course_Time;
                                        UnUsePrice += Convert.ToDecimal((iv.Class_Course_Time - iv.Class_Course_UseTime)* unitPrice);
                                    }
                                });
                            }
                            //按原价退款
                            UnUsePrice = UnUsePrice - model.Added_Amount; //兑换后金额扣除额外金额
                            user.Amount = user.Amount + UnUsePrice;//用户总金额=(用户账户余额+子合同未用课时金额之和)

                            //更新用户基础数据
                            db.Updateable<C_Contrac_User>(user).ExecuteCommand();

                            //更新合同总金额
                            //contrac.Total_Amount = contrac.Total_Amount - model.Saler_Amount;
                            db.Updateable<C_Contrac>(contrac).ExecuteCommand();

                            model.UpdateTime = DateTime.Now;
                            model.Contrac_Child_Status = (int)ConstraChild_Status.ChangeClassOk;// 子合同变更状态
                            db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                        }
                        else
                        {
                            rsg.msg = "该合同不包含小班项";
                            rsg.code = 300;
                            return rsg;
                        }
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "转班操作成功";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "转班操作失败-" + er.Message;
                }
            }
            return rsg;
        }

        /// <summary>
        /// 子合同撤销
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult ContracCancel(string Contra_ChildNo, string uid) {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();

                    if (!string.IsNullOrEmpty(Contra_ChildNo))
                    {
                        var model = db.Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(Contra_ChildNo)).First();
                        //更新子合同状态
                        model.UpdateTime = DateTime.Now;
                        model.UpdateUid = uid;
                        model.Contrac_Child_Status = (int)ConstraChild_Status.CanCel;// 子合同撤销  
                        db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                        
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "子合同已撤销";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "子合同撤销失败-" + er.Message;
                }
            }
            return rsg;
        }

        /// <summary>
        /// 主合同撤销
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult MasterContracCancel(string contraNo, string uid)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (!string.IsNullOrEmpty(contraNo))
                    {
                        var model = db.Queryable<C_Contrac>().Where(ite => ite.ContraNo.Equals(contraNo)).First();
                        //更新子合同状态
                        int noPayStatus = (int)ConstraChild_Pay_Stutas.NoPay;
                        if (model.Pay_Status == (int)Constra_Pay_Status.NoPay)
                        {
                            var listChildContrc = db.Queryable<C_Contrac_Child>().Where(con => con.ContraNo.Equals(contraNo) && con.Pay_Stutas == noPayStatus).ToList();
                            if (listChildContrc != null && listChildContrc.Count > 0)
                            {
                                listChildContrc.ForEach(te =>
                                {
                                    db.Deleteable<C_Contrac_Child_Detail>().Where(con => con.Contra_ChildNo ==te.Contra_ChildNo).ExecuteCommand();
                                });
                                db.Deleteable<C_Contrac_Child>().Where(con => con.ContraNo == contraNo).ExecuteCommand();
                            }
                           int result= db.Deleteable<C_Contrac>().Where(con => con.ContraNo == contraNo).ExecuteCommand();
                            if (result > 0)
                            {
                                rsg.code = 200;
                                rsg.msg = "主合同已删除";
                            }
                            else {
                                rsg.code = 0;
                                rsg.msg = "主合同删除失败";
                            }
                        }
                        else {
                            rsg.code = 0;
                            rsg.msg = "主合同已付款或者部分付款,无法删除";
                        }
                    }
                    db.CommitTran();
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "主合同删除失败-" + er.Message;
                }
            }
            return rsg;
        }


        /// <summary>
        /// 子合同确认
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult ContracChildConfig(string Contra_ChildNo, string uid)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();

                    if (!string.IsNullOrEmpty(Contra_ChildNo))
                    {
                        var model = db.Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(Contra_ChildNo)).First();
                        //更新子合同状态
                        model.UpdateTime = DateTime.Now;
                        model.UpdateUid = uid;
                        model.Contrac_Child_Status = (int)ConstraChild_Status.Confirmationed;// 子合同已确认
                        db.Updateable<C_Contrac_Child>(model).ExecuteCommand();

                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "子合同已确认";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "子合同确认失败-" + er.Message;
                }
            }
            return rsg;
        }


        /// <summary>
        /// 子合同变更确认
        /// </summary>
        /// <param name="Contra_ChildNo"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult ContracChildChangeConfig(string Contra_ChildNo, string uid)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (!string.IsNullOrEmpty(Contra_ChildNo))
                    {
                        var model = db.Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(Contra_ChildNo)).First();
                        model.UpdateUid = uid;
                        model.UpdateTime = DateTime.Now;
                        model.Contrac_Child_Status = (int)ConstraChild_Status.ChangeOk;
                        db.Updateable<C_Contrac_Child>(model).ExecuteCommand();
                        var updateList = db.Queryable<C_Contrac_Child_Detail>().Where(it => it.Contra_ChildNo ==Contra_ChildNo).ToList();
                        var useCourTimeList = db.Queryable<C_User_CourseTime>().Where(con => con.Contra_ChildNo == model.Contra_ChildNo && con.StudentUid == model.StudentUid).ToList();
                        decimal detailSum = 0;
                        updateList.ForEach(con => {
                            detailSum += con.Price;
                        });
                        //判断1对1,并且更改后价格相同则更改课时
                        if (model.StudyMode == 1&& model.Pay_Stutas == (int)ConstraChild_Pay_Stutas.PayOk&& useCourTimeList!=null&& useCourTimeList.Count>0&& (detailSum * (model.ContraRate / 10))==model.Pay_Amount + model.Added_Amount) {
                           foreach(var item in updateList) {
                                var itemTime = useCourTimeList.Find(con => con.Contra_ChildNo == Contra_ChildNo && con.SubjectId == item.SubjectId && con.ProjectId == item.ProjectId&&con.StudentUid==model.StudentUid);
                                //当课时不相同则修改
                                if (item.Course_Time != itemTime.Course_Time) {
                                    //如果已使用课时大于当前课时，则无法修改
                                    if (itemTime.Course_UseTime > item.Course_Time)
                                    {
                                        var project = db.Queryable<C_Project>().Where(pro => pro.ProjectId == item.ProjectId).First();
                                        rsg.code = 300;
                                        rsg.msg = "科目" + project.ProjectName + "课时已使用" + itemTime.Course_UseTime + "小时,无法降低课时,请重新修改后再确认";
                                        return rsg;
                                    }
                                    else if (itemTime.Course_Time == item.Course_Time) {
                                        continue;
                                    }
                                    else
                                    {
                                        db.Updateable<C_User_CourseTime>().SetColumns(vm => new C_User_CourseTime { Course_Time = item.Course_Time })
                                        .Where(vm => vm.Contra_ChildNo == model.Contra_ChildNo && vm.StudentUid == model.StudentUid && vm.SubjectId == item.SubjectId && vm.ProjectId == item.ProjectId).ExecuteCommand();
                                    }
                                   
                                }
                            }
                        }
                        if (model.StudyMode == 1 && model.Pay_Stutas== (int)ConstraChild_Pay_Stutas.PayOk) {
                            //赠送课时
                            var presentTime = db.Queryable<C_User_PresentTime>().Where(pre => pre.Contra_ChildNo == model.Contra_ChildNo && pre.StudentUid == model.StudentUid).First();
                            if (model.PresentTime > 0)
                            {
                                if (presentTime != null)
                                {
                                    presentTime.Present_Time = model.PresentTime;
                                    presentTime.UpdateUid = model.CreateUid;
                                    presentTime.UpdateTime = DateTime.Now;
                                    db.Updateable<C_User_PresentTime>(presentTime).ExecuteCommand();
                                }
                                else
                                {
                                    db.Insertable<C_User_PresentTime>(new C_User_PresentTime
                                    {
                                        Contra_ChildNo = model.Contra_ChildNo,
                                        Present_Time = model.PresentTime,
                                        Present_UseTime = 0,
                                        StudentUid = model.StudentUid,
                                        CreateTime = DateTime.Now,
                                        CreateUid = model.CreateUid,
                                        UpdateTime = DateTime.Now
                                    }).ExecuteCommand();
                                }
                            }
                        }
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "子合同已确认变更";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "子合同已确认变更失败-" + er.Message;
                }
            }
            return rsg;
        }


        /// <summary>
        /// 确认收款
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="through"></param>
        /// <returns></returns>
        public ResResult ConfigCollection(int collectionId, bool through, string uid)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    var collection = db.Queryable<C_Collection>().Where(ite => ite.Id == collectionId).First();
                    if (through)
                    {
                        var stduentU = db.Queryable<C_Contrac_User>().Where(iv => iv.StudentUid == collection.StudentUid).First();
                        if (string.IsNullOrEmpty(collection.RelationShip_Contras)&&stduentU!= null)
                        {
                            //充值用户余额
                            stduentU.Amount = stduentU.Amount + collection.Amount;
                            //更新收款状态
                            collection.PayStatus = 1;
                            collection.AuditUid = uid;
                            db.Updateable<C_Collection>(collection).ExecuteCommand();
                            db.Updateable<C_Contrac_User>(stduentU).ExecuteCommand();
                        }
                        else {
                            var collectionAmount = collection.Amount;//充值余额
                            var userAmount = stduentU.Amount;//用户余额
                            var oldUserAmount= stduentU.Amount;//原用户余额
                            string[] childContracs = collection.RelationShip_Contras.Split(new char[] { ',' });
                            List<C_Contrac_Child> listchild = new List<C_Contrac_Child>();
                            for (var i = 0; i < childContracs.Length; i++)
                            {
                                string childContrac = childContracs[i];
                                C_Contrac_Child ite = db.Queryable<C_Contrac_Child>().Where(v => v.Contra_ChildNo.Equals(childContrac)).First();
                                if (ite.Added_Amount < 1 && collection.AddedAmount > 0)
                                {
                                    rsg.code = 0;
                                    rsg.msg = "该合同未包含额外优惠,无法使用额外优惠";
                                    return rsg;
                                }
                                if (ite.IsUseAddedAmount > 0)
                                {
                                    rsg.code = 0;
                                    rsg.msg = "该合同额外优惠已被使用,无法重复使用额外优惠";
                                    return rsg;
                                }
                                if (collection.AddedAmount > 0)
                                {
                                    ite.IsUseAddedAmount = 1;
                                }
                                if (collection.AddedAmount > ite.Added_Amount)
                                {
                                    rsg.code = 0;
                                    rsg.msg = "优惠金额已超过该合同实际额外优惠金额，请重新输入";
                                    return rsg;
                                }
                                listchild.Add(ite);
                            }
                            //插入课时和更新课时
                            List<C_User_CourseTime> addUcourse = new List<C_User_CourseTime>();
                            List<C_User_CourseTime> updateUcourse = new List<C_User_CourseTime>();
                            //如果有使用余额，则扣除余额金额
                            if (userAmount > 0 && collection.DeductAmount > 0)
                            {
                                if (userAmount >= collection.DeductAmount) {
                                    userAmount = userAmount - collection.DeductAmount;
                                }
                                else
                                {
                                    rsg.code = 0;
                                    rsg.msg = "余额不够!无法扣除";
                                    return rsg;
                                }
                            }
                            decimal payAmountTotal = 0;//总共付款
                                                       //遍历子合同
                            foreach (var item in listchild)
                            {
                                decimal childBackAmount = 0;
                                decimal depositAmount = 0;// 实际收入超出存入学员余额
                                var chidlContracRate = item.ContraRate > 0 ? item.ContraRate / 10 : 1;
                                //判断子合同是否已付款完成
                                if (item.Pay_Stutas != (int)ConstraChild_Pay_Stutas.PayOk)
                                {
                                    //查询当前子合同课时
                                    List<C_User_CourseTime> listCourse = db.Queryable<C_User_CourseTime>().Where(iv => iv.Contra_ChildNo.Equals(item.Contra_ChildNo)).ToList();
                                    //判断类型
                                    if (item.StudyMode == (int)Study_Mode.OneOfOne)
                                    {

                                        List<C_Collection_Detail> collecationDetail = db.Queryable<C_Collection_Detail>().Where(it => it.CollectionId == collection.Id).ToList();
                                        //查找一对一的所有子项目
                                        List<C_Contrac_Child_Detail> listDetail = db.Queryable<C_Contrac_Child_Detail>().Where(itv => itv.Contra_ChildNo.Equals(item.Contra_ChildNo)).ToList();
                                        if (listCourse.Count == 0)
                                        {
                                            decimal realSum = 0;
                                            decimal detailSum = 0;
                                            collecationDetail.ForEach(it =>
                                            {
                                                realSum += it.Amount;
                                            });
                                            //如果多出，则存入余额
                                            if (realSum < (collection.Amount + collection.AddedAmount + collection.DeductAmount))
                                            {
                                                depositAmount = (collection.Amount + collection.AddedAmount + collection.DeductAmount) - realSum;
                                                collection.Amount = realSum - (collection.AddedAmount + collection.DeductAmount);
                                            }
                                            if (realSum != (collection.Amount + collection.AddedAmount + collection.DeductAmount))
                                            {
                                                rsg.code = 0;
                                                rsg.msg = "充值金额和分配明细不相等,请重新确认";
                                                return rsg;
                                            }
                                            collecationDetail.ForEach(it =>
                                            {
                                                decimal unitPrice = 0;
                                                var child_detail = listDetail.Find(iv => iv.Id == it.ChildDetailId);
                                                var subjectModel = db.Queryable<C_Subject>().Where(c => c.SubjectId == child_detail.SubjectId).First();
                                                switch (child_detail.Level)
                                                {
                                                    case 1:
                                                        unitPrice = subjectModel.Lvel1Price * chidlContracRate;
                                                        break;
                                                    case 2:
                                                        unitPrice = subjectModel.Lvel2Price * chidlContracRate;
                                                        break;
                                                    case 3:
                                                        unitPrice = subjectModel.Lvel3Price * chidlContracRate;
                                                        break;
                                                    case 4:
                                                        unitPrice = subjectModel.Lvel3Price * chidlContracRate;
                                                        break;
                                                }
                                                var lessAmount = it.Amount % unitPrice;
                                                C_User_CourseTime ucourse = new C_User_CourseTime();
                                                //新加改动开始
                                                if (Convert.ToInt32((it.Amount - lessAmount) / unitPrice) > child_detail.Course_Time)
                                                {
                                                    childBackAmount += Convert.ToInt32((Convert.ToInt32((it.Amount - lessAmount) / unitPrice) - child_detail.Course_Time)) * unitPrice;
                                                    detailSum += Convert.ToInt32(child_detail.Course_Time) * unitPrice;
                                                    ucourse.Course_Time = child_detail.Course_Time;
                                                }
                                                else
                                                {
                                                    detailSum += it.Amount - lessAmount;
                                                    ucourse.Course_Time = Convert.ToInt32((it.Amount - lessAmount) / unitPrice);
                                                }
                                                //新加改动截止
                                                //原代码
                                                //detailSum += it.Amount - lessAmount;
                                                //ucourse.Course_Time = Convert.ToInt32((it.Amount - lessAmount) / unitPrice);
                                                ucourse.Level = child_detail.Level;
                                                ucourse.SubjectId = child_detail.SubjectId;
                                                ucourse.ProjectId = child_detail.ProjectId;
                                                ucourse.ClassId = 0;
                                                ucourse.Class_Course_UseTime = 0;
                                                ucourse.Class_Course_Time = 0;
                                                ucourse.Contra_ChildNo = child_detail.Contra_ChildNo;
                                                ucourse.StudentUid = child_detail.StudentUid;
                                                ucourse.Course_UseTime = 0;
                                                ucourse.CreateTime = DateTime.Now;
                                                ucourse.CreateUid = uid;
                                                childBackAmount += lessAmount;
                                                addUcourse.Add(ucourse);
                                            });
                                            item.Pay_Amount = detailSum;
                                        }
                                        else
                                        {
                                            decimal realSum = 0;
                                            decimal detailSum = 0;
                                            collecationDetail.ForEach(it =>
                                            {
                                                realSum += it.Amount;
                                            });
                                            //如果多出，则存入余额
                                            if (realSum - collection.DeductAmount < (collection.Amount + collection.AddedAmount))
                                            {
                                                depositAmount = (collection.Amount + collection.AddedAmount + collection.DeductAmount) - realSum;
                                                collection.Amount = realSum - (collection.AddedAmount + collection.DeductAmount);
                                            }
                                            //分配余额一定和到账收据总金额相等
                                            if (realSum - collection.DeductAmount == (collection.Amount + collection.AddedAmount))
                                            {
                                                collecationDetail.ForEach(it =>
                                                {
                                                    decimal unitPrice = 0;
                                                    var child_detail = listDetail.Find(iv => iv.Id == it.ChildDetailId);
                                                    var subjectModel = db.Queryable<C_Subject>().Where(c => c.SubjectId == child_detail.SubjectId).First();
                                                    switch (child_detail.Level)
                                                    {
                                                        case 1:
                                                            unitPrice = subjectModel.Lvel1Price * chidlContracRate;
                                                            break;
                                                        case 2:
                                                            unitPrice = subjectModel.Lvel2Price * chidlContracRate;
                                                            break;
                                                        case 3:
                                                            unitPrice = subjectModel.Lvel3Price * chidlContracRate;
                                                            break;
                                                        case 4:
                                                            unitPrice = subjectModel.Lvel3Price * chidlContracRate;
                                                            break;
                                                    }
                                                    var updateCourseModel = listCourse.Find(c => c.SubjectId == child_detail.SubjectId && c.ProjectId == child_detail.ProjectId && c.Contra_ChildNo.Equals(child_detail.Contra_ChildNo));
                                                    if (updateCourseModel == null)
                                                    {
                                                        C_User_CourseTime ucourse = new C_User_CourseTime();
                                                        var lessAmount = it.Amount % unitPrice;
                                                        //新修改
                                                        if (Convert.ToInt32((it.Amount - lessAmount) / unitPrice) > child_detail.Course_Time)
                                                        {
                                                            childBackAmount += Convert.ToInt32((Convert.ToInt32((it.Amount - lessAmount) / unitPrice) - child_detail.Course_Time)) * unitPrice;
                                                            detailSum += Convert.ToInt32(child_detail.Course_Time) * unitPrice;
                                                            ucourse.Course_Time = child_detail.Course_Time;
                                                        }
                                                        else
                                                        {
                                                            detailSum += it.Amount - lessAmount;
                                                            ucourse.Course_Time = Convert.ToInt32((it.Amount - lessAmount) / unitPrice);
                                                        }
                                                        childBackAmount += lessAmount;
                                                        //新修改截止
                                                        //原代码
                                                        //childBackAmount += lessAmount;
                                                        //detailSum += it.Amount - lessAmount;
                                                        //ucourse.Course_Time = Convert.ToInt32((it.Amount - lessAmount) / unitPrice);
                                                        ucourse.Level = child_detail.Level;
                                                        ucourse.SubjectId = child_detail.SubjectId;
                                                        ucourse.ProjectId = child_detail.ProjectId;
                                                        ucourse.ClassId = 0;
                                                        ucourse.Class_Course_UseTime = 0;
                                                        ucourse.Class_Course_Time = 0;
                                                        ucourse.Contra_ChildNo = child_detail.Contra_ChildNo;
                                                        ucourse.StudentUid = child_detail.StudentUid;
                                                        ucourse.Course_UseTime = 0;
                                                        ucourse.CreateTime = DateTime.Now;
                                                        ucourse.CreateUid = uid;
                                                        addUcourse.Add(ucourse);
                                                    }
                                                    else
                                                    {
                                                        //新代码
                                                        if (updateCourseModel.Course_Time < child_detail.Course_Time)
                                                        {
                                                            var lessAmount = it.Amount % unitPrice;
                                                            if (Convert.ToInt32((it.Amount - lessAmount) / unitPrice) + updateCourseModel.Course_Time <= child_detail.Course_Time)
                                                            {
                                                                updateCourseModel.Course_Time += Convert.ToInt32((it.Amount - lessAmount) / unitPrice);
                                                                detailSum += it.Amount - lessAmount;
                                                            }
                                                            else
                                                            {
                                                                var usPrice = Convert.ToInt32(child_detail.Course_Time - updateCourseModel.Course_Time) * unitPrice;
                                                                updateCourseModel.Course_Time += child_detail.Course_Time - updateCourseModel.Course_Time;
                                                                detailSum += usPrice;
                                                                childBackAmount += (it.Amount - lessAmount - usPrice);

                                                            }
                                                            childBackAmount += lessAmount;

                                                        }
                                                        else
                                                        {
                                                            childBackAmount += it.Amount;
                                                        }
                                                        //新代码截止
                                                        //原代码
                                                        //var lessAmount = it.Amount % unitPrice;
                                                        //updateCourseModel.Course_Time += Convert.ToInt32((it.Amount - lessAmount) / unitPrice);
                                                        //childBackAmount += lessAmount;
                                                        //detailSum += it.Amount - lessAmount;
                                                        updateCourseModel.UpdateTime = DateTime.Now;
                                                        updateCourseModel.UpdateUid = uid;
                                                        updateUcourse.Add(updateCourseModel);
                                                    }
                                                });
                                                item.Pay_Amount += detailSum;
                                                item.Last_Pay_Amount = detailSum;
                                            }
                                            else
                                            {
                                                rsg.code = 0;
                                                rsg.msg = "充值金额和分配明细不相等,请重新确认";
                                                return rsg;
                                            }

                                        }
                                    }
                                    else if (item.StudyMode == (int)Study_Mode.SmallClass)
                                    {
                                        var classVmodel = db.Queryable<C_Class>().Where(c => c.ClassId == item.ClassId).First();
                                        //根据所付款计算课时，剩余余额存入用户余额
                                        decimal sumToatal = collectionAmount + collection.DeductAmount + collection.AddedAmount;
                                        decimal detailSum = 0;
                                        if (listCourse.Count == 0)
                                        {
                                            C_User_CourseTime ucourse = new C_User_CourseTime();
                                            var classPrice = classVmodel.Price * chidlContracRate;
                                            if (classPrice == sumToatal && classPrice == decimal.Parse("0.01"))
                                            {
                                                ucourse.Class_Course_Time = classVmodel.Course_Time;
                                                detailSum = sumToatal;
                                            }
                                            else
                                            {
                                                ucourse.Contra_ChildNo = item.Contra_ChildNo;
                                                ucourse.ClassId = item.ClassId;
                                                //一次性付款
                                                if (sumToatal == classPrice)
                                                {
                                                    ucourse.Class_Course_Time = classVmodel.Course_Time;
                                                    detailSum = sumToatal;
                                                }
                                                //多充金额
                                                else if (sumToatal > classPrice)
                                                {
                                                    ucourse.Class_Course_Time = classVmodel.Course_Time;
                                                    detailSum = classPrice;
                                                    depositAmount = sumToatal - classPrice;

                                                }
                                                else
                                                {
                                                    decimal classUnitPrice = 0;
                                                    var upLess = classPrice % Convert.ToInt32(classVmodel.Course_Time);
                                                    classUnitPrice = (classPrice - upLess) / Convert.ToInt32(classVmodel.Course_Time);
                                                    var lessAmount = sumToatal % classUnitPrice;
                                                    var classCourseTime = (sumToatal - lessAmount) / classUnitPrice;
                                                    ucourse.Class_Course_Time = Convert.ToInt32(classCourseTime);
                                                    childBackAmount += lessAmount;
                                                    detailSum += (sumToatal - lessAmount);
                                                }
                                            }
                                            ucourse.Class_Course_UseTime = 0;
                                            ucourse.Course_Time = 0;
                                            ucourse.Course_UseTime = 0;
                                            ucourse.CreateTime = DateTime.Now;
                                            ucourse.CreateUid = uid;
                                            ucourse.UpdateTime = DateTime.Now;
                                            ucourse.UpdateUid = uid;
                                            ucourse.SubjectId = classVmodel.SubjectId;
                                            ucourse.StudentUid = stduentU.StudentUid;
                                            addUcourse.Add(ucourse);
                                            item.Pay_Amount = detailSum;
                                        }
                                        else
                                        {
                                            listCourse.ForEach(iv =>
                                            {
                                                var classPrice = classVmodel.Price;
                                                if (classPrice == sumToatal && classPrice == decimal.Parse("0.01"))
                                                {
                                                    iv.Class_Course_Time = classVmodel.Course_Time;
                                                    detailSum += sumToatal;
                                                }
                                                else
                                                {
                                                    classPrice = classVmodel.Price * chidlContracRate;
                                                    decimal classUnitPrice = 0;
                                                    var upLess = classPrice % Convert.ToInt32(classVmodel.Course_Time);
                                                    classUnitPrice = (classPrice - upLess) / Convert.ToInt32(classVmodel.Course_Time);
                                                    var lessAmount = sumToatal % classUnitPrice;
                                                    var newClassCourseTime = (sumToatal - lessAmount) / classUnitPrice;
                                                    if (item.Pay_Amount + sumToatal == item.Saler_Amount)
                                                    {
                                                        iv.Class_Course_Time = classVmodel.Course_Time;
                                                        detailSum += sumToatal;
                                                    }
                                                    else if (item.Pay_Amount + sumToatal > item.Saler_Amount)
                                                    {
                                                        iv.Class_Course_Time = classVmodel.Course_Time;
                                                        detailSum += item.Saler_Amount;
                                                        depositAmount = (item.Pay_Amount + sumToatal) - item.Saler_Amount;
                                                    }
                                                    else
                                                    {
                                                        iv.Class_Course_Time += Convert.ToInt32(newClassCourseTime);
                                                        detailSum += (sumToatal - lessAmount);
                                                        childBackAmount += lessAmount;
                                                    }
                                                }
                                                iv.UpdateUid = uid;
                                                iv.UpdateTime = DateTime.Now;
                                            });
                                            listCourse[0].SubjectId = classVmodel.SubjectId;
                                            updateUcourse.Add(listCourse[0]);
                                            item.Pay_Amount += detailSum;
                                            item.Last_Pay_Amount = detailSum;
                                        }
                                    }
                                }
                                stduentU.Amount = userAmount + childBackAmount + depositAmount;
                                payAmountTotal += item.Pay_Amount;
                                //总额排除额外优惠
                                if (collection.AddedAmount > 0)
                                {
                                    payAmountTotal = payAmountTotal - item.Added_Amount;
                                }
                                if (item.Pay_Amount >= item.Saler_Amount)
                                {
                                    item.Pay_Stutas = (int)ConstraChild_Pay_Stutas.PayOk;
                                    //赠送课时有效
                                    var giveTime = db.Queryable<C_User_PresentTime>().Where(tm => tm.Contra_ChildNo == item.Contra_ChildNo).First();
                                    if (giveTime == null && item.StudyMode == 1)
                                    {
                                        db.Insertable<C_User_PresentTime>(new C_User_PresentTime
                                        {
                                            Contra_ChildNo = item.Contra_ChildNo,
                                            Present_Time = item.PresentTime,
                                            Present_UseTime = 0,
                                            StudentUid = item.StudentUid,
                                            CreateTime = DateTime.Now,
                                            CreateUid = uid,
                                            UpdateTime = DateTime.Now
                                        }).ExecuteCommand();
                                    }

                                }
                                else if (item.Pay_Amount > 0 && item.Pay_Amount < item.Saler_Amount)
                                {
                                    item.Pay_Stutas = (int)ConstraChild_Pay_Stutas.PartPay;
                                }
                            }
                            //更新子合同
                            db.Updateable<C_Contrac_Child>(listchild).ExecuteCommand();

                            //重新更新用户余额
                            db.Updateable<C_Contrac_User>(stduentU).ExecuteCommand();

                            //更新学员课时
                            if (addUcourse.Count > 0)
                                db.Insertable<C_User_CourseTime>(addUcourse).ExecuteCommand();
                            if (updateUcourse.Count > 0)
                                db.Updateable<C_User_CourseTime>(updateUcourse).ExecuteCommand();

                            //更新主合同
                            string manContraNo = listchild[0].ContraNo;
                            C_Contrac contrac = db.Queryable<C_Contrac>().Where(ite => ite.ContraNo.Equals(manContraNo)).First();
                            contrac.Pay_Amount = payAmountTotal;
                            var allChild = db.Queryable<C_Contrac_Child>().Where(v => v.ContraNo.Equals(contrac.ContraNo)).ToList();
                            var payChilds = listchild.FindAll(ite => ite.Pay_Stutas == (int)ConstraChild_Pay_Stutas.PayOk);
                            var partChilds = listchild.FindAll(ite => ite.Pay_Stutas == (int)ConstraChild_Pay_Stutas.PartPay);
                            if (allChild.Count == payChilds.Count)
                            {
                                contrac.Constra_Status = (int)Constra_Status.Finsh;
                                contrac.Pay_Status = (int)Constra_Pay_Status.PayOk;
                            }
                            else if (partChilds.Count > 0)
                            {
                                contrac.Pay_Status = (int)Constra_Pay_Status.PartPay;
                            }
                            //更新收款状态
                            collection.PayStatus = 1;
                            collection.AuditUid = uid;
                            //记录扣除余额
                            if (collection.DeductAmount > 0) {
                                collection.koudeductAmount = oldUserAmount - stduentU.Amount - collection.AddedAmount;
                            }
                            db.Updateable<C_Collection>(collection).ExecuteCommand();
                            db.Updateable<C_Contrac>(contrac).ExecuteCommand();
                        }

                    }
                    else
                    {
                        //更新收款凭证
                        collection.PayStatus = 2;
                        collection.AuditUid = uid;
                        db.Updateable<C_Collection>(collection).ExecuteCommand();

                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "审核操作成功";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "审核操作失败-" + er.Message;
                }
            }
            return rsg;
        }

        /// <summary>
        /// 添加/修改收款
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveCollection(CollectionInput input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (input.Id > 0)
                    {
                        C_Collection collection = db.Queryable<C_Collection>().Where(f => f.Id == input.Id).First();
                        if (collection.PayStatus == 1) {
                            rsg.code = 0;
                            rsg.msg = "确认收款已经完成,此次修改保存无效";
                            return rsg;
                        }
                        collection.Amount = input.Amount;
                        collection.FilAmount = collection.Amount;
                        collection.PayMothed = input.PayMothed;
                        collection.PayImg = input.PayImg;
                        collection.RelationShip_Contras = input.RelationShip_Contras;
                        collection.StudentUid = input.StudentUid;
                        collection.StudentName = input.StudentName;
                        collection.DeductAmount = input.DeductAmount;
                        collection.AddedAmount = input.AddedAmount;
                        collection.PayStatus = 0;
                        if (!string.IsNullOrEmpty(input.RelationShip_Contras))
                        {
                            var childContrac = db.Queryable<C_Contrac_Child>().Where(ct => ct.Contra_ChildNo == input.RelationShip_Contras).First();
                            if (childContrac.Added_Amount < 1 && input.AddedAmount > 0)
                            {
                                rsg.code = 0;
                                rsg.msg = "该合同未包含使用额外优惠，无法保存";
                                return rsg;
                            }
                            if (childContrac.Added_Amount > 0 && childContrac.Added_Amount < input.AddedAmount)
                            {
                                rsg.code = 0;
                                rsg.msg = "输入额外优惠已超过该合同实际额外优惠，无法保存";
                                return rsg;
                            }
                            var studentU= db.Queryable<C_Contrac_User>().Where(u =>u.StudentUid == input.StudentUid).First();
                            if (studentU.Amount < input.DeductAmount) {
                                rsg.code = 0;
                                rsg.msg = "输入余额超过该学员实际余额,无法保存";
                                return rsg;
                            }
                        }
                        var result = db.Updateable<C_Collection>(collection).ExecuteCommand();
                        if (result > 0 && input.CollectionDetail != null && input.CollectionDetail.Count > 0)
                        {
                            List<C_Collection_Detail> updateDetail = new List<C_Collection_Detail>();
                            List<C_Collection_Detail> detail = db.Queryable<C_Collection_Detail>().Where(f => f.CollectionId == collection.Id).ToList();
                            detail.ForEach(ite =>
                            {
                                ite.Amount = input.CollectionDetail.Find(it => it.ChildDetailId == ite.ChildDetailId && it.CollectionId == ite.CollectionId).Amount;
                                ite.UpdateTime = DateTime.Now;
                                ite.UpdateUid = input.CreateUid;
                            });
                            db.Updateable<C_Collection_Detail>(detail).ExecuteCommand();
                        }
                    }
                    else
                    {
                        C_Collection collection = new C_Collection();
                        collection.CreateTime = DateTime.Now;
                        collection.CreateUid = input.CreateUid;
                        collection.Registration_Time = DateTime.Now;
                        collection.Amount = input.Amount;
                        collection.FilAmount = collection.Amount;
                        collection.CampusId = input.CampusId;
                        collection.PayStatus = 0;
                        collection.Collection_Time = input.Collection_Time;
                        collection.RelationShip_Contras = input.RelationShip_Contras;
                        collection.StudentName = input.StudentName;
                        collection.StudentUid = input.StudentUid;
                        collection.PayMothed = input.PayMothed;
                        collection.DeductAmount = input.DeductAmount;
                        var useAddedAny = db.Queryable<C_Collection>().Where(n => n.RelationShip_Contras == input.RelationShip_Contras && n.AddedAmount > 0).First();
                        //额外优惠未用，则可继续使用
                        if (useAddedAny == null)
                        {
                            collection.AddedAmount = input.AddedAmount;
                        }
                        else {
                            collection.AddedAmount = 0;
                        }
                        if (!string.IsNullOrEmpty(input.RelationShip_Contras)) {
                            var childContrac = db.Queryable<C_Contrac_Child>().Where(ct => ct.Contra_ChildNo == input.RelationShip_Contras).First();
                            if (childContrac.Added_Amount < 1 && input.AddedAmount > 0) {
                                rsg.code = 0;
                                rsg.msg = "该合同未包含使用额外优惠，无法保存";
                                return rsg;
                            }
                            if (childContrac.Added_Amount > 0&&childContrac.Added_Amount< input.AddedAmount)
                            {
                                rsg.code = 0;
                                rsg.msg = "输入额外优惠已实际超过该合同额外优惠，无法保存";
                                return rsg;
                            }
                            var studentU = db.Queryable<C_Contrac_User>().Where(u => u.StudentUid == input.StudentUid).First();
                            if (studentU.Amount < input.DeductAmount)
                            {
                                rsg.code = 0;
                                rsg.msg = "输入余额超过该学员实际余额,无法保存";
                                return rsg;
                            }
                            collection.ArrearageStatus = childContrac.ArrearageStatus;
                        }
                        var result = db.Insertable<C_Collection>(collection).ExecuteReturnIdentity();
                        if (result > 0 && input.CollectionDetail != null && input.CollectionDetail.Count > 0)
                        {
                            input.CollectionDetail.ForEach(it =>
                            {
                                it.CollectionId = result;
                                it.CreateUid = input.CreateUid;
                                it.CreateTime = DateTime.Now;
                            });
                            db.Insertable<C_Collection_Detail>(input.CollectionDetail).ExecuteCommand();
                        }
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "保存到款收据操作成功";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "保存到款收据操作失败-" + er.Message;
                }
            }
            return rsg;

        }


        public ResResult BackPartCostByTimeId(PartBackCostInput input) {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    if (input.Id > 0)
                    {
                        var timeModel = db.Queryable<C_User_CourseTime>().Where(t => t.Id == input.Id).First();
                        var stundent= db.Queryable<C_Contrac_User>().Where(t => t.StudentUid == timeModel.StudentUid).First();
                        //更新科目课时
                        if (timeModel.Course_Time == timeModel.Course_UseTime)
                        {
                            rsg.code = 0;
                            rsg.msg = "无法部分退费,课时已用完";
                        }
                        else if (timeModel.Course_Time - input.BackCourseTime < timeModel.Course_UseTime)
                        {
                            rsg.code = 0;
                            rsg.msg = "无法部分退费，课时只够退" + (timeModel.Course_Time - timeModel.Course_UseTime) + "小时";
                        }
                        else {
                            db.Updateable<C_User_CourseTime>().SetColumns(v=>new C_User_CourseTime {Course_Time=(timeModel.Course_Time - input.BackCourseTime) }).Where(v=>v.Id==input.Id).ExecuteCommand();
                            //更新学员余额
                            db.Updateable<C_Contrac_User>().SetColumns(v => new C_Contrac_User { Amount = (stundent.Amount + input.BackAmount) }).Where(v => v.StudentUid ==timeModel.StudentUid).ExecuteCommand();
                            rsg.code = 200;
                            rsg.msg = "部分退费成功";
                        }
                    }
                    else
                    {
                        rsg.code = 0;
                        rsg.msg = "缺少参数";
                    }
                    db.CommitTran();
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "保存到款收据操作失败-" + er.Message;
                }
            }
            return rsg;
        }
    }
}
