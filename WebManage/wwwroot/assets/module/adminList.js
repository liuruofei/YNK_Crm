/** EasyWeb iframe v3.1.1 data:2019-03-24 License By http://easyweb.vip */

layui.define(["layer", "table"], function (f) {
  var h = layui.jquery;
  var k = layui.layer;
  var admin = layui.admin;
  var table = layui.table;
  var m = {
    table: table,
    /**
     * 打开一个编辑页面。参考文档：https://www.layui.com/doc/modules/layer.html
     * @param {any} options
     */
    form: function (options) {
      if (options.width && options.height)
        options.area = [options.width, options.height];
      options.skin = "layui-layer-admin"
      top.layer.open(options);
    },
    /**
     * LayUI表格控件，参考文档：https://www.layui.com/doc/modules/table.html
     * @param {any} options
     */
    tableLayUI: function (options) {
      var defaults = {
        elem: '#modelTable',  //指定原始 table 容器的选择器或 DOM，方法渲染方式必填
        url: '',  //异步数据接口相关参数。其中 url 参数为必填项
        page: true, //开启分页（默认：false） 注：从 layui 2.2.0 开始，支持传入一个对象，里面可包含 laypage 组件所有支持的参数（jump、elem除外）
        method: 'post',
        cellMinWidth: 100,//（layui 2.2.1 新增）全局定义所有常规单元格的最小宽度（默认：60），一般用于列宽自动分配的情况。其优先级低于表头参数中的 minWidth
        limit: 20,  //每页显示的条数（默认：10）。值务必对应 limits 参数的选项。 注意：优先级低于 page 参数中的 limit 参数
        limits: [20, 30, 50, 100],  //每页条数的选择项，默认：[10,20,30,40,50,60,70,80,90]。 注意：优先级低于 page 参数中的 limits 参数
        loading: true,  //是否显示加载条（默认：true）。如果设置 false，则在切换分页时，不会出现加载条。该参数只适用于 url 参数开启的方式
        title: '',  //定义 table 的大标题（在文件导出等地方会用到）layui 2.4.0 新增
        text: { none: '暂无相关数据' }, //自定义文本，如空数据时的异常提示等。注：layui 2.2.5 开始新增。
        autoSort: true, //默认 true，即直接由 table 组件在前端自动处理排序。 若为 false，则需自主排序，通常由服务端直接返回排序好的数据。 
        cols: null, //设置表头。值是一个二维数组。方法渲染方式必填
        done: null  //数据渲染完的回调。你可以借此做一些其它的操作
      }
      var optionss = $.extend({}, defaults, options);
      optionss.done = function (res, curr, count) {
        if (options.done != null) {
          options.done(res, curr, count);
        }
        admin.setPower();
      }
      return table.render(optionss);
    },
    delete: function (url, dataobj, callBack,title) {
      top.layer.confirm((title?title:'确定要删除吗？'), {
        skin: 'layui-layer-admin'
      }, function (i) {
        top.layer.close(i);
        admin.layer.load(2);
        $.post(url, dataobj, function (res) {
          admin.layer.closeAll('loading');
          if (res.code == 200) {
            admin.layer.msg(res.msg, { icon: 1 });
            if (callBack)
              callBack();
          } else {
            admin.layer.msg(res.msg, { icon: 2 });
          }
        }, 'json');
      });
    },
    Update: function (isenable, url, dataobj, callBack) {
      var str = "";
      if (isenable == 1) {
        str = "是否停用?";
      } else {
        str = "是否启用?";
      }
      top.layer.confirm(str, {
        skin: 'layui-layer-admin'
      }, function (i) {
        top.layer.close(i);
        admin.layer.load(2);
        $.post(url, dataobj, function (res) {
          admin.layer.closeAll('loading');
          if (res.code == 200) {
            admin.layer.msg(res.msg, { icon: 1 });
            if (callBack)
              callBack();
          } else {
            admin.layer.msg(res.msg, { icon: 2 });
          }
        }, 'json');
      });
    }
  };
  f("adminList", m)
});