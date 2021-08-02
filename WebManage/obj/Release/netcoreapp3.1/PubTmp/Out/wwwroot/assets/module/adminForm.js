/** EasyWeb iframe v3.1.1 data:2019-03-24 License By http://easyweb.vip */

layui.define(["layer"], function (f) {
  var h = layui.jquery;
  var k = layui.layer;
  var admin = layui.admin;
  var form = layui.form;
  var m = {
    /**
     * 加载表单默认数据。参考文档：https://www.layui.com/doc/modules/form.html
     * @param {any} options
     */
    load: function (options) {
      $('html').attr('style','background-color:#fff');
      var defaults = {
        formName: 'modelForm',  //表单ID
        KeyId: "",
        url: "",
        data: null,
        success: null,
        callBack: null,
        title: null
      };
      var options = $.extend({}, defaults, options);
      //初始化表单
      admin.ajax({
        url: options.url,
        dataType: 'json',
        data: options.data ? options.data : { ID: options.KeyId },
        success: function (r) {
          if (JSON.stringify(r) != '{}') {
            form.val(options.formName, r);
          }
          if (options.callBack)
            options.callBack(r);
          m.verify();
          form.render();
        }
      });
    },
    /**自定义表单验证（使用方法lay-verify="required|pass"） */
    verify: function () {
      form.verify({
        username: function (value, item) { //value：表单的值、item：表单的DOM对象
          if (!new RegExp("^[a-zA-Z0-9_\u4e00-\u9fa5\\s·]+$").test(value)) {
            return '用户名不能有特殊字符';
          }
          if (/(^\_)|(\__)|(\_+$)/.test(value)) {
            return '用户名首尾不能出现下划线\'_\'';
          }
          if (/^\d+\d+\d$/.test(value)) {
            return '用户名不能全为数字';
          }
        }
        //我们既支持上述函数式的方式，也支持下述数组的形式
        //数组的两个值分别代表：[正则匹配、匹配不符时的提示文字]
        , pass: [
          /^[\S]{6,12}$/
          , '密码必须6到12位，且不能出现空格'
        ]
      });
    },
    /**
     * 提交保存信息。参考文档：https://www.layui.com/doc/modules/form.html#onsubmit
     * @param {any} options
     */
    Save: function (options) {
      var defaults = {
        isClose: true,
        url: "",
        data: null,
        success: null,
        callBack: null,
        msg: null,
        isupfile: false,
      };
      var options = $.extend({}, defaults, options);
      layer.load(2);
      admin.ajax({
        type: 'post',
        url: options.url,
        data: options.data,
        dataType: 'json',
        success: function (r) {
          layer.closeAll('loading');
          if (r.code == 200) {
            if (options.success) {
              options.success(r);
            }
            else {
              top.layer.msg(r.msg ? r.msg : "操作成功！", { icon: 1 });
            }
            if (options.callBack)
              options.callBack(r);
          } else {
            top.layer.msg(r.msg, { icon: 0 });
          }
        },
        error: function (r) {
          console.log('<<<<<', r);
          top.layer.msg("操作失败！", { icon: 2 });
        }
      });
    }
  };
  f("adminForm", m)
});