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
            try
            {
                if (!string.IsNullOrEmpty(childContrcNo))
                {
                    var model = _currencyService.DbAccess().Queryable<C_Contrac_Child>().Where(ite => ite.Contra_ChildNo.Equals(childContrcNo)).First();
                    projectTimeList = _currencyService.DbAccess().Queryable<C_User_CourseTime, C_Subject, C_Project, C_Class, C_Contrac_User, C_Contrac_Child>((tm, sub, pro, cla, u, chil) => new object[] {
           JoinType.Left,tm.SubjectId==sub.SubjectId,JoinType.Left,tm.ProjectId==pro.ProjectId,JoinType.Left,tm.ClassId==cla.ClassId,JoinType.Left,tm.StudentUid==u.StudentUid,JoinType.Left,tm.Contra_ChildNo==chil.Contra_ChildNo
         }).Where((tm, sub, pro, cla, u) => (tm.Contra_ChildNo == childContrcNo || u.Student_Name == childContrcNo)).Select<C_UserCourseTimeModel>((tm, sub, pro, cla, u, chil) => new C_UserCourseTimeModel
         {
             Id = tm.Id,
             Contra_ChildNo = tm.Contra_ChildNo,
             ProjectId = tm.ProjectId,
             SubjectId = tm.SubjectId,
             ProjectName = pro.ProjectName,
             SubjectName = sub.SubjectName,
             ClassId = tm.ClassId,
             Class_Name = cla.Class_Name,
             Course_Time = tm.Course_Time,
             Course_UseTime = tm.Course_UseTime,
             Class_Course_Time = tm.Class_Course_Time,
             StudentUid = tm.StudentUid,
             Level = tm.Level,
             Lvel1Price = sub.Lvel1Price,
             Lvel2Price = sub.Lvel2Price,
             Lvel3Price = sub.Lvel3Price,
             Lvel4Price = sub.Lvel4Price,
             ContraRate = chil.ContraRate,
             Added_Amount = chil.Added_Amount,
             YhBackTimeSumcount = SqlFunc.Subqueryable<C_User_CourseTime>().Where(pm => pm.Contra_ChildNo == tm.Contra_ChildNo).Sum(pm=>pm.Course_Time)

         }).ToPageList(page, limit, ref total);
                    projectTimeList.ForEach(em => {
                        switch (em.Level)
                        {
                            case 1:
                                em.UnitPrice = em.Lvel1Price * (em.ContraRate / 10);
                                break;
                            case 2:
                                em.UnitPrice = em.Lvel2Price * (em.ContraRate / 10);
                                break;
                            case 3:
                                em.UnitPrice = em.Lvel3Price * (em.ContraRate / 10);
                                break;
                            case 4:
                                em.UnitPrice = em.Lvel4Price * (em.ContraRate / 10);
                                break;
                        };
                        if(em.YhBackTimeSumcount > 0)
                          em.UnitBackAddAmount = Math.Round(em.Added_Amount /Convert.ToDecimal(em.YhBackTimeSumcount));

                    });
                }
            }
            catch (Exception er) { 
            
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
