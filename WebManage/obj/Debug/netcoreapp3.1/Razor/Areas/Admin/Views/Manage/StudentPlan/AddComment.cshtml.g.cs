#pragma checksum "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\AddComment.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1ee01e57dc65398c65141602eb2768dd99aa215a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Manage_StudentPlan_AddComment), @"mvc.1.0.view", @"/Areas/Admin/Views/Manage/StudentPlan/AddComment.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\YNK_Crm\WebManage\Areas\Admin\Views\_ViewImports.cshtml"
using WebManage;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\YNK_Crm\WebManage\Areas\Admin\Views\_ViewImports.cshtml"
using WebManage.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\YNK_Crm\WebManage\Areas\Admin\Views\_ViewImports.cshtml"
using ADT.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\YNK_Crm\WebManage\Areas\Admin\Views\_ViewImports.cshtml"
using ADT.Common;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1ee01e57dc65398c65141602eb2768dd99aa215a", @"/Areas/Admin/Views/Manage/StudentPlan/AddComment.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9e00094c6873c7f1dd971a491bd11b241da9ff38", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Manage_StudentPlan_AddComment : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("modelUserForm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("lay-filter", new global::Microsoft.AspNetCore.Html.HtmlString("modelUserForm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("layui-form model-form"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/kindeditor/themes/default/default.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\AddComment.cshtml"
  
    var KeyId = ViewBag.ID;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "1ee01e57dc65398c65141602eb2768dd99aa215a5759", async() => {
                WriteLiteral("\r\n    <input name=\"Id\" type=\"hidden\"");
                BeginWriteAttribute("value", " value=\"", 156, "\"", 175, 1);
#nullable restore
#line 6 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\AddComment.cshtml"
WriteAttributeValue("", 164, ViewBag.ID, 164, 11, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" />\r\n    <input name=\"StudentUid\" type=\"hidden\"");
                BeginWriteAttribute("value", " value=\"", 223, "\"", 250, 1);
#nullable restore
#line 7 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\AddComment.cshtml"
WriteAttributeValue("", 231, ViewBag.StudentUid, 231, 19, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" />\r\n    <div class=\"layui-form-item\">\r\n        <label class=\"layui-form-label\">计划时间</label>\r\n        <div class=\"layui-input-block\">\r\n            <input name=\"WorkDate\" type=\"text\" id=\"WorkDate\" class=\"layui-input\"");
                BeginWriteAttribute("value", " value=\"", 466, "\"", 490, 1);
#nullable restore
#line 11 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\AddComment.cshtml"
WriteAttributeValue("", 474, ViewBag.DataStr, 474, 16, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
        </div>
    </div>
    <div class=""layui-form-item"">
        <label class=""layui-form-label"">单词</label>
        <div class=""layui-input-block"">
            <textarea name=""ChouciComent"" class=""layui-textarea"" placeholder=""输入抽词任务""></textarea>
        </div>
    </div>
    <div class=""layui-form-item"">
        <label class=""layui-form-label"">课程</label>
        <div class=""layui-input-block"">
            <textarea name=""CourseComent"" class=""layui-textarea"" placeholder=""输入课程任务""></textarea>
        </div>
    </div>
    <div class=""layui-form-item"">
        <label class=""layui-form-label"">作业</label>
        <div class=""layui-input-block"">
            <textarea name=""HomeWorkComent"" class=""layui-textarea"" placeholder=""输入作业任务""></textarea>
        </div>
    </div>
    <div class=""layui-form-item"">
        <label class=""layui-form-label"">其它</label>
        <div class=""layui-input-block"">
            <textarea name=""OtherComent"" class=""layui-textarea"" placeholder=""输入作业任务""></textarea>");
                WriteLiteral(@"
        </div>
    </div>
    <div class=""layui-form-item"">
        <label class=""layui-form-label"">到校时间</label>
        <div class=""layui-input-block"">
            <input name=""InSchoolTime"" class=""layui-input"" placeholder=""输入到校时间"" />
        </div>
    </div>
    <div class=""layui-form-item"">
        <label class=""layui-form-label"">离校时间</label>
        <div class=""layui-input-block"">
            <input name=""OutSchoolTime"" class=""layui-input"" placeholder=""输入离校时间"" />
        </div>
    </div>
    <div class=""layui-form-item"">
        <label class=""layui-form-label"">每日总结</label>
        <div class=""layui-input-block"">
            <textarea name=""SummaryComent"" class=""layui-textarea"" placeholder=""输入作业任务""></textarea>
        </div>
    </div>
    <div class=""layui-form-item"">
        <label class=""layui-form-label"">助教</label>
        <div class=""layui-input-block"">
            <select name=""TaUid"">
            </select>
        </div>
    </div>
    <div class=""layui-form-item posit");
                WriteLiteral(@"ionbottomtop""></div>
    <div class=""layui-form-item text-right positionbottom"">
        <button class=""layui-btn layui-btn-primary"" type=""button"" ew-event=""deleteTask""  id=""deleteTask"">删除当前任务</button>
        <button class=""layui-btn layui-btn-primary"" type=""button"" ew-event=""closeDialog"">取消</button>
        <button class=""layui-btn"" lay-filter=""modelUserSubmit"" lay-submit>保存</button>
    </div>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n");
            DefineSection("css", async() => {
                WriteLiteral("\r\n    <style>\r\n        .ke-container {\r\n            margin-left: 33px !important;\r\n        }\r\n\r\n        .ke-edit, .ke-edit-iframe, .ke-edit-textarea {\r\n            height: 266px !important;\r\n        }\r\n    </style>\r\n");
            }
            );
            WriteLiteral("\r\n");
            DefineSection("js", async() => {
                WriteLiteral("\r\n    <!-- js部分 -->\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "1ee01e57dc65398c65141602eb2768dd99aa215a11825", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    <script>\r\n        layui.use([\'form\', \'admin\', \'adminForm\', \'laydate\'], function () {\r\n        var KeyId = \'");
#nullable restore
#line 88 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\AddComment.cshtml"
                Write(KeyId);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"';
        var $ = layui.jquery;
        var form = layui.form;
        var admin = layui.admin;
        var adminForm = layui.adminForm;
        var laydate = layui.laydate;

        var App = {
            Load: function () {
                adminForm.load({
                    formName: 'modelUserForm',
                    KeyId: '");
#nullable restore
#line 99 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\AddComment.cshtml"
                       Write(KeyId);

#line default
#line hidden
#nullable disable
                WriteLiteral("\',\r\n                    url: \"");
#nullable restore
#line 100 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\AddComment.cshtml"
                     Write(Url.Action("Find"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""",
                    dataType: 'json',
                    callBack: function (r) {
                        if (r.WorkDate != null && r.WorkDate != undefined) {
                            $(""input[name='WorkDate']"").val(dateFtt(""yyyy-MM-dd"",r.WorkDate));
                        }
                                //助教
                                adminForm.load({
                                url: """);
#nullable restore
#line 108 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\AddComment.cshtml"
                                 Write(Url.Action("QueryTa", "CourseWork"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""",
                                dataType: 'json',
                                callBack: function (rou) {
                                    if (rou.data != null) {
                                        var result = rou.data;
                                        for (var i = 0; i < result.length; i++) {
                                            if (r != undefined && r.TaUid != null && r.TaUid == result[i].User_ID) {
                                                $(""select[name='TaUid']"").append('<option value = ' + result[i].User_ID + ' selected>' + result[i].User_Name + '</option>');

                                            } else {
                                                $(""select[name='TaUid']"").append('<option value = ' + result[i].User_ID + '>' + result[i].User_Name + '</option>');
                                            }
                                        }
                                    }
                                   }
                     ");
                WriteLiteral("           });\r\n                        console.log(r);\r\n                    }\r\n                });\r\n            },\r\n            Save: function (data) {\r\n                adminForm.Save({\r\n                    url: \'");
#nullable restore
#line 130 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\AddComment.cshtml"
                     Write(Url.Action("SaveCommend"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                    data: data.field,
                    callBack: function () {
                        admin.closeThisDialog();
                    }
                })
            }
        }
         App.Load();
        $(""#deleteTask"").click(function () {
            if (KeyId != '' && KeyId > 0) {
                adminForm.Save({
                    url: '");
#nullable restore
#line 142 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\AddComment.cshtml"
                     Write(Url.Action("deleteCommend"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                    data: { Id: KeyId },
                    callBack: function () {
                        admin.closeThisDialog();
                    }
                })
            }
            else {
                layer.msg(""没有保存任何任务,无法删除"");
                return false;
            }
        })
        // 表单提交事件
        form.on('submit(modelUserSubmit)', function (data) {
            App.Save(data);
            return false;
        });
        function formatMinutin() {
            $("".laydate-time-list li ol"").eq(2).parent(""li"").remove();
        }
        });
        function dateFtt(fmt, val) { //author: meizz
            var date = new Date(val);
            var o = {
                ""M+"": date.getMonth() + 1,     //月份
                ""d+"": date.getDate(),     //日
                ""h+"": date.getHours(),     //小时
                ""m+"": date.getMinutes(),     //分
                ""s+"": date.getSeconds(),     //秒
                ""q+"": Math.floor((date.getMonth() + 3) ");
                WriteLiteral(@"/ 3), //季度
                ""S"": date.getMilliseconds()    //毫秒
            };
            if (/(y+)/.test(fmt))
                fmt = fmt.replace(RegExp.$1, (date.getFullYear() + """").substr(4 - RegExp.$1.length));
            for (var k in o)
                if (new RegExp(""("" + k + "")"").test(fmt))
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : ((""00"" + o[k]).substr(("""" + o[k]).length)));
            return fmt;
        }
    </script>
");
            }
            );
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
