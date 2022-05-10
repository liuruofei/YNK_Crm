using ADT.Models;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    public class PartCostBackController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public PartCostBackController(ICurrencyService currencyService, IC_ContracService contrac)
        {
            _currencyService = currencyService;
            _contrac = contrac;
        }
        protected override void Init()
        {
            this.MenuID = "H-155";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("H-155", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///查询合同下的科目课时
        /// </summary>
        /// <param name="childContrcNo"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("H-155", FunctionEnum.Have)]
        public IActionResult QueryProjectTime(string childContrcNo,int page=0, int limit=10) {
            int total = 0;
            PageList<C_UserCourseTimeModel> pageModel = new PageList<C_UserCourseTimeModel>();
            List<C_UserCourseTimeModel> projectTimeList = new List<C_UserCourseTimeModel>();
            if (!string.IsNullOrEmpty(childContrcNo)) {
                var model = _currencyService.DbAccess().Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(childContrcNo)).First();
                projectTimeList = _currencyService.DbAccess().Queryable<C_User_CourseTime, C_Subject, C_Project, C_Class>((time, sub, pro, cla) => new object[] {
           JoinType.Left,time.SubjectId==sub.SubjectId,JoinType.Left,time.ProjectId==pro.ProjectId,JoinType.Left,time.ClassId==cla.ClassId
         }).Where((time, sub, pro) => time.Contra_ChildNo == childContrcNo).Select<C_UserCourseTimeModel>((time, sub, pro, cla) => new C_UserCourseTimeModel
         {
             Id = time.Id,
             Contra_ChildNo = time.Contra_ChildNo,
             ProjectId = time.ProjectId,
             SubjectId = time.SubjectId,
             ProjectName = pro.ProjectName,
             SubjectName = sub.SubjectName,
             ClassId = time.ClassId,
             Class_Name = cla.Class_Name,
             Course_Time = time.Course_Time,
             Course_UseTime = time.Course_UseTime,
             Class_Course_Time = time.Class_Course_Time,
             StudentUid = time.StudentUid,
             Level = time.Level,
             Lvel1Price = sub.Lvel1Price,
             Lvel2Price = sub.Lvel2Price,
             Lvel3Price = sub.Lvel3Price,
             Lvel4Price = sub.Lvel4Price,
         }).ToPageList(page, limit, ref total);
                projectTimeList.ForEach(em => {
                    switch (em.Level)
                    {
                        case 1:
                            em.UnitPrice = em.Lvel1Price * (model.ContraRate / 10);
                            break;
                        case 2:
                            em.UnitPrice = em.Lvel2Price * (model.ContraRate / 10);
                            break;
                        case 3:
                            em.UnitPrice = em.Lvel3Price * (model.ContraRate / 10);
                            break;
                        case 4:
                            em.UnitPrice = em.Lvel4Price * (model.ContraRate / 10);
                            break;
                    }


                });
            }
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = projectTimeList;
            return Json(pageModel);
        }

        [UsersRoleAuthFilter("H-155", "Add,Edit")]
        public IActionResult BackPartCostByTimeId(PartBackCostInput input) {
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            input.CreateUid = userId;
            if (input.BackCourseTime < 0) {
                return Json(new { code = 0, msg = "请输入退课课时" });
            }
            if (input.BackAmount < 0)
            {
                return Json(new { code = 0, msg = "退课金额不能小于0" });
            }
            ResResult rsg = new ResResult() { code = 200, msg = "部分课时退课成功" };
            rsg = _contrac.BackPartCostByTimeId(input);
            return Json(rsg);
        }
    }
}
