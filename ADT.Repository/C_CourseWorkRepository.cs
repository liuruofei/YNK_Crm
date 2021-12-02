using ADT.Models;
using ADT.Models.Enum;
using ADT.Models.ResModel;
using ADT.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADT.Repository
{
    public class C_CourseWorkRepository : BaseRepository<C_Course_Work>, IC_CourseWorkRepository
    {

        /// <summary>
        /// 保存排课
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResResult SaveCourseWork(C_Course_Work input)
        {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
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
                        //1对1模式
                        if (input.StudyMode == 1 && input.Work_Stutas == 0)
                        {
                            C_Contrac_User u = db.Queryable<C_Contrac_User>().Where(it => it.StudentUid == work.StudentUid).First();
                            C_Subject sub = db.Queryable<C_Subject>().Where(it => it.SubjectId == work.SubjectId).First();
                            C_Project pro = db.Queryable<C_Project>().Where(it => it.ProjectId == work.ProjectId).First();
                            if (work.StudentUid != input.StudentUid || work.SubjectId != input.SubjectId || work.ProjectId != input.ProjectId || work.Contra_ChildNo != input.Contra_ChildNo)
                            {
                                C_Contrac_User u2 = db.Queryable<C_Contrac_User>().Where(it => it.StudentUid == input.StudentUid).First();
                                C_Subject sub2 = db.Queryable<C_Subject>().Where(it => it.SubjectId == input.SubjectId).First();
                                C_Project pro2 = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                                recored.Msg = "1对1(" + u.Student_Name + ")_" + sub.SubjectName + "_" + pro.ProjectName + "课程变更为" +
                                    "1对1(" + u2.Student_Name + ")_" + sub2.SubjectName + "_" + pro2.ProjectName;
                            }
                            else
                            {
                                if (input.AT_Date != work.AT_Date || !input.StartTime.Equals(work.StartTime) || !input.EndTime.Equals(work.EndTime))
                                {

                                    recored.Msg = work.Work_Title+ "更改课程时间由原时间" + work.AT_Date.ToString("yyyy/MM//dd") + " " + work.StartTime + "-" + work.EndTime +
                                        "已改成" + input.AT_Date.ToString("yyyy/MM/dd") + " " + input.StartTime + "-" + input.EndTime;
                                }
                                else
                                {
                                    recored.Msg = "1对1(" + u.Student_Name + ")_" + sub.SubjectName + "_" + pro.ProjectName + "已修改";
                                }
                            }
                            work.Contra_ChildNo = input.Contra_ChildNo;
                            work.StudyMode = input.StudyMode;
                            work.Work_Title = input.Work_Title;
                            work.SubjectId = input.SubjectId;
                            work.ProjectId = input.ProjectId;
                            work.AT_Date = input.AT_Date;
                            work.StartTime = input.StartTime;
                            work.EndTime = input.EndTime;
                            work.TeacherUid = input.TeacherUid;
                            work.RangTimeId = input.RangTimeId;
                            work.TA_Uid = input.TA_Uid;
                            work.RoomId = input.RoomId;
                            work.CampusId = u.CampusId;
                            TimeSpan span = Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) - Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime);
                            //如果课时修改的学员是同一个人，则判断课时
                            if (work.StudentUid == input.StudentUid)
                            {
                                //判断学员课程是否冲突
                                C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                    .Where("c.Id!=@workId and (c.StudentUid=@StudentUid or c.TeacherUid=@TeacherUid) and " +
                                    "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                    " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                                    .AddParameters(new { workId = input.Id, StudentUid = input.StudentUid, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime }).First();
                                if (anyValue != null)
                                {
                                    rsg.msg = "此时间段，当天该课程老师或者该小班已有其它课程,无法排课";
                                    rsg.code = 0;
                                    return rsg;
                                }
                                C_User_CourseTime useCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == work.StudentUid && it.Contra_ChildNo.Equals(work.Contra_ChildNo) && it.SubjectId == work.SubjectId && it.ProjectId == work.ProjectId).First();
                                var hourse = span.Hours;
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
                                db.Updateable<C_User_CourseTime>(useCourseTime).ExecuteCommand();
                                work.CourseTime = span.Hours;
                            }
                            else
                            {
                                //判断学员课程是否冲突
                                C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                    .Where("c.Id!=@workId and (c.StudentUid=@StudentUid or c.TeacherUid=@TeacherUid) and " +
                                    "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                                    " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                                    .AddParameters(new { workId = input.Id, StudentUid = input.StudentUid, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime }).First();
                                if (anyValue != null)
                                {
                                    rsg.msg = "此时间段，当天该课程老师或者该小班已有其它课程,无法排课";
                                    rsg.code = 0;
                                    return rsg;
                                }

                                //如果课时修改的学员不是同一个人，则判断课时
                                C_User_CourseTime olduseCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == work.StudentUid && it.Contra_ChildNo.Equals(work.Contra_ChildNo) && it.SubjectId == work.SubjectId && it.ProjectId == work.ProjectId).First();
                                C_User_CourseTime useCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == input.StudentUid && it.Contra_ChildNo.Equals(input.Contra_ChildNo) && it.SubjectId == input.SubjectId && it.ProjectId == input.ProjectId).First();
                                var hourse = span.Hours;
                                //则恢复原学员已用课时
                                olduseCourseTime.Course_UseTime = useCourseTime.Course_UseTime - work.CourseTime;
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
                                db.Updateable<C_User_CourseTime>(olduseCourseTime).ExecuteCommand();
                                db.Updateable<C_User_CourseTime>(useCourseTime).ExecuteCommand();
                                work.StudentUid = input.StudentUid;
                                work.CourseTime = span.Hours;
                            }

                        }
                        //小班模式
                        else if (input.StudyMode == 2 & input.Work_Stutas == 0)
                        {
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                         .Where("c.Id!=@workId and (c.ClasssId=@ClasssId or c.TeacherUid=@TeacherUid) and " +
                         "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))"+
                         " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                         .AddParameters(new { workId = input.Id,ClasssId = input.ClasssId, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime,EndTime = input.EndTime }).First();
                            if (anyValue != null)
                            {
                                rsg.msg = "此时间段，当天该课程老师或者该小班已有其它课程,无法排课";
                                rsg.code = 0;
                                return rsg;
                            }
                            C_Class cla = db.Queryable<C_Class>().Where(it => it.ClassId == work.ClasssId).First();
                            //只更新小班课时，不允许更换小班
                            if (input.AT_Date != work.AT_Date || !input.StartTime.Equals(work.StartTime) || !input.EndTime.Equals(work.EndTime))
                            {

                                recored.Msg = input.Work_Title + "更改课程时间由原时间" + work.AT_Date.ToString("yyyy/MM//dd") + " " + work.StartTime + "-" + work.EndTime +
                                    "已改成" + input.AT_Date.ToString("yyyy/MM/dd") + " " + input.StartTime + "-" + input.EndTime;
                            }
                            else
                            {
                                recored.Msg = cla.Class_Name + "已修改";
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
                            work.CampusId =cla.CampusId;
                            TimeSpan span = Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) - Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime);


                            //查询当前班级所有学员的课时
                            List<C_User_CourseTime> useCourseTimelist = db.Queryable<C_User_CourseTime>().Where(it => it.ClassId == work.ClasssId && it.Contra_ChildNo.Equals(work.Contra_ChildNo)&&it.SubjectId==cla.SubjectId).ToList();
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
                            db.Updateable<C_User_CourseTime>(updateUseCourseTimes).ExecuteCommand();

                            work.CourseTime = span.Hours;

                        }
                        else
                        {
                            if (input.Work_Stutas == 1)
                            {
                                rsg.msg = "上课已完成！无法修改";
                                rsg.code = 0;
                                return rsg;
                            }
                            else {
                                sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                                C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                .Where("c.Id!=@workId and c.TeacherUid=@TeacherUid and " +
                                "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                                .AddParameters(new { workId= input.Id,TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime,EndTime = input.EndTime }).First();
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
                                recored.Msg ="更新"+teach.User_Name + input.AT_Date.ToString("yyyy/MM/dd") + " 休息,时间：" + input.StartTime + " - " + input.EndTime;
                                db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                            }
                        }
                        work.UpdateUid = input.UpdateUid;
                        work.UpdateTime = DateTime.Now;
                        db.Updateable<C_Course_Work>(work).ExecuteCommand();
                        rsg.msg = "更新课程成功";
                    }
                    else
                    {
                        //1对1
                        if (input.StudyMode == 1)
                        {
                            //判断学员课程是否冲突
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                                .Where("(c.StudentUid=@StudentUid or c.TeacherUid=@TeacherUid) and " +
                                "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)) " +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                                .AddParameters(new { StudentUid = input.StudentUid, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime=input.EndTime}).First();
                            if (anyValue != null)
                            {
                                rsg.msg = "此时间段，当天该课程老师或者学员已有其它课程,无法排课";
                                rsg.code = 0;
                                return rsg;
                            }
                            C_Contrac_User u = db.Queryable<C_Contrac_User>().Where(it => it.StudentUid == input.StudentUid).First();
                            //C_Subject sub = db.Queryable<C_Subject>().Where(it => it.SubjectId == input.SubjectId).First();
                            //C_Project pro = db.Queryable<C_Project>().Where(it => it.ProjectId == input.ProjectId).First();
                            TimeSpan span = Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) - Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime);
                            sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                            //计算学员课时
                            C_User_CourseTime useCourseTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == input.StudentUid && it.Contra_ChildNo.Equals(input.Contra_ChildNo) && it.SubjectId == input.SubjectId && it.ProjectId == input.ProjectId).First();
                            var hourse = span.Hours;
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
                            input.CourseTime = hourse;
                            input.CampusId = u.CampusId;
                            //批量更新所在班级所在学员课时
                            db.Updateable<C_User_CourseTime>(useCourseTime).ExecuteCommand();
                            //添加记录
                            recored.CampusId = u.CampusId;
                            recored.Msg = "新增课程" + input.Work_Title + ",日期:" + input.AT_Date.ToString("yyyy-MM-dd") + " 时间段:" + input.StartTime + "-" + input.EndTime + ", 教师-" + teach.User_Name;
                        }
                        //小班
                        else if (input.StudyMode == 2)
                        {
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                            .Where("(c.ClasssId=@ClasssId or c.TeacherUid=@TeacherUid) and " +
                            "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                            .AddParameters(new { ClasssId = input.ClasssId, TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime, EndTime = input.EndTime }).First();
                            if (anyValue != null)
                            {
                                rsg.msg = "此时间段，当天该课程老师或者该小班已有其它课程,无法排课";
                                rsg.code = 0;
                                return rsg;
                            }
                            C_Class cla = db.Queryable<C_Class>().Where(it => it.ClassId == input.ClasssId).First();
                            sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                            TimeSpan span = Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.EndTime) - Convert.ToDateTime(input.AT_Date.ToString("yyyy-MM-dd") + " " + input.StartTime);
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
                                        if (useCouseTime.Class_Course_UseTime < useCouseTime.Class_Course_Time && useCouseTime.Class_Course_Time > 0)
                                            useCouseTime.Class_Course_UseTime = useCouseTime.Class_Course_UseTime + hourse;
                                        else if (useCouseTime.Class_Course_Time == 0)
                                        {
                                            useCouseTime.Class_Course_UseTime = useCouseTime.Class_Course_UseTime + hourse;
                                        }
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
                                db.Updateable<C_User_CourseTime>(upUseCourses).ExecuteCommand();
                            }
                            input.CourseTime = hourse;
                            input.CampusId = cla.CampusId;
                            //添加记录
                            recored.CampusId = cla.CampusId;
                            recored.Msg = "新增班课(" + input.Work_Title + ")" + ",日期: " + input.AT_Date.ToString("yyyy - MM - dd") + " 时间段: " + input.StartTime + " - " + input.EndTime + ",教师 - " + teach.User_Name;
                        }
                        else if(input.StudyMode==3){
                            sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == input.TeacherUid).First();
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                            .Where("c.TeacherUid=@TeacherUid and " +
                            "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                              " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                            .AddParameters(new { TeacherUid = input.TeacherUid, AtDate = input.AT_Date.ToString("yyyy-MM-dd"), StartTime = input.StartTime,EndTime=input.EndTime}).First();
                            if (anyValue != null)
                            {
                                rsg.code = 0;
                                rsg.msg = "此时间段，当天段该老师已安排其它课程,无法安排休息";
                                return rsg;
                            }
                            input.Work_Title = teach.User_Name + input.AT_Date.ToString("yyyy/MM/dd") + " 休息";
                            //记录日志
                            recored.CreateTime = DateTime.Now;
                            recored.CreateUid =input.CreateUid;
                            input.CampusId = teach.CampusId;
                            recored.CampusId = teach.CampusId;
                            recored.Msg = "新增"+teach.User_Name + input.AT_Date.ToString("yyyy/MM/dd") + " 休息时间：" + input.StartTime + " - " + input.EndTime;
                            db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                        }
                        input.CreateTime = DateTime.Now;
                        db.Insertable<C_Course_Work>(input).ExecuteCommand();
                        rsg.msg = "新增课程成功";
                      
                    }
                    recored.CreateTime = DateTime.Now;
                    recored.CreateUid = input.CreateUid;
                    db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
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
        public ResResult DropCourseWork(int Id, DateTime upAtDate,string uid) {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    C_Course_Work work = db.Queryable<C_Course_Work>().Where(it => it.Id ==Id).First();
                    sys_user teach= db.Queryable<sys_user>().Where(it => it.User_ID == work.TeacherUid).First();
                    if (work.StudyMode == 1 && work.Work_Stutas == 0)
                    {
                        C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                            .Where("c.Id!=@workId and (c.StudentUid=@StudentUid or c.TeacherUid=@TeacherUid) and " +
                            "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                        " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                            .AddParameters(new { workId=Id, StudentUid = work.StudentUid,TeacherUid=work.TeacherUid, AtDate = upAtDate.ToString("yyyy-MM-dd"), StartTime = work.StartTime, EndTime = work.EndTime }).First();
                        if (anyValue != null) {
                            rsg.code = 0;
                            rsg.msg = "此时间段，当天该课程老师或者学员已有其它课程,无法拖拽";
                            return rsg;
                        }
                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                        if (work.AT_Date != upAtDate)
                        {
                            recored.Msg = "更改课程"+teach.User_Name+"_"+work.Work_Title+ "时间由原时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime +
                                "已改成" + upAtDate.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime;
                        }
                        recored.CreateUid = uid;
                        recored.CreateTime = DateTime.Now;
                        db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();

                    }
                    else if (work.StudyMode ==2 && work.Work_Stutas == 0)
                    {
                        C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                        .Where("c.Id!=@workId and (c.ClasssId=@ClasssId or c.TeacherUid=@TeacherUid) and" +
                        " ((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime))" +
                        " or (CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                        .AddParameters(new { workId = Id, ClasssId = work.ClasssId, TeacherUid = work.TeacherUid, AtDate=upAtDate.ToString("yyyy-MM-dd"), StartTime = work.StartTime, EndTime = work.EndTime }).First();
                        if (anyValue != null)
                        {
                            rsg.msg = "此时间段，当天该课程老师或者小班已有其它课程,无法排课";
                            rsg.code = 0;
                            return rsg;
                        }
                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                        if (work.AT_Date != upAtDate)
                        {
                            recored.Msg = "更改课程" + teach.User_Name + "_" + work.Work_Title+ "时间由原时间" + work.AT_Date.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime +
                                "已改成" + upAtDate.ToString("yyyy/MM/dd") + " " + work.StartTime + "-" + work.EndTime;
                        }
                        recored.CreateUid = uid;
                        recored.CreateTime = DateTime.Now;
                        db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();
                    }
                    else {
                        if (work.Work_Stutas == 1)
                        {
                            rsg.code = 0;
                            rsg.msg = "课程已完成,无法拖拽";
                            return rsg;
                        }
                        else { 
                            C_Course_Work anyValue = db.Queryable<C_Course_Work>("c")
                            .Where("c.Id!=@workId and c.TeacherUid=@TeacherUid and " +
                            "((CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime)<CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.EndTime,108) AS datetime)) or" +
                            "(CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)>=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@StartTime,108) AS datetime) and  CAST(convert(nvarchar,c.AT_Date,23)+' '+convert(nvarchar,c.StartTime,108) AS datetime)<=CAST(convert(nvarchar,@AtDate,23)+' '+convert(nvarchar,@EndTime,108) AS datetime)))")
                            .AddParameters(new { workId = Id, TeacherUid = work.TeacherUid, AtDate = upAtDate.ToString("yyyy-MM-dd"), StartTime = work.StartTime,EndTime=work.EndTime }).First();
                            if (anyValue != null)
                            {
                                rsg.code = 0;
                                rsg.msg = "此时间段，当天段该老师已有其它课程,无法拖拽";
                                return rsg;
                            }
                            work.Work_Title = teach.User_Name+ work.AT_Date.ToString("yyyy/MM/dd")+" 休息";
                            C_Course_Work_Recored recored = new C_Course_Work_Recored();
                            recored.CreateTime = DateTime.Now;
                            recored.CreateUid = uid;
                            recored.Msg =teach.User_Name + " 休息时间"+ work.AT_Date.ToString("yyyy/MM/dd") +" "+work.StartTime+" - "+work.EndTime+"更改成" + upAtDate.ToString("yyyy/MM/dd")+" "+work.StartTime + " - " + work.EndTime;
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
        public ResResult RemoveCourseWork(int Id, string uid) {
            ResResult rsg = new ResResult();
            using (var db = SqlSugarHelper.GetInstance())
            {
                try
                {
                    db.BeginTran();
                    C_Course_Work work = db.Queryable<C_Course_Work>().Where(it => it.Id == Id).First();
                    sys_user teach = db.Queryable<sys_user>().Where(it => it.User_ID == work.TeacherUid).First();
                    if (work.StudyMode == 1 && work.Work_Stutas == 0)
                    {
                        float oldCourseTime = 0;
                        C_Contrac_User u = db.Queryable<C_Contrac_User>().Where(it=>it.StudentUid==work.StudentUid).First();
                        C_User_CourseTime useTime = db.Queryable<C_User_CourseTime>().Where(it => it.StudentUid == work.StudentUid && it.SubjectId == work.SubjectId && it.ProjectId == work.ProjectId && it.Contra_ChildNo == work.Contra_ChildNo).First();
                        if (useTime != null) {
                            oldCourseTime = useTime.Course_UseTime;
                            useTime.Course_UseTime = useTime.Course_UseTime - work.CourseTime;//恢复学员原来已用课时
                        }
                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                        recored.Msg ="课程"+teach.User_Name + "_" + work.Work_Title+" "+work.AT_Date.ToString("yyyy/MM//dd") + " " + work.StartTime + "-" + work.EndTime+"已删除，学员" + u.Student_Name+"已使用课时由"+oldCourseTime+"小时变回"+useTime.Course_UseTime+"小时";
                        recored.CreateUid = uid;
                        recored.CreateTime = DateTime.Now;
                        //更新学员课时
                        db.Updateable<C_User_CourseTime>(useTime).ExecuteCommand();
                        //添加记录
                        db.Insertable<C_Course_Work_Recored>(recored).ExecuteCommand();

                    }
                    else if (work.StudyMode == 2 && work.Work_Stutas == 0)
                    {
                        //查询当前班级所有学员的课时
                        List<C_User_CourseTime> upCourseTime= new List<C_User_CourseTime>();
                        List<C_User_CourseTime> useCourseTimelist = db.Queryable<C_User_CourseTime>().Where(it => it.ClassId == work.ClasssId && it.Contra_ChildNo.Equals(work.Contra_ChildNo) && it.SubjectId == work.SubjectId).ToList();
                        if (useCourseTimelist != null) {
                            useCourseTimelist.ForEach(it => {
                                it.Class_Course_UseTime = it.Class_Course_UseTime - work.CourseTime;
                                upCourseTime.Add(it);
                            });
                        }
                        C_Course_Work_Recored recored = new C_Course_Work_Recored();
                        recored.Msg = "班课" + teach.User_Name + "_" + work.Work_Title+" "+ work.AT_Date.ToString("yyyy/MM//dd") + " " + work.StartTime + "-" + work.EndTime  + "已删除";
                        recored.CreateUid = uid;
                        recored.CreateTime = DateTime.Now;
                        //批量更新学员课时
                        db.Updateable<C_User_CourseTime>(upCourseTime).ExecuteCommand();
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





    }
}
