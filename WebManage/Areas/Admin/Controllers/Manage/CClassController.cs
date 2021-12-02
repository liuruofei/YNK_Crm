using ADT.Models;
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
    public class CClassController : BaseController
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        public CClassController(ICurrencyService currencyService, IOptions<RedisConfig> _redisConfig)
        {
            redisConfig = _redisConfig.Value;
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "C-155";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("C-155", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("C-155", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title,int typeId, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<C_ClassModel> pageModel = new PageList<C_ClassModel>();
            var list = _currencyService.DbAccess().Queryable<C_Class,C_ClassType,C_Campus>((c,ty,ca) => new Object[] { JoinType.Inner, c.TypeId == ty.Id, JoinType.Inner,c.CampusId ==ca.CampusId})
                .Where(c=>c.CampusId==Convert.ToInt32(campusId))
                .WhereIF(!string.IsNullOrEmpty(title),c => c.Class_Name.Contains(title)).WhereIF(typeId>0,c => c.TypeId==typeId).OrderBy(c => c.Sort).Select<C_ClassModel>((c,ty,ca)=>new C_ClassModel { 
            CampusId=c.CampusId,CampusName=ca.CampusName,ClassId=c.ClassId,Class_Name=c.Class_Name,Class_No=c.Class_No,Count_Users=c.Count_Users,Course_Time=c.Course_Time,End_Course_Date=c.End_Course_Date,Start_Course_Date=c.Start_Course_Date
            ,Price=c.Price,TypeId=c.TypeId,StartDate=c.StartDate,Material_Count=c.Material_Count,Remarks=c.Remarks,Sort=c.Sort,Status=c.Status,TypeName=ty.TypeName
            }).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("C-155", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("C-155", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        [UsersRoleAuthFilter("C-155", "Add,Edit")]
        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_Class>().Where(f => f.ClassId == ID).First();
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
        [UsersRoleAuthFilter("C-155", "Add,Edit")]
        public IActionResult SaveInfo(C_Class vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                if (string.IsNullOrEmpty(vmodel.Class_Name))
                    return Json(new { code = 0, msg = "科目名称不能为空" });
                if (vmodel.TypeId<1)
                    return Json(new { code = 0, msg = "班级类型不能为空" });
                if (vmodel.ClassId > 0)
                {
                    C_Class project = _currencyService.DbAccess().Queryable<C_Class>().Where(f => f.ClassId == vmodel.ClassId).First();
                    project.Class_Name = vmodel.Class_Name;
                    project.CampusId = vmodel.CampusId;
                    project.Count_Users = vmodel.Count_Users;
                    project.Course_Time = vmodel.Course_Time;
                    project.End_Course_Date = vmodel.End_Course_Date;
                    project.Material_Count = vmodel.Material_Count;
                    project.Price = vmodel.Price;
                    project.TypeId = vmodel.TypeId;
                    project.SubjectId = vmodel.SubjectId;
                    project.StartDate = vmodel.StartDate;
                    project.Start_Course_Date = vmodel.Start_Course_Date;
                    project.Relevant = vmodel.Relevant;
                    project.Remarks = vmodel.Remarks;
                    project.UpdateTime = DateTime.Now;
                    project.UpdateUid = userId;
                    var result = _currencyService.DbAccess().Updateable<C_Class>(project).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "更新成功";
                    }
                }
                else
                {
                    vmodel.CreateTime = DateTime.Now;
                    vmodel.CreateUid = userId;
                    var result = _currencyService.DbAccess().Insertable<C_Class>(vmodel).ExecuteCommand();
                    if (result > 0)
                    {
                        rsg.code = 200;
                        rsg.msg = "保存成功";
                    }
                }
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
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

        /// <summary>
        /// 获取班级类型
        /// </summary>
        /// <returns></returns>
        public IActionResult QueryClassType()
        {
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            rsg.data = _currencyService.DbAccess().Queryable<C_ClassType>().Where(n => n.Status < 1&&n.CampusId==Convert.ToInt32(campusId)).ToList();
            return Json(rsg);
        }


        public IActionResult SetSort(C_Class vmodel)
        {
            int code = 0;
            try
            {
                if (vmodel.ClassId > 0)
                {
                    var result = _currencyService.DbAccess().Updateable<C_Class>().SetColumns(n => new C_Class { Sort = vmodel.Sort }).Where(n => n.ClassId == vmodel.ClassId).ExecuteCommand();
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
        [UsersRoleAuthFilter("C-155", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var result = _currencyService.DbAccess().Deleteable<C_Class>().Where(p => p.ClassId == Id).ExecuteCommand();
            if (result > 0)
                return Json(new { code = 200, msg = "删除成功" });
            else
                return Json(new { code = 0, msg = "删除失败" });
        }
    }
}
