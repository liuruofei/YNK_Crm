using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ADT.Common;
using ADT.Models;
using ADT.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebManage.Areas.Admin.Controllers
{
    public class UploadFileController : BaseController
    {
        private ICurrencyService _currencyService;
        private readonly IHostingEnvironment _hostingEnvironment;
        protected override void Init()
        {
            this.IsExecutePowerLogic = false;
            base.Init();
        }

        public UploadFileController(IHostingEnvironment hostingEnvironment,ICurrencyService currencyService)
        {
            _hostingEnvironment = hostingEnvironment;
            _currencyService = currencyService;
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

        public IActionResult UploadExcel([FromForm] IFormCollection formCollection)
        {
            MessageDataResult result = new MessageDataResult();
            var files = formCollection.Files;
            if (files == null || files.Count == 0)
            {
                result.msg = "请上传文件";
                result.status = 300;
                return Json(result);
            }
            string imgValite = ".xlsx|.xls";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string fileDicPath = $"/Upload/Excel/{DateTime.Now.ToString("yyyMMddHHmm")}/";
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
                        result.msg = "请上传Excel文件";
                        result.status = 300;
                        return Json(result);
                    }
                    // 创建新文件
                    using (FileStream fs = new FileStream(fullPath+ fileName, FileMode.Create))
                    {
                        file.CopyTo(fs);
                        // 清空缓冲区数据
                        fs.Flush();
                        fs.Dispose();
                        fs.Close();
                    }
                    //读取excel文件
                    DataTable list = GetExcelDatatable(fullPath + fileName);
                    if (list != null)
                    {
                        List<string> classTypeNames = new List<string>();
                        List<string> sbujectNames = new List<string>();
                        List<string> classNames = new List<string>();
                        var errMsg = "";
                        var i = 0;
                        foreach (DataRow row in list.Rows)
                        {
                            i++;
                            if (row["班级类型"] != null && !string.IsNullOrEmpty(row["班级类型"].ToString()))
                            {
                                if (!classTypeNames.Contains(row["班级类型"]))
                                {
                                    classTypeNames.Add(row["班级类型"].ToString());
                                }
                            }
                            else {
                                errMsg +=$"第{i}行的班级类型不能为空";
                            }
                            if (row["学习类别"] != null && !string.IsNullOrEmpty(row["学习类别"].ToString()))
                            {
                                if (!sbujectNames.Contains(row["学习类别"]))
                                {
                                    sbujectNames.Add(row["学习类别"].ToString());
                                }
                            }
                            else
                            {
                                errMsg += $"第{i}行的学习类别不能为空";
                            }
                            if (row["班级编号"] == null || (row["班级编号"] != null && string.IsNullOrEmpty(row["班级编号"].ToString())))
                            {
                                errMsg += $"第{i}行的班级编号不能为空";
                            }
                            if (row["班级名称"] != null && !string.IsNullOrEmpty(row["班级名称"].ToString()))
                            {
                                if (!sbujectNames.Contains(row["班级名称"]))
                                {
                                    classNames.Add(row["班级名称"].ToString());
                                }
                                else {
                                    errMsg += $"第{i}行的班级名称已重复";
                                }
                            }
                            else {
                                errMsg += $"第{i}行的班级名称不能为空";
                            }
                            if (row["课时"] == null || (row["课时"] != null &&string.IsNullOrEmpty(row["课时"].ToString())))
                            {
                                errMsg += $"第{i}行的课时不能为空";
                            }
                            if (row["价格"] == null || (row["价格"] != null && string.IsNullOrEmpty(row["价格"].ToString())))
                            {
                                errMsg += $"第{i}行的价格不能为空";
                            }
                            if (row["开始日期"] == null || (row["开始日期"] != null && string.IsNullOrEmpty(row["开始日期"].ToString())))
                            {
                                errMsg += $"第{i}行的开始日期不能为空";
                            }
                            if (row["开课日期"] == null || (row["开课日期"] != null && string.IsNullOrEmpty(row["开课日期"].ToString())))
                            {
                                errMsg += $"第{i}行的开课日期不能为空";
                            }
                            if (row["结课日期"] == null || (row["结课日期"] != null && string.IsNullOrEmpty(row["结课日期"].ToString())))
                            {
                                errMsg += $"第{i}行的结课日期不能为空";
                            }

                        }
                        if (!string.IsNullOrEmpty(errMsg)) {
                            result.msg =errMsg;
                            result.status = 200;
                            return Json(result);
                        }
                        List<C_Class> hasAny = _currencyService.DbAccess().Queryable<C_Class>().Where(cl => classNames.Contains(cl.Class_Name)).ToList();
                        if (hasAny != null&& hasAny.Count>0) {
                            string disName = "";
                            hasAny.ForEach(it =>
                            {
                                disName += it.Class_Name+",";
                            });
                            result.msg ="系统已包含班级名称"+ disName+",请修正后再导入";
                            result.status = 200;
                            return Json(result);
                        }
                        List<C_Subject> listsub= _currencyService.DbAccess().Queryable<C_Subject>().Where(sub => sbujectNames.Contains(sub.SubjectName)).ToList();
                        List<C_ClassType> listClassType = _currencyService.DbAccess().Queryable<C_ClassType>().Where(typ => classTypeNames.Contains(typ.TypeName)).ToList();
                        List<C_Class> listClass = new List<C_Class>();
                        foreach (DataRow row in list.Rows) {
                           
                            C_Class item = new C_Class();
                            item.TypeId = listClassType.Where(tp => tp.TypeName== row["班级类型"].ToString()).First().Id;
                            item.SubjectId= listsub.Where(tp => tp.SubjectName == row["学习类别"].ToString()).First().SubjectId;
                            item.Class_Name = row["班级名称"].ToString();
                            item.Class_No= row["班级编号"].ToString();
                            item.Course_Time =float.Parse(row["课时"].ToString());
                            item.Price= decimal.Parse(row["价格"].ToString());
                            item.StartDate = DateTime.Parse(row["开始日期"].ToString());
                            item.Start_Course_Date = DateTime.Parse(row["开课日期"].ToString());
                            item.End_Course_Date = DateTime.Parse(row["结课日期"].ToString());
                            item.CampusId = 1;
                            item.Remarks= row["备注"].ToString();
                            listClass.Add(item);

                        }
                        var sult=_currencyService.DbAccess().Insertable<C_Class>(listClass).ExecuteCommand();
                        if (sult > 0)
                        {
                            result.msg = "导入成功";
                        }
                        else {
                            result.msg = "导入失败";
                        }
                    }
                }
            }
            catch (Exception er)
            {
                result.msg = "导入失败"+er.Message;
                result.status = 300;
                return Json(result);
            }
            if (!string.IsNullOrEmpty(result.msg))
            {
                result.msg = "导入成功";
            }
            result.status = 200;
            return Json(result);

        }

        protected System.Data.DataTable GetExcelDatatable(string fileUrl)
        {
            //支持.xls和.xlsx，即包括office2010等版本的   HDR=Yes代表第一行是标题，不是数据；
            const string cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";

            System.Data.DataTable dt = null;
            //建立连接
            OleDbConnection conn = new OleDbConnection(string.Format(cmdText, fileUrl));

            //打开连接
            if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string[] strTableNames = new string[schemaTable.Rows.Count];
            for (int k = 0; k < schemaTable.Rows.Count; k++)
            {
                strTableNames[k] = schemaTable.Rows[k]["TABLE_NAME"].ToString();
            }

            //获取Excel的第一个Sheet名称
            string sheetName = strTableNames[0].ToString().Trim();
            //查询sheet中的数据
            string strSql = "select * from [" + sheetName + "]";
            OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            return dt;
        }
    }
    public class PicUploadResponse
    {
        public int error { get; set; }
        public string url { get; set; }
    }

}
