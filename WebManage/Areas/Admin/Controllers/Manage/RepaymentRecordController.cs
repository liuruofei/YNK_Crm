using ADT.Models;
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
    public class RepaymentRecordController : BaseController
    {
        private ICurrencyService _currencyService;
        public RepaymentRecordController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }
        protected override void Init()
        {
            this.MenuID = "L-152";
        }
        // GET: /<controller>/
        [UsersRoleAuthFilter("L-152", FunctionEnum.Have)]
        public IActionResult Index()
        {

            return View();
        }

        [UsersRoleAuthFilter("L-152", FunctionEnum.Have)]
        public IActionResult GetDataSource(string title, int page = 1, int limit = 10)
        {
            int total = 0;
            PageList<RepaymentRecordModel> pageModel = new PageList<RepaymentRecordModel>();
            var list = _currencyService.DbAccess().Queryable<C_RepaymentRecord,C_Collection>((c,cl)=>new Object[] { JoinType.Left,c.CollgeId==cl.Id}).WhereIF(!string.IsNullOrEmpty(title),(c,cl)=>c.Contra_ChildNo.Contains(title)||cl.StudentName.Contains(title)).OrderBy(c => c.CreateTime).Select((c, cl) =>new RepaymentRecordModel
            {
                Id=c.Id,
                Contra_ChildNo=c.Contra_ChildNo,
                CollgeId=c.CollgeId,
                RepaymentAmount=c.RepaymentAmount,
                CreateTime=c.CreateTime,
                StudentName=cl.StudentName
            }).ToPageList(page, limit, ref total);
            pageModel.msg = "获取成功";
            pageModel.code = 0;
            pageModel.count = total;
            pageModel.data = list;
            return Json(pageModel);
        }
    }
}
