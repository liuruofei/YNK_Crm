using ADT.Models;
using ADT.Models.Enum;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Repository.IRepository;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ADT.Repository
{
    public class C_CourseWorkRepository : BaseRepository<C_Course_Work>, IC_CourseWorkRepository
    {

        public static string HttpPost(string url, string postData = null, string contentType = null, int timeOut = 30, Dictionary<string, string> headers = null)
        {
            postData = postData ?? "";
            using (HttpClient client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                using (HttpContent httpContent = new StringContent(postData, Encoding.UTF8))
                {
                    if (contentType != null)
                        httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    HttpResponseMessage response = client.PostAsync(url, httpContent).Result;
                    return response.Content.ReadAsStringAsync().Result + postData;
                }
            }
        }



        /// <summary>
        /// 保存排课
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveCourseWork(CourseWorkInput vmodel)
        {
          
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    C_Course_Work input = new C_Course_Work()
                    {
                        Id = vmodel.Id,
                        AT_Date = vmodel.AT_Date,
                        TA_Uid = vmodel.TA_Uid,
                        ClasssId=vmodel.ClasssId,
                        CampusId=vmodel.CampusId,
                        Contra_ChildNo=vmodel.Contra_ChildNo,
                        StartTime=vmodel.StartTime,
                        EndTime=vmodel.EndTime,
                        StudentUid=vmodel.StudentUid,
                        StudyMode=vmodel.StudyMode,
                        SubjectId=vmodel.SubjectId,
                        ProjectId=vmodel.ProjectId,
                        RangTimeId=vmodel.RangTimeId,
                        Work_Title=vmodel.Work_Title,
                        Work_Stutas=vmodel.Work_Stutas,
                        CourseTime=vmodel.CourseTime,
                        TeacherUid=vmodel.TeacherUid,
                        Comment=vmodel.Comment,
                        RoomId=vmodel.RoomId,
                        ListeningName=vmodel.ListeningName,
                        CreateUid=vmodel.CreateUid,
                        UpdateUid=vmodel.UpdateUid,
                        UnitId=vmodel.UnitId,
                        IsUsePresent=vmodel.IsUsePresent
                    };
                    db.BeginTran();
                    if (input.RangTimeId > 0)
                    {
                       var rang=db.Queryable<C_Range_Time>().Where(it => it.Id == input.RangTimeId).First();
                        input.StartTime = rang.StartTime;
                        input.EndTime = rang.EndTime;
                    }
                    C_Course_Work_Recored recored = new C_Course_Work_Recored();
                    if (input.Id > 0)
                    {
                        C_Course_Work work = db.Queryable<C_Course_Work>().Where(it => it.Id == input.Id).First();
                        if (work.StudyMode != vmodel.StudyMode&&work.StudyMode!=4) {
                            rsg.msg = "上课模式不能更改其它模式(试听课改成1对1除外)!";
                            rsg.code = 0;
                            return rsg;
                        }
                        //验证教室此时间段是否被占用，并且做出提示
                        if (work.StudyMode != 5 && work.StudyMode != 6 && work.StudyMode != 7 && work.StudyMode != 3 && work.StudyMode != 9) {
                            C_Course_Work ctRoom = db.Queryable<C_Course_Work, C_Room>((ir, rm) => new Object[] { JoinType.Left, ir.RoomId == rm.Id }).Where((ir, rm) => ir.RoomId == input.RoomId && ir.Id != input.Id && ir.AT_Date.ToString("yyyy-MM-dd") == input.AT_Date.ToString("yyyy-MM-dd")
                             &&(
                             (DateTime.Parse(ir.AT_Date.ToString("yyyy-MM-dd") + " " + ir.StartTime)<= DateTime.Parse(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime) && DateTime.Parse(ir.AT_Date.ToString("yyyy-MM-dd") + " " + ir.EndTime)>DateTime.Parse(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime))||
                             (DateTime.Parse(ir.AT_Date.ToString("yyyy-MM-dd") + " " + ir.StartTime)< DateTime.Parse(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime)&& DateTime.Parse(ir.AT_Date.ToString("yyyy-MM-dd") + " " + ir.EndTime)>=DateTime.Parse(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime))||
                             (DateTime.Parse(ir.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime) <= DateTime.Parse(input.AT_Date.ToString("yyyy-MM-dd") + " " +ir.StartTime) && DateTime.Parse(ir.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) > DateTime.Parse(input.AT_Date.ToString("yyyy-MM-dd") + " " + ir.StartTime))||
                              (DateTime.Parse(ir.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime) < DateTime.Parse(input.AT_Date.ToString("yyyy-MM-dd") + " " + ir.EndTime) && DateTime.Parse(ir.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) >= DateTime.Parse(input.AT_Date.ToString("yyyy-MM-dd") + " " + ir.EndTime))
                             )
                           && ir.RoomId > 0 && !rm.RoomName.Contains("线上")&&!rm.RoomName.Contains("考场")).First();
                            if (ctRoom != null)
                            {
                                rsg.msg = "此时间段,该教室已被占用!请更换其它教室";
                                rsg.code = 0;
                                return rsg;
                            }
                        }
                        //1对1模式
                        if (input.StudyMode == 1 && work.Work_Stutas == 0)
                        {
                            string msg = "", utName = "";
                            sys_user teacha = db.Queryable<sys_user>().Where(it => it.User_ID == work.TeacherUid).First();
                            C_Contrac_User u = db.Queryable<C_Contrac_User>().Where(it => it.StudentUid == work.StudentUid).First();
                            C_Subject sub = db.Queryable<C_Subject>().Where(it => it.SubjectId == work.SubjectId).First();
                            C_Project pro = db.Queryable<C_Project>().Where(it => it.ProjectId == work.ProjectId).First();
                            if (work.StudentUid != input.StudentUid || work.SubjectId != input.SubjectId || work.ProjectId != input.ProjectId || work.Contra_ChildNo != input.Contra_ChildNo)
                            {
                                C_Contrac_User u2 = db.Queryable<C_Contrac_User>().Where(it => it.StudentUid == input.StudentUid).First();
                                C_Subject sub2 = db.Queryable<C_Subject>().Where(it => it.SubjectId == input.SubjectId).First();
                                C_Project pro2 = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                                msg = "1对1(" + (u != null ? u.Student_Name : work.ListeningName) + ")_" + sub.SubjectName + "_" + pro.ProjectName + "课程，日期" + work.AT_Date.ToString("yyyy-MM-dd") + ",时间 " + work.StartTime + work.EndTime + "变更为" +
                                    "1对1(" + u2.Student_Name + ")_" + sub2.SubjectName + "_" + pro2.ProjectName + "日期" + input.AT_Date.ToString("yyyy-MM-dd") + ",时间 " + input.StartTime + input.EndTime;
                                recored.Msg = msg;
                            }
                            else
                            {
                                if (input.AT_Date != work.AT_Date || !input.StartTime.Equals(work.StartTime) || !input.EndTime.Equals(work.EndTime))
                                {
                                    msg = work.Work_Title + "更改课程时间由原时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime +
                                        "已改成" + input.AT_Date.ToString("yyyy/MM/dd") + " " + input.StartTime + "-" + input.EndTime;
                                    recored.Msg = msg;
                                }
                                else
                                {
                                    recored.Msg = "1对1(" + u.Student_Name + ")_" + sub.SubjectName + "_" + pro.ProjectName + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime + "已修改";
                                }
                            }
                            work.Contra_ChildNo = input.Contra_ChildNo;
                            work.StudyMode = input.StudyMode;
                            work.Work_Title = input.Work_Title;
                            work.SubjectId = input.SubjectId;
                            work.ProjectId = input.ProjectId;
                            if (input.UnitId > 0)
                            {
                                var unitModel = db.Queryable<C_Project_Unit>().Where(ut => ut.UnitId == input.UnitId).First();
                                work.UnitId = input.UnitId;
                                if (!string.IsNullOrEmpty(work.Work_Title))
                                {
                                    if (work.Work_Title.Split('_').Length == 4)
                                    {
                                        work.Work_Title = work.Work_Title.Substring(0, work.Work_Title.LastIndexOf("_"));
                                    }
                                    work.Work_Title = work.Work_Title + "_" + unitModel.UnitName;
                                }
                                utName = "_" + unitModel.UnitName;
                            }
                            work.AT_Date = input.AT_Date;
                            work.StartTime = input.StartTime;
                            work.EndTime = input.EndTime;
                            if (!string.IsNullOrEmpty(work.TeacherUid) && !string.IsNullOrEmpty(input.TeacherUid) && work.TeacherUid != input.TeacherUid)
                            {
                                sys_user teachb = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                                msg = "1对1(" + u.Student_Name + ")_" + sub.SubjectName + "_" + pro.ProjectName + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime
                                   + "由老师" + teacha.User_Name + "更改成" + teachb.User_Name;
                                recored.Msg = msg;
                            }
                            work.TeacherUid = input.TeacherUid;
                            work.RangTimeId = input.RangTimeId;
                            work.TA_Uid = input.TA_Uid;
                            work.RoomId = input.RoomId;
                            work.CampusId = u != null ? u.CampusId : work.CampusId;
                            TimeSpan span = Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) - Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime);
                            //如果课时修改的学员是同一个人，则判断课时
                            if (work.StudentUid == input.StudentUid)
                            {
                                int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                                var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => con.StudentUid == input.StudentUid && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                                string where = "";
                                //判断该学员其中的班课是否也起冲突
                                if (contansClassIds != null && contansClassIds.Count > 0)
                                {
                                    where += " or c.ClasssId in(" + string.Join(",", contansClassIds) + ")";
                                }
                                //判断学员课程是否冲突
                                C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                    .Where("c.Id!=@workId and (c.StudentUid=@StudentUid or c.TeacherUid=@TeacherUid" + where + ") and c.StudyMode!=5 and " +
                                    "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                    " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                    " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                    " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                    ")")
                                    .AddParameters(new { workId = input.Id, StudentUid = input.StudentUid, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime }).First();
                                if (anyValue != null)
                                {
                                    rsg.msg = "此时间段，当天该课程老师或者该学员已有其它课程,无法排课";
                                    rsg.code = 0;
                                    return rsg;
                                }
                                var oldwork = db.Queryable<C_Course_Work>().Where(c => c.Id == work.Id).First();
                                if (work.SubjectId == oldwork.SubjectId && work.ProjectId == oldwork.ProjectId)
                                {
                                    if (work.IsUsePresent == 0)
                                    {
                                        C_User_CourseTime useCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == work.StudentUid && it.Contra_ChildNo.Equals(work.Contra_ChildNo) && it.SubjectId == work.SubjectId && it.ProjectId == work.ProjectId).First();
                                        float hourse = float.Parse(span.TotalMinutes.ToString()) / 60;

                                        if (oldwork.StudyMode == 1)
                                        {
                                            //原来已上课时大于现在修改课时，则扣掉用户已用课时
                                            if (work.CourseTime > hourse)
                                            {
                                                var less = work.CourseTime - hourse;
                                                useCourseTime.Course_UseTime = useCourseTime.Course_UseTime - less;
                                            }
                                            //原来已上课时小于现在修改课时，则增加用户已用课时
                                            else if (work.CourseTime < hourse)
                                            {
                                                var more = hourse - work.CourseTime;
                                                if (useCourseTime.Course_UseTime + more > useCourseTime.Course_Time)
                                                {
                                                    rsg.code = 0;
                                                    rsg.msg = "学员所剩课时不足！无法再排课";
                                                    return rsg;
                                                }
                                                else if (useCourseTime.Course_UseTime + more <= useCourseTime.Course_Time)
                                                {
                                                    useCourseTime.Course_UseTime = useCourseTime.Course_UseTime + more;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            //试听课改成1对1
                                            if ((useCourseTime.Course_Time- useCourseTime.Course_UseTime) < hourse)
                                            {
                                                rsg.code = 0;
                                                rsg.msg = "学员所剩课时不足！无法再排课";
                                                return rsg;
                                            }
                                            else
                                            {
                                                useCourseTime.Course_UseTime = useCourseTime.Course_UseTime + hourse;
                                            }
                                        }
                                        db.Updateable<C_User_CourseTime>(useCourseTime).ExecuteCommand();
                                    }
                                    else
                                    {
                                        C_User_PresentTime useCourseTime = db.Queryable<C_User_PresentTime>().Where(it => it.StudentUid == work.StudentUid && it.Contra_ChildNo.Equals(work.Contra_ChildNo)).First();
                                        //var hourse = span.Hours;
                                        float hourse = float.Parse(span.TotalMinutes.ToString()) / 60;
                                        if (oldwork.StudyMode == 1)
                                        {
                                            //原来已上课时大于现在修改课时，则扣掉用户已用课时
                                            if (work.CourseTime > hourse)
                                            {
                                                var less = work.CourseTime - hourse;
                                                useCourseTime.Present_UseTime = useCourseTime.Present_UseTime - less;
                                            }
                                            //原来已上课时小于现在修改课时，则增加用户已用课时
                                            else if (work.CourseTime < hourse)
                                            {
                                                var more = hourse - work.CourseTime;
                                                if (useCourseTime.Present_UseTime + more > useCourseTime.Present_Time)
                                                {
                                                    rsg.code = 0;
                                                    rsg.msg = "学员所剩赠送课时不足！无法再排课";
                                                    return rsg;
                                                }
                                                else if (useCourseTime.Present_UseTime + more <= useCourseTime.Present_Time)
                                                {
                                                    useCourseTime.Present_UseTime = useCourseTime.Present_UseTime + more;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            //试听课改成1对1
                                            if (useCourseTime.Present_UseTime < hourse)
                                            {
                                                rsg.code = 0;
                                                rsg.msg = "学员所剩赠送课时不足！无法再排课";
                                                return rsg;
                                            }
                                            else
                                            {
                                                useCourseTime.Present_UseTime = useCourseTime.Present_UseTime + hourse;
                                            }
                                        }
                                        db.Updateable<C_User_PresentTime>(useCourseTime).ExecuteCommand();
                                        work.Work_Title = "1对1(" + u.Student_Name + ")_" + sub.SubjectName + "_" + pro.ProjectName + utName;
                                    }
                                }
                                else
                                {
                                    if (work.IsUsePresent == 0)
                                    {
                                        if (oldwork.StudyMode == 1)
                                        {
                                            C_User_CourseTime olduseCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == work.StudentUid && it.Contra_ChildNo.Equals(work.Contra_ChildNo) && it.SubjectId == oldwork.SubjectId && it.ProjectId == oldwork.ProjectId).First();
                                            olduseCourseTime.Course_UseTime = olduseCourseTime.Course_UseTime - oldwork.CourseTime;
                                            db.Updateable<C_User_CourseTime>(olduseCourseTime).ExecuteCommand();
                                        }
                                        float hourse = float.Parse(span.TotalMinutes.ToString()) / 60;
                                        C_User_CourseTime usenewCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == work.StudentUid && it.Contra_ChildNo.Equals(work.Contra_ChildNo) && it.SubjectId == input.SubjectId && it.ProjectId == input.ProjectId).First();
                                        if ((usenewCourseTime.Course_Time- usenewCourseTime.Course_UseTime)< hourse)
                                        {
                                            rsg.code = 0;
                                            rsg.msg = "学员所剩课时不足！无法再排课";
                                            return rsg;
                                        }
                                        usenewCourseTime.Course_UseTime = usenewCourseTime.Course_UseTime + hourse;
                                        db.Updateable<C_User_CourseTime>(usenewCourseTime).ExecuteCommand();
                                    }
                                    else
                                    {
                                        C_User_PresentTime useCourseTime = db.Queryable<C_User_PresentTime>().Where(it => it.StudentUid == work.StudentUid && it.Contra_ChildNo.Equals(work.Contra_ChildNo)).First();
                                        float hourse = float.Parse(span.TotalMinutes.ToString()) / 60;
                                        if (oldwork.StudyMode == 1)
                                        {
                                            //原来已上课时大于现在修改课时，则扣掉用户已用课时
                                            if (work.CourseTime > hourse)
                                            {
                                                var less = work.CourseTime - hourse;
                                                useCourseTime.Present_UseTime = useCourseTime.Present_UseTime - less;
                                            }
                                            //原来已上课时小于现在修改课时，则增加用户已用课时
                                            else if (work.CourseTime < hourse)
                                            {
                                                var more = hourse - work.CourseTime;
                                                if (useCourseTime.Present_UseTime + more > useCourseTime.Present_Time)
                                                {
                                                    rsg.code = 0;
                                                    rsg.msg = "学员所剩赠送课时不足！无法再排课";
                                                    return rsg;
                                                }
                                                else if (useCourseTime.Present_UseTime + more <= useCourseTime.Present_Time)
                                                {
                                                    useCourseTime.Present_UseTime = useCourseTime.Present_UseTime + more;
                                                }

                                            }

                                            db.Updateable<C_User_PresentTime>(useCourseTime).ExecuteCommand();
                                        }
                                        else
                                        {
                                            if (useCourseTime.Present_Time < hourse)
                                            {
                                                rsg.code = 0;
                                                rsg.msg = "学员所剩赠送课时不足！无法再排课";
                                                return rsg;
                                            }
                                            else
                                            {
                                                useCourseTime.Present_UseTime = useCourseTime.Present_UseTime + hourse;
                                            }
                                            db.Updateable<C_User_PresentTime>(useCourseTime).ExecuteCommand();
                                        }
                                        C_Subject sub2 = db.Queryable<C_Subject>().Where(it => it.SubjectId == input.SubjectId).First();
                                        C_Project pro2 = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                                        work.Work_Title = "1对1(" + u.Student_Name + ")_" + sub2.SubjectName + "_" + pro2.ProjectName;

                                    }
                                }
                                work.CourseTime = float.Parse(span.TotalMinutes.ToString()) / 60;
                            }
                            else
                            {
                                int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                                var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => con.StudentUid == input.StudentUid && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                                string where = "";
                                //判断该学员其中的班课是否也起冲突
                                if (contansClassIds != null && contansClassIds.Count > 0)
                                {
                                    where += " or c.ClasssId in(" + string.Join(",", contansClassIds) + ")";
                                }
                                //判断学员课程是否冲突
                                C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                    .Where("c.Id!=@workId and (c.StudentUid=@StudentUid or c.TeacherUid=@TeacherUid" + where + ") and c.StudyMode!=5 and " +
                                    "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                    " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                    " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                    " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                    ")")
                                    .AddParameters(new { workId = input.Id, StudentUid = input.StudentUid, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime }).First();
                                if (anyValue != null)
                                {
                                    rsg.msg = "此时间段，当天该课程老师或者该学员已有其它课程,无法排课";
                                    rsg.code = 0;
                                    return rsg;
                                }

                                //如果课时修改的学员不是同一个人，则判断课时
                                C_User_CourseTime olduseCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == work.StudentUid && it.Contra_ChildNo.Equals(work.Contra_ChildNo) && it.SubjectId == work.SubjectId && it.ProjectId == work.ProjectId).First();
                                C_User_CourseTime useCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == input.StudentUid && it.Contra_ChildNo.Equals(input.Contra_ChildNo) && it.SubjectId == input.SubjectId && it.ProjectId == input.ProjectId).First();
                                float hourse = float.Parse(span.TotalMinutes.ToString()) / 60;
                                //则恢复原学员已用课时
                                if (olduseCourseTime != null)
                                {
                                    olduseCourseTime.Course_UseTime = useCourseTime.Course_UseTime - work.CourseTime;
                                }
                                //判断新学员所剩课时是否足够
                                if (useCourseTime.Course_UseTime + hourse > useCourseTime.Course_Time)
                                {
                                    rsg.code = 0;
                                    rsg.msg = "学员所剩课时不足！无法再排课";
                                    return rsg;
                                }
                                else if (useCourseTime.Course_UseTime + hourse <= useCourseTime.Course_Time)
                                {
                                    useCourseTime.Course_UseTime = useCourseTime.Course_UseTime + hourse;
                                }
                                if (olduseCourseTime != null)
                                {
                                    db.Updateable<C_User_CourseTime>(olduseCourseTime).ExecuteCommand();
                                }
                                db.Updateable<C_User_CourseTime>(useCourseTime).ExecuteCommand();
                                work.StudentUid = input.StudentUid;
                                work.CourseTime = hourse;
                            }
                            recored.CreateUid = input.CreateUid;
                            recored.CampusId = work.CampusId;
                            recored.CreateTime = DateTime.Now;
                        }
                        //小班模式
                        else if (input.StudyMode == 2 && work.Work_Stutas == 0)
                        {
                            string where = "";
                            int childContracStatu = (int)ConstraChild_Status.RetrunClassOk;//排除退班的学生
                            var contansStudentUids = db.Queryable<C_Contrac_Child>().Where(con => con.ClassId == input.ClasssId && con.StudyMode == 2 && con.Contrac_Child_Status != childContracStatu).Select(con => con.StudentUid).ToList();
                            //判断该班课其中的学员是否也起冲突
                            if (contansStudentUids != null && contansStudentUids.Count > 0)
                            {
                                where += " or c.StudentUid in(" + string.Join(",", contansStudentUids) + ")";
                                //判断该班课学员的其它班课是否也起冲突
                                int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                                var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => contansStudentUids.Contains(con.StudentUid) && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                                if (contansClassIds != null && contansClassIds.Count > 0)
                                {
                                    where += " or c.ClasssId in(" + string.Join(",", contansClassIds) + ")";
                                }
                            }
                            string msg = "";
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                         .Where("c.Id!=@workId and (c.ClasssId=@ClasssId or c.TeacherUid=@TeacherUid" + where + ") and c.StudyMode!=5 and " +
                         "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                         " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                         " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                         " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                         ")")
                         .AddParameters(new { workId = input.Id, ClasssId = input.ClasssId, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime }).First();
                            if (anyValue != null)
                            {
                                rsg.msg = "此时间段，当天该课程老师或者该小班成员已有其它课程,无法排课";
                                rsg.code = 0;
                                return rsg;
                            }
                            C_Class cla = db.Queryable<C_Class>().Where(it => it.ClassId == work.ClasssId).First();
                            //只更新小班课时，不允许更换小班
                            if (input.AT_Date != work.AT_Date || !input.StartTime.Equals(work.StartTime) || !input.EndTime.Equals(work.EndTime))
                            {
                                msg = input.Work_Title + "更改课程时间由原时间" + work.AT_Date.ToString("yyyy/MM//dd") + " " + work.StartTime + "-" + work.EndTime +
                                    "已改成" + input.AT_Date.ToString("yyyy/MM/dd") + " " + input.StartTime + "-" + input.EndTime;
                                recored.Msg = msg;
                            }
                            else
                            {
                                recored.Msg = cla.Class_Name + "已修改";
                            }
                            if (work.ProjectId != input.ProjectId)
                            {
                                C_Project pro = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                                work.Work_Title = cla.Class_Name + "_" + pro.ProjectName;
                            }
                            work.Contra_ChildNo = input.Contra_ChildNo;
                            work.StudyMode = input.StudyMode;
                            work.SubjectId = cla.SubjectId;
                            work.ProjectId = input.ProjectId;
                            work.AT_Date = input.AT_Date;
                            work.StartTime = input.StartTime;
                            work.EndTime = input.EndTime;
                            work.TeacherUid = input.TeacherUid;
                            work.RangTimeId = input.RangTimeId;
                            work.TA_Uid = input.TA_Uid;
                            work.ClasssId = input.ClasssId;
                            work.RoomId = input.RoomId;
                            work.CampusId = cla.CampusId;
                            if (input.UnitId > 0)
                            {
                                work.UnitId = input.UnitId;
                            }
                            TimeSpan span = Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) - Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime);


                            //查询当前班级所有学员的课时
                            List<C_User_CourseTime> useCourseTimelist = db.Queryable<C_User_CourseTime>().Where(it => it.ClassId == work.ClasssId && it.Contra_ChildNo.Equals(work.Contra_ChildNo) && it.SubjectId == cla.SubjectId).ToList();
                            List<C_User_CourseTime> updateUseCourseTimes = new List<C_User_CourseTime>();
                            var hourse = span.Hours;
                            foreach (var item in useCourseTimelist)
                            {
                                //原来已上课时大于现在修改课时，则扣掉用户已用课时
                                if (work.CourseTime > hourse)
                                {
                                    var less = work.CourseTime - hourse;
                                    item.Class_Course_UseTime = item.Class_Course_UseTime - less;
                                }
                                //原来已上课时小于现在修改课时，则增加用户已用课时
                                else if (work.CourseTime < hourse)
                                {
                                    var more = hourse - work.CourseTime;
                                    if (item.Class_Course_UseTime + more > item.Course_Time && item.Course_Time > 0)//小班特殊处理,如果所用课时未
                                    {
                                        rsg.code = 0;
                                        rsg.msg = "学员所剩课时不足！无法再排课";
                                        return rsg;
                                    }
                                    else if (item.Course_UseTime + more <= item.Course_Time)
                                    {
                                        item.Course_UseTime = item.Course_UseTime + more;
                                    }

                                }
                                updateUseCourseTimes.Add(item);
                            }
                            //批量更新所在班级所在学员课时
                            if (updateUseCourseTimes.Count > 0)
                            {
                                db.Updateable<C_User_CourseTime>(updateUseCourseTimes).ExecuteCommand();
                            }
                            work.CourseTime = span.Hours;

                        }
                        else if (input.StudyMode == 4 && work.Work_Stutas == 0)
                        {
                            C_Subject sub = db.Queryable<C_Subject>().Where(it => it.SubjectId == input.SubjectId).First();
                            C_Project pro = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                            string where = "";
                            if (input.StudentUid > 0)
                            {
                                int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                                where = "c.StudentUid=@StudentUid or ";
                                var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => con.StudentUid == input.StudentUid && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                                if (contansClassIds != null && contansClassIds.Count > 0)
                                {
                                    where += " c.ClasssId in(" + string.Join(",", contansClassIds) + ") or ";
                                }
                            }
                            //判断学员课程是否冲突
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                .Where("c.Id!=@workId and (" + where + " c.TeacherUid=@TeacherUid or c.ListeningName=@ListeningName) and c.StudyMode!=5 and " +
                                "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                ")")
                                .AddParameters(new { workId = input.Id, StudentUid = input.StudentUid, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime, ListeningName = input.ListeningName }).First();
                            if (anyValue != null)
                            {
                                rsg.msg = "此时间段，当天该课程老师或者该学员已有其它课程,无法排课";
                                rsg.code = 0;
                                return rsg;
                            }
                            if (input.AT_Date != work.AT_Date || !input.StartTime.Equals(work.StartTime) || !input.EndTime.Equals(work.EndTime))
                            {

                                recored.Msg = input.ListeningName + "_" + sub.SubjectName + "_" + pro.ProjectName + "试听课课程时间由原时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime +
                                    "已改成" + input.AT_Date.ToString("yyyy/MM/dd") + " " + input.StartTime + "-" + input.EndTime;
                            }
                            sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                            TimeSpan span = Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) - Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime);
                            float spanHours = float.Parse(span.TotalMinutes.ToString()) / 60;
                            work.CourseTime = spanHours;
                            work.StudyMode = input.StudyMode;
                            work.SubjectId = input.SubjectId;
                            work.ProjectId = input.ProjectId;
                            work.AT_Date = input.AT_Date;
                            work.StartTime = input.StartTime;
                            work.EndTime = input.EndTime;
                            work.TeacherUid = input.TeacherUid;
                            work.RangTimeId = input.RangTimeId;
                            work.TA_Uid = input.TA_Uid;
                            work.RoomId = input.RoomId;
                            work.CampusId = teach.CampusId;
                            work.StudentUid = input.StudentUid;
                            if (input.UnitId > 0)
                            {
                                work.UnitId = input.UnitId;
                            }
                            work.Work_Title = "试听课" + input.ListeningName + "_" + sub.SubjectName + "_" + pro.ProjectName;
                        }
                        else if (input.StudyMode == 5 || input.StudyMode == 6)
                        {
                            C_Subject sub = db.Queryable<C_Subject>().Where(it => it.SubjectId == input.SubjectId).First();
                            C_Project pro = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                            C_Project_Unit unt = db.Queryable<C_Project_Unit>().Where(it => it.UnitId == input.UnitId).First();
                            //判断学员课程是否冲突
                            string where = "";
                            if (input.StudentUid > 0)
                            {
                                where = "c.StudentUid=@StudentUid or ";
                            }
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                .Where("c.Id!=@workId and (" + where + " c.ListeningName=@ListeningName) and " +
                                "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                ")")
                                .AddParameters(new { workId = input.Id, StudentUid = input.StudentUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime, ListeningName = input.ListeningName }).First();
                            if (anyValue != null)
                            {
                                rsg.msg = "此时间段，当天该学员已有其它课程,无法排课";
                                rsg.code = 0;
                                return rsg;
                            }
                            TimeSpan span = Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) - Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime);
                            work.CourseTime = span.Hours;
                            work.StudyMode = input.StudyMode;
                            work.SubjectId = input.SubjectId;
                            work.ProjectId = input.ProjectId;
                            work.UnitId = input.UnitId;
                            work.AT_Date = input.AT_Date;
                            work.StartTime = input.StartTime;
                            work.EndTime = input.EndTime;
                            work.RangTimeId = input.RangTimeId;
                            work.TA_Uid = input.TA_Uid;
                            work.RoomId = input.RoomId;
                            work.StudentUid = input.StudentUid;
                            work.Work_Title = input.Work_Title;
                            if (input.StudyMode == 5)
                            {
                                work.TeacherUid = input.TeacherUid;
                            }
                            else
                            {
                                work.TeacherUid = "";
                            }
                            //添加记录
                            recored.CampusId = vmodel.CampusId;
                            recored.Msg = "更改" + (input.StudyMode == 5 ? "模考" : "实考") + work.Work_Title + input.ListeningName + "_" + sub.SubjectName + "_" + pro.ProjectName + "_单元" + unt.UnitName + ",日期:" + input.AT_Date + " 时间段:" + input.StartTime + "-" + input.EndTime;
                        }
                        else if (input.StudyMode == 7)
                        {
                            sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                            .Where("c.Id!=@workId and c.TeacherUid=@TeacherUid and c.StudyMode!=5 and " +
                            "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                            " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                            " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                            " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                          ")")
                            .AddParameters(new { workId = input.Id, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime }).First();
                            if (anyValue != null)
                            {
                                rsg.code = 0;
                                rsg.msg = "此时间段，当天段该老师已安排其它课程,无法参加会议";
                                return rsg;
                            }
                            work.StartTime = input.StartTime;
                            work.EndTime = input.EndTime;
                            work.AT_Date = input.AT_Date;
                            work.TeacherUid = input.TeacherUid;
                            work.RangTimeId = input.RangTimeId;
                            //记录日志
                            recored.CreateTime = DateTime.Now;
                            recored.CreateUid = input.CreateUid;
                            work.CampusId = teach.CampusId;
                            recored.Msg = "更新" + teach.User_Name + input.AT_Date.ToString("yyyy/MM/dd") + " 教师会议,时间：" + input.StartTime + " - " + input.EndTime;
                        }
                        else if (input.StudyMode == 8 && work.Work_Stutas == 0)
                        {
                            int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                            string where = "";
                            if (input.StudentUid > 0)
                            {
                                where = "c.StudentUid=@StudentUid or ";
                                var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => con.StudentUid == input.StudentUid && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                                if (contansClassIds != null && contansClassIds.Count > 0)
                                {
                                    where += " c.ClasssId in(" + string.Join(",", contansClassIds) + ") or";
                                }
                            }
                            //判断学员课程是否冲突
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                .Where("c.Id!=@workId and (" + where + " c.TeacherUid=@TeacherUid or c.ListeningName=@ListeningName) and c.StudyMode!=5 and " +
                                "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                ")")
                                .AddParameters(new { workId = input.Id, StudentUid = input.StudentUid, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime, ListeningName = input.ListeningName }).First();
                            if (anyValue != null)
                            {
                                rsg.msg = "此时间段，当天该课程老师或者该学员已有其它课程,无法排课";
                                rsg.code = 0;
                                return rsg;
                            }
                            if (input.AT_Date != work.AT_Date || !input.StartTime.Equals(work.StartTime) || !input.EndTime.Equals(work.EndTime))
                            {

                                recored.Msg = "留学规划_" + input.ListeningName + "课程时间由原时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime +
                                    "已改成" + input.AT_Date.ToString("yyyy/MM/dd") + " " + input.StartTime + "-" + input.EndTime;
                            }
                            sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                            TimeSpan span = Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) - Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime);
                            float spanHours = float.Parse(span.TotalMinutes.ToString()) / 60;
                            work.CourseTime = spanHours;
                            work.StudyMode = input.StudyMode;
                            work.AT_Date = input.AT_Date;
                            work.StartTime = input.StartTime;
                            work.EndTime = input.EndTime;
                            work.TeacherUid = input.TeacherUid;
                            work.RangTimeId = input.RangTimeId;
                            work.TA_Uid = input.TA_Uid;
                            work.RoomId = input.RoomId;
                            work.CampusId = teach.CampusId;
                            work.StudentUid = input.StudentUid;
                        }
                        else if (input.StudyMode == 9) {
                            //判断学员课程是否冲突
                            string where = "";
                            if (input.StudentUid > 0)
                            {
                                where = "c.StudentUid=@StudentUid or ";
                            }
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                .Where("c.Id!=@workId and (" + where + " c.ListeningName=@ListeningName) and " +
                                "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                ")")
                                .AddParameters(new { workId = input.Id, StudentUid = input.StudentUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime, ListeningName = input.ListeningName }).First();
                            if (anyValue != null)
                            {
                                rsg.msg = "此时间段，当天该学员已有其它课程,无法排课";
                                rsg.code = 0;
                                return rsg;
                            }
                            TimeSpan span = Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) - Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime);
                            work.CourseTime = span.Hours;
                            work.StudyMode = input.StudyMode;
                            work.AT_Date = input.AT_Date;
                            work.StartTime = input.StartTime;
                            work.EndTime = input.EndTime;
                            work.RangTimeId = input.RangTimeId;
                            work.TA_Uid = input.TA_Uid;
                            work.RoomId = input.RoomId;
                            work.StudentUid = input.StudentUid;
                            work.Work_Title = input.Work_Title;
                            work.TeacherUid = "";
                            work.Comment = input.Comment;
                            //添加记录
                            recored.CampusId = vmodel.CampusId;
                            recored.Msg = "更改学生请假" + work.Work_Title + ",日期:" + input.AT_Date + " 时间段:" + input.StartTime + "-" + input.EndTime;
                        }
                        else
                        {
                            if (work.Work_Stutas == 1)
                            {
                                rsg.msg = "上课已完成！无法修改";
                                rsg.code = 0;
                                return rsg;
                            }
                            else
                            {
                                sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                                C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                .Where("c.Id!=@workId and c.TeacherUid=@TeacherUid and c.StudyMode!=5 and " +
                                "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                              ")")
                                .AddParameters(new { workId = input.Id, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime }).First();
                                if (anyValue != null)
                                {
                                    rsg.code = 0;
                                    rsg.msg = "此时间段，当天段该老师已安排其它课程,无法安排休息";
                                    return rsg;
                                }
                                work.Work_Title = teach.User_Name + input.AT_Date.ToString("yyyy/MM/dd") + " 休息";
                                work.StartTime = input.StartTime;
                                work.EndTime = input.EndTime;
                                work.AT_Date = input.AT_Date;
                                work.TeacherUid = input.TeacherUid;
                                work.RangTimeId = input.RangTimeId;
                                //记录日志
                                recored.CreateTime = DateTime.Now;
                                recored.CreateUid = input.CreateUid;
                                work.CampusId = teach.CampusId;
                                recored.Msg = "更新" + teach.User_Name + input.AT_Date.ToString("yyyy/MM/dd") + " 休息,时间：" + input.StartTime + " - " + input.EndTime;

                            }
                        }
                        work.UpdateUid = input.UpdateUid;
                        work.UpdateTime = DateTime.Now;
                        db.Updateable<C_Course_Work>(work).ExecuteCommand();
                        rsg.msg = "更新课程成功";
                        recored.CreateTime = DateTime.Now;
                        recored.CreateUid = input.CreateUid;
                        recored.CampusId = work.CampusId;
                        db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                    }
                    else
                    {
                        string unitName = "";
                        if (input.ProjectId > 0&& input.StudyMode == 2)
                        {
                            var projectModel = db.Queryable<C_Project>().Where(fv => fv.ProjectId == input.ProjectId).First();
                            input.Work_Title = input.Work_Title + "_" + projectModel.ProjectName;
                        }
                        if (input.UnitId > 0 && input.StudyMode ==1)
                        {
                          
                            var unt = db.Queryable<C_Project_Unit>().Where(it => it.UnitId == input.UnitId).First();
                            unitName = "_" + unt.UnitName;
                            input.Work_Title += unitName;
                        }
                        List<int> shitUserClassIds = null;
                        if ((input.StudyMode == 4|| input.StudyMode == 8) && input.StudentUid > 0) {
                            int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                            shitUserClassIds = db.Queryable<C_Contrac_Child>().Where(con => con.StudentUid == input.StudentUid && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                        }
                        foreach (var wkTime in vmodel.WorkDateGroup) {
                            #region  1对1原判断代码
                            //1对1
                            //if (input.StudyMode == 1)
                            //{
                            //    int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                            //    var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => con.StudentUid == input.StudentUid && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                            //    string where = "";
                            //    //判断该学员其中的班课是否也起冲突
                            //    if (contansClassIds != null && contansClassIds.Count > 0)
                            //    {
                            //        where += " or c.ClasssId in(" + string.Join(",", contansClassIds) + ")";
                            //    }
                            //    //判断学员课程是否冲突
                            //    C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                            //        .Where("(c.StudentUid=@StudentUid or c.TeacherUid=@TeacherUid" + where + ") and c.StudyMode!=5 and " +
                            //        "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)) " +
                            //      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                            //      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                            //       " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                            //      ")")
                            //        .AddParameters(new { StudentUid = input.StudentUid, TeacherUid = input.TeacherUid, AtDate = wkTime, StartTime = input.StartTime, EndTime = input.EndTime }).First();
                            //    if (anyValue != null)
                            //    {
                            //        rsg.msg = wkTime + "此时间段，当天该课程老师或者学员已有其它课程,无法排课";
                            //        rsg.code = 0;
                            //        return rsg;
                            //    }
                            //    C_Contrac_User u = db.Queryable<C_Contrac_User>().Where(it => it.StudentUid == input.StudentUid).First();
                            //    TimeSpan span = Convert.ToDateTime(wkTime + " " + input.EndTime) - Convert.ToDateTime(wkTime + " " + input.StartTime);
                            //    sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                            //    float hourse = float.Parse(span.TotalMinutes.ToString()) / 60;
                            //    if (input.IsUsePresent == 0)
                            //    {
                            //        C_User_CourseTime useCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == input.StudentUid && it.Contra_ChildNo.Equals(input.Contra_ChildNo) && it.SubjectId == input.SubjectId && it.ProjectId == input.ProjectId).First();
                            //        //计算学员课时
                            //        if (useCourseTime == null)
                            //        {
                            //            rsg.code = 0;
                            //            rsg.msg = "学员未充课时！无法再排课";
                            //            return rsg;
                            //        }
                            //        //判断新学员所剩课时是否足够
                            //        if (useCourseTime.Course_UseTime + hourse > useCourseTime.Course_Time)
                            //        {
                            //            rsg.code = 0;
                            //            rsg.msg = "学员所剩课时不足！无法再排课";
                            //            return rsg;
                            //        }
                            //        else if (useCourseTime.Course_UseTime + hourse <= useCourseTime.Course_Time)
                            //        {
                            //            useCourseTime.Course_UseTime = useCourseTime.Course_UseTime + hourse;
                            //        }
                            //        db.Updateable<C_User_CourseTime>(useCourseTime).ExecuteCommand();
                            //    }
                            //    else
                            //    {
                            //        C_Subject sub = db.Queryable<C_Subject>().Where(it => it.SubjectId == input.SubjectId).First();
                            //        C_Project pro = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                            //        C_User_PresentTime useCourseTime = db.Queryable<C_User_PresentTime>().Where(it => it.StudentUid == input.StudentUid && it.Contra_ChildNo == input.Contra_ChildNo).First();
                            //        if (useCourseTime == null)
                            //        {
                            //            rsg.code = 0;
                            //            rsg.msg = "学员未充课时！无法再排课";
                            //            return rsg;
                            //        }
                            //        //判断新学员所剩课时是否足够
                            //        if (useCourseTime.Present_UseTime + hourse > useCourseTime.Present_Time)
                            //        {
                            //            rsg.code = 0;
                            //            rsg.msg = "学员赠送课时不足！无法再排课";
                            //            return rsg;
                            //        }
                            //        else if (useCourseTime.Present_UseTime + hourse <= useCourseTime.Present_Time)
                            //        {
                            //            useCourseTime.Present_UseTime = useCourseTime.Present_UseTime + hourse;
                            //        }
                            //        db.Updateable<C_User_PresentTime>(useCourseTime).ExecuteCommand();
                            //        input.Work_Title = "1对1(" + u.Student_Name + ")_" + sub.SubjectName + "_" + pro.ProjectName+ unitName;
                            //    }
                            //    input.CourseTime = hourse;
                            //    input.CampusId = u.CampusId;
                            //    //添加记录
                            //    recored.CampusId = u.CampusId;
                            //    string msg = "新建课程" + input.Work_Title + ",日期:" + wkTime + " 时间段:" + input.StartTime + "-" + input.EndTime + ", 教师-" + teach.User_Name;
                            //    recored.Msg = msg;

                            //    //公共模块
                            //    input.AT_Date = DateTime.Parse(wkTime);
                            //    input.CreateTime = DateTime.Now;
                            //    db.Insertable<C_Course_Work>(input).ExecuteCommand();
                            //    recored.CreateTime = DateTime.Now;
                            //    recored.CreateUid = input.CreateUid;
                            //    db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                            //}
                            #endregion

                            //小班
                            if (input.StudyMode == 2)
                            {
                                string where = "";
                                int childContracStatu = (int)ConstraChild_Status.RetrunClassOk;//排除退班的学生
                                var contansStudentUids = db.Queryable<C_Contrac_Child>().Where(con => con.ClassId == input.ClasssId && con.StudyMode == 2 && con.Contrac_Child_Status != childContracStatu).Select(con => con.StudentUid).ToList();
                                //判断该班课其中的学员是否也起冲突(不包含班课)
                                if (contansStudentUids != null && contansStudentUids.Count > 0)
                                {
                                    where += " or c.StudentUid in(" + string.Join(",", contansStudentUids) + ")";
                                    //判断该班课学员的其它班课是否也起冲突
                                    int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                                    var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => contansStudentUids.Contains(con.StudentUid) && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                                    if (contansClassIds != null && contansClassIds.Count > 0)
                                    {
                                        where += " or c.ClasssId in(" + string.Join(",", contansClassIds) + ")";
                                    }
                                }
                                List<C_Course_Work> anyValue = db.Queryable<C_Course_Work>("c")
                                .Where("(c.ClasssId=@ClasssId or c.TeacherUid=@TeacherUid" + where + ") and c.StudyMode!=5 and " +
                                "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                 ")")
                                .AddParameters(new { ClasssId = input.ClasssId, TeacherUid = input.TeacherUid, AtDate = wkTime, StartTime = input.StartTime, EndTime = input.EndTime }).ToList();
                                if (anyValue != null&& anyValue.Count>0)
                                {
                                    string infomationMsg = "";
                                    var ctTeacher= anyValue.Find(tc => tc.TeacherUid == input.TeacherUid);
                                    var ctStudents = anyValue.FindAll(st =>st.StudentUid>0&& contansStudentUids.Contains(st.StudentUid));
                                    if (ctTeacher != null) {
                                        var tcModel = db.Queryable<sys_user>().Where(tc => tc.User_ID == input.TeacherUid).First();
                                        infomationMsg += tcModel.User_Name+"老师";
                                    }
                                    if (ctStudents != null && ctStudents.Count > 0) {
                                        List<int> ctUids = new List<int>();
                                        ctStudents.ForEach(ct =>
                                        {
                                            ctUids.Add(ct.StudentUid);
                                        });
                                      List<string> ctStudentName= db.Queryable<C_Contrac_User>().Where(ct => ctUids.Contains(ct.StudentUid)).Select(ct => ct.Student_Name).ToList();
                                      infomationMsg +="("+string.Join(",", ctStudentName)+")学员";
                                    }
                                    rsg.msg = wkTime + "此时间段，当天"+ infomationMsg+ "或者该小班学员已有其它课程,无法排课";
                                    rsg.code = 0;
                                    return rsg;
                                }
                                C_Class cla = db.Queryable<C_Class>().Where(it => it.ClassId == input.ClasssId).First();
                                sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                                TimeSpan span = Convert.ToDateTime(wkTime + " " + input.EndTime) - Convert.ToDateTime(wkTime + " " + input.StartTime);
                                var hourse = span.Hours;
                                //查询子合同所包含该班级的学员
                                List<C_Contrac_Child> listch = db.Queryable<C_Contrac_Child>().Where(it => it.ClassId == input.ClasssId && it.Contrac_Child_Status != (int)ConstraChild_Status.CanCel).ToList();
                                List<int> childStudentUids = new List<int>();
                                listch.ForEach(it =>
                                {
                                    childStudentUids.Add(it.StudentUid);
                                });
                                List<C_User_CourseTime> addUseCourses = new List<C_User_CourseTime>();
                                List<C_User_CourseTime> upUseCourses = new List<C_User_CourseTime>();
                                //查询班级下学员课时
                                List<C_User_CourseTime> courseTimelist = db.Queryable<C_User_CourseTime>().Where(it => it.ClassId == input.ClasssId && childStudentUids.Contains(it.StudentUid)).ToList();
                                if (courseTimelist == null)
                                {
                                    listch.ForEach(it =>
                                    {
                                        C_User_CourseTime useCourseTime = new C_User_CourseTime();
                                        useCourseTime.ClassId = it.ClassId;
                                        useCourseTime.Class_Course_Time = 0;
                                        useCourseTime.Class_Course_UseTime = hourse;
                                        useCourseTime.Contra_ChildNo = it.Contra_ChildNo;
                                        useCourseTime.SubjectId = cla.SubjectId;
                                        useCourseTime.ProjectId = input.ProjectId;
                                        useCourseTime.StudentUid = it.StudentUid;
                                        useCourseTime.CreateTime = DateTime.Now;
                                        useCourseTime.CreateUid = input.CreateUid;
                                        addUseCourses.Add(useCourseTime);
                                    });
                                }
                                else
                                {
                                    listch.ForEach(it =>
                                    {
                                        var useCouseTime = courseTimelist.Find(iv => iv.StudentUid == it.StudentUid && iv.ClassId == it.ClassId && iv.Contra_ChildNo.Equals(it.Contra_ChildNo));
                                        if (useCouseTime != null)
                                        {
                                            useCouseTime.Class_Course_UseTime = useCouseTime.Class_Course_UseTime + hourse;
                                            useCouseTime.UpdateTime = DateTime.Now;
                                            upUseCourses.Add(useCouseTime);
                                        }
                                        else
                                        {
                                            C_User_CourseTime useCourseTime = new C_User_CourseTime();
                                            useCourseTime.ClassId = it.ClassId;
                                            useCourseTime.Class_Course_Time = 0;
                                            useCourseTime.Class_Course_UseTime = hourse;
                                            useCourseTime.Contra_ChildNo = it.Contra_ChildNo;
                                            useCourseTime.SubjectId = cla.SubjectId;
                                            useCourseTime.ProjectId = input.ProjectId;
                                            useCourseTime.StudentUid = it.StudentUid;
                                            useCourseTime.CreateTime = DateTime.Now;
                                            useCourseTime.CreateUid = input.CreateUid;
                                            addUseCourses.Add(useCourseTime);

                                        }
                                    });

                                }
                                //添加学员班级课时
                                if (addUseCourses.Count > 0)
                                {

                                    db.Insertable<C_User_CourseTime>(addUseCourses).ExecuteCommand();
                                }
                                //更新学员班级课时
                                if (upUseCourses.Count > 0)
                                {
                                    var isUseClass = upUseCourses.Find(vl => vl.Course_UseTime > 0);
                                    if (isUseClass != null)
                                    {
                                        upUseCourses.ForEach(vl =>
                                        {
                                            vl.Class_Course_UseTime = isUseClass.Class_Course_UseTime;
                                        });
                                    }
                                    db.Updateable<C_User_CourseTime>(upUseCourses).ExecuteCommand();
                                }
                                input.CourseTime = hourse;
                                input.CampusId = cla.CampusId;
                                //添加记录
                                recored.CampusId = cla.CampusId;
                                recored.Msg = "新增班课(" + input.Work_Title + ")" + ",日期: " + input.AT_Date.ToString("yyyy-MM-dd") + " 时间段: " + input.StartTime + " - " + input.EndTime + ",教师 - " + teach.User_Name;

                                //公共模块
                                input.AT_Date = DateTime.Parse(wkTime);
                                input.CreateTime = DateTime.Now;
                                db.Insertable<C_Course_Work>(input).ExecuteCommand();
                                recored.CreateTime = DateTime.Now;
                                recored.CreateUid = input.CreateUid;
                                db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                            }
                            //休息
                            else if (input.StudyMode == 3)
                            {
                                sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                                C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                .Where("c.TeacherUid=@TeacherUid and c.StudyMode<>5 and " +
                                "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                  ")")
                                .AddParameters(new { TeacherUid = input.TeacherUid, AtDate = wkTime, StartTime = input.StartTime, EndTime = input.EndTime }).First();
                                if (anyValue != null)
                                {
                                    rsg.code = 0;
                                    rsg.msg = "此时间段，当天段该老师已安排其它课程,无法安排休息";
                                    return rsg;
                                }
                                input.Work_Title = teach.User_Name + wkTime + " 休息";
                                //记录日志
                                input.CampusId = teach.CampusId;
                                recored.CampusId = teach.CampusId;
                                recored.Msg = "新增" + teach.User_Name + wkTime + " 休息时间：" + input.StartTime + " - " + input.EndTime;


                                //公共模块
                                input.AT_Date = DateTime.Parse(wkTime);
                                input.CreateTime = DateTime.Now;
                                db.Insertable<C_Course_Work>(input).ExecuteCommand();
                                recored.CreateTime = DateTime.Now;
                                recored.CreateUid = input.CreateUid;
                                db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                            }
                            //留学规划
                            else if (input.StudyMode == 8) {
                                sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                                //判断学员课程是否冲突
                                string where = "";
                                if (input.StudentUid > 0)
                                {
                                    where += "c.StudentUid=@StudentUid or ";
                                    if (shitUserClassIds != null && shitUserClassIds.Count > 0)
                                    {
                                        where += " c.ClasssId in(" + string.Join(",", shitUserClassIds) + ") or ";
                                    }
                                }
                                C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                .Where("c.Id!=@workId and (" + where + "c.TeacherUid=@TeacherUid or c.ListeningName=@ListeningName) and c.StudyMode<>5 and " +
                                "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                ")")
                                .AddParameters(new { workId = input.Id, StudentUid = input.StudentUid, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime, ListeningName = input.ListeningName }).First();
                                if (anyValue != null)
                                                            {
                                                                rsg.msg = wkTime + "此时间段，当天该课程老师或者该小班已有其它课程,无法排课";
                                                                rsg.code = 0;
                                                                return rsg;
                                                            }
                                input.Work_Title = "留学规划_" + input.ListeningName;
                                input.CampusId = teach.CampusId;
                                TimeSpan span = Convert.ToDateTime(wkTime + " " + input.EndTime) - Convert.ToDateTime(wkTime + " " + input.StartTime);
                                float spanHours = float.Parse(span.TotalMinutes.ToString()) / 60;
                                input.CourseTime = spanHours;
                                //添加记录
                                recored.CampusId = teach.CampusId;
                                recored.Msg = "新增留学规划_" + input.ListeningName + ",日期:" + wkTime + " 时间段:" + input.StartTime + "-" + input.EndTime + ", 教师-" + teach.User_Name;
                                //公共模块
                                input.AT_Date = DateTime.Parse(wkTime);
                                input.CreateTime = DateTime.Now;
                                input.Contra_ChildNo = "";
                                db.Insertable<C_Course_Work>(input).ExecuteCommand();
                                recored.CreateTime = DateTime.Now;
                                recored.CreateUid = input.CreateUid;
                                db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                            }
                        }
                        if (input.StudyMode == 1) {
                            var errorMsg = "";

                            C_Contrac_User u = db.Queryable<C_Contrac_User>().Where(it => it.StudentUid == input.StudentUid).First();
                            sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();

                            int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                            var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => con.StudentUid == input.StudentUid && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                            string where = "";
                            //判断该学员其中的班课是否也起冲突
                            if (contansClassIds != null && contansClassIds.Count > 0)
                            {
                                where += " or c.ClasssId in(" + string.Join(",", contansClassIds) + ")";
                            }
                            float takeCourse = 0;
                            foreach (var wkTime in vmodel.WorkDateGroup) {
                                //判断学员课程是否冲突
                                C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                    .Where("(c.StudentUid=@StudentUid or c.TeacherUid=@TeacherUid" + where + ") and c.StudyMode!=5 and " +
                                    "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)) " +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                   " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                  ")")
                                    .AddParameters(new { StudentUid = input.StudentUid, TeacherUid = input.TeacherUid, AtDate = wkTime, StartTime = input.StartTime, EndTime = input.EndTime }).First();
                                if (anyValue != null)
                                {
                                    errorMsg=wkTime + "此时间段，当天该课程老师或者学员已有其它课程,无法排课";
                                }
                                TimeSpan span = Convert.ToDateTime(wkTime + " " + input.EndTime) - Convert.ToDateTime(wkTime + " " + input.StartTime);
                                takeCourse+= float.Parse(span.TotalMinutes.ToString()) / 60;
                            }
                            if (string.IsNullOrEmpty(errorMsg))
                            {
                                if (input.IsUsePresent > 0)
                                {
                                    C_Subject sub = db.Queryable<C_Subject>().Where(it => it.SubjectId == input.SubjectId).First();
                                    C_Project pro = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                                    input.Work_Title = "1对1(" + u.Student_Name + ")_" + sub.SubjectName + "_" + pro.ProjectName + unitName;
                                    C_User_PresentTime useCourseTime = db.Queryable<C_User_PresentTime>().Where(it => it.StudentUid == input.StudentUid && it.Contra_ChildNo == input.Contra_ChildNo).First();
                                    //判断新学员所剩课时是否足够
                                    if (useCourseTime.Present_UseTime + takeCourse > useCourseTime.Present_Time)
                                    {
                                        rsg.code = 0;
                                        rsg.msg = "学员赠送课时不足！无法再排课";
                                        return rsg;
                                    }
                                }
                                else {
                                    C_User_CourseTime useCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == input.StudentUid && it.Contra_ChildNo.Equals(input.Contra_ChildNo) && it.SubjectId == input.SubjectId && it.ProjectId == input.ProjectId).First();
                                    //判断新学员所剩课时是否足够
                                    if (useCourseTime.Course_UseTime + takeCourse > useCourseTime.Course_Time)
                                    {
                                        rsg.code = 0;
                                        rsg.msg = "学员所剩课时不足！无法再排课";
                                        return rsg;
                                    }
                                }
                                foreach (var wkTime in vmodel.WorkDateGroup)
                                {
                                    TimeSpan span = Convert.ToDateTime(wkTime + " " + input.EndTime) - Convert.ToDateTime(wkTime + " " + input.StartTime);
                                    float hourse = float.Parse(span.TotalMinutes.ToString()) / 60;
                                    if (input.IsUsePresent == 0)
                                    {
                                        C_User_CourseTime useCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == input.StudentUid && it.Contra_ChildNo.Equals(input.Contra_ChildNo) && it.SubjectId == input.SubjectId && it.ProjectId == input.ProjectId).First();
                                        //计算学员课时
                                        if (useCourseTime == null)
                                        {
                                            rsg.code = 0;
                                            rsg.msg = "学员未充课时！无法再排课";
                                            return rsg;
                                        }
                                        //判断新学员所剩课时是否足够
                                        if (useCourseTime.Course_UseTime + hourse > useCourseTime.Course_Time)
                                        {
                                            rsg.code = 0;
                                            rsg.msg = "学员所剩课时不足！无法再排课";
                                            return rsg;
                                        }
                                        else if (useCourseTime.Course_UseTime + hourse <= useCourseTime.Course_Time)
                                        {
                                            useCourseTime.Course_UseTime = useCourseTime.Course_UseTime + hourse;
                                        }
                                        db.Updateable<C_User_CourseTime>(useCourseTime).ExecuteCommand();
                                    }
                                    else
                                    {
                                        C_User_PresentTime useCourseTime = db.Queryable<C_User_PresentTime>().Where(it => it.StudentUid == input.StudentUid && it.Contra_ChildNo == input.Contra_ChildNo).First();
                                        if (useCourseTime == null)
                                        {
                                            rsg.code = 0;
                                            rsg.msg = "学员未充课时！无法再排课";
                                            return rsg;
                                        }
                                        //判断新学员所剩课时是否足够
                                        if (useCourseTime.Present_UseTime + hourse > useCourseTime.Present_Time)
                                        {
                                            rsg.code = 0;
                                            rsg.msg = "学员赠送课时不足！无法再排课";
                                            return rsg;
                                        }
                                        else if (useCourseTime.Present_UseTime + hourse <= useCourseTime.Present_Time)
                                        {
                                            useCourseTime.Present_UseTime = useCourseTime.Present_UseTime + hourse;
                                        }
                                        db.Updateable<C_User_PresentTime>(useCourseTime).ExecuteCommand();
                                    }
                                    input.CourseTime = hourse;
                                    input.CampusId = u.CampusId;
                                    //添加记录
                                    recored.CampusId = u.CampusId;
                                    string msg = "新建课程" + input.Work_Title + ",日期:" + wkTime + " 时间段:" + input.StartTime + "-" + input.EndTime + ", 教师-" + teach.User_Name;
                                    recored.Msg = msg;

                                    //公共模块
                                    input.AT_Date = DateTime.Parse(wkTime);
                                    input.CreateTime = DateTime.Now;
                                    db.Insertable<C_Course_Work>(input).ExecuteCommand();
                                    recored.CreateTime = DateTime.Now;
                                    recored.CreateUid = input.CreateUid;
                                    db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                                }
                            }
                            else {
                                rsg.code = 0;
                                rsg.msg = errorMsg;
                                return rsg;
                            }
                        }
                        if (input.StudyMode == 4) {
                            var errorMsg = "";
                            List<C_Course_Work> moreWork = new List<C_Course_Work>();
                            List<C_Course_Work_Recored> moreRecord = new List<C_Course_Work_Recored>();
                            C_Subject sub = db.Queryable<C_Subject>().Where(it => it.SubjectId == input.SubjectId).First();
                            C_Project pro = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                            sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                            string where = "";
                            if (input.StudentUid > 0)
                            {
                                where += "c.StudentUid=@StudentUid or ";
                                if (shitUserClassIds != null && shitUserClassIds.Count > 0)
                                {
                                    where += " c.ClasssId in(" + string.Join(",", shitUserClassIds) + ") or ";
                                }
                            }
                            foreach (var wkTime in vmodel.WorkDateGroup) {
                                C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                    .Where("c.Id!=@workId and (" + where + "c.TeacherUid=@TeacherUid or c.ListeningName=@ListeningName) and c.StudyMode<>5 and " +
                                    "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                    ")")
                                    .AddParameters(new { workId = input.Id, StudentUid = input.StudentUid, TeacherUid = input.TeacherUid, AtDate = DateTime.Parse(wkTime).ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime, ListeningName = input.ListeningName }).First();
                                if (anyValue != null)
                                {
                                    errorMsg+= wkTime + "此时间段，当天该课程老师或者该小班已有其它课程,无法排课";
                                }
                                C_Course_Work_Recored saveRecordModel = new C_Course_Work_Recored();

                                input.Work_Title = "试听课" + input.ListeningName + "_" + sub.SubjectName + "_" + pro.ProjectName;
                                input.CampusId = teach.CampusId;
                                TimeSpan span = Convert.ToDateTime(wkTime + " " + input.EndTime) - Convert.ToDateTime(wkTime + " " + input.StartTime);
                                float spanHours = float.Parse(span.TotalMinutes.ToString()) / 60;
                                input.CourseTime = spanHours;
                                //添加记录
                                saveRecordModel.CampusId = teach.CampusId;
                                saveRecordModel.Msg = "新增试听课" + input.ListeningName + "_" + sub.SubjectName + "_" + pro.ProjectName + ",日期:" + wkTime + " 时间段:" + input.StartTime + "-" + input.EndTime + ", 教师-" + teach.User_Name;
                                saveRecordModel.CreateTime = DateTime.Now;
                                saveRecordModel.CreateUid = input.CreateUid;
                                //公共模块
                                C_Course_Work saveWork = new C_Course_Work();
                                saveWork.StudentUid = input.StudentUid;
                                saveWork.StartTime = input.StartTime;
                                saveWork.EndTime = input.EndTime;
                                saveWork.CourseTime = input.CourseTime;
                                saveWork.Work_Title = input.Work_Title;
                                saveWork.CampusId = input.CampusId;
                                saveWork.StudyMode = input.StudyMode;
                                saveWork.TeacherUid = input.TeacherUid;
                                saveWork.SubjectId = input.SubjectId;
                                saveWork.ProjectId = input.ProjectId;
                                saveWork.UnitId = input.UnitId;
                                saveWork.RoomId = input.RoomId;
                                saveWork.ListeningName = input.ListeningName;
                                saveWork.Work_Stutas = input.Work_Stutas;
                                saveWork.CreateTime = DateTime.Now;
                                saveWork.Contra_ChildNo = "";
                                saveWork.AT_Date = DateTime.Parse(wkTime);
                                saveWork.RangTimeId = input.RangTimeId;
                                saveWork.TA_Uid = input.TA_Uid;
                                moreWork.Add(saveWork);
                                moreRecord.Add(saveRecordModel);
                            }
                            if (!string.IsNullOrEmpty(errorMsg))
                            {
                                rsg.code = 0;
                                rsg.msg = errorMsg;
                                return rsg;
                            }
                            else
                            {
                                if (moreWork.Count > 0)
                                {
                                    db.Insertable<C_Course_Work>(moreWork).ExecuteCommand();
                                }
                                if (moreWork.Count > 0 && moreRecord.Count > 0 && string.IsNullOrEmpty(errorMsg))
                                {
                                    db.Insertable<C_Course_Work_Recored>(moreRecord).ExecuteCommand();
                                }
                            }

                        }
                        if (input.StudyMode == 5) {
                            var errorMsg = "";
                            List<C_Course_Work> mockWork = new List<C_Course_Work>();
                            List<C_Course_Work_Recored> mockRecord = new List<C_Course_Work_Recored>();
                            C_Subject sub = db.Queryable<C_Subject>().Where(it => it.SubjectId == input.SubjectId).First();
                            C_Project pro = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                            C_Project_Unit unt = db.Queryable<C_Project_Unit>().Where(it => it.UnitId == input.UnitId).First();
                            List<C_Contrac_User> listMockUser= db.Queryable<C_Contrac_User>().Where(it => vmodel.arrMockUser.Contains(it.Student_Name)).ToList();
                            foreach (var wkTime in vmodel.WorkDateGroup)
                            {
                                //判断学员课程是否冲突
                                for (var q = 0; q < listMockUser.Count; q++)
                                {
                                    C_Course_Work mockModel = new C_Course_Work();
                                    C_Course_Work_Recored mockRecordModel = new C_Course_Work_Recored();
                                    string where = "";
                                    if (listMockUser[q].StudentUid > 0)
                                    {
                                        where = "c.StudentUid=@StudentUid or ";
                                    }
                                    C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                        .Where("c.Id!=@workId and (" + where + " c.ListeningName=@ListeningName) and " +
                                        "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                        ")")
                                        .AddParameters(new { workId = input.Id, StudentUid = listMockUser[q].StudentUid, AtDate = wkTime, StartTime = input.StartTime, EndTime = input.EndTime, ListeningName = listMockUser[q].Student_Name }).First();
                                    if (anyValue != null)
                                    {
                                        errorMsg += wkTime + "此时间段，当天学员" + listMockUser[q].Student_Name + "已有其它课程,无法排课";
                                    }
                                    mockModel.StartTime = input.StartTime;
                                    mockModel.EndTime = input.EndTime;
                                    mockModel.SubjectId = input.SubjectId;
                                    mockModel.ProjectId = input.ProjectId;
                                    mockModel.UnitId = input.UnitId;
                                    mockModel.StudyMode = input.StudyMode;
                                    mockModel.RangTimeId = input.RangTimeId;
                                    mockModel.RoomId = input.RoomId;
                                    mockModel.TeacherUid = input.TeacherUid;
                                    mockModel.ListeningName = listMockUser[q].Student_Name;
                                    mockModel.StudentUid = listMockUser[q].StudentUid;
                                    mockModel.Work_Title = vmodel.Work_Title + "(" + sub.SubjectName + "_" + pro.ProjectName + "_" + unt.UnitName + ")";
                                    mockModel.CampusId = vmodel.CampusId;
                                    mockModel.IsUsePresent = input.IsUsePresent;
                                    mockModel.Work_Stutas = input.Work_Stutas;
                                    TimeSpan span = Convert.ToDateTime(wkTime + " " + input.EndTime) - Convert.ToDateTime(wkTime + " " + input.StartTime);
                                    mockModel.CourseTime = span.Hours;
                                    mockModel.AT_Date = DateTime.Parse(wkTime);
                                    mockModel.CreateTime = DateTime.Now;
                                    mockModel.CreateUid = input.CreateUid;
                                    mockModel.UpdateUid = input.UpdateUid;
                                    mockModel.TA_Uid = input.TA_Uid;
                                    mockWork.Add(mockModel);
                                    //添加记录
                                    mockRecordModel.CampusId = vmodel.CampusId;
                                    mockRecordModel.Msg = "新增" + (input.StudyMode == 5 ? "模考," : "实考.") + input.Work_Title + ",学员" + listMockUser[q].Student_Name + ",日期:" + wkTime + " 时间段:" + input.StartTime + "-" + input.EndTime;
                                    mockRecordModel.CreateTime = DateTime.Now;
                                    mockRecordModel.CreateUid = input.CreateUid;
                                    mockRecord.Add(mockRecordModel);

                                }
                            }
                            if (!string.IsNullOrEmpty(errorMsg))
                            {
                                rsg.code = 0;
                                rsg.msg = errorMsg;
                                return rsg;
                            }
                            else {
                                if (mockWork.Count > 0)
                                {
                                    db.Insertable<C_Course_Work>(mockWork).ExecuteCommand();
                                }
                                if (mockWork.Count>0&& mockRecord.Count > 0 && string.IsNullOrEmpty(errorMsg))
                                {
                                    db.Insertable<C_Course_Work_Recored>(mockRecord).ExecuteCommand();
                                }
                            }
                        }
                        if (input.StudyMode == 6) {
                            var errorMsg = "";
                            List<C_Course_Work> shikWork = new List<C_Course_Work>();
                            List<C_Course_Work_Recored> shikRecord = new List<C_Course_Work_Recored>();
                            C_Subject sub = db.Queryable<C_Subject>().Where(it => it.SubjectId == input.SubjectId).First();
                            C_Project pro = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                            C_Project_Unit unt = db.Queryable<C_Project_Unit>().Where(it => it.UnitId == input.UnitId).First();
                            List<C_Contrac_User> listMockUser = db.Queryable<C_Contrac_User>().Where(it => vmodel.arrMockUser.Contains(it.Student_Name)).ToList();
                            foreach (var wkTime in vmodel.WorkDateGroup) {
                                for (var q = 0; q < listMockUser.Count; q++) {
                                    C_Course_Work shikModel = new C_Course_Work();
                                    C_Course_Work_Recored shikRecordModel = new C_Course_Work_Recored();
                                    string where = "";
                                    if (listMockUser[q].StudentUid > 0)
                                    {
                                        where = "and c.StudentUid=@StudentUid ";
                                    }
                                    C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                        .Where("c.Id!=@workId  " + where + " and " +
                                        "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                        ")")
                                        .AddParameters(new { workId = input.Id, StudentUid = listMockUser[q].StudentUid, AtDate = wkTime, StartTime = input.StartTime, EndTime = input.EndTime }).First();
                                    if (anyValue != null)
                                    {
                                        errorMsg+=wkTime+"此时间段，学员" + listMockUser[q].Student_Name + "已有其它课程,无法排课";
                                    }
                                    shikModel.AT_Date = DateTime.Parse(wkTime);
                                    shikModel.StartTime = input.StartTime;
                                    shikModel.EndTime = input.EndTime;
                                    shikModel.SubjectId = input.SubjectId;
                                    shikModel.ProjectId = input.ProjectId;
                                    shikModel.UnitId = input.UnitId;
                                    shikModel.StudyMode = input.StudyMode;
                                    shikModel.RangTimeId = input.RangTimeId;
                                    shikModel.ListeningName = listMockUser[q].Student_Name;
                                    shikModel.StudentUid = listMockUser[q].StudentUid;
                                    shikModel.Work_Title = vmodel.Work_Title + "(" + sub.SubjectName + "_" + pro.ProjectName + "_" + unt.UnitName + ")";
                                    shikModel.CampusId = vmodel.CampusId;
                                    TimeSpan span = Convert.ToDateTime(wkTime + " " + input.EndTime) - Convert.ToDateTime(wkTime + " " + input.StartTime);
                                    shikModel.CourseTime = span.Hours;
                                    shikModel.CreateTime = DateTime.Now;
                                    shikModel.CreateUid = input.CreateUid;
                                    shikModel.UpdateUid = input.UpdateUid;
                                    shikModel.TA_Uid = input.TA_Uid;
                                    shikModel.RoomId = input.RoomId;
                                    shikWork.Add(shikModel);
                                    //添加记录
                                    shikRecordModel.CampusId = vmodel.CampusId;
                                    shikRecordModel.Msg = "新增" + (input.StudyMode == 5 ? "模考," : "实考.") + input.Work_Title + ",学员" + listMockUser[q].Student_Name + ",日期:" + wkTime + " 时间段:" + input.StartTime + "-" + input.EndTime;
                                    shikRecordModel.CreateTime = DateTime.Now;
                                    shikRecordModel.CreateUid = input.CreateUid;
                                    shikRecord.Add(shikRecordModel);
                                }
                            }
                            if (!string.IsNullOrEmpty(errorMsg))
                            {
                                rsg.code = 0;
                                rsg.msg = errorMsg;
                                return rsg;
                            }
                            else
                            {
                                if (shikWork.Count > 0)
                                {
                                    db.Insertable<C_Course_Work>(shikWork).ExecuteCommand();
                                }
                                if (shikWork.Count>0&&shikRecord.Count > 0 && string.IsNullOrEmpty(errorMsg))
                                {
                                    db.Insertable<C_Course_Work_Recored>(shikRecord).ExecuteCommand();
                                }
                            }
                        }
                        if (input.StudyMode == 7) {
                            List<C_Course_Work> meetWork = new List<C_Course_Work>();
                            List<C_Course_Work_Recored> meetRecord = new List<C_Course_Work_Recored>();
                            var meetingTeacher = db.Queryable<sys_user>().Where(meet => vmodel.meetingTeacher.Contains(meet.User_Name)).ToList();
                            foreach (var wkTime in vmodel.WorkDateGroup) {
                                for (var q = 0; q < meetingTeacher.Count; q++)
                                {
                                    C_Course_Work meetModel = new C_Course_Work();
                                    C_Course_Work_Recored meetRecordModel = new C_Course_Work_Recored();
                                    C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                    .Where("c.TeacherUid=@TeacherUid and c.StudyMode<>5 and " +
                                    "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                      ")")
                                    .AddParameters(new { TeacherUid = meetingTeacher[q].User_ID, AtDate = wkTime, StartTime = input.StartTime, EndTime = input.EndTime }).First();
                                    if (anyValue != null)
                                    {
                                        rsg.code = 0;
                                        rsg.msg = wkTime + "此时间段，当天" + meetingTeacher[q].User_Name + "老师已安排其它课程,无法参加该会议。";
                                        return rsg;
                                    }
                                    meetModel.TeacherUid = meetingTeacher[q].User_ID;
                                    meetModel.CampusId = meetingTeacher[q].CampusId;
                                    meetModel.AT_Date = DateTime.Parse(wkTime);
                                    meetModel.CreateTime = DateTime.Now;
                                    meetModel.RoomId = input.RoomId;
                                    meetModel.Work_Title = input.Work_Title;
                                    meetModel.StartTime = input.StartTime;
                                    meetModel.EndTime = input.EndTime;
                                    meetModel.RangTimeId = input.RangTimeId;
                                    meetModel.CreateUid = input.CreateUid;
                                    meetModel.CreateTime = DateTime.Now;
                                    meetModel.StudyMode = 7;
                                    meetModel.Status = input.Status;

                                    //记录日志
                                    meetRecordModel.CampusId = meetingTeacher[q].CampusId;
                                    meetRecordModel.Msg = "新增" + input.Work_Title + "-" + meetingTeacher[q].User_Name + wkTime + " 会议时间：" + input.StartTime + " - " + input.EndTime;
                                    //公共模块
                                    meetWork.Add(meetModel);
                                    meetRecordModel.CreateTime = DateTime.Now;
                                    meetRecordModel.CreateUid = input.CreateUid;
                                    meetRecord.Add(meetRecordModel);
                                }
                            }
                            if (meetWork.Count > 0) {
                                db.Insertable<C_Course_Work>(meetWork).ExecuteCommand();
                            }
                            if (meetRecord.Count > 0) {
                               db.Insertable<C_Course_Work_Recored>(meetRecord).ExecuteCommand();
                            }
                        }
                        if (input.StudyMode == 9) {
                            var errorMsg = "";
                            List<C_Course_Work> mockWork = new List<C_Course_Work>();
                            List<C_Course_Work_Recored> mockRecord = new List<C_Course_Work_Recored>();
                            List<C_Contrac_User> listMockUser = db.Queryable<C_Contrac_User>().Where(it => vmodel.arrMockUser.Contains(it.Student_Name)).ToList();
                            foreach (var wkTime in vmodel.WorkDateGroup)
                            {
                                //判断学员课程是否冲突
                                for (var q = 0; q < listMockUser.Count; q++)
                                {
                                    C_Course_Work mockModel = new C_Course_Work();
                                    C_Course_Work_Recored mockRecordModel = new C_Course_Work_Recored();
                                    string where = "";
                                    if (listMockUser[q].StudentUid > 0)
                                    {
                                        where = "c.StudentUid=@StudentUid or ";
                                    }
                                    C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                        .Where("c.Id!=@workId and (" + where + " c.ListeningName=@ListeningName) and " +
                                        "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                        ")")
                                        .AddParameters(new { workId = input.Id, StudentUid = listMockUser[q].StudentUid, AtDate = wkTime, StartTime = input.StartTime, EndTime = input.EndTime, ListeningName = listMockUser[q].Student_Name }).First();
                                    if (anyValue != null)
                                    {
                                        errorMsg += wkTime + "此时间段，当天学员" + listMockUser[q].Student_Name + "已有其它课程,无法安排请假";
                                    }
                                    mockModel.StartTime = input.StartTime;
                                    mockModel.EndTime = input.EndTime;
                                    mockModel.StudyMode = input.StudyMode;
                                    mockModel.RangTimeId = input.RangTimeId;
                                    mockModel.RoomId = input.RoomId;
                                    mockModel.ListeningName = listMockUser[q].Student_Name;
                                    mockModel.StudentUid = listMockUser[q].StudentUid;
                                    mockModel.Work_Title = vmodel.Work_Title + "_" + listMockUser[q].Student_Name;
                                    mockModel.CampusId = vmodel.CampusId;
                                    mockModel.IsUsePresent = input.IsUsePresent;
                                    mockModel.Work_Stutas = input.Work_Stutas;
                                    TimeSpan span = Convert.ToDateTime(wkTime + " " + input.EndTime) - Convert.ToDateTime(wkTime + " " + input.StartTime);
                                    mockModel.CourseTime = span.Hours;
                                    mockModel.AT_Date = DateTime.Parse(wkTime);
                                    mockModel.CreateTime = DateTime.Now;
                                    mockModel.CreateUid = input.CreateUid;
                                    mockModel.UpdateUid = input.UpdateUid;
                                    mockModel.TA_Uid = input.TA_Uid;
                                    mockModel.Comment = input.Comment;
                                    mockWork.Add(mockModel);
                                    //添加记录
                                    mockRecordModel.CampusId = vmodel.CampusId;
                                    mockRecordModel.Msg = "新增学生请假" + input.Work_Title + ",学员" + listMockUser[q].Student_Name + ",日期:" + wkTime + " 时间段:" + input.StartTime + "-" + input.EndTime;
                                    mockRecordModel.CreateTime = DateTime.Now;
                                    mockRecordModel.CreateUid = input.CreateUid;
                                    mockRecord.Add(mockRecordModel);

                                }
                            }
                            if (!string.IsNullOrEmpty(errorMsg))
                            {
                                rsg.code = 0;
                                rsg.msg = errorMsg;
                                return rsg;
                            }
                            else
                            {
                                if (mockWork.Count > 0)
                                {
                                    db.Insertable<C_Course_Work>(mockWork).ExecuteCommand();
                                }
                                if (mockWork.Count > 0 && mockRecord.Count > 0 && string.IsNullOrEmpty(errorMsg))
                                {
                                    db.Insertable<C_Course_Work_Recored>(mockRecord).ExecuteCommand();
                                }
                            }
                        }
                        rsg.msg = "新增课程成功";
                      
                    }
                    db.CommitTran();
                    rsg.code = 200;
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "更新失败-" + er.Message;
                }
            }
            return rsg;
        }


        //拖拽排课
        public ResResult DropCourseWork(int Id, DateTime upAtDate,string uid,string templateId) {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    C_Course_Work work = db.Queryable<C_Course_Work>().Where(it => it.Id ==Id).First();
                    sys_user teach= db.Queryable<sys_user>().Where(it => it.User_ID == work.TeacherUid).First();
                    //1对1
                    if (work.StudyMode == 1 && work.Work_Stutas == 0)
                    {
                        string where = "";
                        if (work.StudentUid > 0)
                        {
                            where = "c.StudentUid=@StudentUid or ";
                            int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                            var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => con.StudentUid == work.StudentUid && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                            if (contansClassIds != null && contansClassIds.Count > 0)
                            {
                                where += " c.ClasssId in(" + string.Join(",", contansClassIds) + ") or ";
                            }

                        }
                        C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                            .Where("c.Id!=@workId and (" + where + " c.TeacherUid=@TeacherUid) and c.StudyMode!=5 and " +
                            "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                         ")")
                            .AddParameters(new { workId = Id, StudentUid = work.StudentUid, TeacherUid = work.TeacherUid, AtDate = upAtDate.ToString("yyyy-MM-dd"), StartTime = work.StartTime, EndTime = work.EndTime }).First();
                        if (anyValue != null)
                        {
                            rsg.code = 0;
                            rsg.msg = "此时间段，当天该课程老师或者学员已有其它课程,无法拖拽";
                            return rsg;
                        }
                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                        if (work.AT_Date != upAtDate)
                        {
                            recored.Msg = "更改课程" + teach.User_Name + "_" + work.Work_Title + "时间由原时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime +
                                "已改成" + upAtDate.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime;
                        }
                        recored.CampusId = work.CampusId;
                        recored.CreateUid = uid;
                        recored.CreateTime = DateTime.Now;
                        db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                    }
                    //班课
                    else if (work.StudyMode == 2 && work.Work_Stutas == 0)
                    {
                        string where = "";
                        int childContracStatu = (int)ConstraChild_Status.RetrunClassOk;//排除退班的学生
                        var contansStudentUids = db.Queryable<C_Contrac_Child>().Where(con => con.ClassId == work.ClasssId && con.StudyMode == 2 && con.Contrac_Child_Status != childContracStatu).Select(con => con.StudentUid).ToList();
                        //判断该班课其中的学员是否也起冲突(不包含班课)
                        if (contansStudentUids != null && contansStudentUids.Count > 0)
                        {
                            where += " or c.StudentUid in(" + string.Join(",", contansStudentUids) + ")";
                            //判断该班课学员的其它班课是否也起冲突
                            int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                            var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => contansStudentUids.Contains(con.StudentUid) && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                            if (contansClassIds != null && contansClassIds.Count > 0)
                            {
                                where += " or c.ClasssId in(" + string.Join(",", contansClassIds) + ")";
                            }
                        }
                        C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                        .Where("c.Id!=@workId and (c.ClasssId=@ClasssId or c.TeacherUid=@TeacherUid " + where + ") and c.StudyMode!=5 and" +
                        " ((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                        " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                        " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                        " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                        ")")
                        .AddParameters(new { workId = Id, ClasssId = work.ClasssId, TeacherUid = work.TeacherUid, AtDate = upAtDate.ToString("yyyy-MM-dd"), StartTime = work.StartTime, EndTime = work.EndTime }).First();
                        if (anyValue != null)
                        {
                            rsg.msg = "此时间段，当天该课程老师或者小班中学生已有其它课程,无法排课";
                            rsg.code = 0;
                            return rsg;
                        }
                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                        if (work.AT_Date != upAtDate)
                        {
                            recored.Msg = "更改课程" + teach.User_Name + "_" + work.Work_Title + "时间由原时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime +
                                "已改成" + upAtDate.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime;
                        }
                        recored.CampusId = work.CampusId;
                        recored.CreateUid = uid;
                        recored.CreateTime = DateTime.Now;
                        db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();


                    }
                    //试听
                    else if (work.StudyMode == 4 && work.Work_Stutas == 0)
                    {
                        string where = "";
                        if (work.StudentUid > 0)
                        {
                            int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                            var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => con.StudentUid == work.StudentUid && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                            if (contansClassIds != null && contansClassIds.Count > 0)
                            {
                                where += " or c.ClasssId in(" + string.Join(",", contansClassIds) + ")";
                            }
                        }
                        C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                        .Where("c.Id!=@workId and (c.StudentUid=@StudentUid or c.TeacherUid=@TeacherUid or c.ListeningName=@ListeningName " + where + ") and c.StudyMode!=5 and " +
                        "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                        " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                        " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                        " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                      ")")
                        .AddParameters(new { workId = Id, StudentUid = work.StudentUid, TeacherUid = work.TeacherUid, AtDate = upAtDate.ToString("yyyy-MM-dd"), StartTime = work.StartTime, EndTime = work.EndTime, ListeningName = work.ListeningName }).First();
                        if (anyValue != null)
                        {
                            rsg.code = 0;
                            rsg.msg = "此时间段，当天该课程老师或者学员已有其它课程,无法拖拽";
                            return rsg;
                        }
                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                        if (work.AT_Date != upAtDate)
                        {
                            recored.Msg = "更改试听课" + teach.User_Name + "_" + work.Work_Title + "时间由原时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime +
                                "已改成" + upAtDate.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime;
                        }
                        recored.CampusId = work.CampusId;
                        recored.CreateUid = uid;
                        recored.CreateTime = DateTime.Now;
                        db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                    }
                    //模考和实考
                    else if (work.StudyMode == 5 || work.StudyMode == 6||work.StudyMode==9)
                    {

                        //判断学员课程是否冲突
                        string where = "";
                        if (work.StudentUid > 0)
                        {
                            where = "c.StudentUid=@StudentUid or ";
                        }
                        C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                            .Where("c.Id!=@workId and (" + where + " c.ListeningName=@ListeningName) and " +
                            "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                          " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                          " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                          " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                            ")")
                            .AddParameters(new { workId = work.Id, StudentUid = work.StudentUid, AtDate = work.AT_Date.ToString("yyyy-MM-dd"), StartTime = work.StartTime, EndTime = work.EndTime, ListeningName = work.ListeningName }).First();
                        if (anyValue != null)
                        {
                            rsg.msg = "此时间段，当天该学员已有其它课程,无法排课";
                            rsg.code = 0;
                            return rsg;
                        }
                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                        if (work.AT_Date != upAtDate)
                        {
                            recored.Msg = "更改课程" + work.Work_Title + "时间由原时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime +
                                "已改成" + upAtDate.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime;
                        }
                        recored.CampusId = work.CampusId;
                        recored.CreateUid = uid;
                        recored.CreateTime = DateTime.Now;
                        db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                    }
                    //会议
                    else if (work.StudyMode == 7)
                    {
                        C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                        .Where("c.Id!=@workId and c.TeacherUid=@TeacherUid and c.StudyMode!=5 and " +
                        "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)) or" +
                        "(CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                        .AddParameters(new { workId = Id, TeacherUid = work.TeacherUid, AtDate = upAtDate.ToString("yyyy-MM-dd"), StartTime = work.StartTime, EndTime = work.EndTime }).First();
                        if (anyValue != null)
                        {
                            rsg.code = 0;
                            rsg.msg = "此时间段，当天段该老师休息或者已有其它课程,无法拖拽";
                            return rsg;
                        }
                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                        recored.CreateTime = DateTime.Now;
                        recored.CreateUid = uid;
                        recored.CampusId = work.CampusId;
                        recored.Msg = teach.User_Name + " 教师会议时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + " - " + work.EndTime + "更改成" + upAtDate.ToString("yyyy/MM/dd") + " " + work.StartTime + " - " + work.EndTime;
                        db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                    }
                    else if (work.StudyMode == 8 && work.Work_Stutas == 0)
                    {
                        string where = "";
                        if (work.StudentUid > 0)
                        {
                            int contracStatus = (int)ConstraChild_Status.RetrunClassOk;
                            var contansClassIds = db.Queryable<C_Contrac_Child>().Where(con => con.StudentUid == work.StudentUid && con.StudyMode == 2 && con.Contrac_Child_Status != contracStatus).Select(con => con.ClassId).ToList();
                            if (contansClassIds != null && contansClassIds.Count > 0)
                            {
                                where += " or c.ClasssId in(" + string.Join(",", contansClassIds) + ")";
                            }
                        }
                        C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                        .Where("c.Id!=@workId and (c.StudentUid=@StudentUid or c.TeacherUid=@TeacherUid " + where + ") and c.StudyMode!=5 and " +
                        "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                        " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                        " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                        " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime))" +
                      ")")
                        .AddParameters(new { workId = Id, StudentUid = work.StudentUid, TeacherUid = work.TeacherUid, AtDate = upAtDate.ToString("yyyy-MM-dd"), StartTime = work.StartTime, EndTime = work.EndTime }).First();
                        if (anyValue != null)
                        {
                            rsg.code = 0;
                            rsg.msg = "此时间段，当天该课程老师或者学员已有其它课程,无法拖拽";
                            return rsg;
                        }
                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                        if (work.AT_Date != upAtDate)
                        {
                            recored.Msg = "更改留学规划" + work.ListeningName + "时间由原时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime +
                                "已改成" + upAtDate.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime;
                        }
                        recored.CampusId = work.CampusId;
                        recored.CreateUid = uid;
                        recored.CreateTime = DateTime.Now;
                        db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                    }
                    else
                    {
                        if (work.Work_Stutas == 1)
                        {
                            rsg.code = 0;
                            rsg.msg = "课程已完成,无法拖拽";
                            return rsg;
                        }
                        else
                        {
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                            .Where("c.Id!=@workId and c.TeacherUid=@TeacherUid and c.StudyMode!=5 and " +
                            "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)) or" +
                            "(CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                            .AddParameters(new { workId = Id, TeacherUid = work.TeacherUid, AtDate = upAtDate.ToString("yyyy-MM-dd"), StartTime = work.StartTime, EndTime = work.EndTime }).First();
                            if (anyValue != null)
                            {
                                rsg.code = 0;
                                rsg.msg = "此时间段，当天段该老师已有其它课程,无法拖拽";
                                return rsg;
                            }
                            work.Work_Title = teach.User_Name + work.AT_Date.ToString("yyyy/MM/dd") + " 休息";
                            C_Course_Work_Recored recored = new C_Course_Work_Recored();
                            recored.CreateTime = DateTime.Now;
                            recored.CreateUid = uid;
                            recored.CampusId = work.CampusId;
                            recored.Msg = teach.User_Name + " 休息时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + " - " + work.EndTime + "更改成" + upAtDate.ToString("yyyy/MM/dd") + " " + work.StartTime + " - " + work.EndTime;
                            db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                        }
                    }
                    work.UpdateUid = uid;
                    work.AT_Date = upAtDate;
                    work.UpdateTime = DateTime.Now;
                    db.Updateable<C_Course_Work>(work).ExecuteCommand();
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "更新课程成功";
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
        /// 删除课程
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ResResult RemoveCourseWork(int Id, string uid,string templateId) {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    C_Course_Work work = db.Queryable<C_Course_Work>().Where(it => it.Id == Id).First();
                    sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == work.TeacherUid).First();
                    if ((work.StudyMode == 1|| work.StudyMode == 8) && work.Work_Stutas == 0)
                    {
                        float oldCourseTime = 0;
                        C_Contrac_User u = db.Queryable<C_Contrac_User>().Where(it=>it.StudentUid==work.StudentUid).First();
                        if (work.IsUsePresent == 0)
                        {
                            C_User_CourseTime useTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == work.StudentUid && it.SubjectId == work.SubjectId && it.ProjectId == work.ProjectId && it.Contra_ChildNo == work.Contra_ChildNo).First();
                            if (useTime != null)
                            {
                                oldCourseTime = useTime.Course_UseTime;
                                useTime.Course_UseTime = useTime.Course_UseTime - work.CourseTime;//恢复学员原来已用课时
                            }
                            C_Course_Work_Recored recored = new C_Course_Work_Recored();
                            recored.Msg = "课程" + teach.User_Name + "_" + work.Work_Title + " " + work.AT_Date.ToString("yyyy/MM//dd") + " " + work.StartTime + "-" + work.EndTime + "已删除，学员" + u.Student_Name + "已使用课时由" + oldCourseTime + "小时变回" + useTime.Course_UseTime + "小时";
                            recored.CampusId = work.CampusId;
                            recored.CreateUid = uid;
                            recored.CreateTime = DateTime.Now;
                            string msg = "课程" + teach.User_Name + "_" + work.Work_Title + " " + work.AT_Date.ToString("yyyy/MM//dd") + " " + work.StartTime + "-" + work.EndTime + "已删除";
                            //更新学员课时
                            db.Updateable<C_User_CourseTime>(useTime).ExecuteCommand();
                            //添加记录
                            db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                        }
                        else {
                            C_User_PresentTime useTime = db.Queryable<C_User_PresentTime>().Where(it => it.StudentUid == work.StudentUid && it.Contra_ChildNo == work.Contra_ChildNo).First();
                            if (useTime != null)
                            {
                                oldCourseTime = useTime.Present_UseTime;
                                useTime.Present_UseTime = useTime.Present_UseTime - work.CourseTime;//恢复学员原来已用课时
                            }
                            C_Course_Work_Recored recored = new C_Course_Work_Recored();
                            recored.Msg = "课程" + teach.User_Name + "_" + work.Work_Title + " " + work.AT_Date.ToString("yyyy/MM//dd") + " " + work.StartTime + "-" + work.EndTime + "已删除，学员" + u.Student_Name + "已使用赠送课时由" + oldCourseTime + "小时变回" + useTime.Present_UseTime + "小时";
                            recored.CampusId = work.CampusId;
                            recored.CreateUid = uid;
                            recored.CreateTime = DateTime.Now;
                            string msg = "课程" + teach.User_Name + "_" + work.Work_Title + " " + work.AT_Date.ToString("yyyy/MM//dd") + " " + work.StartTime + "-" + work.EndTime + "已删除";
                            //更新学员课时
                            db.Updateable<C_User_PresentTime>(useTime).ExecuteCommand();
                            //添加记录
                            db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                        }
                    }
                    else if (work.StudyMode == 2 && work.Work_Stutas == 0)
                    {
                        //查询当前班级所有学员的课时
                        List<C_User_CourseTime> upCourseTime= new List<C_User_CourseTime>();
                        List<C_User_CourseTime> useCourseTimelist = db.Queryable<C_User_CourseTime>().Where(it => it.ClassId == work.ClasssId && it.SubjectId == work.SubjectId).ToList();
                        if (useCourseTimelist != null) {
                            useCourseTimelist.ForEach(it => {
                                it.Class_Course_UseTime = it.Class_Course_UseTime - work.CourseTime;
                                upCourseTime.Add(it);
                            });
                        }
                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                        recored.Msg = "班课" + teach.User_Name + "_" + work.Work_Title+" "+ work.AT_Date.ToString("yyyy/MM//dd") + " " + work.StartTime + "-" + work.EndTime  + "已删除";
                        recored.CreateUid = uid;
                        recored.CampusId = work.CampusId;
                        recored.CreateTime = DateTime.Now;
                        //批量更新学员课时
                        if (upCourseTime.Count > 0) {
                            db.Updateable<C_User_CourseTime>(upCourseTime).ExecuteCommand();
                        }
                        //添加记录
                        db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                    }
                    else
                    {
                        if (work.Work_Stutas == 1)
                        {
                            rsg.code = 0;
                            rsg.msg = "课程已完成,无法删除";
                            return rsg;
                        }

                    }
                    //删除排课
                    db.Deleteable<C_Course_Work>().Where(it => it.Id == work.Id).ExecuteCommand();
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "删除课程成功";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "删除失败-" + er.Message;
                }
            }
            return rsg;
        }

        //复制课程
        public ResResult CopyCourseWork(int[] workIds, string uid, string templateId, DateTime?  workDate=null) {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    List<C_Course_Work> worklist = db.Queryable<C_Course_Work>().In(workIds).ToList();
                    if (worklist != null && worklist.Count > 0) {
                        List<C_User_CourseTime> UpdateCourseTime = new List<C_User_CourseTime>();
                        List<C_Course_Work_Recored> listRecords = new List<C_Course_Work_Recored>();
                        foreach (var it in worklist) {
                            sys_user teach = db.Queryable<sys_user>().Where(t => t.User_ID == it.TeacherUid).First();
                            if (it.StudyMode == 1)
                            {
                                C_Contrac_User u = db.Queryable<C_Contrac_User>().Where(c => c.StudentUid ==it.StudentUid).First();
                                C_Subject sub = db.Queryable<C_Subject>().Where(c => c.SubjectId == it.SubjectId).First();
                                C_Project pro = db.Queryable<C_Project>().Where(c => c.ProjectId == it.ProjectId).First();
                                var oneCourseHose = db.Queryable<C_User_CourseTime>().Where(iv => iv.Contra_ChildNo == it.Contra_ChildNo && iv.StudentUid == it.StudentUid && iv.SubjectId == it.SubjectId && iv.ProjectId == it.ProjectId).First();
                                if (oneCourseHose.Course_Time - oneCourseHose.Course_UseTime < it.CourseTime)
                                {
                                    rsg.msg = "学员"+u.Student_Name+",所学"+sub.SubjectName+"_"+pro.ProjectName+",总课时"+oneCourseHose.Course_Time+"h,已试用课时"+oneCourseHose.Course_UseTime+"h,所剩课时为"+ (oneCourseHose.Course_Time - oneCourseHose.Course_UseTime)+"h"+",所剩课时不足,无法复制相同排课。";
                                    rsg.code = 0;
                                    return rsg;
                                }
                                else
                                {
                                    C_Course_Work work = it;
                                    work.Id = 0;
                                    if (workDate.HasValue)
                                    {
                                        work.AT_Date= workDate.Value;
                                    }
                                    else {
                                        work.AT_Date = work.AT_Date.AddDays(7);
                                    }
                                    //判断该时间断课程是否冲突
                                    C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                        .Where("(c.StudentUid=@StudentUid or c.TeacherUid=@TeacherUid) and " +
                                        "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)) " +
                                      " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                                        .AddParameters(new { StudentUid = work.StudentUid, TeacherUid = work.TeacherUid, AtDate = work.AT_Date.ToString("yyyy-MM-dd"), StartTime = work.StartTime, EndTime = work.EndTime }).First();
                                    if (anyValue != null)
                                    {
                                        rsg.msg = "学员"+u.Student_Name+"或者老师"+teach.User_Name+"在"+work.AT_Date.ToString("yyyy-MM-dd")+"时间段"+work.StartTime+"-"+work.EndTime+"排课有冲突,无法再复制。";
                                        rsg.code = 0;
                                        return rsg;
                                    }
                                    else
                                    {
                                        //添加排课
                                        work.Work_Stutas = 0;
                                        work.CreateTime = DateTime.Now;
                                        work.CreateUid = uid;
                                        work.Comment = "";
                                        db.Insertable<C_Course_Work>(work).ExecuteCommand();

                                        //增加学员使用课时
                                        oneCourseHose.Course_UseTime = oneCourseHose.Course_UseTime + it.CourseTime;
                                        oneCourseHose.UpdateTime = DateTime.Now;
                                        oneCourseHose.UpdateUid = uid;
                                        UpdateCourseTime.Add(oneCourseHose);

                                        //添加排课记录
                                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                                        recored.CampusId = work.CampusId;
                                        recored.Msg = "新增课程" + work.Work_Title + ",日期:" + work.AT_Date.ToString("yyyy-MM-dd") + " 时间段:" + work.StartTime + "-" + work.EndTime + ", 教师-" + teach.User_Name;
                                    }
                                }
                            }
                            else if (it.StudyMode == 2)
                            {

                                C_Class cla = db.Queryable<C_Class>().Where(cl => cl.ClassId == it.ClasssId).First();
                                List<C_User_CourseTime> classCourse = db.Queryable<C_User_CourseTime>().Where(iv => iv.ClassId == it.ClasssId&& iv.SubjectId == it.SubjectId).ToList();
                                //判断班课课时是否足够
                                if (classCourse != null && classCourse.Count > 0&&(classCourse[0].Class_Course_Time-classCourse[0].Class_Course_UseTime)<it.CourseTime) {
                                    rsg.code = 0;
                                    rsg.msg = "班课"+ cla.Class_Name+ "目前只剩"+ (classCourse[0].Class_Course_Time - classCourse[0].Class_Course_UseTime)+"h,无法复制该排课";
                                    return rsg;
                                }

                                C_Course_Work work = it;
                                work.Id = 0;
                                if (workDate.HasValue)
                                {
                                    work.AT_Date = workDate.Value;
                                }
                                else
                                {
                                    work.AT_Date = work.AT_Date.AddDays(7);
                                }
                                //判断班课是否冲突
                                C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                .Where("(c.ClasssId=@ClasssId or c.TeacherUid=@TeacherUid) and " +
                                "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                  " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                                .AddParameters(new { ClasssId = work.ClasssId, TeacherUid = work.TeacherUid, AtDate = work.AT_Date.ToString("yyyy-MM-dd"), StartTime = work.StartTime, EndTime = work.EndTime }).First();
                                if (anyValue != null)
                                {
                                    rsg.msg = "班课" + cla.Class_Name + "或者老师" + teach.User_Name + "在" + work.AT_Date.ToString("yyyy-MM-dd") + "时间段" + work.StartTime + "-" + work.EndTime + "排课有冲突,无法再复制。";
                                    rsg.code = 0;
                                    return rsg;
                                }
                                //扣除该班所有学员课时
                                if (classCourse != null && classCourse.Count > 0) {
                                    foreach (var cor in classCourse) {
                                        cor.Class_Course_UseTime = cor.Class_Course_UseTime + it.CourseTime;
                                        UpdateCourseTime.Add(cor);
                                    }
                                }
                                //添加班级排课
                                work.UpdateTime = DateTime.Now;
                                work.Work_Stutas = 0;
                                work.CreateUid = uid;
                                work.CreateTime = DateTime.Now;
                                work.Comment = "";
                                db.Insertable<C_Course_Work>(work).ExecuteCommand();

                                //添加记录
                                C_Course_Work_Recored recored = new C_Course_Work_Recored();
                                recored.CampusId = work.CampusId;
                                recored.Msg = "新增班课(" + work.Work_Title + ")" + ",日期: " + work.AT_Date.ToString("yyyy - MM - dd") + " 时间段: " + work.StartTime + " - " + work.EndTime + ",教师 - " + teach.User_Name;
                            }
                        }
                        //更新学员课时
                        if (UpdateCourseTime.Count > 0) {
                            db.Updateable<C_User_CourseTime>(UpdateCourseTime).ExecuteCommand();
                        }
                    }
                    db.CommitTran();
                    rsg.code = 200;
                    rsg.msg = "复制课程成功";
                }
                catch (Exception er)
                {
                    db.RollbackTran();//回滚
                    rsg.msg = "复制课程失败-" + er.Message;
                }
            }
            return rsg;
        }


    }
}
