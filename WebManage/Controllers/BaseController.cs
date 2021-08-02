using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ADT.Common;
using ADT.Models;
using ADT.Service.IService;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace WebManage.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Init() { }
        public BaseController()
        {
            this.Init();
        }
        /// <summary>
        /// Action 执行之前 发生
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var token = string.Empty;
            var cktoken = filterContext.HttpContext.Request.Cookies["authtoken"];
            var attoken = filterContext.HttpContext.Request.Headers["authtoken"].ToString();
            if (!string.IsNullOrEmpty(cktoken))
            {
                //Console.WriteLine(cktoken);
                token = cktoken;
            }
            if (!string.IsNullOrEmpty(attoken))
                token = attoken;
            if (!string.IsNullOrEmpty(token))
            {
                var userid = string.Empty;
                var handers = new JwtSecurityTokenHandler().ReadJwtToken(token.Replace("Bearer ", ""));
                foreach (var item in handers.Claims)
                {
                    if (item.Type == "ID")
                        userid = item.Value;
                }
            }
            else
            {
                throw new MessageBox("请重新登陆", EMsgStatus.无效权限401);
            }
        }

        /// <summary>
        /// Action 执行之后 发生
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
    }
}
