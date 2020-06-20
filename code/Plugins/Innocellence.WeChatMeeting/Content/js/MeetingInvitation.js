var arry = "";

$(function () {
    //资料存在
    if ($("#hid_file").val() != "") {
        $("#show_file").show();
    }


    show_time();
    sleect_time();

    var hid_timeclass = $("#hid_timeclass").val();
    if (hid_timeclass != "") {

        $('.meeting-choose i').each(function () {
            $(this).removeClass('fa-check-circle-o choose-btn-red').addClass('fa-circle-thin');

        });
        $("#" + hid_timeclass + "").removeClass('fa-circle-thin').addClass('fa-check-circle-o choose-btn-red');
        arry = $("#" + hid_timeclass + "").next('input').val();
       
    }

})

//资料下载
function DownloadData(file_path) {
    var file_url = $("#file_url").val();
    window.location.href = file_url + file_path;
}

//时间
function show_time() {


    var time_slot = $("#hid_timeslot").val().toString().replace("&quot;", "\"\"");
    


    var li_str = "";
    $.each(JSON.parse(time_slot), function (index, item) {

            if (index == 0) {
                li_str += '<li class="choose-item">' +
                              '<div class="item-time">' +
                                  ' <div class="meeting-time">' +
                                        '<span>会议：</span>' +
                                        '<p>' + ChangeDateFormat(item.StartDateTime.replace(/-/g, "/")) + '→' + ChangeDateFormat(item.EndDateTime.replace(/-/g, "/")) + '</p>' +
                                   '</div>' +
                                   '<div class="meeting-time">' +
                                       '<span>签到：</span>' +
                                       '<p>' + ChangeDateFormat(item.SignStartTime.replace(/-/g, "/")) + '→' + ChangeDateFormat(item.SignEndTime.replace(/-/g, "/")) + '</p>' +
                                   '</div>' +
                           '</div>' +
                           '<div class="choose-btn"><i class="fa fa-check-circle-o choose-btn-red" aria-hidden="true" id="'+item.Type+'"></i>' +
                          '<input type="hidden" id="def_data" value="' + item.StartDateTime + ',' + item.EndDateTime + ',' + item.SignStartTime + ',' + item.SignEndTime + ',' + item.Type + '" /></div>' +
                         '</li>';
            }
            else {
                li_str += '<li class="choose-item">' +
                              '<div class="item-time">' +
                                  ' <div class="meeting-time">' +
                                        '<span>会议：</span>' +
                                        '<p>' + ChangeDateFormat(item.StartDateTime.replace(/-/g, "/")) + '→' + ChangeDateFormat(item.EndDateTime.replace(/-/g, "/")) + '</p>' +
                                   '</div>' +
                                   '<div class="meeting-time">' +
                                       '<span>签到：</span>' +
                                       '<p>' + ChangeDateFormat(item.SignStartTime.replace(/-/g, "/")) + '→' + ChangeDateFormat(item.SignEndTime.replace(/-/g, "/")) + '</p>' +
                                   '</div>' +
                           '</div>' +
                           '<div class="choose-btn"><i class="fa fa-circle-thin" aria-hidden="true" id="' + item.Type + '"></i>' +
                           '<input type="hidden" value="' + item.StartDateTime + ',' + item.EndDateTime + ',' + item.SignStartTime + ',' + item.SignEndTime + ',' + item.Type + '" /></div>' +
                         '</li>';
            }
        

    });


    $("#ul_time").html(li_str);
}


//选取时间
function sleect_time() {
    $('.choose-item').click(function () {
        var that = this;
        $('.meeting-choose i').each(function () {
            $(this).removeClass('fa-check-circle-o choose-btn-red').addClass('fa-circle-thin');

        });
        $(that).find('i').removeClass('fa-circle-thin').addClass('fa-check-circle-o choose-btn-red');
        arry = $(that).find('input').val();
    });
}


//时间格式化
function ChangeDateFormat(time) {
    if (time != null) {
        var date = new Date(time);
        Y = date.getFullYear() + '/';
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var h = date.getHours();
        var minute = date.getMinutes();
        minute = minute < 10 ? ('0' + minute) : minute;

        return date.getFullYear() + "/" + month + "/" + currentDate + " " + h + ":" + minute;
    }
    return "";
}

//确认报名
function ConfirmSignUp() {
    if (arry == "") {
        arry = $("#def_data").val();
    }

    $.ajax({
        url: "/WeChatMeeting/MeetingInvitation/ConfirmSignUp?wechatid=" + $("#hid_appid").val(),
        type: "post",
        data: { "data": arry, "meetid": $("#hid_meet").val() },
        success: function success(data) {

            window.location.href = "InvitationDetail?MeetId=" + $("#hid_meet").val() + "&AppID=" + $("#hid_appid").val() + "&wechatid=" + $("#hid_appid").val();
          

        },
        complete: function complete(data) {

        }
    });

}