

/*
 * 
 * 
 * 
 * 
 * 
 * 
 * 作者：hzy 开源地址：https://gitee.com/hao-zhi-ying/HzyAdmin
 * 
 * 
 * 
 * 
 */


//admin 封装 对象

var admin = {
    layer: window.top.layer,
    upload: layui.upload,
    laydate: layui.laydate,
    init: function () {
        admin.setPower();
        admin.ajaxFilter();
        admin.onError();
        jQuery(function () {

        });
    },
    ajaxFilter: function () {
        jQuery(document).ajaxStart(function () {

        });
        jQuery(document).ajaxSend(function () {

        });
        jQuery(document).ajaxSuccess(function () {

        });
        jQuery(document).ajaxComplete(function (event, xhr, options) {
            var text = xhr.responseText;
            try {
                var json = JSON.parse(text);

                //信息提示 = 10,
                //登录超时 = 20,
                //手动处理 = 30,
                //程序异常 = 50

                switch (json.status) {
                    case 10:
                        admin.msg(json.msg, "警告");
                        break;
                    case 20:
                        alert('登录超时！系统将退出重新登录！');
                        top.window.location = json.msg;
                        //admin.confirm("登录超时！是否退出系统重新登录？", function () { top.window.location = json.msg }, function () { }, "登录超时！");
                        break;
                    case 50:
                        admin.ajax({
                            type: "post",
                            url: "/Admin/Error/Index",
                            data: json,
                            dataType: "html",
                            success: function (h) {
                                $('html').empty().html(h);
                            }
                        });
                        break;
                };


            } catch (e) {
                //throw new Error(e.message);
            }
        });
        jQuery(document).ajaxError(function (event, xhr, options, exc) {
            //event - 包含 event 对象
            //xhr - 包含 XMLHttpRequest 对象
            //options - 包含 AJAX 请求中使用的选项
            //exc - 包含 JavaScript exception

            admin.loading.end();

            var text = xhr.responseText;
            var json = JSON.parse(text);
            if (json.status !== 10 && json.status !== 20 && json.status !== 30 && json.status !== 50) {
                throw new Error("ajax请求错误!");
            }

        });
        jQuery(document).ajaxStop(function () {

        });
    },
    //异常捕获
    onError: function () {
        window.onerror = function (msg, url, line, col, error) {
            //alert(msg);
        }
    },
    //消息框
    alert: function (content, type, title, callBack) {
        if (type == "警告")
            return admin.layer.alert(content, { icon: 0, offset: "t", title: title || "警告" }, callBack);
        else if (type == "成功")
            return admin.layer.alert(content, { icon: 1, offset: "t", title: title || "成功" }, callBack);
        else if (type == "错误")
            return admin.layer.alert(content, { icon: 2, offset: "t", title: title || "错误" }, callBack);
        else
            return admin.layer.alert(content, { offset: "t", title: title || "提醒" }, callBack);
    },
    //提示框
    msg: function (content, type, time) {
        if (type == "警告")
            return admin.layer.msg(content, { icon: 0, offset: "t", time: time || 10 * 1000 });
        else if (type == "成功")
            return admin.layer.msg(content, { icon: 1, offset: "t", time: time || 10 * 1000 });
        else if (type == "错误")
            return admin.layer.msg(content, { icon: 2, offset: "t", time: time || 10 * 1000 });
        else
            return admin.layer.msg(content, { offset: "t", time: time || 10 * 1000 });
    },
    //询问框
    confirm: function (content, yes, cancel, title) {
        admin.layer.confirm(content, { icon: 3, offset: "t", title: title || "提醒" }, yes, cancel);
    },
    //加载
    loading: {
        index: 0,
        start: function () {
            this.index = admin.layer.load(1, { shade: [0.1, "#fff"], time: 600 * 60 * 1000 });
        },
        end: function () {
            var _this = this;
            admin.layer.close(_this.index);
        }
    },
    //Ajax请求
    ajax: function (options) {
        var defaults = {
            type: "post",
            url: "",
            dataType: "json",
            data: {},
            success: null,
            async: true,
            isupfile: false,//是否 开启上传文件
            loading: false,
            isauth: true
        };
        var options = $.extend(defaults, options);
        if (!options.url)
            return false;
        if (options.loading)
            admin.loading.start();

        var ajaxOptions = {
            type: options.type,
            url: options.url,
            dataType: options.dataType,
            data: options.data,
            async: options.async,
            success: function (r) {
                if (options.loading)
                    admin.loading.end();
                options.success(r);
            },
            beforeSend: function (XMLHttpRequest) {
            },
            complete: function () {

            }
        };

        if (options.isupfile) {
            ajaxOptions.processData = false;
            ajaxOptions.contentType = false;
            ajaxOptions.cache = false;
        }

        $.ajax(ajaxOptions);
    },
    //根据键取地址栏中的值
    getQueryString: function (key) {
        var reg = new RegExp("(^|&)" + key + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return r[2]; return "";
    },
    //建立一個可存取到該file的url  用于上传图片，，可通过该地址浏览图片
    getObjectUrl: function (file) {
        var url = "";
        if (window.createObjectURL != undefined) { // basic
            url = window.createObjectURL(file);
        } else if (window.URL != undefined) { // mozilla(firefox)
            url = window.URL.createObjectURL(file);
        } else if (window.webkitURL != undefined) { // webkit or chrome
            url = window.webkitURL.createObjectURL(file);
        }
        return url;
    },
    //将图片对象转换为 base64
    readFile: function (obj, callBack) {
        var file = obj.files[0];
        var resVal;
        //判断类型是不是图片  
        if (!/image\/\w+/.test(file.type)) {
            alert("请确保文件为图像类型");
            return false;
        }
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function (e) {
            //alert(this.result); //就是base64  
            resVal = this.result;
            if (callBack) callBack(resVal);
            //return resVal;
        }

    },
    setSession: function (key, value) {
        window.top.localStorage.setItem(key, JSON.stringify(value));
    },
    getSession: function (key) {
        return JSON.parse(window.top.localStorage.getItem(key));
    },
    koMapping: function (json, obj, objName) {
        //json 映射到 ko 实体中去
        var code = "";
        for (var prop in json) {
            if (obj.hasOwnProperty(prop)) {
                code += objName + "." + prop + "(";

                var value = json[prop];
                if (value || value === 0 || value === false) {
                    if ((typeof value == 'string') && value.constructor == String) {
                        value = value.replace(/\r/g, "\\r").replace(/\n/g, "\\n");
                        //判断是否 是这种格式 /Date(1539187200000)/
                        if (value.indexOf('/Date(') >= 0 && value.lastIndexOf(')/') >= 0) {
                            value = new Date(parseInt(value.substring(6, value.lastIndexOf(')/')))).Format('yyyy-MM-dd hh:mm');
                        }
                    }
                    code += "'" + value + "'";
                }

                code += "); ";
            }
        }
        if (code) eval(code);
    },
    vModelEmpty: function (obj, objName) {
        //重置ko 的 实体
        var code = "";
        for (item in obj) {
            code += objName + "." + item + "(''); ";
        }
        if (code) eval(code);
    },
    getParentFrameName: function () {
        return top.$("#" + admin.getIframeName()).attr("data-parentiframename");
    },
    //获取系统框架 中 打开的 iframe 对象
    getWinIframe: function () {
        return top.$("Content>.hzy-navTab-li.active").attr("name");
    },
    //获取 layer iframe 窗口的 index
    getLayerIframeIndex: function () {
        return admin.layer.getFrameIndex(window.name);
    },
    getIframeName: function () {
        return "layui-layer-iframe" + admin.getLayerIframeIndex();
    },
    //根据Iframe的name 获取Ifram对象 dx对象可不用传递
    getIframeObj: function (name, dx) {
        var f;
        if (dx == "" || dx == null || dx == undefined) {
            dx = "top"; f = eval(dx + ".frames");
        }
        else {
            f = eval(dx);
        }
        for (var i = 0; i < f.length; i++) {
            if (f[i].length > 0) {
                return admin.getIframeObj(name, dx + ".frames['" + f[i].name + "']");
            }
            else {
                if (f[i].name == name) {
                    return dx + ".frames['" + name + "']";
                }
            }
        }
    },
    //执行某个Iframe中的函数 funName：函数名（参数1，参数2，...）  iframeName：iframe名字
    exFunction: function (funName, iframeName) {
        try {
            eval(admin.getIframeObj(iframeName) + "." + funName + ";");
            return true;
        } catch (e) {
            throw new Error(e.message);
            return false;
        }
    },
    //查找带回
    findBack: {
        close: function (row) {
            var findback = admin.getQueryString("findback");
            if (findback) {
                admin.setSession('findBackJson', row);
                admin.layer.close(admin.getLayerIframeIndex());
            }
        },
        open: function (content, title, callBack, w, h, isFull, parentIframeName) {
            admin.openWindow({
                content: content,
                title: title || "查找带回",
                IsFull: isFull,
                width: w ? w : "1200px",
                height: h ? h : "1200px",
                btn: false,
                success: function (layero) {
                    $(layero).find("iframe").attr("data-parentiframename", parentIframeName || admin.getIframeName());
                },
                end: function () {
                    var findBackJson = admin.getSession("findBackJson");
                    if (callBack && findBackJson && findBackJson.length > 0) {
                        callBack(findBackJson);
                    }
                    admin.setSession("findBackJson", []);
                }
            });
        }
    },
    //打开 iframe 窗口
    openWindow: function (options) {
        var defaults = {
            id: "",
            url: "",
            title: "系统窗口",
            width: "1200px",
            height: "1200px",
            btn: ["保存", "关闭"],
            btnClass: ['btn btn-primary', 'btn btn-danger'],
            shade: [0.1, "#fff"],
            parentIframeName: "",
            callBack: null,
            IsFull: false,
            success: null,
            end: null,
        };
        var options = $.extend({}, defaults, options);
        var _width = top.$("html").width() > parseInt(options.width.replace('px', '')) ? options.width : top.$("html").width() + 'px';
        var _height = top.$("html").height() > parseInt(options.height.replace('px', '')) ? options.height : top.$("html").height() + 'px';

        var json = $.extend({}, {
            id: options.id,
            type: 2,
            shade: options.shade,
            title: options.title,
            fix: false,
            area: [_width, _height],
            content: options.url,
            shift: 0,
            btn: options.btn,
            btnClass: options.btnClass,
            maxmin: true,
            anim: 2,
            //zIndex: Lay.zIndex, //重点1
            success: options.success,
            yes: function () {
                options.callBack(options.id);
            }, cancel: function () {
                return true;
            }, end: function () {
                options.end();
            }
        }, options);

        var index = admin.layer.open(json);
        if (options.IsFull) {
            admin.layer.full(index);
        }

    },
    setPower: function () {
        try {
            if (PowerModel) {
                for (var item in PowerModel) {
                    if (!PowerModel[item]) {
                        $("*[data-power=" + item + "]").remove();
                    }
                }
            }
        } catch (e) {

        }
    },
    setStorage: function (key, value) {
        var v = value;
        if (typeof v == 'object') {
            v = JSON.stringify(v);
            v = 'obj-' + v;
        } else {
            v = 'str-' + v;
        }
        var ls = uzStorage();
        if (ls) {
            ls.setItem(key, v);
        }
    },
    getStorage: function (key) {
        var ls = uzStorage();
        if (ls) {
            var v = ls.getItem(key);
            if (!v) {
                return;
            }
            if (v.indexOf('obj-') === 0) {
                v = v.slice(4);
                return JSON.parse(v);
            } else if (v.indexOf('str-') === 0) {
                return v.slice(4);
            }
        }
    },
    uploadImg: function (options) {
        var defaults = {
            elem: null,//指向容器选择器，如：elem: '#id'。也可以是DOM对象
            url: '/Admin/UploadFile',//服务端上传接口，返回的数据规范请详见下文
            data: { exits: '.jpg|.png|.gif|.jpeg' },//请求上传接口的额外参数。如：data: {id: 'xxx'} 从 layui 2.2.6 开始，支持动态值，如:
            headers: null,//接口的请求头。如：headers: {token: 'sasasas'}。注：该参数为 layui 2.2.6 开始新增
            accept: 'images',//指定允许上传时校验的文件类型，可选值有：images（图片）、file（所有文件）、video（视频）、audio（音频）
            acceptMime: null,//规定打开文件选择框时，筛选出的文件类型，值为用逗号隔开的 MIME 类型列表。如： acceptMime: 'image/*'（只显示图片文件） acceptMime: 'image/jpg, image/png'（只显示 jpg 和 png 文件） 注：该参数为 layui 2.2.6 开始新增
            exts: 'jpg|png|gif|bmp|jpeg',//允许上传的文件后缀。一般结合 accept 参数类设定。假设 accept 为 file 类型时，那么你设置 exts: 'zip|rar|7z' 即代表只允许上传压缩格式的文件。如果 accept 未设定，那么限制的就是图片的文件格式
            auto: true,//是否选完文件后自动上传。如果设定 false，那么需要设置 bindAction 参数来指向一个其它按钮提交上传
            bindAction: null,//指向一个按钮触发上传，一般配合 auto: false 来使用。值为选择器或DOM对象，如：bindAction: '#btn'
            field: null,//设定文件域的字段名
            size: 500,//设置文件最大可允许上传的大小，单位 KB。不支持ie8/9
            multiple: false,//是否允许多文件上传。设置 true即可开启。不支持ie8/9
            number: null,//设置同时可上传的文件数量，一般配合 multiple 参数出现。注意：该参数为 layui 2.2.3 开始新增
            drag: true,//是否接受拖拽的文件上传，设置 false 可禁用。不支持ie8/9
            choose: function () { },//选择文件后的回调函数。返回一个object参数，
            before: function () { },//文件提交上传前的回调。返回一个object参数（同上）
            done: function () { },//执行上传请求后的回调。返回三个参数，分别为：res（服务端响应信息）、index（当前文件的索引）、upload（重新上传的方法，一般在文件上传失败后使用）
            error: function () { }//执行上传请求出现异常的回调（一般为网络异常、URL 404等）。返回两个参数，分别为：index（当前文件的索引）、upload（重新上传的方法）
        };
        var options = $.extend(defaults, options);
        //console.log(options.exts);
        admin.upload.render(options);
    },
    uploadFile: function (options) {
        var defaults = {
            accept: 'file',
            data: { exits: '.pdf' },
            exts: 'pdf'
        };
        var options = $.extend(defaults, options);
        admin.uploadImg(options);
    },
    initImg: function (options) {
        var defaults = {
            key: '#demo',
            data: '',
            number: 1
        };
        var options = $.extend(defaults, options);
        var _Html = $('' + options.key).parent().find('.hzy-upfile-item');
        if (options.data) {
            if (options.data.indexOf(',') >= 0) {
                var hlist = options.data.split(',');
                for (var i = 0; i < hlist.length && i < options.number; i++) {
                    _Html.append("<div><img src='" + hlist[i] + "' style='height:140px;' /><a onclick='admin.deleteFile(\"" + options.key + "\",\"" + options.number + "\",this)'>删除</a></div>");
                }
            } else {
                if (options.number > 1) {
                    _Html.append("<div><img src='" + options.data + "' style='height:140px;' /><a onclick='admin.deleteFile(\"" + options.key + "\",\"" + options.number + "\",this)'>删除</a></div>");
                    //console.log(_Html.find('div').length);
                } else
                    _Html.html("<div><img src='" + options.data + "' style='height:140px;' /><a onclick='admin.deleteFile(\"" + options.key + "\",\"" + options.number + "\",this)'>删除</a></div>");
            }
            if (options.number > 1) {
                //console.log(_Html.find('div').length);
                _Html.attr('style', 'text-align:left');
                if (_Html.find('div').length >= options.number) {
                    $('' + options.key).parent().find('.hzy-upfile-shade').hide();
                    $('' + options.key).parent().find('.layui-upload-file').hide();
                }
            }
        }
    },
    initFile: function (options) {
        var defaults = {
            key: '#demo',
            data: '',
            number: 1
        };
        var options = $.extend(defaults, options);
        var _Html = $('' + options.key).parent().find('.hzy-upfile-item');
        if (options.data) {
            if (options.data.indexOf(',') >= 0) {
                _Html.attr('style', 'text-align:left');
                var hlist = options.data.split(',');
                for (var i = 0; i < hlist.length && i < options.number; i++) {
                    _Html.append("<div><img src='/images/t1.png' title='图片路径：" + hlist[i] + "' style='height:140px;' /><a onclick='admin.deleteFile(\"" + options.key + "\",\"" + options.number + "\",this)'>删除</a></div>");
                }
            } else {
                if (options.number > 1) {
                    _Html.attr('style', 'text-align:left');
                    _Html.append("<div><img src='/images/t1.png' title='图片路径：" + options.data + "' style='height:140px;' /><a onclick='admin.deleteFile(\"" + options.key + "\",\"" + options.number + "\",this)'>删除</a></div>");
                } else
                    _Html.html("<div><img src='/images/t1.png' title='图片路径：" + options.data + "' style='height:140px;' /><a onclick='admin.deleteFile(\"" + options.key + "\",\"" + options.number + "\",this)'>删除</a></div>");
            }
            if (options.number > 1) {
                _Html.attr('style', 'text-align:left');
                if (_Html.find('div').length >= options.number) {
                    $('' + options.key).parent().find('.hzy-upfile-shade').hide();
                    $('' + options.key).parent().find('.layui-upload-file').hide();
                }
            }
        }
    },
    deleteFile: function (key, number, _this) {
        $(_this).parent().remove();
        if (number > 1) {
            var _Html = $('' + key).parent().find('.hzy-upfile-item');
            if (_Html.find('div').length < number) {
                $('' + key).parent().find('.hzy-upfile-shade').show();
                $('' + key).parent().find('.layui-upload-file').show();
            }
        }
    }
};

//admin 框架初始化
admin.init();

//定义全局变量函数
var uzStorage = function () {
    var ls = window.localStorage;
    return ls;
};
// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
