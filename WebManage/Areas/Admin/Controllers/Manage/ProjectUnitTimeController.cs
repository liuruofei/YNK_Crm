using ADT.Models;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class ProjectUnitTimeController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ProjectService _project;
        public ProjectUnitTimeController(ICurrencyService currencyService, IC_ProjectService project)
        {
            _currencyService = currencyService;
            _project = project;
        }
        protected override void Init()
        {
            this.MenuID = "C-161";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("C-161", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("C-161", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int subjectId, int projectId,int unitId, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<C_ProjectUnitTimeModel> pageModel = new PageList<C_ProjectUnitTimeModel>();
            var list = _currencyService.DbAccess().Queryable<C_Project_Unit_Time, C_Subject, C_Project,C_Project_Unit>((time, sub, pro,unt) => new object[] { JoinType.Left, time.SubjectId == sub.SubjectId, JoinType.Left, time.ProjectId == pro.ProjectId, JoinType.Left,time.UnitId==unt.UnitId })
                .Where((time, sub, pro, unt) => sub.CampusId == Convert.ToInt32(campusId))
                .WhereIF(!string.IsNullOrEmpty(title), (time, sub, pro, unt) => time.Unit_TimeName.Contains(title) || sub.SubjectName.Contains(title) || pro.ProjectName.Contains(title)||unt.UnitName.Contains(title) || time.Unit_TimeType.Contains(title) || time.Unit_TimeName.Contains(title))
                .WhereIF(subjectId > 0, (time, sub, pro, unt) => unt.SubjectId == subjectId)
                .WhereIF(projectId > 0, (time, sub, pro, unt) => unt.ProjectId == projectId)
                .WhereIF(unitId > 0, (time, sub, pro, unt) => unt.UnitId == unitId)
                .Select<C_ProjectUnitTimeModel>((time, sub, pro, unt) => new C_ProjectUnitTimeModel
                {
                    Id=time.Id,
                    UnitId = unt.UnitId,
                    UnitName = unt.UnitName,
                    SubjectId = unt.SubjectId,
                    SubjectName = sub.SubjectName,
                    ProjectId = unt.ProjectId,
                    ProjectName = pro.ProjectName,
                    Unit_TimeName=time.Unit_TimeName,
                    Unit_TimeType=time.Unit_TimeType,
                    At_Date=time.At_Date
                })
                .ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }



        [UsersRoleAuthFilter("C-161", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("C-161", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("C-161", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                C_ProjectUnitTimeModel vmodel = _currencyService.DbAccess().Queryable<C_Project_Unit_Time>().Where(f => f.Id == ID).Select<C_ProjectUnitTimeModel>(iv=>new C_ProjectUnitTimeModel {
                 SubjectId=iv.SubjectId,
                 ProjectId=iv.ProjectId,
                 UnitId=iv.UnitId
                }).First();
                list = vmodel;
                if (vmodel != null)
                {
                    vmodel.UnitTimeList = _currencyService.DbAccess().Queryable<C_Project_Unit_Time>().Where(ite => ite.SubjectId == vmodel.SubjectId && ite.ProjectId == vmodel.ProjectId&&ite.UnitId==vmodel.UnitId).ToList();
                }
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("C-161", "Add,Edit")]
        public IActionResult SaveInfo(ProjectUnitTimeInput vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var result = 0;
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (vmodel.UnitTimeList == null || (vmodel.UnitTimeList != null && vmodel.UnitTimeList.Count == 0))
                    return Json(new { code = 0, msg = "单元考试局不能为空" });
                rsg = _project.SaveProjectUnitTime(vmodel);
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }

        /// <summary>
        /// 获取单元中的考试局
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public IActionResult GetTimeByUnitId(int unitId)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "获取失败" };
            var list = _currencyService.DbAccess().Queryable<C_Project_Unit_Time>().Where(con => con.UnitId == unitId).ToList();
            if (list != null && list.Count > 0)
            {
                rsg.data = list;
                rsg.code = 200;
                rsg.msg = "获取成功";
            }
            return Json(rsg);
        }

        public IActionResult Delete(int Id)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "删除成功" };
            _currencyService.DbAccess().Deleteable<C_Project_Unit_Time>().Where(con => con.Id == Id).ExecuteCommand();
            return Json(rsg);
        }

        /// <summary>
        /// 查询科目集合
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public IActionResult QueryProject(int subjectId)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<C_Project> list = _currencyService.DbAccess().Queryable<C_Project>().Where(it => it.SubjectId == subjectId).ToList();
            rsg.data = list;
            return Json(rsg);
        }

        /// <summary>
        /// 查询单元集合
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public IActionResult QueryUnit(int projetId)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            List<C_Project_Unit> list = _currencyService.DbAccess().Queryable<C_Project_Unit>().Where(it => it.ProjectId== projetId).ToList();
            rsg.data = list;
            return Json(rsg);
        }
    }
}
