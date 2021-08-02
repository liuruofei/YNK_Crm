var http="http://www.younengkao.com/";
//var http="http://localhost:5000/";
//var http = "http://192.168.2.9:92/";
function ajaxRequst(method, url, dataModel, callBack) {
    $.ajax({
        type:method,
        url: http +url,
        data: dataModel,
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded',
        success: callBack
    });
}