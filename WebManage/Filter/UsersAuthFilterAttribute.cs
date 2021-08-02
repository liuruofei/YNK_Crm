using log4net;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using WebManage.Models;

namespace WebManage.Filter
{
    public class UsersAuthFilterAttribute : Attribute, IActionFilter
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(UsersAuthFilterAttribute));
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        /// <summary>
        /// Action之前发生
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
           
            UserInfoAuth userInfoAuth = new UserInfoAuth();
            userInfoAuth.UserId = "1";
            filterContext.ActionArguments["UserData"] = userInfoAuth;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
