#pragma checksum "D:\Dingzhihua2\WebManage\Areas\Admin\Views\Login\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fdf8976dc385ac09ba9918e5246c813575762baa"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Login_Index), @"mvc.1.0.view", @"/Areas/Admin/Views/Login/Index.cshtml")]
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
#line 1 "D:\Dingzhihua2\WebManage\Areas\Admin\Views\_ViewImports.cshtml"
using WebManage;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Dingzhihua2\WebManage\Areas\Admin\Views\_ViewImports.cshtml"
using WebManage.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Dingzhihua2\WebManage\Areas\Admin\Views\_ViewImports.cshtml"
using ADT.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Dingzhihua2\WebManage\Areas\Admin\Views\_ViewImports.cshtml"
using ADT.Common;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fdf8976dc385ac09ba9918e5246c813575762baa", @"/Areas/Admin/Views/Login/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9e00094c6873c7f1dd971a491bd11b241da9ff38", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Login_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 2 "D:\Dingzhihua2\WebManage\Areas\Admin\Views\Login\Index.cshtml"
  
    Layout = null;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<!DOCTYPE html>\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fdf8976dc385ac09ba9918e5246c813575762baa3775", async() => {
                WriteLiteral(@"
    <meta charset=""utf-8"">
    <title>登入</title>
    <meta name=""renderer"" content=""webkit"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, minimum-scale=1.0, maximum-scale=1.0, user-scalable=0"">
    <link rel=""stylesheet"" href=""/assets/libs/layui/css/layui.css"" media=""all"">
    <link rel=""stylesheet"" href=""/style/admin.css"" media=""all"">
    <link rel=""stylesheet"" href=""/style/login.css"" media=""all"">
    <!--基础js-->
    <script src=""/assets/libs/jquery/jquery-3.2.1.min.js""></script>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "fdf8976dc385ac09ba9918e5246c813575762baa5366", async() => {
                WriteLiteral(@"

    <div class=""layadmin-user-login layadmin-user-display-show"" id=""LAY-user-login"" style=""display: none;"">

        <div class=""layadmin-user-login-main"">
            <div class=""layadmin-user-login-box layadmin-user-login-header"">
                <h2>ALL</h2>
                <p>用户报名管理平台</p>
            </div>
            <div class=""layadmin-user-login-box layadmin-user-login-body layui-form"">
                <div class=""layui-form-item"">
                    <label class=""layadmin-user-login-icon layui-icon layui-icon-username"" for=""LAY-user-login-username""></label>
                    <input type=""text"" name=""username"" id=""LAY-user-login-username"" lay-verify=""required"" placeholder=""用户名"" class=""layui-input"">
                </div>
                <div class=""layui-form-item"">
                    <label class=""layadmin-user-login-icon layui-icon layui-icon-password"" for=""LAY-user-login-password""></label>
                    <input type=""password"" name=""password"" id=""LAY-user-login-password""");
                WriteLiteral(" lay-verify=\"required\" placeholder=\"密码\" class=\"layui-input\">\r\n                </div>\r\n");
                WriteLiteral(@"                <div class=""layui-form-item"">
                    <button class=""layui-btn layui-btn-fluid"" onclick=""App.Checked()"" lay-submit>登 入</button>
                </div>
            </div>
        </div>

    </div>
    <script src=""/assets/libs/layui/layui.all.js""></script>
    <script src=""/admin.js""></script>
");
                WriteLiteral(@"
    <script type=""text/javascript"">
        $(function () {
            $(document).keydown(function (e) {
                if (e.which == 13) App.Checked();
            });
        });

        var App = {
            Checked: function () {
                var uName = $(""input[name=username]"").val();
                var uPwd = $(""input[name=password]"").val();
                //var loginCode = $(""input[name=loginCode]"").val();
                if (!uName) return admin.msg(""请输入用户名"", ""警告"",2000);
                if (!uPwd) return admin.msg(""请输入密码"", ""警告"");
                //if (!loginCode) return admin.msg(""请输入验证码"", ""警告"");
                admin.ajax({
                    loading: true,
                    isauth: false,
                    url: """);
#nullable restore
#line 103 "D:\Dingzhihua2\WebManage\Areas\Admin\Views\Login\Index.cshtml"
                     Write(Url.Action("Index"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""",
                    data: {
                        username: uName,
                        password: uPwd
                    },
                    success: function (r) {
                        if (r.status == 200) {
                            //admin.loading.start();
                            window.location = '/Admin';
                        }

                    }
                });
            }
        };
    </script>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</html>");
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
