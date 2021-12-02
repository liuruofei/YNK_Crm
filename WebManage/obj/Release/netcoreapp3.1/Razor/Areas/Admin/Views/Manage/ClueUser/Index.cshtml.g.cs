#pragma checksum "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ClueUser\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "cf177c6dca40845cc544108d62cfa30d55ae2372"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Manage_ClueUser_Index), @"mvc.1.0.view", @"/Areas/Admin/Views/Manage/ClueUser/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"cf177c6dca40845cc544108d62cfa30d55ae2372", @"/Areas/Admin/Views/Manage/ClueUser/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9e00094c6873c7f1dd971a491bd11b241da9ff38", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Manage_ClueUser_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
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
                        <button id=""btnAdd"" class=""layui-btn icon-btn"" data-power=""Add""><i class=""layui-icon"">&#xe654;</i>添加</button>
                    </div>
                    <div class=""layui-inline"">
             ");
            WriteLiteral(@"           <button id=""btnAddContract"" class=""layui-btn icon-btn"" data-power=""AddContract""><i class=""layui-icon"">&#xe654;</i>创建合同</button>
                    </div>
                </div>
            </div>

            <table class=""layui-table"" id=""userTable"" lay-filter=""userTable""></table>
        </div>
    </div>
</div>

<!-- 表格操作列 -->
<script type=""text/html"" id=""tableBar"">
    <a class=""layui-btn layui-btn-primary layui-btn-xs"" data-power=""Edit"" lay-event=""edit"">修改</a>
    <a class=""layui-btn layui-btn-danger layui-btn-xs"" data-power=""Delete"" lay-event=""del"">删除</a>
    <a class=""layui-btn layui-btn-primary layui-btn-xs"" data-power=""Edit"" lay-event=""change"">转移线索</a>
    <a class=""layui-btn layui-btn-danger layui-btn-xs"" data-power=""Audit"" lay-event=""Audit"">指派线索</a>
    <a class=""layui-btn layui-btn-danger layui-btn-xs"" data-power=""Add"" lay-event=""record"">添加跟踪记录</a>
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
#line 68 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ClueUser\Index.cshtml"
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
            { field: 'Student_Name', title: '学生名称', sort: true, width: 130,fixed: 'left'},
            { field: 'Sex', title: '性别', sort: true },
            { field: 'Grade', title: '年级', sort: true },
            { field: 'Student_Phone', title: '手机', sort: true },
            { field: 'Elder_Name', title: '联系人', sort: true },
            { field: 'CC_UserName', title: '顾问拥有者', sort: true },
            { field: 'Default_CC_UserName', title: '顾问转移者', sort: true },
            { field: 'CR_Uid', title: 'CR', sort: true },
            { field: 'CampusName', title: '校区', sort: true },
            { field: 'IsVisitName', title: '是否上门', sort: true },
            { field: 'Visit_Date', title: '上门日期', sort: true, width: 100},
            { field: 'ContracRateName', title: '签约可能性', sort: true },
            { field: 'Follow_Date', title: '跟踪日期', sort:");
                WriteLiteral(@" true, width: 100 },
            { field: 'Follow_Plan', title: '跟踪计划', sort: true, width: 100 },
            { field: 'Follow_Count', title: '跟进次数', sort: true, width: 100 },
            { field: 'CreateTime', title: '创建时间', sort: true },
            {align: 'center', toolbar: '#tableBar', title: '操作', width: 360}
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
        } else if (l");
                WriteLiteral(@"ayEvent === 'del') { // 删除
            doDel(data.Id);
        }
        else if (layEvent === 'Audit') { // 指派线索
            AssignModel(data);
        }
        else if (layEvent === 'change') { // 转移线索
            OwinModel(data);
        }
        else if (layEvent === 'record') { // 添加跟踪记录
            AddRecord(data);
        }
    });

    function showAddContract() {
        var checkStatus = table.checkStatus('userTable');
        if (checkStatus.data.length > 0) {
            if (checkStatus.data.length > 1) {
                layer.msg('不能同时选择多条线索!');
            }
            else {
                adminList.form({
                title: '创建合同',
                type: 2,
                width: ""1060px"",
                height: ""690px"",
                    content: '");
#nullable restore
#line 148 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ClueUser\Index.cshtml"
                         Write(Url.Action("CreateContrc", "ContracUser"));

#line default
#line hidden
#nullable disable
                WriteLiteral("?studentId=0&clueId=\' + checkStatus.data[0].ClueId,\r\n                    end: function (data) {\r\n                        adminForm.load({\r\n                            url: \'");
#nullable restore
#line 151 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ClueUser\Index.cshtml"
                             Write(Url.Action("QuerYContrcByClueId", "ContracUser"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?clueId=' + checkStatus.data[0].ClueId,
                            dataType: 'json',
                            callBack: function (rou) {
                                if (rou.data != null) {
                                    layer.msg(""合同已创建,请在学员合同管理中查看"");
                                }
                            }
                        });
                }
        });
            }
        }
        else {
            layer.msg('请勾选一条线索创建合同!');
        }
    }

    //显示表单弹窗
    function showEditModel(vmodel) {
        adminList.form({
            title: (vmodel ? '修改' : '添加') + '线索',
            type: 2,
            width: ""1060px"",
            height: ""690px"",
            content: vmodel ? '");
#nullable restore
#line 175 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ClueUser\Index.cshtml"
                          Write(Url.Action("Edit"));

#line default
#line hidden
#nullable disable
                WriteLiteral("?ID=\' + vmodel.ClueId : \'");
#nullable restore
#line 175 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ClueUser\Index.cshtml"
                                                                      Write(Url.Action("Add"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?ID=0',
            end: function () {
                insTb.reload();
            }
        });
    }
    function AssignModel(vmodel) {
        adminList.form({
            title:'指派线索',
            type: 2,
            width: ""1060px"",
            height: ""435px"",
            content:'");
#nullable restore
#line 187 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ClueUser\Index.cshtml"
                Write(Url.Action("SetAssign"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?ID=' + vmodel.ClueId,
            end: function () {
                insTb.reload();
            }
        });
     }
    function OwinModel(vmodel) {
        adminList.form({
            title:'转移线索',
            type: 2,
            width: ""1060px"",
            height: ""435px"",
            content:'");
#nullable restore
#line 199 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ClueUser\Index.cshtml"
                Write(Url.Action("OwinClue"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?ID=' + vmodel.ClueId,
            end: function () {
                insTb.reload();
            }
        });
    }
    function AddRecord(vmodel) {
        console.log('a');
            adminList.form({
            title:'添加追踪记录',
            type: 2,
            width: ""1060px"",
            height: ""635px"",
            content: '");
#nullable restore
#line 212 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ClueUser\Index.cshtml"
                 Write(Url.Action("AddRecord"));

#line default
#line hidden
#nullable disable
                WriteLiteral("?ID=\' + vmodel.ClueId,\r\n            end: function () {\r\n                insTb.reload();\r\n            }\r\n        });\r\n\r\n    }\r\n    //删除\r\n    function doDel(Id) {\r\n        adminList.delete(\'");
#nullable restore
#line 221 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\ClueUser\Index.cshtml"
                     Write(Url.Action("Delete"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\', { Id: Id}, function () {\r\n            insTb.reload();\r\n        });\r\n    }\r\n\r\n});\r\n    </script>\r\n");
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
