using ADT.Models;
using ADT.Models.Enum;
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
using WebManage.Models.Req;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class CollectionController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public CollectionController(ICurrencyService currencyService, IC_ContracService contrac) {
            _currencyService = currencyService;
            _contrac = contrac;
        }
        protected override void Init()
        {
            this.MenuID = "L-150";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("L-150", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        [UsersRoleAuthFilter("L-150", FunctionEnum.Have)]
        public IActionResult GetDataSource(CollectionQuery query)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<C_CollectionModel> pageModel = new PageList<C_CollectionModel>();
            var list = _currencyService.DbAccess().Queryable<C_Collection,C_Contrac_User,C_Campus,sys_user>((c, cu,ca,sy) => new Object[] { JoinType.Inner, c.StudentUid==cu.StudentUid,JoinType.Left,cu.CampusId==ca.CampusId, JoinType.Left, c.CreateUid == sy.User_ID }).WhereIF(!string.IsNullOrEmpty(query.title), (c,cu)=>cu.Student_Name.Contains(query.title))
                .Where(c=>c.CampusId==Convert.ToInt32(campusId))
                .WhereIF(query.startTime!=null,(c,cu) => c.Collection_Time>=query.startTime).WhereIF(query.endTime != null, (c, cu) => c.Collection_Time<= query.endTime).OrderBy(c =>c.Registration_Time).Select<C_CollectionModel>((c, cu,ca,sy) => new C_CollectionModel
            {
                CampusId = c.CampusId,
                CampusName =ca.CampusName,
                StudentName = c.StudentName,
                Amount = c.Amount,
                StudentUid = c.StudentUid,
                PayStatus = c.PayStatus,
                RelationShip_Contras = c.RelationShip_Contras,
                Collection_Time = c.Collection_Time,
                PayMothed = c.PayMothed,
                PayImg = c.PayImg,
                Id = c.Id,
                Registration_Time = c.Registration_Time,
                User_Name=sy.User_Name
            }).ToPageList(query.page, query.limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }


        [UsersRoleAuthFilter("L-150", FunctionEnum.Add)]
        public IActionResult Add()
        {
            ViewBag.ID = 0;
            return View();
        }

        [UsersRoleAuthFilter("L-150", FunctionEnum.Edit)]
        public IActionResult Edit(int ID)
        {
            ViewBag.ID = ID;
            return View("Add");
        }

        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                C_CollectionModel model = _currencyService.DbAccess().Queryable<C_Collection>().Where(f => f.Id == ID).Select<C_CollectionModel>(f => new C_CollectionModel {
                    Id = f.Id,
                    StudentUid = f.StudentUid,
                    StudentName = f.StudentName,
                    Amount = f.Amount,
                    DeductAmount = f.DeductAmount,
                    RelationShip_Contras = f.RelationShip_Contras,
                    CampusId = f.CampusId,
                    Collection_Time = f.Collection_Time,
                    Registration_Time = f.Registration_Time
                }).First();
                if(model!=null)
                model.ListCollectionDetail = _currencyService.DbAccess().Queryable<C_Collection_Detail>().Where(n => n.CollectionId == model.Id).ToList();
                list = model;
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }

        public IActionResult QueryUserByName(string username) {
            var list = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(f => f.Student_Name.Contains(username)).ToList();
            return Json(list);
        }
        /// <summary>
        /// 查询该学员下未付款,部分付款子合同
        /// </summary>
        /// <param name="studentUid"></param>
        /// <returns></returns>
        public IActionResult QueryRelationShipChildContrac(int studentUid) {
            var list = _currencyService.DbAccess().Queryable<C_Contrac_Child>().Where(f => f.StudentUid==studentUid&&(f.Pay_Stutas==(int)ConstraChild_Pay_Stutas.NoPay|| f.Pay_Stutas == (int)ConstraChild_Pay_Stutas.PartPay)
            && (f.Contrac_Child_Status == (int)ConstraChild_Status.Confirmationed || f.Contrac_Child_Status == (int)ConstraChild_Status.ChangeClassOk ||f.Contrac_Child_Status == (int)ConstraChild_Status.ChangeOk)).ToList();
            return Json(list);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("L-150", "Add,Edit")]
        public IActionResult SaveInfo(CollectionInput vmodel)
        {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                vmodel.CreateUid = userId;
                vmodel.CampusId =Convert.ToInt32(campusId);
                if (string.IsNullOrEmpty(vmodel.RelationShip_Contras))
                    return Json(new { code = 0, msg = "关联合同不能为空" });
                if (vmodel.StudentUid < 1)
                    return Json(new { code = 0, msg = "学员不能为空" });
                rsg = _contrac.SaveCollection(vmodel);
            }
            else
            {
                rsg.msg = "缺少参数";
            }
            return Json(rsg);
        }


        /// <summary>
        /// 审核到账
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="through"></param>
        /// <returns></returns>
        public IActionResult ConfigCollection(int Id, bool through) {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (Id>0)
            {
                reg = _contrac.ConfigCollection(Id,through,userId);
            }
            else
            {
                reg.msg = "缺少参数";
                reg.code = 300;
            }
            return Json(reg);
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
        /// 获取关联合同下的项目
        /// </summary>
        /// <param name="contrac"></param>
        /// <returns></returns>
        public IActionResult QuerySubjectByContrac(string childcontracNo) {
          var list=_currencyService.DbAccess().Queryable<C_Contrac_Child_Detail,C_Subject,C_Project>((detail, sub, pro)=>new Object[] { JoinType.Left, detail.SubjectId==sub.SubjectId, JoinType.Left,detail.ProjectId==pro.ProjectId })
                .Where((detail, sub, pro) => detail.Contra_ChildNo.Equals(childcontracNo)).Select<C_ContracChildDetailModel>((detail, sub, pro)=>new C_ContracChildDetailModel
                {
                    Id=detail.Id,Contra_ChildNo=detail.Contra_ChildNo,
                    Course_Time=detail.Course_Time,
                    Level=detail.Level,
                    Price=detail.Price,
                    ProjectId=detail.ProjectId,
                    SubjectId=detail.SubjectId,
                    SubjectName=sub.SubjectName,
                    ProjectName=pro.ProjectName,
                    StudentUid=detail.StudentUid
                }).ToList();
          return Json(list);
        }



        /// <summary>
        /// 删除收款记录
        /// </summary>
        /// <param name="id">id集合</param>
        /// <returns></returns>
        [UsersRoleAuthFilter("L-150", FunctionEnum.Delete)]
        public IActionResult Delete(int Id)
        {
            var result = _currencyService.DbAccess().Deleteable<C_Collection>().Where(p => p.Id == Id).ExecuteCommand();
            if (result > 0)
                return Json(new { code = 200, msg = "删除成功" });
            else
                return Json(new { code = 0, msg = "删除失败" });
        }
    }
}
