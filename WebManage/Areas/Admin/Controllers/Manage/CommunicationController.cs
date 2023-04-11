using ADT.Common;
using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class CommunicationController : BaseController
    {

        private ICurrencyService _currencyService;
        private RedisConfig _redsconfig;
        private WXSetting _wxConfig;
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(CommunicationController));
        public CommunicationController(ICurrencyService currencyService, IOptions<WXSetting> wxConfig, IOptions<RedisConfig> redisConfig)
        {
            _currencyService = currencyService;
            _wxConfig = wxConfig.Value;
            _redsconfig = redisConfig.Value;
        }

        protected override void Init()
        {
            this.MenuID = "H-157";
        }

        [UsersRoleAuthFilter("H-157", FunctionEnum.Have)]
        public IActionResult Index(int studentUid)
        {
            C_Contrac_User u = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(it => it.StudentUid == studentUid).First();
            ViewBag.studentUid = studentUid;
            ViewBag.studentName = u.Student_Name;
            return View();
        }


        [UsersRoleAuthFilter("H-157", FunctionEnum.Have)]
        public IActionResult GetDataSource(int studentUid,DateTime? startTime,DateTime ?endTime,string title, int page = 1, int limit = 10)
        {
            int total = 0;
            PageList<C_Summary> pageModel = new PageList<C_Summary>();
            var list = _currencyService.DbAccess().Queryable<C_Summary, sys_user>((sum, u) => new Object[] { JoinType.Left, sum.CreateUid == u.User_ID })
            .Where(sum => sum.StudentUid == studentUid)
            .WhereIF(startTime.HasValue, sum => sum.CommunicationTime >= startTime.Value)
            .WhereIF(endTime.HasValue, sum => sum.CommunicationTime <= endTime.Value)
            .WhereIF(!string.IsNullOrEmpty(title), sum => sum.SummaryTitle.Contains(title))
            .Select<C_Summary>((sum, u) => sum).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }

        public IActionResult Find(int ID)
        {
            dynamic list;
            if (ID > 0)
            {
                list = _currencyService.DbAccess().Queryable<C_Summary>().Where(f => f.Id == ID).First();
            }
            else
            {
                list = new { };
            }
            return Json(list);
        }


        public IActionResult Add(int Id,int studentUid) {
            C_Contrac_User user = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(cu => cu.StudentUid == studentUid).First();
            ViewBag.StudentUid = studentUid;
            ViewBag.ID= Id;
            return View(user);
        }

        public IActionResult SaveCommunication(CommunicationModel vmodel) {
            var resultId = 0;
            var userId = this.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
            ResResult rsg = new ResResult() { code = 200, msg = "保存沟通纪要成功" };
            try {
                if (vmodel.openIds.Length < 0) {
                    rsg.code = 0;
                    rsg.msg = "推送对象不能为空！";
                    return Json(rsg);
                }
                if (string.IsNullOrEmpty(vmodel.SummaryTitle)) {
                    rsg.code = 0;
                    rsg.msg = "推送重点不能为空";
                    return Json(rsg);
                }
                if (!vmodel.CommunicationTime.HasValue)
                {
                    rsg.code = 0;
                    rsg.msg = "沟通时间不能为空";
                    return Json(rsg);
                }
                if (vmodel.Id > 0)
                {
                    resultId = vmodel.Id;
                    C_Summary inf = _currencyService.DbAccess().Queryable<C_Summary>().Where(am => am.Id == vmodel.Id).First();
                    if (inf.IsSend > 0) {
                        rsg.code = 0;
                        rsg.msg = "内容已被推送，无法更改";
                        return Json(rsg);
                    }
                    inf.openIds = string.Join(",", vmodel.openIds);
                    inf.UserNames = string.Join(",", vmodel.UserNames);
                    inf.SummaryTitle = vmodel.SummaryTitle;
                    inf.Summary = vmodel.Summary;
                    _currencyService.DbAccess().Updateable<C_Summary>(inf).ExecuteCommand();
                }
                else {
                    C_Summary inf = new C_Summary();
                    inf.openIds = string.Join(",",vmodel.openIds);
                    inf.UserNames= string.Join(",", vmodel.UserNames);
                    inf.StudentUid = vmodel.StudentUid;
                    inf.SummaryTitle = vmodel.SummaryTitle;
                    inf.Summary = vmodel.Summary;
                    inf.CreateTime = DateTime.Now;
                    inf.CreateUid = userId;
                    inf.CommunicationTime = vmodel.CommunicationTime;
                    resultId=_currencyService.DbAccess().Insertable<C_Summary>(inf).ExecuteReturnIdentity();
                }
               
            }
            catch (Exception er) {
                rsg.code = 0;
                rsg.msg = "保存沟通纪要失败"+er.Message;
            }
            rsg.data = resultId;
            return Json(rsg);
        }


        public IActionResult SendCommunication(int Id) {
            ResResult rsg = new ResResult() { code = 200, msg = "发送沟通纪要成功" };
            try
            {
                C_Summary inf = _currencyService.DbAccess().Queryable<C_Summary>().Where(am => am.Id ==Id).First();
                if (Id < 1) {
                    rsg.code = 0;
                    rsg.msg = "内容未保存!请先保存内容";
                    return Json(rsg);
                }
                if (inf.IsSend > 0)
                {
                    rsg.code = 0;
                    rsg.msg = "内容已被推送，无法重复推送";
                    return Json(rsg);
                }
                var token = GetWXToken();
                if (inf!=null&&!string.IsNullOrEmpty(inf.openIds)&&!string.IsNullOrEmpty(_wxConfig.TemplateSummary))
                {
                    string[] openids = inf.openIds.Split(",");
                    foreach (var opid in openids)
                    {
                        SendMsg(opid, _wxConfig.TemplateSummary, token, "沟通提醒", inf.SummaryTitle, inf.CommunicationTime.Value.ToString("yyyy-MM-dd"), inf.Summary, inf.Id);
                    }
                    inf.IsSend = 1;
                    inf.SendTime = DateTime.Now;
                    _currencyService.DbAccess().Updateable<C_Summary>(inf).ExecuteCommand();
                }
            }
            catch (Exception er) {
                rsg.msg = "发送沟通内容失败!"+er.Message;
                rsg.code = 0;
            }
            return Json(rsg);
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

        public void SendMsg(string openId, string templateId, string wxaccessToken, string msg, string cmTitle,  string wkTime, string cmCotent, int Id)
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
                keyword1.Add("value", cmTitle);
                keyword1.Add("color", "#173177");
                Dictionary<string, string> keyword2 = new Dictionary<string, string>();
                keyword2.Add("value",wkTime);
                keyword2.Add("color", "#173177");
                Dictionary<string, string> remark = new Dictionary<string, string>();
                remark.Add("value", "沟通内容:" + cmCotent);
                remark.Add("color", "#173177");
                data.Add("first", first);
                data.Add("keyword1", keyword1);
                data.Add("keyword2", keyword2);
                data.Add("remark", remark);
                jsonObject.Add("data", data);
                jsonObject.Add("url", "http://crm.younengkao.com/WxTemplateChild/SummaryDetail?Id=" + Id);//设置链接
                var jsonStr = JsonConvert.SerializeObject(jsonObject);
                var api = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + wxaccessToken;
                string content = HttpHelper.HttpPost(api, jsonStr, "application/json");
            }
            catch(Exception er)
            {
                log.Info("沟通内容发送异常:"+er.Message);
            }
        }




        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="studentUid"></param>
        /// <param name="endtime"></param>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public virtual IActionResult ExportPlan(int studentUid, DateTime? startTime, DateTime? endTime, string title)
        {
            var student = _currencyService.DbAccess().Queryable<C_Contrac_User>().Where(n => n.StudentUid == studentUid).First();
            var list = _currencyService.DbAccess().Queryable<C_Summary, sys_user>((suy, u) => new Object[] { JoinType.Left, suy.CreateUid == u.User_ID })
            .Where(suy => suy.StudentUid == studentUid)
            .WhereIF(startTime.HasValue, suy => suy.CommunicationTime >= startTime.Value)
            .WhereIF(endTime.HasValue, suy => suy.CommunicationTime <= endTime.Value)
            .WhereIF(!string.IsNullOrEmpty(title), suy => suy.SummaryTitle.Contains(title))
            .Select<C_Summary>((suy, u) => suy).ToList();
            //导出代码
            var workbook = new HSSFWorkbook();
            //标题列样式
            var headFont = workbook.CreateFont();
            headFont.IsBold = true;
            var headStyle = workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Center;
            headStyle.BorderBottom = BorderStyle.Thin;
            headStyle.BorderLeft = BorderStyle.Thin;
            headStyle.BorderRight = BorderStyle.Thin;
            headStyle.BorderTop = BorderStyle.Thin;
            headStyle.WrapText = true;//自动换行
            headStyle.SetFont(headFont);
            var sheet = workbook.CreateSheet(student.Student_Name + "家庭沟通纪要");
            sheet.DefaultColumnWidth = 25;
            string[] cellNames = new string[] { "沟通对象", "沟通重点", "沟通日期", "沟通内容", "是否已发送"};
            //循环行
            var row0 = sheet.CreateRow(0);
            for (var i = 0; i < cellNames.Length; i++)
            {
                var cell = row0.CreateCell(i);
                cell.SetCellValue(cellNames[i]);
                cell.CellStyle = headStyle;
            }
            if (list != null && list.Count > 0)
            {
                for (var y = 0; y < list.Count; y++)
                {
                    var row = sheet.CreateRow(y + 1);
                    var cell0 = row.CreateCell(0);
                    cell0.SetCellValue(list[y].UserNames);
                    cell0.CellStyle = headStyle;
                    var cell1 = row.CreateCell(1);
                    cell1.SetCellValue(list[y].SummaryTitle);
                    cell1.CellStyle = headStyle;
                    var cell2 = row.CreateCell(2);
                    cell2.SetCellValue(list[y].CommunicationTime.Value.ToString("yyyy-MM-dd"));
                    cell2.CellStyle = headStyle;
                    var cell3 = row.CreateCell(3);
                    cell3.SetCellValue(list[y].Summary);
                    cell3.CellStyle = headStyle;
                    var cell4 = row.CreateCell(4);
                    cell4.SetCellValue((list[y].IsSend>0?"已发送":"待发送"));
                    cell4.CellStyle = headStyle;
                }
            }
            //保存
            byte[] streamArr = null;
            //保存
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                streamArr = ms.ToArray();
            }
            return File(streamArr, "application/vnd.ms-excel", student.Student_Name + "家长沟通记录" + DateTime.Now.ToString("yyyyMMddHHmmssss") + ".xls");
        }
    }
}
