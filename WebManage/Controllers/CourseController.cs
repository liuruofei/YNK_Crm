using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Models.Res;

namespace WebManage.Controllers
{
    public class CourseController : Controller
    {

        private ICurrencyService _currencyService;
        public CourseController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取当天课程
        /// </summary>
        /// <param name="startStr"></param>
        /// <param name="endStr"></param>
        /// <returns></returns>
        public IActionResult QueryWorkSource(string startStr, string endStr)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            if (string.IsNullOrEmpty(startStr))
                startStr = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00";
            if (string.IsNullOrEmpty(startStr))
                endStr = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59";
            var list = _currencyService.DbAccess().Queryable<C_Course_Work, C_Subject, C_Project, C_Class, C_Contrac_User, C_Room,sys_user>((cou, sub, pro, cla, conU, room,ta) => new object[] { JoinType.Left,cou.SubjectId==sub.SubjectId,JoinType.Left,cou.ProjectId==pro.ProjectId, JoinType.Left, cou.ClasssId==cla.ClassId, JoinType.Left,
                cou.StudentUid ==conU.StudentUid,JoinType.Left,cou.RoomId==room.Id,JoinType.Left,cou.TeacherUid==ta.User_ID})
                .Where(cou =>(cou.StudyMode==1|| cou.StudyMode == 2 || cou.StudyMode == 4) &&cou.AT_Date.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                .OrderBy(cou=>DateTime.Parse(cou.AT_Date.ToString("yyyy-MM-dd")+" "+ cou.StartTime))
                .Select<CourseWorkModel>((cou,sub,pro,cla,conU,room,ta)=>new CourseWorkModel{ Id=cou.Id,StudentUid=cou.StudentUid,Work_Title=cou.Work_Title,AT_Date=cou.AT_Date,StartTime=cou.StartTime,EndTime=cou.EndTime,StudyMode=cou.StudyMode,TeacherName=ta.User_Name,RoomName=room.RoomName}).ToList();
            rsg.code = 0;
            rsg.data = list;
            return Json(rsg);
        }
    }
}
