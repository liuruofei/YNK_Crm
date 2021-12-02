#pragma checksum "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7d9ed6fa79bb353aede970b8f9240d081a558a3a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Manage_StudentPlan_Index), @"mvc.1.0.view", @"/Areas/Admin/Views/Manage/StudentPlan/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7d9ed6fa79bb353aede970b8f9240d081a558a3a", @"/Areas/Admin/Views/Manage/StudentPlan/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9e00094c6873c7f1dd971a491bd11b241da9ff38", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Manage_StudentPlan_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"
<div style=""margin-top: 15px; margin-left: 10px; margin-right: 10px"">
    <div style=""display: flex;"">
        <div style=""text-align:left;flex:1"">
            <button class=""layui-btn"" type=""button"" id=""preWeek"">Pre</button>
            <button class=""layui-btn"" type=""button"" id=""nextWeek"">Next</button>
        </div>
        <div style=""text-align:center;flex:1""><span style="" line-height: 38px;font-weight:700"">当前学员:");
#nullable restore
#line 8 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\Index.cshtml"
                                                                                               Write(ViewBag.studentName);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</span></div>
        <div style=""flex:1;text-align:right"">
            <button  class=""layui-btn"" type=""button"" onclick=""exportPlan()"">导出</button>
        </div>
    </div>
    <table class=""layui-table"" >
        <tbody id=""plantbody"">
        </tbody>
    </table>
    <input type=""hidden"" name=""StudentUid""");
            BeginWriteAttribute("value", " value=\"", 768, "\"", 795, 1);
#nullable restore
#line 17 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\Index.cshtml"
WriteAttributeValue("", 776, ViewBag.studentUid, 776, 19, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n</div>\r\n");
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
        .planPart {
            border-color: rgb(224, 224, 224);
            background-color: yellowgreen;
            box-shadow: 0px 0px 0px 1px #fff;
        }
        .planPartBlue {
            border-color: rgb(224, 224, 224);
            background-color:cadetblue;
            box-shadow: 0px 0px 0px 1px #fff;
            color:white;
        }
    </style>
");
            }
            );
            DefineSection("js", async() => {
                WriteLiteral(@"
    <!-- js部分 -->
    <script>
        var plan = {
            isAdd: 0,
            starttime:null,
            endtime:null
        }
        $(""#preWeek"").click(function () {
            plan.isAdd = 1;
            planSearch(plan);
        })
        $(""#nextWeek"").click(function () {
            plan.isAdd =2;
            planSearch(plan);
        })
        function planSearch(plan) {
            layui.use(['form','admin', 'adminForm'], function () {
                     var admin = layui.admin;
                     var adminForm = layui.adminForm;
                     $(""#plantbody"").html("""");
                     adminForm.load({
                        url: """);
#nullable restore
#line 65 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\Index.cshtml"
                         Write(Url.Action("QueryStudentPlan"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""",
                        data: { studentUid: $(""input[name='StudentUid']"").val(), isAdd: plan.isAdd, starttime: plan.starttime, endtime: plan.endtime },
                        dataType: 'json',
                        callBack: function (rou) {
                            if (rou != null) {
                                var result = rou;
                                for (var i = 0; i < 14; i++) {
                                    if (i == 0) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td style='min-width:80px'>日期</td>"");
                                        for (var j = 0; j < result.length; j++) {
                                            if (j == 0) {
                                                plan.starttime = dateFtt(""yyyy-MM-dd"", result[j].WorkDate);
                                            }
                                            if (j == result.length-1) {
           ");
                WriteLiteral(@"                                     plan.endtime = dateFtt(""yyyy-MM-dd"", result[j].WorkDate);
                                            }
                                            $(""#plantbody"").append(""<td style='text-align:center;min-width:72px;'> "" + dateFtt(""yyyy-MM-dd"", result[j].WorkDate) + "" </td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                    else if (i == 1) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>星期</td>"");
                                        for (var j = 0; j < result.length; j++) {
                                            $(""#plantbody"").append(""<td style='text-align:center;'>"" + result[j].WorkDateName + ""</td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
    ");
                WriteLiteral(@"                                }
                                    else if (i == 2) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>8:00-10:00</td>"");
                                        for (var j = 0; j < result.length; j++) {
                                            var planVmodel = {
                                                Id: result[j].Eight_Ten_Id,
                                                workTtile: result[j].Eight_Ten_OlockTitle,
                                                workDate: result[j].WorkDate,
                                                startTime: '8:00',
                                                endTime:'10:00'
                                            }
                                            var jsonVmodel = JSON.stringify(planVmodel);
                                            $(""#plantbody"").append(""<td onclick='planShow("" + jsonVmodel + ");
                WriteLiteral(@""")'><div class="" + (planVmodel.Id > 0 ? 'planPartBlue' :'planPart')+"">"" + (result[j].Eight_Ten_OlockTitle != null ? result[j].Eight_Ten_OlockTitle : """") + ""</div></td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                    else if (i == 3) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>10:00-12:00</td>"");
                                        for (var j = 0; j < result.length; j++) {
                                            var planVmodel = {
                                                Id: result[j].Ten_Twelve_Id,
                                                workTtile: result[j].Ten_Twelve_OlockTitle,
                                                workDate: result[j].WorkDate,
                                                startTime: '10:00',
             ");
                WriteLiteral(@"                                   endTime: '12:00'
                                            }
                                            var jsonVmodel = JSON.stringify(planVmodel);
                                            $(""#plantbody"").append(""<td onclick='planShow("" + jsonVmodel + "")'><div class="" + (planVmodel.Id > 0 ? 'planPartBlue' : 'planPart') +"">"" + (result[j].Ten_Twelve_OlockTitle == null ? """" : result[j].Ten_Twelve_OlockTitle) +""</div></td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                    else if (i == 4) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>13:00-15:00</td>"");
                                        for (var j = 0; j < result.length; j++) {
                                            var planVmodel = {
                                ");
                WriteLiteral(@"                Id: result[j].Thirteen_Fifteen_Id,
                                                workTtile: result[j].Thirteen_Fifteen_OlockTitle,
                                                workDate: result[j].WorkDate,
                                                startTime: '13:00',
                                                endTime: '15:00'
                                            }
                                            var jsonVmodel = JSON.stringify(planVmodel);
                                            $(""#plantbody"").append(""<td onclick='planShow("" + jsonVmodel + "")'><div class="" + (planVmodel.Id > 0 ? 'planPartBlue' : 'planPart') +"">"" + (result[j].Thirteen_Fifteen_OlockTitle == null ? """" : result[j].Thirteen_Fifteen_OlockTitle) +""</div></td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                    else if (i == 5) {
                 ");
                WriteLiteral(@"                       $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>15:10-17:10</td>"");
                                        for (var j = 0; j < result.length; j++) {
                                            var planVmodel = {
                                                Id: result[j].Fifteen_Seventeen_Id,
                                                workTtile: result[j].Fifteen_Seventeen_OlockTitle,
                                                workDate: result[j].WorkDate,
                                                startTime: '15:10',
                                                endTime: '17:10'
                                            }
                                            var jsonVmodel = JSON.stringify(planVmodel);
                                            $(""#plantbody"").append(""<td onclick='planShow("" + jsonVmodel + "")'><div class="" + (planVmodel.Id > 0 ? 'planPartBlue' : 'planPart') +"">"" + (result[j].Fi");
                WriteLiteral(@"fteen_Seventeen_OlockTitle == null ? """" : result[j].Fifteen_Seventeen_OlockTitle) +""</div></td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                    else if (i == 6) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>17:30-19:30</td>"");
                                        for (var j = 0; j < result.length; j++) {
                                            var planVmodel = {
                                                Id: result[j].Seventeen_Nineteen_Id,
                                                workTtile: result[j].Seventeen_Nineteen_OlockTitle,
                                                workDate: result[j].WorkDate,
                                                startTime: '17:30',
                                                endTime: '19:30'
   ");
                WriteLiteral(@"                                         }
                                            var jsonVmodel = JSON.stringify(planVmodel);
                                            $(""#plantbody"").append(""<td onclick='planShow("" + jsonVmodel + "")'><div class="" + (planVmodel.Id > 0 ? 'planPartBlue' : 'planPart') +"">"" + (result[j].Seventeen_Nineteen_OlockTitle == null ? """" : result[j].Seventeen_Nineteen_OlockTitle) +""</div></td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                    else if (i == 7) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>单词</td>"");
                                        for (var j = 0; j < result.length; j++) {
                                            var planVmodel = {
                                                Id: result[j].Id,
              ");
                WriteLiteral(@"                                  workTtile: result[j].ChouciComent,
                                                workDate: result[j].WorkDate
                                            }
                                            var jsonVmodel = JSON.stringify(planVmodel);
                                            $(""#plantbody"").append(""<td onclick='planShow2("" + jsonVmodel+"")'>"" + (result[j].ChouciComent == null ? """" : result[j].ChouciComent)+""</td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                    else if (i == 8) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>课程</td>"");
                                        for (var j = 0; j < result.length; j++) {
                                            var planVmodel = {
                                        ");
                WriteLiteral(@"        Id: result[j].Id,
                                                workTtile: result[j].CourseComent,
                                                workDate: result[j].WorkDate
                                            }
                                            var jsonVmodel = JSON.stringify(planVmodel);
                                            $(""#plantbody"").append(""<td onclick='planShow2("" + jsonVmodel +"")'>"" + (result[j].CourseComent == null ? """" : result[j].CourseComent) +""</td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                    else if (i == 9) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>作业</td>"");
                                        for (var j = 0; j < result.length; j++) {
                                            var planVmodel = ");
                WriteLiteral(@"{
                                                Id: result[j].Id,
                                                workTtile: result[j].HomeWorkComent,
                                                workDate: result[j].WorkDate
                                            }
                                            var jsonVmodel = JSON.stringify(planVmodel);
                                            $(""#plantbody"").append(""<td onclick='planShow2("" + jsonVmodel +"")'>"" + (result[j].HomeWorkComent == null ? """" : result[j].HomeWorkComent) +""</td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                    else if (i == 10) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>其它</td>"");
                                        for (var j = 0; j < result.length; j++) {
           ");
                WriteLiteral(@"                                 var planVmodel = {
                                                Id: result[j].Id,
                                                workTtile: result[j].OtherComent,
                                                workDate: result[j].WorkDate
                                            }
                                            var jsonVmodel = JSON.stringify(planVmodel);
                                            $(""#plantbody"").append(""<td onclick='planShow2("" + jsonVmodel +"")'>"" + (result[j].OtherComent == null ? """" : result[j].OtherComent)+ ""</td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                    else if (i == 11) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>责任助教</td>"");
                                        for (var j ");
                WriteLiteral(@"= 0; j < result.length; j++) {
                                            var planVmodel = {
                                                Id: result[j].Id,
                                                workTtile:null,
                                                workDate: result[j].WorkDate
                                            }
                                            var jsonVmodel = JSON.stringify(planVmodel);
                                            $(""#plantbody"").append(""<td onclick='planShow2("" + jsonVmodel + "")'>"" + (result[j].TaUseName == null ? """" : result[j].TaUseName)+ ""</td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                    else if (i == 12) {
                                        $(""#plantbody"").append(""<tr>"");
                                        $(""#plantbody"").append(""<td>每日总结</td>"");
                             ");
                WriteLiteral(@"           for (var j = 0; j < result.length; j++) {
                                            var planVmodel = {
                                                Id: result[j].Id,
                                                workTtile: result[j].SummaryComent,
                                                workDate: result[j].WorkDate
                                            }
                                            var jsonVmodel = JSON.stringify(planVmodel);
                                            $(""#plantbody"").append(""<td onclick='planShow2("" + jsonVmodel +"")'>"" + (result[j].SummaryComent == null ? """" : result[j].SummaryComent)+ ""</td>"");
                                        }
                                        $(""#plantbody"").append(""<tr>"");
                                    }
                                }
                            }
                        }
                    });
             });
        }
        planSearch(plan);
        function e");
                WriteLiteral("xportPlan() {\r\n            var studentUid = $(\"input[name=\'StudentUid\']\").val();\r\n            window.location.href = \'");
#nullable restore
#line 267 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\Index.cshtml"
                               Write(Url.Action("ExportPlan"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?studentUid=' + studentUid + '&isAdd=' + plan.isAdd + '&starttime=' + plan.starttime + '&endtime=' + plan.endtime;
       }
      function planShow(vmodel) {
        var studentUid = $(""input[name='StudentUid']"").val();
        if (vmodel.Id==0 &&vmodel.workTtile !=null) {return false;}
         layui.use([ 'adminList'], function () {
            var adminList = layui.adminList;
                adminList.form({
                title: (vmodel.Id > 0 ? '修改' : '添加') + '任务计划',
                type: 2,
                width: ""660px"",
                height: ""530px"",
                content: vmodel.Id > 0 ? '");
#nullable restore
#line 279 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\Index.cshtml"
                                     Write(Url.Action("Edit"));

#line default
#line hidden
#nullable disable
                WriteLiteral("?ID=\' + vmodel.Id : \'");
#nullable restore
#line 279 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\Index.cshtml"
                                                                             Write(Url.Action("Add"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?studentUid=' + studentUid + '&dataStr=' + dateFtt(""yyyy-MM-dd"",vmodel.workDate) + '&startTime=' + vmodel.startTime + '&endTime=' + vmodel.endTime,
                end: function () {
                    plan.isAdd = 0;
                    planSearch(plan);
                }
            });
        });
    }
      function planShow2(vmodel) {
          var studentUid = $(""input[name='StudentUid']"").val();
          if (vmodel.Id == 0 && vmodel.workTtile != null && vmodel.workTtile !='') {return false;}
         layui.use([ 'adminList'], function () {
            var adminList = layui.adminList;
                adminList.form({
                title: (vmodel.Id > 0 ? '修改' : '添加') + '任务计划',
                type: 2,
                width: ""660px"",
                height: ""530px"",
                content: vmodel.Id > 0 ? '");
#nullable restore
#line 297 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\Index.cshtml"
                                     Write(Url.Action("EditComment"));

#line default
#line hidden
#nullable disable
                WriteLiteral("?ID=\' + vmodel.Id : \'");
#nullable restore
#line 297 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\StudentPlan\Index.cshtml"
                                                                                    Write(Url.Action("AddComment"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?studentUid=' + studentUid + '&dataStr=' + dateFtt(""yyyy-MM-dd"",vmodel.workDate),
                end: function () {
                    plan.isAdd = 0;
                    planSearch(plan);
                }
            });
        });
    }
      function dateFtt(fmt, val) { //author: meizz
        var date = new Date(val);
        var o = {
            ""M+"": date.getMonth() + 1,     //月份
            ""d+"": date.getDate(),     //日
            ""h+"": date.getHours(),     //小时
            ""m+"": date.getMinutes(),     //分
            ""s+"": date.getSeconds(),     //秒
            ""q+"": Math.floor((date.getMonth() + 3) / 3), //季度
            ""S"": date.getMilliseconds()    //毫秒
        };
        if (/(y+)/.test(fmt))
            fmt = fmt.replace(RegExp.$1, (date.getFullYear() + """").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp(""("" + k + "")"").test(fmt))
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : ((""00"" + o[k]).substr((""");
                WriteLiteral("\" + o[k]).length)));\r\n        return fmt;\r\n    }\r\n    </script>\r\n");
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
