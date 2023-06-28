using ADT.Common;
using ADT.Models;
using ADT.Models.Enum;
using ADT.Models.InputModel;
using ADT.Models.ResModel;
using ADT.Service.IService;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models;
using WebManage.Models.Req;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class CollectionController : BaseController
    {
        private ICurrencyService _currencyService;
        private IC_ContracService _contrac;
        private RedisConfig _redsconfig;
        private WXSetting _wxConfig;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(CollectionController));
        public CollectionController(ICurrencyService currencyService, IC_ContracService contrac, IOptions<WXSetting> wxConfig, IOptions<RedisConfig> redisConfig) {
            _currencyService = currencyService;
            _contrac = contrac;
            _wxConfig = wxConfig.Value;
            _redsconfig = redisConfig.Value;
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
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            var ccUse = _currencyService.DbAccess().Queryable<sys_user, sys_userrole, sys_role>((u, ur, r) => new object[] { JoinType.Inner, u.User_ID == ur.UserRole_UserID, JoinType.Inner, ur.UserRole_RoleID == r.Role_ID }).Where(u => u.User_ID == userId).Select<sys_role>().First();
            PageList<C_CollectionModel> pageModel = new PageList<C_CollectionModel>();
            var list = _currencyService.DbAccess().Queryable<C_Collection,C_Contrac_User,C_Campus,sys_user>((c, cu,ca,sy) => new Object[] { JoinType.Inner, c.StudentUid==cu.StudentUid,JoinType.Left,cu.CampusId==ca.CampusId, JoinType.Left, c.CreateUid == sy.User_ID }).WhereIF(!string.IsNullOrEmpty(query.title), (c,cu)=>cu.Student_Name.Contains(query.title))
                .Where(c=>c.CampusId==Convert.ToInt32(campusId))
                .WhereIF(query.startTime!=null,(c,cu) => c.Collection_Time>=query.startTime).WhereIF(query.endTime != null, (c, cu) => c.Collection_Time<= query.endTime)
                .WhereIF(query.arrearageStatus>0, (c, cu)=>c.ArrearageStatus==query.arrearageStatus)
                //.WhereIF(ccUse != null && ccUse.Role_Name == "顾问", (c, cu, ca, sy)=>c.CreateUid==userId).AddParameters(new { CCuid = userId })
                .OrderBy(c =>c.Collection_Time,OrderByType.Desc)
                .Select<C_CollectionModel>((c, cu,ca,sy) => new C_CollectionModel
            {
                CampusId = c.CampusId,
                CampusName =ca.CampusName,
                StudentName = c.StudentName,
                Amount = c.Amount,
                FilAmount=c.FilAmount,
                koudeductAmount=c.koudeductAmount,
                AddedAmount =c.AddedAmount,
                DeductAmount=c.DeductAmount,
                StudentUid = c.StudentUid,
                PayStatus = c.PayStatus,
                RelationShip_Contras = c.RelationShip_Contras,
                Collection_Time = c.Collection_Time,
                PayMothed = c.PayMothed,
                ArrearageStatus=c.ArrearageStatus,
                PayImg = c.PayImg,
                Id = c.Id,
                Registration_Time = c.Registration_Time,
                User_Name=sy.User_Name,
                RepaymentTotal=SqlFunc.Subqueryable<C_RepaymentRecord>().Where(pm=>pm.CollgeId==c.Id).Sum(pm=>pm.RepaymentAmount)
                }).ToPageList(query.page, query.limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }

        /// <summary>
        /// 收款汇总
        /// </summary>
        /// <param name="title"></param>
        /// <param name="endTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public IActionResult TotalAmount(string title, DateTime? endTime=null,DateTime? startTime=null) {
            ResResult rsg = new ResResult() { code = 0, msg = "获取失败" };
            DateTime now = DateTime.Now;
            if (!startTime.HasValue) {
                startTime= new DateTime(now.Year, now.Month, 1);
            }
            if (!endTime.HasValue)
            {
                endTime = startTime.Value.AddMonths(1).AddDays(-1);
            }
            var result = _currencyService.DbAccess().Queryable<C_Collection>().WhereIF(!string.IsNullOrEmpty(title), a => a.StudentName.Contains(title))
                .Where(a => a.Collection_Time >= startTime.Value && a.Collection_Time <= endTime.Value&&a.FilAmount>0&&a.ArrearageStatus!=1).Sum(a => a.FilAmount);
            rsg.code = 200;
            rsg.msg = "获取成功";
            rsg.data = result;
            return Json(rsg);
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


        [UsersRoleAuthFilter("L-150", FunctionEnum.Edit)]
        public IActionResult AddPayment(int collgeId) {
            ViewBag.ID =collgeId;
            C_Collection model = _currencyService.DbAccess().Queryable<C_Collection>().Where(c => c.Id == collgeId).First();
            return View("AddPayment", model);
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
                    Amount = f.Amount<0?0:f.Amount,
                    FilAmount=f.FilAmount,
                    koudeductAmount=f.koudeductAmount,
                    DeductAmount = f.DeductAmount,
                    AddedAmount=f.AddedAmount,
                    RelationShip_Contras = f.RelationShip_Contras,
                    CampusId = f.CampusId,
                    Collection_Time = f.Collection_Time,
                    Registration_Time = f.Registration_Time,
                    PayStatus=f.PayStatus
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
            && (f.Contrac_Child_Status == (int)ConstraChild_Status.Confirmationed || f.Contrac_Child_Status == (int)ConstraChild_Status.ChangeClassOk ||f.Contrac_Child_Status == (int)ConstraChild_Status.ChangeOk)).OrderBy(f=>f.CreateTime,OrderByType.Desc).ToList();
            return Json(list);
        }

        /// <summary>
        /// 查询合同信息
        /// </summary>
        /// <param name="contracNo"></param>
        /// <returns></returns>
        public IActionResult QueryContracVmodel(string childcontracNo) {
            var vmodel = _currencyService.DbAccess().Queryable<C_Contrac_Child>().Where(i => i.Contra_ChildNo == childcontracNo).First();
            //查询该合同收款是否使用过额外优惠金额
            var anyMount = _currencyService.DbAccess().Queryable<C_Collection>().Where(i => i.RelationShip_Contras == childcontracNo && i.AddedAmount > 0).First();
            if (anyMount!= null) {
                vmodel.Added_Amount = 0;
            }
            return Json(new {code= 200, msg = "获取成功",data=vmodel});
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
        //还款
        public IActionResult SavePayment(C_RepaymentRecord vmodel) {
            ResResult rsg = new ResResult() { code = 0, msg = "保存失败" };
            if (vmodel != null)
            {
                var result = 0;
                var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
                var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
                vmodel.CreateUid = userId;
                vmodel.CreateTime = DateTime.Now;
                if (string.IsNullOrEmpty(vmodel.Contra_ChildNo))
                    return Json(new { code = 0, msg = "合同号不能为空" });
                if (vmodel.Id < 1)
                {
                    var colle = _currencyService.DbAccess().Queryable<C_Collection>().Where(v => v.Id == vmodel.CollgeId).First();
                    var repaymentTotal = _currencyService.DbAccess().Queryable<C_RepaymentRecord>().Where(v=>v.CollgeId==vmodel.CollgeId).Sum(v => v.RepaymentAmount);
                    result =_currencyService.DbAccess().Insertable<C_RepaymentRecord>(vmodel).ExecuteCommand();
                    if (result > 0&& repaymentTotal+vmodel.RepaymentAmount== colle.FilAmount-colle.AddedAmount-colle.DeductAmount) {
                        _currencyService.DbAccess().Updateable<C_Collection>().SetColumns(v => new C_Collection{ArrearageStatus=0}).Where(v=>v.Id==vmodel.CollgeId).ExecuteCommand();
                    }
                }
                else {
                    result = _currencyService.DbAccess().Updateable<C_RepaymentRecord>(vmodel).ExecuteCommand();
                }
                if (result > 0) {
                    rsg.code = 200;
                    rsg.msg="保存成功";
                    return Json(rsg);
                }
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
            var collModel = _currencyService.DbAccess().Queryable<C_Collection>().Where(c => c.Id == Id).First();
            var studentModel = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(c => c.StudentUid == collModel.StudentUid).First();
            if (reg.code==200&&through&&!string.IsNullOrEmpty(collModel.BusinesTitle)&& !string.IsNullOrEmpty(collModel.BusinesCotent)&&(!string.IsNullOrEmpty(studentModel.OpenId)||!string.IsNullOrEmpty(studentModel.Elder2_OpenId)|| !string.IsNullOrEmpty(studentModel.Elder_OpenId)))
            {
                var token = GetWXToken();
                if (collModel.Amount > 0) {
                    //发送业务消息给学生或者家长
                    string payMothName = "";
                    if (collModel.PayMothed == 1)
                        payMothName = "在线支付";
                    else if (collModel.PayMothed ==2)
                        payMothName = "刷卡";
                    else
                        payMothName = "汇款";
                    SendMsg(studentModel.OpenId,_wxConfig.TemplateBusines, token,"业务交易通知",collModel.BusinesTitle,collModel.Collection_Time.ToString("yyyy-MM-dd"),collModel.BusinesCotent,collModel.Amount.ToString(), payMothName);
                }
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
          var list=_currencyService.DbAccess().Queryable<C_Contrac_Child_Detail,C_Subject,C_Project,C_Contrac_Child>((detail, sub, pro,chil)=>new Object[] { JoinType.Left, detail.SubjectId==sub.SubjectId, JoinType.Left,detail.ProjectId==pro.ProjectId, JoinType.Left, detail.Contra_ChildNo ==chil.Contra_ChildNo })
                .Where((detail, sub, pro) => detail.Contra_ChildNo.Equals(childcontracNo)).Select<C_ContracChildDetailModel>((detail, sub, pro, chil) =>new C_ContracChildDetailModel
                {
                    Id=detail.Id,Contra_ChildNo=detail.Contra_ChildNo,
                    Course_Time=detail.Course_Time,
                    Level=detail.Level,
                    Price=detail.Price,
                    ProjectId=detail.ProjectId,
                    SubjectId=detail.SubjectId,
                    SubjectName=sub.SubjectName,
                    ProjectName=pro.ProjectName,
                    StudentUid=detail.StudentUid,
                    TotalSelaPrice=detail.Price*(chil.ContraRate>0? chil.ContraRate/10:1)
                }).ToList();
            var useCourseTimeList = _currencyService.DbAccess().Queryable<C_User_CourseTime>().Where(v => v.Contra_ChildNo == childcontracNo).ToList();
            if (list != null && list.Count > 0)
            {
                list.ForEach(item =>
                {
                    var unitPrice = item.TotalSelaPrice / Convert.ToInt32(item.Course_Time);
                    if (useCourseTimeList != null && useCourseTimeList.Count > 0) {
                        var hourseModel = useCourseTimeList.FindAll(n => n.SubjectId == item.SubjectId && n.ProjectId == item.ProjectId).First();
                        decimal useTimePrice = Convert.ToInt32(hourseModel.Course_Time) * unitPrice;
                        item.TotalSelaPrice = item.TotalSelaPrice - useTimePrice;
                    }
                });
            }
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
            var vmodel = _currencyService.DbAccess().Queryable<C_Collection>().Where(p => p.Id == Id).First();
            if (vmodel.PayStatus == 0) {
                var result = _currencyService.DbAccess().Deleteable<C_Collection>().Where(p => p.Id == Id).ExecuteCommand();
                if (result > 0)
                    return Json(new { code = 200, msg = "删除成功" });
                else
                    return Json(new { code = 0, msg = "删除失败" });
            }
            else
                return Json(new { code = 0, msg = "已确认,无法删除" });
        }

        public string GetWXToken()
        {
            WXAcceSSToken wxtokenModel = new WXAcceSSToken(); ;
            if (RedisLock.KeyExists("wxAccessToken", _redsconfig.RedisCon))
            {
                wxtokenModel = RedisLock.GetStringKey<WXAcceSSToken>("wxAccessToken", _redsconfig.RedisCon);
            }
            else
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", _wxConfig.AppId, _wxConfig.AppSecret);
                string tokenCotent = HttpHelper.HttpGet(url);
                wxtokenModel = JsonConvert.DeserializeObject<WXAcceSSToken>(tokenCotent);
                RedisLock.SetStringKey<WXAcceSSToken>("wxAccessToken", wxtokenModel, wxtokenModel.Expires_in, _redsconfig.RedisCon);
            }
            return wxtokenModel.Access_Token;
        }

        public void SendMsg(string openId, string templateId, string wxaccessToken, string msg, string businesTitle, string payTime, string businesCotent,string Amount,string payMoth)
        {
            try
            {
                Dictionary<string, object> jsonObject = new Dictionary<string, object>();
                jsonObject.Add("touser", openId);   // openid
                jsonObject.Add("template_id", templateId);
                Dictionary<string, object> data = new Dictionary<string, object>();
                Dictionary<string, string> first = new Dictionary<string, string>();
                first.Add("value", msg);
                first.Add("color", "#173177");
                Dictionary<string, string> keyword1 = new Dictionary<string, string>();
                keyword1.Add("value", businesTitle);
                keyword1.Add("color", "#173177");
                Dictionary<string, string> keyword2 = new Dictionary<string, string>();
                keyword2.Add("value", businesCotent);
                keyword2.Add("color", "#173177");
                Dictionary<string, string> keyword3 = new Dictionary<string, string>();
                keyword3.Add("value", Amount);
                keyword3.Add("color", "#173177");
                Dictionary<string, string> keyword4= new Dictionary<string, string>();
                keyword4.Add("value", payMoth);
                keyword4.Add("color", "#173177");
                Dictionary<string, string> remark = new Dictionary<string, string>();
                remark.Add("value", "业务时间:" +payTime);
                remark.Add("color", "#173177");
                data.Add("first", first);
                data.Add("keyword1", keyword1);
                data.Add("keyword2", keyword2);
                data.Add("keyword3", keyword3);
                data.Add("keyword4", keyword4);
                data.Add("remark", remark);
                jsonObject.Add("data", data);
                var jsonStr = JsonConvert.SerializeObject(jsonObject);
                var api = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + wxaccessToken;
                string content = HttpHelper.HttpPost(api, jsonStr, "application/json");
            }
            catch (Exception er)
            {
                log.Info("交易业务消息发送异常:" + er.Message);
            }
        }

    }
}
