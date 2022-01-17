function ajaxRequst(method, url, dataModel, callBack) {
    $.ajax({
        type:method,
        url:url,
        data: dataModel,
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded',
        success: callBack
    });
}