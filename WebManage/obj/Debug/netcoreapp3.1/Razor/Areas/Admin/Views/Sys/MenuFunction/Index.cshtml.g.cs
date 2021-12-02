#pragma checksum "D:\YNK_Crm\WebManage\Areas\Admin\Views\Sys\MenuFunction\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1c4d43407eab14cab7952b26a43ef497175a13ff"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Sys_MenuFunction_Index), @"mvc.1.0.view", @"/Areas/Admin/Views/Sys/MenuFunction/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1c4d43407eab14cab7952b26a43ef497175a13ff", @"/Areas/Admin/Views/Sys/MenuFunction/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9e00094c6873c7f1dd971a491bd11b241da9ff38", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Sys_MenuFunction_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"
<!-- 正文开始 -->
<div class=""layui-fluid"">
    <div class=""layui-row layui-col-space15"">
        <!-- 左树 -->
        <div class=""layui-col-sm12 layui-col-md4 layui-col-lg2"">
            <div class=""layui-card"">
                <div class=""layui-card-body mini-bar"" id=""ltTree"">

                </div>
            </div>
        </div>
        <!-- 右表 -->
        <div class=""layui-col-sm12 layui-col-md8 layui-col-lg10"">
            <div class=""layui-card"">
                <div class=""layui-card-body"" style=""min-height: 535px;"">

                    <div class=""layui-form toolbar"">
                        <div class=""layui-form-item"">
                            <div class=""layui-inline"">
                                <button id=""btnAdd"" class=""layui-btn icon-btn"" data-power=""Add"">
                                    <i class=""layui-icon"">&#xe654;</i>新增
                                </button>
                            </div>
                        </div>
                    </div>
");
            WriteLiteral(@"
                    <table class=""layui-table"" id=""functionTable"" lay-filter=""functionTable""></table>

                </div>
            </div>
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
                WriteLiteral("\r\n    <link rel=\"stylesheet\" href=\"../../../assets/module/dtree/dtree.css\" />\r\n    <link rel=\"stylesheet\" href=\"../../../assets/module/dtree/font/dtreefont.css\" />\r\n");
            }
            );
            DefineSection("js", async() => {
                WriteLiteral(@"
    <!-- js部分 -->
    <script>
layui.use(['form', 'util', 'admin', 'adminList', 'dtree'], function () {
    var $ = layui.jquery;
    var layer = layui.layer;
    var form = layui.form;
    var util = layui.util;
    var admin = layui.admin;
    var adminList = layui.adminList;
    var dtree = layui.dtree

    // 渲染表格
    var insTb = adminList.tableLayUI({
        elem: '#functionTable',
        url: '");
#nullable restore
#line 65 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Sys\MenuFunction\Index.cshtml"
         Write(Url.Action("GetDataSource"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
        page: true,
        cellMinWidth: 100,
        cols: [[
            { type: 'numbers' },
            { field: 'Menu_Name', title: '菜单名称', sort: true },
            { field: 'Menu_Url', title: '地址', sort: true },
            { field: 'Parent_Name', title: '父级菜单', sort: true },
            { field: 'Menu_Num', title: '编号', sort: true },
            { field: 'Menu_Icon', title: '菜单图标', sort: true },
            { field: 'ShowName', title: '是否显示', sort: true },
            { field: 'Menu_CreateTime', title: '创建时间', sort: true },
            {align: 'center', toolbar: '#tableBar', title: '操作'}
        ]]
    });

    // 添加
    $('#btnAdd').click(function () {
        showEditModel();
    });

    // 搜索
    $('#btnSearch').click(function () {
        var key = $('#sltKey').val();
        var value = $('#edtSearch').val();
        if (value && !key) {
            admin.msg('请选择搜索条件', 2);
        }
        insTb.reload({where: {searchKey: key, searchValue: value}});
    });

");
                WriteLiteral(@"    // 工具条点击事件
    adminList.table.on('tool(functionTable)', function (obj) {
        var data = obj.data;
        var layEvent = obj.event;
        if (layEvent === 'edit') { // 修改
            showEditModel(data);
        } else if (layEvent === 'del') { // 删除
            doDel(data.Menu_ID, data.User_LoginName);
        }
    });

    //显示表单弹窗
    function showEditModel(mUser) {
        adminList.form({
            title: (mUser ? '修改' : '添加') + '菜单',
            type: 2,
            width: ""1000px"",
            height: ""600px"",
            content: mUser ? '");
#nullable restore
#line 114 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Sys\MenuFunction\Index.cshtml"
                         Write(Url.Action("Edit"));

#line default
#line hidden
#nullable disable
                WriteLiteral("?ID=\' + mUser.Menu_ID + \'&pId=\' + pId : \'");
#nullable restore
#line 114 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Sys\MenuFunction\Index.cshtml"
                                                                                     Write(Url.Action("Add"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?pId=' + pId,
            end: function () {
                insTb.reload();
            }
        });
    }
    var pId = '';
    //删除
    function doDel(Id, nickName) {
        var ids = new Array();
        ids.push(Id);
        adminList.delete('");
#nullable restore
#line 125 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Sys\MenuFunction\Index.cshtml"
                     Write(Url.Action("Delete"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\', { id: JSON.stringify(ids) }, function () {\r\n            insTb.reload();\r\n        });\r\n    }\r\n\r\n    // 树形渲染\r\n    dtree.render({\r\n        elem: \'#ltTree\',\r\n        url: \'");
#nullable restore
#line 133 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Sys\MenuFunction\Index.cshtml"
         Write(Url.Action("GetMenuAndFunctionTree"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
        type: 'all',
        initLevel: '2',
        dot: false,
        method: 'GET'
    });
    // 树形点击事件
    dtree.on('node(""ltTree"")', function (obj) {
        var data = obj.param;
        //layer.msg('你选择了：' + data.nodeId + '-' + data.context);
        if (data.nodeId)
            pId = data.nodeId;
        insTb.reload({ where: { Menu_ID: data.nodeId } });
    });

});
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
