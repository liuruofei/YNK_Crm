using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UEditor.Core;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers
{
    public class UEditorController : BaseController
    {
        protected override void Init()
        {
            base.Init();
            this.IsExecutePowerLogic = false;
        }

        private readonly UEditorService _ueditorService;
        public UEditorController(UEditorService ueditorService)
        {
            this._ueditorService = ueditorService;
        }

        //如果是API，可以按MVC的方式特别指定一下API的URI
        [HttpGet, HttpPost]
        public ContentResult Upload()
        {
            var response = _ueditorService.UploadAndGetResponse(HttpContext);
            return Content(response.Result, response.ContentType);
        }
    }
}
