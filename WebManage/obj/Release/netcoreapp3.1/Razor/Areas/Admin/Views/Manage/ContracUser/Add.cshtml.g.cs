#pragma checksum "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Add.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4b10a29e0ade111df1f57c7b793f7629cbac7a76"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Manage_ContracUser_Add), @"mvc.1.0.view", @"/Areas/Admin/Views/Manage/ContracUser/Add.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4b10a29e0ade111df1f57c7b793f7629cbac7a76", @"/Areas/Admin/Views/Manage/ContracUser/Add.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9e00094c6873c7f1dd971a491bd11b241da9ff38", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Manage_ContracUser_Add : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
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
#line 1 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Add.cshtml"
  
    var KeyId = ViewBag.ID;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "4b10a29e0ade111df1f57c7b793f7629cbac7a765717", async() => {
                WriteLiteral("\r\n    <input name=\"StudentUid\" type=\"hidden\"");
                BeginWriteAttribute("value", " value=\"", 164, "\"", 183, 1);
#nullable restore
#line 6 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Add.cshtml"
WriteAttributeValue("", 172, ViewBag.ID, 172, 11, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" />\r\n    <div class=\"layui-form-item\">\r\n        <div class=\"layui-inline\">\r\n            <label class=\"layui-form-label\">学生编码</label>\r\n            <div class=\"layui-input-inline\" style=\"line-height:37px;\">\r\n                <span>");
#nullable restore
#line 11 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Add.cshtml"
                 Write(ViewBag.StudentNo);

#line default
#line hidden
#nullable disable
                WriteLiteral("</span>\r\n                <input name=\"Student_No\" type=\"hidden\"");
                BeginWriteAttribute("value", " value=\"", 493, "\"", 519, 1);
#nullable restore
#line 12 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Add.cshtml"
WriteAttributeValue("", 501, ViewBag.StudentNo, 501, 18, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
            </div>
        </div>
        <div class=""layui-inline"">
            <label class=""layui-form-label required"">学员姓名</label>
            <div class=""layui-input-inline"">
                <input name=""Student_Name"" placeholder=""请输入学生姓名"" maxlength=""120"" lay-verify=""required"" class=""layui-input"" required />
            </div>
        </div>
        <div class=""layui-inline"">
            <label class=""layui-form-label"">性别</label>
            <div class=""layui-input-inline"">
                <input type=""radio"" value=""男"" name=""Sex"" title=""男"" />
                <input type=""radio"" value=""女"" name=""Sex"" title=""女"" />
            </div>
        </div>
    </div>
    <div class=""layui-form-item"">
        <div class=""layui-inline"">
            <label class=""layui-form-label"">学员电话</label>
            <div class=""layui-input-inline"">
                <input name=""Student_Phone"" type=""text"" class=""layui-input"" />
            </div>
        </div>
        <div class=""layui-inline"">
       ");
                WriteLiteral(@"     <label class=""layui-form-label"">在读学校</label>
            <div class=""layui-input-inline"">
                <input name=""InSchool"" type=""text"" class=""layui-input"" />
            </div>
        </div>
        <div class=""layui-inline"">
            <label class=""layui-form-label"">年级</label>
            <div class=""layui-input-inline"">
                <input name=""Grade"" type=""text"" class=""layui-input"" />
            </div>
        </div>
    </div>
    <div class=""layui-form-item"">
        <div class=""layui-inline"">
            <label class=""layui-form-label"">学员微信</label>
            <div class=""layui-input-inline"">
                <input name=""Student_Wechat"" type=""text"" class=""layui-input"" />
            </div>
        </div>
        <div class=""layui-inline"">
            <label class=""layui-form-label"">生日</label>
            <div class=""layui-input-inline"">
                <input name=""Birthday"" type=""text"" class=""layui-input"" id=""Birthday"" />
            </div>
        </div>
   ");
                WriteLiteral(@"     <div class=""layui-inline"">
            <label class=""layui-form-label"">分配校区</label>
            <div class=""layui-input-inline"">
                <select name=""CampusId"" id=""CampusId""></select>
            </div>
        </div>
    </div>
    <div class=""layui-form-item"">
        <div class=""layui-inline"">
            <label class=""layui-form-label required"">家长姓名</label>
            <div class=""layui-input-inline"">
                <input name=""Elder_Name"" type=""text"" class=""layui-input"" required />
            </div>
        </div>
        <div class=""layui-inline"">
            <label class=""layui-form-label"">家长电话</label>
            <div class=""layui-input-inline"">
                <input name=""Elder_Phone"" type=""text"" class=""layui-input"" />
            </div>
        </div>
        <div class=""layui-inline"">
            <label class=""layui-form-label"">家长微信</label>
            <div class=""layui-input-inline"">
                <input name=""Elder_Wechat"" type=""text"" class=""layui-input""");
                WriteLiteral(@" />
            </div>
        </div>
    </div>
    <div class=""layui-form-item"">
        <div class=""layui-inline"">
            <label class=""layui-form-label"">家长2姓名</label>
            <div class=""layui-input-inline"">
                <input name=""Elder2_Name"" type=""text"" class=""layui-input"" required />
            </div>
        </div>
        <div class=""layui-inline"">
            <label class=""layui-form-label"">家长2电话</label>
            <div class=""layui-input-inline"">
                <input name=""Elder2_Phone"" type=""text"" class=""layui-input"" />
            </div>
        </div>
        <div class=""layui-inline"">
            <label class=""layui-form-label"">家长2微信</label>
            <div class=""layui-input-inline"">
                <input name=""Elder2_Wechat"" type=""text"" class=""layui-input"" />
            </div>
        </div>
    </div>
    <div class=""layui-form-item"">
        <label class=""layui-form-label"">学生简介</label>
        <div class=""layui-input-block"">
            <text");
                WriteLiteral(@"area name=""Remarks"" style=""width:98%;height:120px;""></textarea>
        </div>
    </div>
    <div class=""layui-form-item positionbottomtop""></div>
    <div class=""layui-form-item text-right positionbottom"">
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
                WriteLiteral("\r\n    <style>\r\n        .layui-form-label.required:after {\r\n            content: \' *\';\r\n            color: red;\r\n        }\r\n    </style>\r\n");
            }
            );
            WriteLiteral("\r\n");
            DefineSection("js", async() => {
                WriteLiteral("\r\n    <!-- js部分 -->\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "4b10a29e0ade111df1f57c7b793f7629cbac7a7613805", async() => {
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
#line 136 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Add.cshtml"
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
        laydate.render({
            elem: '#Birthday',
            type: 'date'
        });
        var App = {
            Load: function () {
                    adminForm.load({
                        formName: 'modelUserForm',
                        KeyId: '");
#nullable restore
#line 150 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Add.cshtml"
                           Write(KeyId);

#line default
#line hidden
#nullable disable
                WriteLiteral("\',\r\n                        url: \"");
#nullable restore
#line 151 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Add.cshtml"
                         Write(Url.Action("Find"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""",
                        dataType: 'json',
                        callBack: function (r) {
                            if (r.Sex != undefined && r.Sex != null) {
                                $(""input[name='Sex'][value='"" + r.Sex + ""']"").attr(""checked"");
                            }
                            if (r.Birthday != undefined && r.Birthday != null) {
                                $(""input[name='Birthday']"").val(dateFormat(""yyyy-mm-dd"", r.Birthday));
                            }
                            adminForm.load({
                                url: """);
#nullable restore
#line 161 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Add.cshtml"
                                 Write(Url.Action("QueryCampus"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""",
                                dataType: 'json',
                                callBack: function (rou) {
                                    if (rou.data != null) {
                                        var result = rou.data;
                                        for (var i = 0; i < result.length; i++) {
                                            if (r != undefined && r.CampusId != null && r.CampusId == result[i].CampusId) {
                                                $(""select[name='CampusId']"").append('<option value = ' + result[i].CampusId + ' selected>' + result[i].CampusName +'</option>');

                                            } else {
                                                $(""select[name='CampusId']"").append('<option value = ' + result[i].CampusId + '>' + result[i].CampusName + '</option>');
                                            }
                                        }
                                    }
                                }
        ");
                WriteLiteral("                    });\r\n                    }\r\n                });\r\n            },\r\n            Save: function (data) {\r\n                adminForm.Save({\r\n                    url: KeyId ? \'");
#nullable restore
#line 182 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Add.cshtml"
                             Write(Url.Action("SaveContracUser"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\' : \'");
#nullable restore
#line 182 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Add.cshtml"
                                                                Write(Url.Action("SaveContracUser"));

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
        // 表单提交事件
        form.on('submit(modelUserSubmit)', function (data) {
            App.Save(data);
            return false;
        });
        });
        function dateFormat(fmt, datestr) {
            var date = new Date(datestr);
            let ret;
            const opt = {
                ""y+"": date.getFullYear().toString(),        // 年
                ""m+"": (date.getMonth() + 1).toString(),     // 月
                ""d+"": date.getDate().toString(),            // 日
                ""H+"": date.getHours().toString(),           // 时
                ""M+"": date.getMinutes().toString(),         // 分
                ""S+"": date.getSeconds().toString()          // 秒
                // 有其他格式化字符需求可以继续添加，必须转化成字符串
            };
            for (let k in opt) {
  ");
                WriteLiteral(@"              ret = new RegExp(""("" + k + "")"").exec(fmt);
                if (ret) {
                    fmt = fmt.replace(ret[1], (ret[1].length == 1) ? (opt[k]) : (opt[k].padStart(ret[1].length, ""0"")))
                };
            };
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
