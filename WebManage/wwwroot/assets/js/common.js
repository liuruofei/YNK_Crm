// 以下代码是配置layui扩展模块的目录，每个页面都需要引入
layui.config({
    base: getProjectUrl() + 'assets/module/'
}).extend({
    formSelects: 'formSelects/formSelects-v4',
    treetable: 'treetable-lay/treetable',
    dropdown: 'dropdown/dropdown',
    notice: 'notice/notice',
    step: 'step-lay/step',
    dtree: 'dtree/dtree',
    citypicker: 'city-picker/city-picker',
    tableSelect: 'tableSelect/tableSelect'
}).use(['layer', 'admin'], function () {
    var $ = layui.jquery;
    var layer = layui.layer;
    var admin = layui.admin;

    // 移除loading动画
    setTimeout(function () {
        admin.removeLoading();
    }, window == top ? 600 : 100);

});

// 获取当前项目的根路径，通过获取layui.js全路径截取assets之前的地址
function getProjectUrl() {
    var layuiDir = layui.cache.dir;
    if (!layuiDir) {
        var js = document.scripts, last = js.length - 1, src;
        for (var i = last; i > 0; i--) {
            if (js[i].readyState === 'interactive') {
                src = js[i].src;
                break;
            }
        }
        var jsPath = src || js[last].src;
        layuiDir = jsPath.substring(0, jsPath.lastIndexOf('/') + 1);
    }
    return layuiDir.substring(0, layuiDir.indexOf('assets'));
}
function deleteFile(key, number, _this) {
  $(_this).parent().remove();
  if (number > 1) {
    var _Html = $('#' + key).parent().find('.file-list');
    if (_Html.find('div').length < number) {
      $('#' + key).parent().find('#' + key).show();
    }
  }
}