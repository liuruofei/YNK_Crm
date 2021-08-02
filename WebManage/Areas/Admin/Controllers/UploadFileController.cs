using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ADT.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers
{
    public class UploadFileController : BaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        protected override void Init()
        {
            this.IsExecutePowerLogic = false;
            base.Init();
        }

        public UploadFileController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [RequestSizeLimit(9999_000_000)]
        public IActionResult Index([FromForm] IFormCollection formCollection, string exits)
        {
            string path = "";
            if (string.IsNullOrEmpty(exits))
                return Json(new { code = 1, msg = "无效扩展名" });
            FormFileCollection filelist = (FormFileCollection)formCollection.Files;
            ContentResult errResult = new ContentResult();
            foreach (IFormFile file in filelist)
            {
                HandleUpFile(file, exits.Split('|'), (files) =>
                {
                    string ExtensionName = Path.GetExtension(files.FileName).ToLower().Trim();//获取后缀名
                    if (exits.Split('|') != null && !exits.Split('|').Contains(ExtensionName.ToLower()))
                    {
                        errResult = new ContentResult()
                        {
                            StatusCode = 200,
                            Content = "{\"code\":1,\"msg\":\"请上传后缀名为：" + string.Join("、", exits.Split('|')) + " 格式的文件\"}",
                            ContentType = "application/json"
                        };
                        //throw new MessageBox("请上传后缀名为：" + string.Join("、", Format) + " 格式的文件");
                    }
                }, (_Path) =>
                {
                    path = _Path;
                });
            }
            if (errResult != null && !string.IsNullOrEmpty(errResult.Content))
            {
                return errResult;
            }
            return Json(new { code = 0, data = path });
        }

        [HttpPost]
        public IActionResult LayUpload([FromForm] IFormCollection formCollection, string exits = ".jpg|.png|.gif|.jpeg")
        {
            string path = "";
            if (string.IsNullOrEmpty(exits))
                return Json(new { code = 1, msg = "无效扩展名" });
            FormFileCollection filelist = (FormFileCollection)formCollection.Files;
            ContentResult errResult = new ContentResult();
            foreach (IFormFile file in filelist)
            {
                HandleUpFile(file, exits.Split('|'), (files) =>
                {
                    string ExtensionName = Path.GetExtension(files.FileName).ToLower().Trim();//获取后缀名
                    if (exits.Split('|') != null && !exits.Split('|').Contains(ExtensionName.ToLower()))
                    {
                        errResult = new ContentResult()
                        {
                            StatusCode = 200,
                            Content = "{\"code\":1,\"msg\":\"请上传后缀名为：" + string.Join("、", exits.Split('|')) + " 格式的文件\"}",
                            ContentType = "application/json"
                        };
                        //throw new MessageBox("请上传后缀名为：" + string.Join("、", Format) + " 格式的文件");
                    }
                }, (_Path) =>
                {
                    path = _Path;
                });
            }
            if (errResult != null && !string.IsNullOrEmpty(errResult.Content))
            {
                return errResult;
            }
            return Json(new { code = 0, msg = "上传成功", data = new { src = path } });
        }
        public IActionResult UploadImg([FromForm] IFormCollection formCollection)
        {
            MessageDataResult result = new MessageDataResult();
            var files = formCollection.Files;
            if (files == null || files.Count == 0)
            {
                result.msg = "请上传文件";
                result.status = 300;
                return Json(result);
            }
            string imgValite = ".jpg|.png|.gif|.jpeg";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string fileDicPath = $"/Upload/Tables/Parts_Image/{DateTime.Now.ToString("yyyMMddHHmmsss") + new Random().Next(1000, 9999)}/";
            string fullPath = webRootPath + fileDicPath;
            string fileName = "";
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
            try
            {
                foreach (IFormFile file in files)
                {
                    fileName = file.FileName;
                    string extensionName = Path.GetExtension(file.FileName).ToLower().Trim();//获取后缀名
                    if (!imgValite.Split("|").Contains(extensionName))
                    {
                        result.msg = "请上传图片文件";
                        result.status = 300;
                        return Json(result);
                    }
                    // 创建新文件
                    using (FileStream fs = System.IO.File.Create(fullPath + fileName))
                    {
                        file.CopyTo(fs);
                        // 清空缓冲区数据
                        fs.Flush();
                        fs.Dispose();
                        fs.Close();
                    }
                }
            }
            catch (Exception er)
            {
                result.msg = "上传失败";
                result.status = 300;
                return Json(result);
            }
            result.msg = "上传成功";
            result.status = 200;
            result.data = fileDicPath + fileName;
            return Json(result);

        }


        /// <summary>
        /// 上传编辑器的图片
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public IActionResult KindeditorPicUpload([FromForm] IFormCollection formCollection)
        {
            PicUploadResponse rspJson = new PicUploadResponse() { error = 0, url = "/upload/" };
            //var _apiUrl = ConfigHelper.GetConfig("ApiUrl");
            var files = formCollection.Files;
            if (files == null || files.Count == 0)
            {
                rspJson.error = 1;
                return Json(rspJson);
            }
            string imgValite = ".jpg|.png|.gif|.jpeg";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string fileDicPath = $"/Upload/Tables/Parts_Image/{DateTime.Now.ToString("yyyMMddHHmmsss") + new Random().Next(1000, 9999)}/";
            string fullPath = webRootPath + fileDicPath;
            string fileName = "";
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
            try
            {
                foreach (IFormFile file in files)
                {
                    fileName = file.FileName;
                    string extensionName = Path.GetExtension(file.FileName).ToLower().Trim();//获取后缀名
                    if (!imgValite.Split("|").Contains(extensionName))
                    {

                        rspJson.error = 1;
                        return Json(rspJson);
                    }
                    // 创建新文件
                    using (FileStream fs = System.IO.File.Create(fullPath + fileName))
                    {
                        file.CopyTo(fs);
                        // 清空缓冲区数据
                        fs.Flush();
                        fs.Dispose();
                        fs.Close();
                    }
                }
            }
            catch (Exception er)
            {
                rspJson.error = 1;
                return Json(rspJson);
            }
            rspJson.error = 0;
            rspJson.url =fileDicPath + fileName;
            return Json(rspJson);
        }


        /// <summary>
        /// 处理上传文件
        /// </summary>
        /// <param name="_HttpPostedFileBase"></param>
        /// <param name="Format">文件格式</param>
        /// <param name="Check">执行前 验证回调</param>
        /// <param name="CallBack">如果有回调则保存 否则不保存</param>
        public void HandleUpFile(IFormFile _IFormFile, string[] Format, Action<IFormFile> Check = null, Action<string> CallBack = null)
        {
            var _webRootPath = ConfigHelper.GetConfig("FilePath");
            if (Check != null) Check(_IFormFile);

            string ExtensionName = Path.GetExtension(_IFormFile.FileName).ToLower().Trim();//获取后缀名

            if (Format != null && !Format.Contains(ExtensionName.ToLower()))
            {
                new ContentResult()
                {
                    StatusCode = 200,
                    Content = "{\"code\":1,\"msg\":\"请上传后缀名为：" + string.Join("、", Format) + " 格式的文件\"}",
                    ContentType = "application/json"
                };
                //throw new MessageBox("请上传后缀名为：" + string.Join("、", Format) + " 格式的文件");
            }

            if (CallBack != null)
            {
                var catalog = DateTime.Now.ToString("yyyyMMdd");
                var path = $"{_webRootPath}\\file\\{catalog}\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string filePath = $"/file/{catalog}/{Guid.NewGuid() + ExtensionName}";
                // 创建新文件
                using (FileStream fs = System.IO.File.Create(_webRootPath + filePath))
                {
                    _IFormFile.CopyTo(fs);
                    // 清空缓冲区数据
                    fs.Flush();
                }

                CallBack($"/upload{filePath}");
            }
        }
    }
    public class PicUploadResponse
    {
        public int error { get; set; }
        public string url { get; set; }
    }
}
