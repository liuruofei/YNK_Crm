using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Filter
{
    public class HttpGlobalExceptionAttribute : Attribute, IExceptionFilter
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(HttpGlobalExceptionAttribute));
        public void OnException(ExceptionContext context)
        {
            log.Error($"{context.HttpContext.Request.Path.Value}访问出错：", context.Exception);
            context.Result = new ContentResult()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = "{\"status\":500,\"msg\":\"" + context.Exception.ToString() + "\",\"data\":\"\"}"
            };
        }
    }
}
