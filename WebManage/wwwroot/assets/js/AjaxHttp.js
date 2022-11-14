//var serverApi = "https://cc.younengkao.com/";
//var serverApi = "https://localhost:5001/";
var serverApi = "https://http://192.168.2.9:92/";
function ajaxRequst(method, url, contentType, dataModel, callBack) {
    var accountUser = sessionStorage.getItem("accountUser");
    if (accountUser != undefined && accountUser != null && accountUser != "") {
        var accountJson = JSON.parse(accountUser);
        var requestData = JSON.stringify(dataModel);//默认application/json
        if (contentType == 'application/x-www-form-urlencoded' || method == 'Get') {
            requestData = dataModel;
        }
        $.ajax({
            type: method,
            url: serverApi + url,
            data: requestData,
            dataType: 'json',
            contentType: contentType,
            beforeSend: function (request) {
                request.setRequestHeader("token", accountJson.token);
            },
            success: callBack,
            error: function (data) {
                if (data.status == 401) {
                    $.mvalidateTip("登录过期！请重新登录");
                    setTimeout(function () {
                        window.top.location = serverApi+ 'Home/Index';
                    }, 1000);
                }
            }
        });
    }
    else {
        $.mvalidateTip("请先登录");
        setTimeout(function () {
            window.top.location = serverApi +'Home/Index';
        }, 1000);
    }
}

function ajaxQuest(method, url, contentType, dataModel,callBack) {
    var requestData = JSON.stringify(dataModel);//默认application/json
    if (contentType == 'application/x-www-form-urlencoded') {
        requestData = dataModel;
    }
    $.ajax({
        type: method,
        url:url,
        data: requestData,
        dataType: 'json',
        beforeSend: function (request) {

        },
        success: callBack,
        error: function (data) {
            $.mvalidateTip(data.msg);
        }
    });
}

//获取传值
function getUrlParam(name) {
    console.log();
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}

function timestamp(value) {
    var date = new Date(value);
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    m = m < 10 ? ('0' + m) : m;
    var d = date.getDate();
    d = d < 10 ? ('0' + d) : d;
    return y + '-' + m + '-' + d;
}