using ADT.Common;
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
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class UserContracController : BaseController
    {
        private RedisConfig redisConfig;
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        public UserContracController(ICurrencyService currencyService, IC_ContracService contrac, IOptions<RedisConfig> _redisConfig)
        {
            redisConfig = _redisConfig.Value;
            _currencyService = currencyService;
            _contrac = contrac;
        }
        protected override void Init()
        {
            this.MenuID = "T-350";
        }

        [UsersRoleAuthFilter("T-350", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 更新合同
        /// </summary>
        /// <param name="contraNo"></param>
        /// <returns></returns>
        [UsersRoleAuthFilter("T-350", FunctionEnum.Edit)]
        public IActionResult EditContrc(string contraNo)
        {
            ViewBag.ContraNo = contraNo;
            ContracInfmotion vmodel = _currencyService.DbAccess().Queryable<C_Contrac, C_Contrac_User>((c, cu) => new Object[] { JoinType.Inner, c.StudentUid == cu.StudentUid }).Where((c, cu) => c.ContraNo == contraNo)
                .Select<ContracInfmotion>((c, cu) => new ContracInfmotion { CampusId = c.CampusId, ContraNo = c.ContraNo, Amount = cu.Amount, ContraCenterId = c.ContraCenterId, StudentNo = cu.Student_No, StudentName = cu.Student_Name,
                Sex=cu.Sex,Student_Phone=cu.Student_Phone,Grade=cu.Grade,Student_Wechat=cu.Student_Wechat,InSchool=cu.InSchool,Elder_Name=cu.Elder_Name,Elder_Phone=cu.Elder_Phone,Elder_Wechat=cu.Elder_Wechat,Birthday=cu.Birthday
                
                }).First();
            return View(vmodel);
        }

        public IActionResult ContrcItem(string childContraNo, string contraNo)
        {
            ViewBag.ChildContraNo = childContraNo;
            ViewBag.ContraNo = contraNo;
            return View();
        }

        /// <summary>
        /// 获取主合同及子合同信息
        /// </summary>
        /// <param name="contraNo"></param>
        /// <returns></returns>
        public IActionResult QueryFind(string contraNo)
        {
            ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            ContracInfmotion vmodel = _currencyService.DbAccess().Queryable<C_Contrac, C_Contrac_User>((c, cu) => new Object[] { JoinType.Inner, c.StudentUid == cu.StudentUid }).Where((c, cu) => c.ContraNo == contraNo)
            .Select<ContracInfmotion>((c, cu) => new ContracInfmotion { CampusId = c.CampusId, ContraNo = c.ContraNo, Amount = cu.Amount, ContraCenterId = c.ContraCenterId, StudentNo = cu.Student_No, StudentName = cu.Student_Name }).First();
            vmodel.childList = _currencyService.DbAccess().Queryable<C_Contrac_Child, C_Class>((cont, c) => new Object[] { JoinType.Left, cont.ClassId == c.ClassId }).Where(cont => cont.ContraNo == contraNo).Select<UserChildContracModel>((cont, c) => new UserChildContracModel
            {
                ContraNo = cont.ContraNo,
                Contra_ChildNo = cont.Contra_ChildNo,
                Contrac_Child_Status = cont.Contrac_Child_Status,
                StudyMode = cont.StudyMode,
                StudentUid = cont.StudentUid,
                Saler_Amount = cont.Saler_Amount,
                Original_Amount = cont.Original_Amount,
                Course_Time = cont.Course_Time,
                Class_Course_Time = cont.Class_Course_Time,
                ClassName = c.Class_Name
            }).ToList();
            rsg.data = vmodel;
            return Json(rsg);
        }


        public IActionResult FindChild(string childcontraNo)
        {
            //ResResult rsg = new ResResult() { code = 200, msg = "获取成功" };
            C_ContracChildModel vmodel = _currencyService.DbAccess().Queryable<C_Contrac_Child>().Where(c => c.Contra_ChildNo == childcontraNo)
            .Select<C_ContracChildModel>(c => new C_ContracChildModel
            {
                Id = c.Id,
                StudyStatus = c.StudyStatus,
                StudentUid = c.StudentUid,
                StartTime = c.StartTime,
                SignIn_Data = c.SignIn_Data,
                ClassId = c.ClassId,
                Class_Course_Time = c.Class_Course_Time,
                Cycle = c.Cycle,
                Remarks = c.Remarks,
                StudyMode = c.StudyMode,
                Saler_Amount = c.Saler_Amount,
                ContraNo = c.ContraNo,
                ContraRate = c.ContraRate,
                Contra_ChildNo = c.Contra_ChildNo,
                Contra_Property = c.Contra_Property,
                Discount_Amount = c.Discount_Amount
            ,
                IsPreferential = c.IsPreferential,
                Pay_Amount = c.Pay_Amount,
                Original_Amount = c.Original_Amount
            }).First();
            if (!string.IsNullOrEmpty(childcontraNo))
                vmodel.ListItem = _currencyService.DbAccess().Queryable<C_Contrac_Child_Detail>().Where(n => n.Contra_ChildNo == childcontraNo).ToList();
            return Json(vmodel);
        }

        //学员签约合同列表
        [UsersRoleAuthFilter("T-350", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int studymode, int page = 1, int limit = 10)
        {
            int total = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            var ccUse = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where(u => u.User_ID == userId).Select<sys_role>().First();
            PageList<UserContracModel> pageModel = new PageList<UserContracModel>();
            var list = _currencyService.DbAccess().Queryable(@"(select c.*,stuff((select ','+convert(varchar(25), child.StudyMode) FROM C_Contrac_Child child WHERE child.ContraNo=c.ContraNo AND child.StudentUid =c.StudentUid 
                FOR XML PATH('')), 1, 1, '') as StudyModes,camp.CampusName,contracU.Student_Name,contracU.Student_Phone,cc.User_Name as CCUserName from C_Contrac c left join C_Contrac_User contracU on contracU.StudentUid=c.StudentUid
                left join Sys_User cc on c.CC_Uid=cc.User_ID left join C_Campus camp on c.CampusId=camp.CampusId where c.CampusId=@CampusId)", "orginSql").AddParameters(new { CampusId = campusId })
                .WhereIF(!string.IsNullOrEmpty(title), " charindex(@title,orginSql.Student_Name)>0 or charindex(@title,orginSql.Student_Phone)>0 charindex(@title,orginSql.ContraNo)>0").AddParameters(new { title = title })
                .WhereIF(studymode > 0, "charindex('" + studymode + "',orginSql.StudyModes)>0")
                .WhereIF(ccUse != null && ccUse.Role_Name == "顾问", "orginSql.CC_Uid=@CCuid").AddParameters(new { CCuid=userId})
                .Select<UserContracModel>("*").OrderBy("orginSql.CreateTime desc").ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }

        /// <summary>
        /// 保存子合同
        /// </summary>
        /// <param name="vmodel"></param>
        /// <returns></returns>
        public IActionResult SaveChildContrac(ContracChildInput vmodel)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            if (string.IsNullOrEmpty(vmodel.ContraNo))
            {
                reg.code = 300;
                reg.msg = "缺少参数";
                return Json(reg);
            }
            reg = _contrac.SaveChildContrac(vmodel);
            return Json(reg);
        }

        /// <summary>
        /// 更新主合同
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult SaveContrc(ContracInput input)
        {
            ResResult reg = new ResResult();
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            input.CreateUid = userId;
            reg = _contrac.SaveContrc(input);
            return Json(reg);
        }

        /// <summary>
        /// 审核状态
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="stutas"></param>
        /// <returns></returns>
        public IActionResult Audit(int Id,bool thought)
        {
            ResResult reg = new ResResult();
            reg = _contrac.Audit(Id,thought); 
            return Json(reg);
        }
    }
}
