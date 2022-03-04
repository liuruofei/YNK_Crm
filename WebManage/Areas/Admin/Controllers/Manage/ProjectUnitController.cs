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
    public class ProjectUnitController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ProjectService _project;
        public ProjectUnitController(ICurrencyService currencyService, IC_ProjectService project)
        {
            _currencyService = currencyService;
            _project = project;
        }
        protected override void Init()
        {
            this.MenuID = "C-160";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("C-160", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("C-160", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title,int subjectId,int projectId,int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<C_ProjectUnitModel> pageModel = new PageList<C_ProjectUnitModel>();
            var list = _currencyService.DbAccess().Queryable<C_Project_Unit,C_Subject, C_Project>((unt,sub, pro) => new object[] { JoinType.Left,unt.SubjectId ==sub.SubjectId, JoinType.Left,unt.ProjectId==pro.ProjectId })
                .Where((unt, sub, pro) => sub.CampusId == Convert.ToInt32(campusId))
                .WhereIF(!string.IsNullOrEmpty(title), (unt, sub, pro) => unt.UnitName.Contains(title)||sub.SubjectName.Contains(title) || pro.ProjectName.Contains(title))
                .WhereIF(subjectId > 0, (unt, sub, pro) => unt.SubjectId == subjectId)
                .WhereIF(projectId>0, (unt, sub, pro)=>unt.ProjectId==projectId)
                .OrderBy(unt => unt.Sort).Select<C_ProjectUnitModel>((unt, sub, pro) => new C_ProjectUnitModel
                {
                    UnitId=unt.UnitId,
                    UnitName=unt.UnitName,
                    SubjectId = unt.SubjectId,
                    SubjectName = sub.SubjectName,
                    ProjectId=unt.ProjectId,
                    ProjectName=pro.ProjectName,
                    Sort = unt.Sort
                })
                .ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }



        [UsersRoleAuthFilter("C-160", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("C-160", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("C-160", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                C_ProjectUnitModel vmodel = _currencyService.DbAccess().Queryable<C_Project_Unit>().Where(f => f.UnitId == ID).Select<C_ProjectUnitModel>(f => new C_ProjectUnitModel
                {
                    SubjectId = f.SubjectId,
                    ProjectId = f.ProjectId
                }).First();
                if (vmodel != null)
                {
                    vmodel.UnitList = _currencyService.DbAccess().Queryable<C_Project_Unit>().Where(ite => ite.SubjectId == vmodel.SubjectId&&ite.ProjectId == vmodel.ProjectId).ToList();
                }
                list = vmodel;
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
        [UsersRoleAuthFilter("C-160", "Add,Edit")]
        public IActionResult SaveInfo(ProjectUnitInput vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (vmodel.UnitList==null||(vmodel.UnitList!=null&& vmodel.UnitList.Count==0))
                    return Json(new { code = 0, msg = "科目单元不能为空" });
                rsg = _project.SaveProjectUnit(vmodel);
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }

        public IActionResult GetProjetUnitByProjectId(int projectId) {
            ResResult rsg = new ResResult() { code = 0, msg = "获取失败" };
            var list=_currencyService.DbAccess().Queryable<C_Project_Unit>().Where(con => con.ProjectId == projectId).ToList();
            if (list != null && list.Count > 0) {
                rsg.data = list;
                rsg.code = 200;
                rsg.msg = "获取成功";
            }
            return Json(rsg);
        }


        public IActionResult SetSort(C_Project_Unit vmodel)
        {
            int code = 0;
            try
            {
                if (vmodel.UnitId > 0)
                {
                    var result = _currencyService.DbAccess().Updateable<C_Project_Unit>().SetColumns(n => new C_Project_Unit { Sort = vmodel.Sort }).Where(n => n.UnitId == vmodel.UnitId).ExecuteCommand();
                    if (result > 0)
                    {
                        return Json(new { code = 200, msg = "更新成功" });
                    }
                    else
                    {
                        return Json(new { code = code, msg = "更新失败" });
                    }
                }
            }
            catch (Exception er)
            {

            }
            return Json(new { code = code, msg = "缺少参数" });
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
    }
}
