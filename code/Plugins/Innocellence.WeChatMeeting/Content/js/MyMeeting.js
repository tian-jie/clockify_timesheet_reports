
    $(function () {
        var $body = $('body');

        $body.off('click', '.select_time', pickTime).on('click', '.select_time', pickTime);

        var hid_ml = $("#hid_ml").val();
        if (hid_ml != "") {
            $(".weui-panel").show();
        }

        var dlegth = $(".weui-panel").length;
        if (dlegth > 20) {
            $("#lm").show();
        }
        else { $("#lm").hide(); }

        if (getUrlParam("strWhere") == null || parseInt(getUrlParam("strWhere")) == 1)
        {
            $("#times").text("最近三个月");
        }
        else if(parseInt(getUrlParam("strWhere")) == 0)
        {
            $("#times").text("全部");
        
        } else if (parseInt(getUrlParam("strWhere")) == 2) {
            $("#times").text("最近半年");
        }
        else {
            $("#times").text("最近一年");
        }

    });


//加载更多
function LoadMore() {
    var appStr = "";
    //$.ajax({
    //    url: "/WeChatMeeting/MeetingInvitation/GetMoreData",
    //    type: "post",
    //    data: null,
    //    success: function success(data) {
    //        if (data.length > 0) {
    //            for (var i = 0; i < data.length; i++) {
    //                appStr += '<div class="weui-panel">' +
    //        '<div class="weui-panel__bd">' +
    //       '<div class="weui-media-box weui-media-box_text">' +
    //        '<a href="InvitationDetail?MeetId=' + data[i].Id + '"><h4>' + data[i].Title + '</h4></a>' +
    //            '</div>' +
    //               '<div class="weui-panel__hd">' + data[i].Location + '</div>' +
    //               '<div class="weui-cell weui-cell_access">' +
    //               '<div class="weui-cell__hd">' +
    //               ' 会议：' +
    //               '</div>' +
    //               ' <div class="weui-cell__bd weui-cell_primary">' +
    //               ' ' + data[i].STime + '→' + data[i].ETime + '' +
    //               ' </div>' +
    //               '</div>' +
    //               ' <div class="weui-cell weui-cell_access">' +
    //               '<div class="weui-cell__hd">' +
    //               '签到：' +
    //               '</div>' +
    //               ' <div class="weui-cell__bd weui-cell_primary">' +
    //               ' ' + data[i].SsTime + '→' + data[i].SeTime + '' +
    //               ' </div>' +
    //               ' </div>' +
    //                ' <div class="weui-cell weui-cell_access">' +
    //                ' <a class="weui-cell__hd flex_1 font_black" href="SignUp">' +
    //                '报名信息：<span>' +
    //                ' ' + data[i].IsPerInfo + '' +
    //                ' </span><i class="fa fa-pencil-square-o mar_l" aria-hidden="true"></i>' +
    //                '</a>' +
    //                '<a class="weui-cell__bd font_black weui-cell_primary " href="Travel?meetingid=' + data[i].Id + '" style="margin-left: 10px;">' +
    //                ' 行程信息：<span>' + data[i].IsTravel + '</span><i class="fa fa-pencil-square-o mar_l" aria-hidden="true"></i>' +
    //                '</a>' +
    //                ' </div>' +
    //                ' </div>' +
    //                ' </div>';
    //            }

    //            $("#mdata").html(appStr);
    //            $("#lm").hide();
    //        }
    //        else { layer.msg("没有更多数据了！"); $("#lm").hide(); }
    //    }
    //});
}
function getUrlParam(parameters) {
    var reg = new RegExp("(^|&)" + parameters + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}
function pickTime() {
    var _this = this;
    var def = getUrlParam("strWhere") == null ? 1 : parseInt(getUrlParam("strWhere"));
    weui.picker([{
        label: '全部',
        value: 0
    }, {
        label: '最近三个月',
        value: 1
    }, {
        label: '最近半年',
        value: 2
    }, {
        label: '最近一年',
        value: 3
    }], {
        defaultValue: [def],
        className: 'custom-classname',
        onChange: function onChange(result) {
              

        },
        onConfirm: function onConfirm(result) {
            //console.log(result);
            $(_this).find('span').text(result[0].label);

            //$.ajax({
            //    url: "/WeChatMeeting/MeetingInvitation/MyMeeting",
            //    type: "post",
            //    data: { "strWhere": result[0].value },
            //    success: function success(data) {


            //    },
            //    complete: function complete(data) {

            //    }
            //});
            window.location.href = "/WeChatMeeting/MeetingInvitation/MyMeeting?strWhere=" + result[0].value + "&wechatid=" + $("#wechatId").val();
               
        },
        id: 'picker'
    });
    console.log('pickTravel');
}
  
