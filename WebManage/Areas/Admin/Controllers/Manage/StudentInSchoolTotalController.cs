using ADT.Models;
using ADT.Models.ResModel;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebManage.Areas.Admin.Filter;
using WebManage.Areas.Admin.Models;
using WebManage.Models.Res;

namespace WebManage.Areas.Admin.Controllers.Manage
{
    [Authorize]
    public class StudentInSchoolTotalController : BaseController
    {
        private ICurrencyService _currencyService;
        public StudentInSchoolTotalController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "R-155";
        }

        [UsersRoleAuthFilter("R-155", FunctionEnum.Have)]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 统计学生在校总天数
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <param name="dateTime"></param>
        /// <param name="userName"></param>
        /// <param name="subjectId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public IActionResult QueryWorkSource(string startTime, string endTime, int monthStatu, int page = 1, int limit = 10)
        {
            int total = 0;
            var campusId = this.User.Claims.FirstOrDefault(c => c.Type == "CampusId")?.Value;
            PageList<StudentInSchoolTotalDayModel> pageModel = new PageList<StudentInSchoolTotalDayModel>();
            StringBuilder manwhere = new StringBuilder("");
            //本月
            if (monthStatu == 1 && string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and convert(varchar(6),pln.WorkDate,112)=convert(varchar(6),getdate(),112) and pln.WorkDate<getdate()");
            }
            //上月
            if (monthStatu == 2 && string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and convert(varchar(6),pln.WorkDate,112)=convert(varchar(6),dateadd(mm,-1,getdate()),112)");
            }
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                manwhere.Append(" and pln.WorkDate>=CAST(@startStr AS date) AND pln.WorkDate<CAST(@endStr AS date)");
            }
            StringBuilder sql = new StringBuilder("(select pln.StudentUid,st.Student_Name,count(pln.StudentUid)as TotalDay ");
            sql.Append(" from  C_Student_Work_Plan pln left join Sys_User sysU  on pln.TaUid=sysU.[User_ID] left join C_Contrac_User st on pln.StudentUid=st.StudentUid");
            sql.Append(" where  ((pln.InSchoolTime!='' and pln.InSchoolTime is not null) or (pln.OutSchoolTime!='' and pln.OutSchoolTime is not null)) and pln.CampusId=" + campusId + manwhere.ToString());
            sql.Append(" group by pln.StudentUid,st.Student_Name)");
            List<StudentInSchoolTotalDayModel> list = _currencyService.DbAccess().Queryable(sql.ToString(), "orginSql")
            .AddParameters(new { startStr = startTime, endStr = endTime })
            .Select<StudentInSchoolTotalDayModel>("*").ToPageList(page, limit, ref total);
            //查询这段时间学生的所有到校出入
            StringBuilder sql2 = new StringBuilder("(select pln.StudentUid,st.Student_Name,pln.WorkDate,pln.TotalTime");
            sql2.Append(" from  C_Student_Work_Plan pln left join Sys_User sysU  on pln.TaUid=sysU.[User_ID] left join C_Contrac_User st on pln.StudentUid=st.StudentUid");
            sql2.Append(" where ((pln.InSchoolTime!='' and pln.InSchoolTime is not null) or (pln.OutSchoolTime!='' and pln.OutSchoolTime is not null)) and pln.CampusId=" + campusId + manwhere.ToString());
            sql2.Append(" group by pln.WorkDate,pln.StudentUid,st.Student_Name,pln.TotalTime)");
            List<InSchoolHourseTotalModel> list2 = _currencyService.DbAccess().Queryable(sql2.ToString(), "orginSql")
            .AddParameters(new { startStr = startTime, endStr = endTime })
            .Select<InSchoolHourseTotalModel>("*").ToList();
            //开始累加在校课时
            if (list != null && list.Count > 0) {
                list.ForEach(item=>
                {
                    var studentInhourSer = list2.Where(a => a.StudentUid == item.StudentUid).ToList();
                    if (studentInhourSer != null && studentInhourSer.Count > 0) {
                        float stuH = 0, stuM = 0;
                        studentInhourSer.ForEach(b =>
                        {
                          
                            if (!string.IsNullOrEmpty(b.TotalTime) && b.TotalTime.IndexOf("小时")>-1) {
                                string[] strOption = b.TotalTime.Split("小时");
                                stuH += Convert.ToInt32(strOption[0]);
                            }
                            if (!string.IsNullOrEmpty(b.TotalTime) && b.TotalTime.IndexOf("分钟")>-1)
                            {
                                if (b.TotalTime.IndexOf("小时") > -1)
                                {
                                    string[] strOption = b.TotalTime.Split("小时");
                                    var filterMinuStr = strOption[1];
                                    stuM += Convert.ToInt32(filterMinuStr.Split("分钟")[0]);
                                }
                                else {
                                    string[] strOption = b.TotalTime.Split("分钟");
                                    stuM += Convert.ToInt32(strOption[0]);
                                }
                            }
                        });
                        if (stuM > 60) {
                            var yushu = stuM % 60;
                            stuH += (stuM - yushu) / 60;
                            stuM= yushu;
                        }
                        item.TotalMinus =(stuH * 60)+ stuM;

                        item.TotalHourse= (stuH>0? stuH + "小时":"")+ (stuM > 0 ? stuM + "分钟" : "");
                    }
                });
            }
            pageModel.data = list;
            pageModel.code = 0;
            pageModel.msg = "获取成功";
            pageModel.count = total;
            return Json(pageModel);
        }

    }
}
