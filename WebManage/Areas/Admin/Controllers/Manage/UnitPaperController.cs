using ADT.Models;
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
    public class UnitPaperController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ProjectService _project;
        public UnitPaperController(ICurrencyService currencyService, IC_ProjectService project)
        {
            _currencyService = currencyService;
            _project = project;
        }
        protected override void Init()
        {
            this.MenuID = "C-162";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("C-162", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("C-162", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int subjectId, int projectId,int unitId, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<UnitPaperModel> pageModel = new PageList<UnitPaperModel>();
            var list = _currencyService.DbAccess().Queryable<C_Unit_Paper, C_Subject, C_Project,C_Project_Unit>((pper, sub, pro,ut) => new object[] { JoinType.Left, pper.SubjectId == sub.SubjectId, JoinType.Left, pper.ProjectId == pro.ProjectId, JoinType.Left, pper.UnitId ==ut.UnitId})
                .Where((pper, sub, pro) => sub.CampusId == Convert.ToInt32(campusId))
                .WhereIF(!string.IsNullOrEmpty(title), (pper, sub, pro, ut) => pper.PaperCode.Contains(title) || sub.SubjectName.Contains(title) || pro.ProjectName.Contains(title) || ut.UnitName.Contains(title))
                .WhereIF(subjectId > 0, (pper, sub, pro, ut) => pper.SubjectId == subjectId)
                .WhereIF(projectId > 0, (pper, sub, pro, ut) => pper.ProjectId == projectId)
                .WhereIF(unitId > 0, (pper, sub, pro, ut) => pper.UnitId == unitId)
                .OrderBy(pper => pper.PaperId).Select<UnitPaperModel>((pper, sub, pro, ut) => new UnitPaperModel
                {
                    PaperId=pper.PaperId,
                    UnitId = pper.UnitId,
                    UnitName = ut.UnitName,
                    PaperCode = pper.PaperCode,
                    SubjectId = pper.SubjectId,
                    SubjectName = sub.SubjectName,
                    ProjectId = pper.ProjectId,
                    ProjectName = pro.ProjectName,
                    AvgScore=pper.AvgScore
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

        [UsersRoleAuthFilter("C-162", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                C_Unit_Paper vmodel = _currencyService.DbAccess().Queryable<C_Unit_Paper>().Where(f => f.PaperId == ID).First();
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
        [UsersRoleAuthFilter("C-162", "Add,Edit")]
        public IActionResult SaveInfo(C_Unit_Paper vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            int result = 0;
            if (vmodel != null)
            {

                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (vmodel.SubjectId <1||vmodel.ProjectId<1||vmodel.UnitId<1)
                    return Json(new { code = 0, msg = "类别,科目,单元都不能为空" });
                if (string.IsNullOrEmpty(vmodel.PaperCode))
                    return Json(new { code = 0, msg = "试卷编码不能为空" });
                if (string.IsNullOrEmpty(vmodel.AvgScore))
                    return Json(new { code = 0, msg = "分数线不能为空" });
                var model = _currencyService.DbAccess().Queryable<C_Unit_Paper>().Where(f =>f.SubjectId==vmodel.SubjectId&&f.ProjectId==vmodel.ProjectId&&f.UnitId==vmodel.UnitId&&f.PaperCode.ToUpper().Equals(vmodel.PaperCode.Trim().ToUpper())).First();
                if (model != null&&model.PaperId!=vmodel.PaperId) {
                    return Json(new { code = 0, msg = "该试卷编号已经存在!请更换试卷编号" });
                }
                if (vmodel.PaperId > 0)
                {
                    result=_currencyService.DbAccess().Updateable<C_Unit_Paper>(vmodel).ExecuteCommand();
                }
                else {
                    result=_currencyService.DbAccess().Insertable<C_Unit_Paper>(vmodel).ExecuteCommand();
                }
                if (result > 0) {
                    rsg.code = 200;
                    rsg.msg = "保存成功";
                }
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }

        public IActionResult DeleteUnit(int Id)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "删除成功" };
            var anyWorkHas = _currencyService.DbAccess().Queryable<C_Course_Work>().Where(km => km.PaperId == Id).First();
            if (anyWorkHas != null)
            {
                rsg.code = 0;
                rsg.msg = "该试卷已被使用，无法删除";
            }
            else
            {
                _currencyService.DbAccess().Deleteable<C_Unit_Paper>().Where(con => con.PaperId== Id).ExecuteCommand();
            }
            return Json(rsg);
        }
    }
}
