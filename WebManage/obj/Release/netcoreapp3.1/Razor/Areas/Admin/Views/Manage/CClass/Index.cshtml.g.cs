#pragma checksum "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\CClass\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ebf4a1e16627b82874524b18f5669649316085de"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Manage_CClass_Index), @"mvc.1.0.view", @"/Areas/Admin/Views/Manage/CClass/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ebf4a1e16627b82874524b18f5669649316085de", @"/Areas/Admin/Views/Manage/CClass/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9e00094c6873c7f1dd971a491bd11b241da9ff38", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Manage_CClass_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
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
                        <label class=""layui-form-label w-auto"">班级名称：</label>
                        <div class=""layui-input-inline mr0"">
                            <input id=""txtTitle"" class=""layui-input"" type=""text"" placeholder=""输入标题"" />
                        </div>
                    </div>
                    <div class=""layui-inline"">
                        <label class=""layui-form-label w-auto"">班类型：</label>
                        <div class=""layui-input-inline mr0"">
                            <select id=""classType"" name=""TypeId"" lay-filter=""TypeId"">
                            </select>
                        </div>
                    </div>
                    <div class=""layui-inline"">
                        <button id=""btnSearch"" ");
            WriteLiteral(@"class=""layui-btn icon-btn""><i class=""layui-icon"">&#xe615;</i>搜索</button>
                    </div>
                    <div class=""layui-inline"">
                        <button id=""btnAdd"" class=""layui-btn icon-btn"" data-power=""Add""><i class=""layui-icon"">&#xe654;</i>添加</button>
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
layui.use(['form', 'util', 'admin', 'adminList'], function () {
    var $ = layui.jquery;
    var util = layui.util;
    var admin = layui.admin;
    var form = layui.form;
    var adminList = layui.adminList;

    // 渲染表格
    var insTb = adminList.tableLayUI({
        elem: '#userTable',
        url: '");
#nullable restore
#line 69 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\CClass\Index.cshtml"
         Write(Url.Action("GetDataSource"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
        page: true,
        cellMinWidth: 100,
        cols: [[
            { type: 'numbers' },
            { field: 'TypeName', title: '类型', sort: true},
            { field: 'Class_No', title: '班级编号', sort: true },
            { field: 'Class_Name', title: '班级名称', sort: true },
            { field: 'CampusName', title: '校区', sort: true },
            { field: 'Course_Time', title: '课时', sort: true },
            { field: 'Price', title: '价格', sort: true },
            { field: 'StartDate', title: '开始日期', sort: true },
            { field: 'Start_Course_Date', title: '开课日期', sort: true },
            { field: 'End_Course_Date', title: '结课日期', sort: true },
            { field: 'Count_Users', title: '人数', sort: true },
            { field: 'Material_Count', title: '教材发放', sort: true },
            {
                field: 'Status', title: '是否取消', sort: true, templet: function (d) {
                    return d.Status > 0 ? ""已取消"" : ""正常"";
                }
            },
            {
");
                WriteLiteral(@"            field: 'Sort', title: '排序', sort: true, width: 120, templet: function (d) {
                return ""<input type='number' value='"" + d.Sort + ""' data-id='"" + d.ProjectId + ""' style='width:80px' onchange='orderBy(this)'/>"";
            }
            },
            { field: 'Remarks', title: '备注', sort: true },
            {align: 'center', toolbar: '#tableBar', title: '操作', width: 200}
        ]]
    });

    // 添加
    $('#btnAdd').click(function () {
        showEditModel();
    });

    // 搜索
    $('#btnSearch').click(function () {
        var title = $('#txtTitle').val();
        var type = $('#classType').val();
        insTb.reload({
            where: { title: title, type: type}
            , page: {
                curr: 1 //重新从第 1 页开始
            }
        });
    });
    form.on('select(classType)', function (data) {
        var title = $('#txtTitle').val();
        insTb.reload({
            where: { title: title, type: data.value }
            , page: {
     ");
                WriteLiteral(@"           curr: 1 //重新从第 1 页开始
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
            doDel(data.ClassId);
        }
    });

    //显示表单弹窗
    function showEditModel(vmodel) {
        adminList.form({
            title: (vmodel ? '修改' : '添加') + '班级',
            type: 2,
            width: ""1060px"",
            height: ""690px"",
            content: vmodel ? '");
#nullable restore
#line 144 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\CClass\Index.cshtml"
                          Write(Url.Action("Edit"));

#line default
#line hidden
#nullable disable
                WriteLiteral("?ID=\' + vmodel.ClassId : \'");
#nullable restore
#line 144 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\CClass\Index.cshtml"
                                                                       Write(Url.Action("Add"));

#line default
#line hidden
#nullable disable
                WriteLiteral("?ID=0\',\r\n            end: function () {\r\n                insTb.reload();\r\n            }\r\n        });\r\n    }\r\n    //删除\r\n    function doDel(Id) {\r\n        adminList.delete(\'");
#nullable restore
#line 152 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\CClass\Index.cshtml"
                     Write(Url.Action("Delete"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"', { Id: Id}, function () {
            insTb.reload();
        });
    }

});
        function orderBy(obj) {
            console.log($(obj).attr(""data-id""));
            $.ajax({
                type: ""post"",
                url: ""SetSort"",
                data: { ProjectId: $(obj).attr(""data-id""), sort: $(obj).val() },
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded',
                success: function (data) {
                    console.log(data);
                }
            });
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
