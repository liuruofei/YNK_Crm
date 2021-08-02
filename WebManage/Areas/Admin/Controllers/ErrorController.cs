using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADT.Common;
using log4net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebManage;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    public class ErrorController : Controller
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, "RollingLogFileAppender");
        public IActionResult Index(ErrorModel _ErrorModel)
        {
            return View(_ErrorModel);
        }
        /// <summary>
        /// 异常拦截
        /// </summary>
        /// <returns></returns>
        public IActionResult Ajax()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;

            var IsAjaxRequest = (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest");

            //判断是否是自定义异常类型
            if (error is MessageBox)
            {
                if (IsAjaxRequest)
                {
                    return Json(MessageBox.errorModel);
                }
                if (MessageBox.errorModel.status == 403)
                {
                    return new ViewResult()
                    {
                        ViewName = "403",
                        StatusCode = 200
                    };
                }
                else
                {
                    var errorModel = new ErrorModel(error.Message);
                    var sb = new StringBuilder();
                    sb.Append("<script src=\"/assets/libs/jquery/jquery-3.2.1.min.js\"></script>");
                    sb.Append("<script src=\"/assets/libs/layui/layui.js\"></script>");
                    sb.Append("<script src=\"/assets/js/common.js?v=311\"></script>");
                    sb.Append("<script type='text/javascript'>");
                    sb.Append("layui.use(['admin'], function () {");
                    sb.Append("var admin = layui.admin;");
                    var MsgText = errorModel.msg.Trim();
                    MsgText = MsgText.Replace("'", "“");
                    MsgText = MsgText.Replace("\"", "”");
                    sb.Append("admin.alert('" + MsgText + "', 0, '警告'" + (MessageBox.errorModel.status == 401 ? ",function(){window.top.location.href=\"/admin/login/out\";}" : "") + ");");
                    sb.Append("});");
                    sb.Append("</script>");
                    return new ContentResult()
                    {
                        ContentType = "text/html;charset=utf-8;",
                        Content = sb.ToString(),
                        StatusCode = 200
                    };
                }
            }
            else
            {
                //Tools.Log.WriteLog(error, HttpContext.Connection.RemoteIpAddress.ToString());//log4net 写入日志到 txt
                var errorModel = new ErrorModel(error);
                if (IsAjaxRequest)
                {
                    return Json(errorModel);
                }
                log.Error("服务器错误：", error);
                return View("500");
                //return View(AppConfig.ErrorPageUrl, errorModel);
                //return View("~/Views/Shared/Error.cshtml", error);
            }
        }
    }
}
