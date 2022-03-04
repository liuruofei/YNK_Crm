#pragma checksum "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\MyCourseWork\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3ee731ea89e9f92400080682eeef5d60c2f9fb33"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Manage_MyCourseWork_Index), @"mvc.1.0.view", @"/Areas/Admin/Views/Manage/MyCourseWork/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3ee731ea89e9f92400080682eeef5d60c2f9fb33", @"/Areas/Admin/Views/Manage/MyCourseWork/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9e00094c6873c7f1dd971a491bd11b241da9ff38", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Manage_MyCourseWork_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
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
                        <label class=""layui-form-label w-auto"">标题：</label>
                        <div class=""layui-input-inline mr0"">
                            <input id=""txtTitle"" class=""layui-input"" type=""text"" placeholder=""学生或者班级名称"" />
                        </div>
                    </div>
                    <div class=""layui-inline"">
                        <button id=""btnSearch"" class=""layui-btn icon-btn""><i class=""layui-icon"">&#xe615;</i>搜索</button>
                    </div>
                </div>
            </div>


        </div>
    </div>
</div>
<div class=""layui-tab layui-tab-card"" lay-filter=""tabWork"" style=""margin-top:0px"">
    <ul class=""layui-tab-title"">
        <li class=""layui-this"">周视图</li>
        <li>表格视图</li>
   ");
            WriteLiteral(@" </ul>
    <div class=""layui-tab-content"" style=""padding:0px;"">
        <div class=""layui-tab-item layui-show"" style=""padding-top:10px"">
            <div id='calendar'></div>
        </div>
        <div class=""layui-tab-item"" style=""padding-top:10px"">
            <div style=""display:flex;flex-wrap:wrap;"">
                <div style=""max-width:908px;display:flex"">
                    <label class=""layui-form-label w-auto"">开始时间：</label>
                    <input type=""text"" class=""layui-input"" name=""StartTime"" id=""StartTime"" style=""flex:1"" />
                    <label class=""layui-form-label w-auto"">截止时间之前：</label>
                    <input type=""text"" class=""layui-input"" name=""EndTime"" id=""EndTime"" style=""flex:1"" />
                </div>
                <div style=""flex: 1;line-height:39px;text-align:center;font-weight:600"">
                    <label id=""totalCourseTime""></label>
                </div>
            </div>
            <table class=""layui-table"" id=""userTable"" lay-filter=""us");
            WriteLiteral("erTable\"></table>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div style=\"position:absolute;background-color:yellowgreen\" id=\"flexPlay\"></div>\r\n");
            DefineSection("css", async() => {
                WriteLiteral(@"
    <link href=""/assets/libs/fullCalendar/main.css"" rel=""stylesheet"" />
    <style>
        body {
            margin: 6px 10px;
            padding: 0;
            font-family: Arial, Helvetica Neue, Helvetica, sans-serif;
            font-size: 14px;
        }

        #calendar {
            max-width: 100%;
            margin: 0 20px;
        }

        .fc-daygrid-dot-event {
            display: block !important;
            align-items: center;
            padding: 2px 0;
            /*background-color: yellowgreen;*/
        }

        .fc-h-event .fc-event-main-frame {
            /* display: flex; */
            flex-wrap: wrap;
        }

        .tagbackColor {
            background-color: yellowgreen;
        }
    </style>
");
            }
            );
            DefineSection("js", async() => {
                WriteLiteral(@"
    <!-- 表格操作列 -->
    <script type=""text/html"" id=""tableBar"">
        <a class=""layui-btn layui-btn-primary layui-btn-xs"" data-power=""Edit"" lay-event=""edit"">点评</a>
    </script>
    <script src=""/assets/libs/fullCalendar/main.js""></script>
    <!-- js部分 -->
    <script type=""text/javascript"">
    var defaultStartTime = '';
    var defaultEndTime = '';
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        //initialDate: '2020-09-12',//默认日期
        //自定义按钮
        //customButtons: {
        //    prev: {
        //        text: 'prev',
        //        click: function (event, span, value) {
        //            preMonth();
        //            var currentDate = dateFtt(""yyyy-MM-dd"", calendar.currentData.currentDate);
        //            console.log(cur");
                WriteLiteral(@"rentDate);
        //        }
        //    }
        //},
        allDayText: 'all-day',
        firstDay: true,
        locale: ""zh"",//中文
        initialView:'timeGridWeek',
        dayMaxEvents: true,//允许更多连接allow ""more"" link when too many events
        displayEventEnd: true,//允许展示时间段
        buttonIcons: true,//显示上月，下月中文按钮
        allDaySlot: false,//allday 整天的日程是否显示
        showNonCurrentDates: false,//true显示非本月日期
        //slotDuration: '00:30',//设置时间间隔，
        //设置日历y轴上显示的时间文本格式
        slotLabelFormat: [
            { hour: '2-digit', minute: '2-digit', omitZeroMinute: false, hour12: false, meridiem: 'short'}
        ],
        scrollTime:'07:00',//默认时间
        slotMinTime:'07:00',//最小时间
        slotMaxTime:'24:00',//最大时间
        weekNumbers: false, //周显示开关
        navLinks: false, //允许天/周名称是否可点击
        navLinkDayClick: function (date, jsEvent) {
            // 这里进行日名称点击事件处理，Ajax请求，date格式化后为当日
        },
        dateClick: function (date, jsEvent, view) { //单个时间点击
      ");
                WriteLiteral(@"  },
        showNonCurrentDates: true,//是否显示非本月日期
        selectable: false, //是否允许用户单击或者拖拽日历中的天和时间隙
        selectMirror: true, //是否允许显示用户单击或者拖拽错误提示
        //select: function (arg) {  //在日历中选择某个时间之后触发该回调函数。
        //    //点击后退出选中事件
        //    calendar.unselect()
        //},
        eventClick: function (info) {  //当点击日历中某个事件的时候触发 eventClick 回调：
            //修改
            showEditModel(info.event.id);
            //if (confirm('Are you sure you want to delete this event?')) {
            //    arg.event.remove()
            //}
        },
        eventResize: function (event, delta, revertFunc) {//事件被拉长或缩短
            event.revert();
        },
        eventDrop: function (event, oldEvent) { //拖拽事件
              event.revert();
        },
        editable: false,
        events: function (arg, callback) {
            var events = [];
            var title = $(""#txtTitle"").val();
            var startStr = dateFtt(""yyyy-MM-dd"", arg.startStr);
            var endStr = dateFtt(");
                WriteLiteral("\"yyyy-MM-dd\", arg.endStr);\r\n            defaultStartTime = startStr;\r\n            defaultEndTime = endStr;\r\n            $.ajax({\r\n                url: \'");
#nullable restore
#line 163 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\MyCourseWork\Index.cshtml"
                 Write(Url.Action("QueryWorkSource"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                dataType: 'json',
                type: 'post',
                data: { startStr: startStr, endStr: endStr, userName: title},
                success: function (data) {
                    var result = data.data;
                    if (result.length > 0) {
                        for (var i = 0; i < result.length; i++) {
                            var atDate = dateFtt(""yyyy-MM-dd"", result[i].AT_Date)
                            var atStart = atDate + "" "" + result[i].StartTime;
                            var atEnd = atDate + "" "" + result[i].EndTime;
                            var backColor = 'yellowgreen';
                            if (result[i].StudyMode == 3) {
                                backColor = 'brown';
                            }
                            else {
                                if (result[i].Work_Stutas == 1) {
                                    backColor = '#8b9825';
                                }
                            }
 ");
                WriteLiteral(@"                           events.push({
                                id: result[i].Id,
                                title:result[i].Work_Title + ' 教师:' + result[i].TeacherName,
                                start: atStart,
                                end: atEnd,
                                display:'block',
                                color: '#CCCCFF',//设置颜色#666666#1ab394
                                backgroundColor: backColor,//设置背景颜色
                                borderColor: ""#e0e0e0"",   //日程边框颜色
                            });
                        }
                    }
                    callback(events);  //回调  doc中就是一个个上面所说数据库Urlrlrlmodel的json串
                }
            });
        },
        eventMouseEnter: function (event, el) {
            var title = event.event.title + "" <br/>时间:"" + dateFormat(""HH:MM"", event.event.startStr) + "" - "" + dateFormat(""HH:MM"", event.event.endStr);
            var pageX = (event.jsEvent.pageX-20)+ ""px"";
            var");
                WriteLiteral(@" pageY = (event.jsEvent.pageY+10)+ ""px"";
            layer.tips(title,$(event.ele), {
                time:3000,
                tips: [1, '#CCCCFF'], //设置tips方向和颜色 类型：Number/Array，默认：2 tips层的私有参数。支持上右下左四个方向，通过1-4进行方向设定。如tips: 3则表示在元素的下面出现。有时你还可能会定义一些颜色，可以设定tips: [1, '#c00']
                success: function (layero, index) {
                    layero.css(""left"", pageX);
                    layero.css(""top"", pageY);
                    //$("".layui-layer-tips"").css(""left"", pageX);
                    //$("".layui-layer-tips"").css(""top"", pageY);
                }
            });
        },
        eventMouseLeave: function (event) {

        }
    });
    calendar.render();
    function showEditModel(wkId,iniTable) {
        layui.use(['form', 'admin', 'adminList'], function () {
            var $ = layui.jquery;
            var admin = layui.admin;
            var adminList = layui.adminList;
            adminList.form({
                title:'添加点评',
                type: 2,
          ");
                WriteLiteral("      width: \"820px\",\r\n                height: \"690px\",\r\n                content:\'");
#nullable restore
#line 229 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\MyCourseWork\Index.cshtml"
                    Write(Url.Action("Add"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"?ID=' + wkId,
                end: function () {
                    //重新渲染日历
                    calendar.refetchEvents();
                    if (iniTable != undefined && iniTable != null && iniTable!='') {
                        iniTable.reload({
                            where: { startStr: defaultStartTime, endStr: defaultEndTime, userName: $(""#txtTitle"").val()}
                        });
                    }
                }
            });

        });
    }
        layui.use(['adminList', 'element', 'adminForm', 'laydate'], function () {
            var adminList = layui.adminList;
            var element = layui.element;
            var adminForm = layui.adminForm;
            var laydate = layui.laydate;
            laydate.render({
                elem: '#StartTime',
                type: 'date'
            });
            laydate.render({
                elem: '#EndTime',
                type: 'date'
            });
            $(""#btnSearch"").click(function () {
 ");
                WriteLiteral(@"               calendar.refetchEvents();
                if ($(""input[name='StartTime']"").val()=="""")
                    $(""input[name='StartTime']"").val(defaultStartTime);
                if ($(""input[name='EndTime']"").val() == """")
                    $(""input[name='EndTime']"").val(defaultEndTime);
                initable.reload({
                    where: { startStr: $(""input[name='StartTime']"").val(), endStr: $(""input[name='EndTime']"").val(), userName: $(""#txtTitle"").val() }
                });
            })
            var initable=adminList.tableLayUI({
                elem: '#userTable',
                url: '");
#nullable restore
#line 268 "D:\YNK_Crm\WebManage\Areas\Admin\Views\Manage\MyCourseWork\Index.cshtml"
                 Write(Url.Action("QueryWorkSource2"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"',
                page: true,
                where: {
                    startStr: defaultStartTime, endStr: defaultEndTime, userName: $(""#txtTitle"").val()
                },
                cellMinWidth: 100,
                cols: [[
                    { type: 'numbers' },
                    { field: 'Work_Title', title: '上课科目', sort: true },
                    {
                        field: 'AT_Date', title: '上课日期', sort: true, width: 120, templet: function (d) {
                            return dateFtt(""yyyy-MM-dd"", d.AT_Date);
                        }
                    },
                    {
                        field: 'StartTime', title: '时间段', sort: true, width: 120, templet: function (d) {
                            return d.StartTime + "" - "" + d.EndTime;
                        }
                    },
                    { field: 'TeacherName', title: '教师', sort: true, width: 120 },
                    { field: 'RoomName', title: '教室', sort: true, width: 120 },");
                WriteLiteral(@"
                    { field: 'TaUserName', title: '督学', sort: true, width: 120 },
                    {
                        field: 'Work_Stutas', title: '状态', sort: true, width: 120, templet: function (d) {
                            if (d.StudyMode != 3)
                                return d.Work_Stutas > 0 ? "" <span style=' background-color:#6ab36a;color: white;'>已完成</span>"" : ""未完成"";
                            else
                                return ""休息中"";
                        }
                    },
                    { align: 'center', toolbar: '#tableBar', title: '操作', width: 200 }
                ]],
                parseData: function (res) {
                    if (res.totalRow != null)
                        $(""#totalCourseTime"").text($(""#txtTitle"").val() + ""已上课时："" + res.totalRow.totalCourseTime + ""h"");
                    else
                        $(""#totalCourseTime"").text("""");
                }
            });
            // 工具条点击事件
            adminList.t");
                WriteLiteral(@"able.on('tool(userTable)', function (obj) {
                var data = obj.data;
                var layEvent = obj.event;
                if (layEvent === 'edit') { // 点评
                    showEditModel(data.Id, initable);
                }
            });
            //一些事件监听
            element.on('tab(tabWork)', function (data) {
                if (data.index == 0) {
                    calendar.refetchEvents();
                } else if (data.index == 1) {
                   $(""input[name='StartTime']"").val(defaultStartTime);
                   $(""input[name='EndTime']"").val(defaultEndTime);
                    initable.reload({
                        where: { startStr: $(""input[name='StartTime']"").val(), endStr: $(""input[name='EndTime']"").val(), userName: $(""#txtTitle"").val() }
                    });
                }
            });
        });

    function dateFtt(fmt, val) { //author: meizz
        var date = new Date(val);
        var o = {
            ""M+"": date.getMon");
                WriteLiteral(@"th() + 1,     //月份
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
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : ((""00"" + o[k]).substr(("""" + o[k]).length)));
        return fmt;
    }


    function dateFormat(fmt, datestr) {
        var date = new Date(datestr);
        let ret;
        const opt = {
            ""Y+"": date.getFullYear().toString(),        // 年
            ""m+"": (date.getMonth() + 1).toString(),     // 月
            ""d+"": date.getDate().toString(),            // 日
            ""H+"": date.ge");
                WriteLiteral(@"tHours().toString(),           // 时
            ""M+"": date.getMinutes().toString(),         // 分
            ""S+"": date.getSeconds().toString()          // 秒
            // 有其他格式化字符需求可以继续添加，必须转化成字符串
        };
        for (let k in opt) {
            ret = new RegExp(""("" + k + "")"").exec(fmt);
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
