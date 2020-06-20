'use strict';

$(function () {
    $('#AllHistoryList').on('click', 'tr', function (e) {

        var table = $('#AllHistoryList .data-table').DataTable();
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        } else {
            table.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }


        var appid = getUrlParameter('appid');
        var hiddenAutoReply = getUrlParameter('hiddenAutoReply') == null ? false : true;
        var type = 8;
        var id = $(this).find('img:first').data('id') || 0;

        if ($("#AllHistoryList").find(".dataTables_empty").length === 0)
        {
            layer.open({
                skin: "historylistlayer",
                type: 2,
                closeBtn: false, //不显示关闭按钮
                title: false,
                offset: ['0px', $(document).width() - 400 + 16 + 'px'], //右下角弹出
                shadeClose: true,
                shade: [0.8, '#393D49'],
                maxmin: false, //开启最大化最小化按钮
                area: ['400px', $(window).height() + 'px'],
                scrollbar: false,
                content: ['/Scripts/history/html/template-v2.html?' + new Date().getTime().toString() + '&type=' + type + '&id=' + id + '&appid=' + appid + '&hiddenAutoReply=' + hiddenAutoReply]
            });

        }

        

    });

    var getUrlParameter = function getUrlParam(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
        var r = window.location.search.substr(1).match(reg); //匹配目标参数
        if (r != null) return unescape(r[2]); return null; //返回参数值
    };

    $('#openSendMsglayer').on('click', function () {
        var openId = $("#SendMsgOpenId").val();
        var appId = getUrlParameter('appid');
        var openUrl = '/Scripts/history/html/leftSendMsg.html?' + new Date().getTime().toString() + '&openId=' + openId + '&appid=' + appId;
        layer.open({
            skin: "quickreplylayer",
            type: 2,
            closeBtn: false, //不显示关闭按钮
            title: false,
            offset: ['0px', '0px'], //初始坐标
            shadeClose: true,
            shade: ["0", '#fff'],
            maxmin: false, //开启最大化最小化按钮
            area: [$(document).width() - 401 + 'px', $(window).height() + 'px'],
            //scrollbar: false,
            content: [openUrl],
        });
    });
});



//# sourceMappingURL=app_bk-compiled.js.map