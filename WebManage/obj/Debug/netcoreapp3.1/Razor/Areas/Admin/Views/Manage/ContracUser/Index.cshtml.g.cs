#pragma checksum "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "131bea8fcce4c204157d94815f218c0b83759193"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Manage_ContracUser_Index), @"mvc.1.0.view", @"/Areas/Admin/Views/Manage/ContracUser/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"131bea8fcce4c204157d94815f218c0b83759193", @"/Areas/Admin/Views/Manage/ContracUser/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9e00094c6873c7f1dd971a491bd11b241da9ff38", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Manage_ContracUser_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<!-- 正文开始 -->
<div class=""layui-fluid"">
    <div class=""layui-card"">
        <div class=""layui-card-body"">
            <div class=""layui-form toolbar"">
                <div class=""layui-form-item"">
                    <div class=""layui-inline"">
                        <label class=""layui-form-label w-auto"">关键字：</label>
                        <div class=""layui-input-inline mr0"">
                            <input id=""txtTitle"" class=""layui-input"" type=""text"" placeholder=""输入标题"" />
                        </div>
                    </div>
                    <div class=""layui-inline"">
                        <button id=""btnSearch"" class=""layui-btn icon-btn""><i class=""layui-icon"">&#xe615;</i>搜索</button>
                    </div>
                    <div class=""layui-inline"">
                        <button id=""btnAdd"" class=""layui-btn icon-btn"" data-power=""Add""><i class=""layui-icon"">&#xe654;</i>新签学员</button>
                    </div>
                    <div class=""layui-inline"">
           ");
            WriteLiteral(@"             <button id=""btnAssignCC"" class=""layui-btn icon-btn"" data-power=""Audit""><i class=""layui-icon"">&#xe66f;</i>分配CC</button>
                    </div>
                    <div class=""layui-inline"">
                        <button id=""btnAssignCR"" class=""layui-btn icon-btn"" data-power=""Audit""><i class=""layui-icon"">&#xe66f;</i>分配CR</button>
                    </div>
                    <div class=""layui-inline"">
                        <button id=""btnAddAccount"" class=""layui-btn icon-btn"" data-power=""Audit""><i class=""layui-icon"">&#xe66f;</i>创建账号</button>
                    </div>
                    <div class=""layui-inline"">
                        <button id=""btnAddContract"" class=""layui-btn icon-btn"" data-power=""AddContract""><i class=""layui-icon"">&#xe654;</i>创建合同</button>
                    </div>
                </div>
            </div>

            <table class=""layui-table"" id=""userTable"" lay-filter=""userTable""></table>
        </div>
    </div>
</div>

<!-- 表格操作列 -->
<scri");
            WriteLiteral(@"pt type=""text/html"" id=""tableBar"">
    <a class=""layui-btn layui-btn-primary layui-btn-xs"" data-power=""Edit"" lay-event=""edit"">修改</a>
    <a class=""layui-btn layui-btn-primary layui-btn-xs"" data-power=""Edit"" lay-event=""plan"">任务计划</a>
    <a class=""layui-btn layui-btn-primary layui-btn-xs"" lay-event=""courseDetail"">课程详情</a>
    <a class=""layui-btn layui-btn-danger layui-btn-xs"" data-power=""Delete"" lay-event=""del"">删除</a>
</script>

");
            DefineSection("css", async() => {
                WriteLiteral(@"
    <link rel=""stylesheet"" href=""/assets/module/formSelects/formSelects-v4.css"" />
   <style>
       .layui-table-cell {
           height: auto;
           line-height: 28px;
       }
       .layui-table img {
           max-width: 600px;
       }
   </style>
");
            }
            );
            DefineSection("js", async() => {
                WriteLiteral(@"
    <!-- js部分 -->
    <script>
    layui.use(['form', 'util', 'admin', 'adminList', 'table', 'adminForm'], function () {
    var $ = layui.jquery;
    var util = layui.util;
    var admin = layui.admin;
    var table = layui.table;
    var adminList = layui.adminList;
    var adminForm = layui.adminForm;

    // 渲染表格
    var insTb = adminList.tableLayUI({
        elem: '#userTable',
        url: '");
#nullable restore
#line 76 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Index.cshtml"
         Write(Url.Action("GetDataSource"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
        page: true,
        cellMinWidth: 100,
        cols: [[
            { type: 'checkbox', fixed: 'left' },
            { type: 'numbers' },
            { field: 'Student_Name', title: '学生名称', sort: true },
            { field: 'Sex', title: '性别', sort: true,width:50 },
            { field: 'Grade', title: '年级', sort: true },
            { field: 'Student_Phone', title: '手机', sort: true },
            { field: 'CC_UserName', title: '顾问', sort: true },
            { field: 'CR_UserName', title: 'CR', sort: true },
            { field: 'CampusName', title: '校区', sort: true },
            {
                field: 'Student_Account', title: '账号', sort: true, width: 100,templet: function (d) {
                    return (d.Student_Account != """" && d.Student_Account != null) ?""<span style='color:green'>""+d.Student_Account+"" <i class='layui-icon layui-icon-ok'></i></span>"":"""";
                }
            },
            { field: 'Amount', title: '余额', sort: true, width: 100 },
            ");
                WriteLiteral(@"{ field: 'Elder_Name', title: '联系人', sort: true },
            { align: 'center', toolbar: '#tableBar', title: '操作', width: 250 }
        ]]
    });

    // 添加
    $('#btnAdd').click(function () {
        showEditModel();
    });

    // 新增合同
    $('#btnAddContract').click(function () {
        showAddContract();
    });
    //添加账号
   $(""#btnAddAccount"").click(function () {
        var checkStatus = table.checkStatus('userTable');
        if (checkStatus.data.length > 0) {
            if (checkStatus.data.length > 1) {
                layer.msg('只能选择一个学员!');
            }
            else {
                adminList.form({
                title: '添加账号',
                type: 2,
                width: ""560px"",
                height: ""460px"",
                    content: '");
#nullable restore
#line 122 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Index.cshtml"
                         Write(Url.Action("SettingAccount", "ContracUser"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?ID=' + checkStatus.data[0].StudentUid,
                    end: function (data) {
                        insTb.reload();
                     }
                });
            }
        }
        else {
            layer.msg('请勾选一位学员!');
        }
   });
      //分配CC
  $(""#btnAssignCC"").click(function () {
        var checkStatus = table.checkStatus('userTable');
        if (checkStatus.data.length > 0) {
            if (checkStatus.data.length > 1) {
                layer.msg('只能选择一个学员!');
            }
            else {
                adminList.form({
                title: '分配CC',
                type: 2,
                width: ""430px"",
                height: ""390px"",
                    content: '");
#nullable restore
#line 146 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Index.cshtml"
                         Write(Url.Action("AssinCC"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?ID=' + checkStatus.data[0].StudentUid,
                    end: function (data) {
                        insTb.reload();
                     }
                });
            }
        }
        else {
            layer.msg('请勾选一位学员!');
        }
  });
      //分配CR
  $(""#btnAssignCR"").click(function () {
        var checkStatus = table.checkStatus('userTable');
        if (checkStatus.data.length > 0) {
            if (checkStatus.data.length > 1) {
                layer.msg('只能选择一个学员!');
            }
            else {
                adminList.form({
                title: '分配CR',
                type: 2,
                    width: ""430px"",
                    height: ""390px"",
                    content: '");
#nullable restore
#line 170 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Index.cshtml"
                         Write(Url.Action("AssinCR"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?ID=' + checkStatus.data[0].StudentUid,
                    end: function (data) {
                        insTb.reload();
                     }
                });
            }
        }
        else {
            layer.msg('请勾选一位学员!');
        }
    });
    // 搜索
    $('#btnSearch').click(function () {
        var title = $('#txtTitle').val();
        insTb.reload({
            where: { title: title }
            , page: {
                curr: 1 //重新从第 1 页开始
            }
        });
    });

    // 工具条点击事件
    adminList.table.on('tool(userTable)', function (obj) {
        var data = obj.data;
        var layEvent = obj.event;
        if (layEvent === 'edit') { // 修改
            showEditModel(data);
        } else if (layEvent === 'del') { // 删除
            doDel(data.Id);
        } else if (layEvent === 'plan') { // 学员计划
            showplan(data.Student_Name,data.StudentUid);
        } else if (layEvent == 'courseDetail') {
            showCourseDetail(data.Student_Name");
                WriteLiteral(@",data.StudentUid);
        }
    });

    function showAddContract() {
        var checkStatus = table.checkStatus('userTable');
        if (checkStatus.data.length > 0) {
            if (checkStatus.data.length > 1) {
                layer.msg('只能选择一个学员!');
            }
            else {
                    adminList.form({
                    title: '创建合同',
                    type: 2,
                    width: ""1060px"",
                    height: ""760px"",
                        content: '");
#nullable restore
#line 219 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Index.cshtml"
                             Write(Url.Action("CreateContrc", "ContracUser"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?clueId=0&studentId=' + checkStatus.data[0].StudentUid,
                        end: function (data) {
                            layer.msg(""请在学员合同管理中查看合同"");
                    }
                 });
            }
        }
        else {
            layer.msg('请勾选一位学员创建合同!');
        }
    }

    //显示表单弹窗
    function showEditModel(vmodel) {
        adminList.form({
            title: (vmodel ? '修改' : '添加') + '签约用户',
            type: 2,
            width: ""1060px"",
            height: ""590px"",
            content: vmodel ? '");
#nullable restore
#line 238 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Index.cshtml"
                          Write(Url.Action("Edit"));

#line default
#line hidden
#nullable disable
                WriteLiteral("?ID=\' + vmodel.StudentUid : \'");
#nullable restore
#line 238 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Index.cshtml"
                                                                          Write(Url.Action("Add"));

#line default
#line hidden
#nullable disable
                WriteLiteral("?ID=0\',\r\n            end: function () {\r\n                insTb.reload();\r\n            }\r\n        });\r\n    }\r\n    //删除\r\n    function doDel(Id) {\r\n        adminList.delete(\'");
#nullable restore
#line 246 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Index.cshtml"
                     Write(Url.Action("Delete"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"', { Id: Id}, function () {
            insTb.reload();
        });
    }
    //任务计划
        function showplan(userName,studentUid) {
        window.parent.layui.element.tabAdd('admin-pagetabs', {
            title: userName +'任务计划',
            content: '<iframe lay-id=""' + studentUid + '"" src=""");
#nullable restore
#line 254 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Index.cshtml"
                                                          Write(Url.Action("index","StudentPlan"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?studentUid=' + studentUid + '"" frameborder=""0"" class=""admin-iframe"" style=""width: 100%;height: 100%""></iframe>' //支持传入html
            , studentUid: studentUid
        });
    };
    function showCourseDetail(userName,studentUid) {
        window.parent.layui.element.tabAdd('admin-pagetabs', {
            title:userName+'课程详情',
            content: '<iframe lay-id=""item' + studentUid + '"" src=""");
#nullable restore
#line 261 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ContracUser\Index.cshtml"
                                                              Write(Url.Action("CourseDetail"));

#line default
#line hidden
#nullable disable
                WriteLiteral("?studentUid=\' + studentUid + \'\" frameborder=\"0\" class=\"admin-iframe\" style=\"width: 100%;height: 100%\"></iframe>\' //支持传入html\r\n            , studentUid: studentUid\r\n        });\r\n    }\r\n });\r\n    </script>\r\n");
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
