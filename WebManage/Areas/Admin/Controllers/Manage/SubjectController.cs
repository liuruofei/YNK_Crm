using ADT.Models;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
    public class SubjectController : BaseController
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public SubjectController(ICurrencyService currencyService, IC_ContracService contrac, IOptions<RedisConfig> _redisConfig)
        {
            redisConfig = _redisConfig.Value;
            _currencyService = currencyService;
            _contrac = contrac;
        }
        protected override void Init()
        {
            this.MenuID = "C-153";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("C-153", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("C-153", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<C_ProjectModel> pageModel = new PageList<C_ProjectModel>();
            var list = _currencyService.DbAccess().Queryable<C_Subject,C_Campus>((sub,cap)=>new object[] {JoinType.Left,sub.CampusId==cap.CampusId})
                .Where(sub=>sub.CampusId==Convert.ToInt32(campusId))
                .WhereIF(!string.IsNullOrEmpty(title), (sub, cap) => sub.SubjectName.Contains(title)).OrderBy(sub => sub.Sort).Select<C_ProjectModel>((sub, cap)=>new C_ProjectModel {
                 SubjectId=sub.SubjectId,SubjectName=sub.SubjectName,CampusId=sub.CampusId,Lvel1Price=sub.Lvel1Price,Lvel2Price=sub.Lvel2Price,Lvel3Price=sub.Lvel3Price,Lvel4Price=sub.Lvel4Price
                 ,CampusName=cap.CampusName,CreateTime=sub.CreateTime,Sort=sub.Sort,CreateUid=sub.CreateUid,UnitCourse_Time=sub.UnitCourse_Time,UpdateTime=sub.UpdateTime,UpdateUid=sub.UpdateUid,Description=sub.Description
                })
                .ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        /// <summary>
        /// 获取校区列表
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryCampus()
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<C_Campus>().Where(n => n.Status < 1).ToList();
            return Json(rsg);
        }


        [UsersRoleAuthFilter("C-153", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("C-153", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("C-153", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                C_ProjectModel vmodel = _currencyService.DbAccess().Queryable<C_Subject>().Where(f => f.SubjectId == ID).Select<C_ProjectModel>(f=>new C_ProjectModel {SubjectId=f.SubjectId,SubjectName=f.SubjectName,CampusId=f.CampusId,Lvel1Price=f.Lvel1Price,
                    Lvel2Price=f.Lvel2Price,Lvel3Price=f.Lvel3Price,Lvel4Price=f.Lvel4Price,UnitCourse_Time=f.UnitCourse_Time,Sort=f.Sort,Status=f.Status,CreateTime=f.CreateTime,CreateUid=f.CreateUid,UpdateTime=f.UpdateTime,
                    UpdateUid=f.UpdateUid,Description=f.Description
                } ).First();
                if (vmodel != null) {
                    vmodel.ProjectList = _currencyService.DbAccess().Queryable<C_Project>().Where(ite => ite.SubjectId == vmodel.SubjectId).ToList();
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
        [UsersRoleAuthFilter("C-153", "Add,Edit")]
        public IActionResult SaveInfo(SubjectInput vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (string.IsNullOrEmpty(vmodel.SubjectName))
                    return Json(new { code = 0, msg = "考试类型名称不能为空" });
                vmodel.CreateUid = userId;
                rsg=_contrac.SaveSubject(vmodel);
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }

        public IActionResult SetSort(C_Subject vmodel)
        {
            int code = 0;
            try
            {
                if (vmodel.SubjectId > 0)
                {
                    var result = _currencyService.DbAccess().Updateable<C_Subject>().SetColumns(n => new C_Subject { Sort = vmodel.Sort }).Where(n => n.SubjectId == vmodel.SubjectId).ExecuteCommand();
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
        /// 删除科目
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("C-153", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var result = _currencyService.DbAccess().Deleteable<C_Subject>().Where(p => p.SubjectId== Id).ExecuteCommand();
            if (result > 0)
                return Json(new { code = 200, msg = "删除成功" });
            else
                return Json(new { code = 0, msg = "删除失败" });
        }
    }
}
