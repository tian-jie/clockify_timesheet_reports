var num = 2;
var info = "";
var $str;
var $body = $('body');
var $wrapper = $('#wrapper');
var i = 1;
var j = 0;
var date = new Date();

$(function () {

    $("#txtDepartureTime0").text(ChangeDateFormat(date));
    $("#txtArrivalTime0").text(ChangeDateFormat(date));


    var travelList = $("#travelList").val();
    if (travelList != "")
    {
        $.each(JSON.parse(travelList), function (index, item) {

            var con = item.Type.substring(6, item.Type.length);
            if (con == "0") {
                $("#txtMode0").text(item.Mode);
                $("input[name='txtFlight_Train0']").val(item.Flight_Train);
                $("#txtDepartureTime0").text(ChangeDateFormat(item.DepartureTime));
                $("#txtArrivalTime0").text(ChangeDateFormat(item.ArrivalTime));
                $("#txtExpectArrivalTime0").text(item.ExpectArrivalTime);

            }
            else {
                
                travel_data(j);
                $str = '<div class="travel_item" data-tag="' + con + '"><div class="weui-cells travel_item_title"><div class="weui-cell"><div class="weui-cell__bd weui-cell_primary"><p class="travel_p_title">' +
                '' + info + '' +
                '</p></div><a class="weui-cell__ft del_btn" href="javascript:;">' +
                '删除' +
                '</a></div></div><div class="travel_item_wrapper"><div class="weui-cells__title font_grey">出行方式' +
                '</div><div class="weui-cells"><div class="weui-cell"><div class="weui-cell__bd pickerBtn"><p id="txtMode' + con + '">' +
                ''+item.Mode+'&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' +
                '</p></div><div class="weui-cell__ft select_icon"><i class="fa fa-angle-down" aria-hidden="true"></i></div></div></div>' +
                '<div class="weui-cells__title font_grey">航班号/车次/轮船班次</div><div class="weui-cells"><div class="weui-cell"><div class="weui-cell__bd"><input class="weui-input" value="' + item.Flight_Train + '" type="text" name="txtFlight_Train' + con + '" placeholder="请输入航班号/车次/轮船班次"></div></div></div>' +
                '<div class="weui-cells__title font_grey">出发日期</div><div class="weui-cells"><div class="weui-cell datePickerBtn"><div class="weui-cell__bd "><p id="txtDepartureTime' + con + '">' +
                '' + ChangeDateFormat(item.DepartureTime) + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' +
                '</p></div><div class="weui-cell__ft select_icon"><i class="fa fa-calendar" aria-hidden="true"></i></div></div></div>' +
                '<div class="weui-cells__title font_grey">到达日期</div><div class="weui-cells"><div class="weui-cell datePickerStartBtn"><div class="weui-cell__bd "><p id="txtArrivalTime' + con + '">' +
                '' + ChangeDateFormat(item.ArrivalTime) + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' +
                '</p></div><div class="weui-cell__ft select_icon"><i class="fa fa-calendar" aria-hidden="true"></i></div></div></div>' +
                '<div class="weui-cells__title font_grey">预计到达时间(12小时制)</div><div class="weui-cells"><div class="weui-cell timePickBtn"><div class="weui-cell__bd "><p id="txtExpectArrivalTime' + con + '">' +
                '' + item.ExpectArrivalTime+ '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' +
                '</p></div><div class="weui-cell__ft select_icon"><i class="fa fa-angle-down" aria-hidden="true"></i></div></div></div></div></div>';
                num = num + 1;
                i = parseInt(con) + 1;
                j = parseInt(con);
                
                $wrapper.append($str);
            }
            
        });
    }




});




//绑定WEUI中pick插件
$('#wrapper').off('click', '.pickerBtn', pickTravel).on('click', '.pickerBtn', pickTravel);
$('#wrapper').off('click', '.datePickerBtn', datePick).on('click', '.datePickerBtn', datePick);
$('#wrapper').off('click', '.datePickerStartBtn', datePickStart).on('click', '.datePickerStartBtn', datePickStart);
$('#wrapper').off('click', '.timePickBtn', timePick).on('click', '.timePickBtn', timePick);



//添加
$body.off('click', '.add_item', function () {
    $wrapper.append($str);

}).on('click', '.add_item', function () {

    var txtFlight_Train = $("input[name='txtFlight_Train" + j + "']").val();
    var txtDepartureTime = $("#txtDepartureTime" + j + "").text();
    var txtArrivalTime = $("#txtArrivalTime" + j + "").text();
    var txtExpectArrivalTime = $("#txtExpectArrivalTime" + j + "").text();


    if (txtFlight_Train == "") {
        layer.msg("请填写航班号/车次/轮船班次！");
    }
    else if (txtDepartureTime == "") {
        layer.msg("请填写出发日期！");
    }
    else if (txtArrivalTime == "") {
        layer.msg("请填写到达日期！");
    }
    else if (txtExpectArrivalTime == "") {
        layer.msg("请填写预计到达时间(12小时制)！");
    }
    else {
        if (num > 8) {
            layer.msg("您在一个会议中最多可以添加8个行程信息！");
        } else {

            travel_data(j);
            $str = '<div class="travel_item" data-tag="' + i + '"><div class="weui-cells travel_item_title"><div class="weui-cell"><div class="weui-cell__bd weui-cell_primary"><p class="travel_p_title">' +
               '' + info + '' +
               '</p></div><a class="weui-cell__ft del_btn" href="javascript:;">' +
               '删除' +
               '</a></div></div><div class="travel_item_wrapper"><div class="weui-cells__title font_grey">出行方式' +
               '</div><div class="weui-cells"><div class="weui-cell"><div class="weui-cell__bd pickerBtn"><p id="txtMode' + i + '">' +
               '飞机&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' +
               '</p></div><div class="weui-cell__ft select_icon"><i class="fa fa-angle-down" aria-hidden="true"></i></div></div></div>' +
               '<div class="weui-cells__title font_grey">航班号/车次/轮船班次</div><div class="weui-cells"><div class="weui-cell"><div class="weui-cell__bd"><input class="weui-input" type="text" name="txtFlight_Train' + i + '" placeholder="请输入航班号/车次/轮船班次"></div></div></div>' +
               '<div class="weui-cells__title font_grey">出发日期</div><div class="weui-cells"><div class="weui-cell datePickerBtn"><div class="weui-cell__bd "><p id="txtDepartureTime' + i + '">' +
               '' + ChangeDateFormat(date) + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' +
               '</p></div><div class="weui-cell__ft select_icon"><i class="fa fa-calendar" aria-hidden="true"></i></div></div></div>' +
               '<div class="weui-cells__title font_grey">到达日期</div><div class="weui-cells"><div class="weui-cell datePickerStartBtn"><div class="weui-cell__bd "><p id="txtArrivalTime' + i + '">' +
               '' + ChangeDateFormat(date) + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' +
               '</p></div><div class="weui-cell__ft select_icon"><i class="fa fa-calendar" aria-hidden="true"></i></div></div></div>' +
               '<div class="weui-cells__title font_grey">预计到达时间(12小时制)</div><div class="weui-cells"><div class="weui-cell timePickBtn"><div class="weui-cell__bd "><p id="txtExpectArrivalTime' + i + '">' +
               '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' +
               '</p></div><div class="weui-cell__ft select_icon"><i class="fa fa-angle-down" aria-hidden="true"></i></div></div></div></div></div>';
            num = num + 1;
            i = i + 1;
            j = j + 1;
            $wrapper.append($str);
        }
    }


});


//删除
$body.off('click', '.del_btn', function () {
    $(this).closest('.travel_item').remove();
}).on('click', '.del_btn', function () {
    $(this).closest('.travel_item').remove();
     num = num - 1;
    j = j - 1;
});




//PICK方法
function pickTravel() {
    var _this = this;
    weui.picker([{
        label: '飞机',
        value: 0
    }, {
        label: '火车',
        value: 1
    }, {
        label: '轮船',
        value: 2
    }], {
        defaultValue: [1],
        className: 'custom-classname',
        onChange: function onChange(result) {
            //console.log(item, index);



        },
        onConfirm: function onConfirm(result) {
            // console.log(result[0].label);
            $(_this).find('p').text(result[0].label)
        },
        id: 'picker'
    });
    console.log('pickTravel');
}
function datePick() {

    var _this = this;
    weui.datePicker({
        start: '2000-12-29',
        end: '2030-12-29',
        defaultValue: get_time(),
        onChange: function onChange(result) {
            // console.log(result);
        },
        onConfirm: function onConfirm(result) {
            // console.log(result);
            var str = '';
            result.forEach(function (item) {
                str += item.label;
            });
            $(_this).find('p').text(str)
        },
        id: 'datePicker'
    });
    console.log('datePick');
}
function datePickStart() {
    var _this = this;
    weui.datePicker({
        start: '2000-12-29',
        end: '2030-12-29',
        defaultValue: get_time(),
        onChange: function onChange(result) {
            // console.log(result);
        },
        onConfirm: function onConfirm(result) {
            // console.log(result);
            var str = '';
            result.forEach(function (item) {
                str += item.label;
            });
            $(_this).find('p').text(str)
        },
        id: 'dateStartPicker'
    });
    console.log('datePickStart');
}
function timePick() {
    var _this = this;
    weui.picker([
        {
            label: 1,
            value: 1
        }, {
            label: 2,
            value: 2
        }, {
            label: 3,
            value: 3
        }, {
            label: 4,
            value: 4
        }, {
            label: 5,
            value: 5
        }, {
            label: 6,
            value: 6
        }, {
            label: 7,
            value: 7
        }, {
            label: 8,
            value: 8
        }, {
            label: 9,
            value: 9
        }, {
            label: 10,
            value: 10
        }, {
            label: 11,
            value: 11
        }, {
            label: 12,
            value: 12
        }
    ], [
        {
            label: '00',
            value: '0'
        }, {
            label: '15',
            value: '15'
        }, {
            label: '30',
            value: '30'
        }, {
            label: '45',
            value: '45'
        }
    ], [
        {
            label: 'AM',
            value: 'AM'
        }, {
            label: 'PM',
            value: 'PM'
        }
    ], {
        defaultValue: [3, '15', 'PM'],
        onChange: function (result) {
            // console.log(result);
        },
        onConfirm: function (result) {
            console.log(result);
            var str = '';
            str = result[0].label + ':' + result[1].label + " " + result[2].label;
            $(_this).find('p').text(str)
        },
        id: 'multiPickerBtn'
    });
    console.log('timePick');
}

function travel_data(j) {

    // var t_count = $("#wrapper").find(".travel_p_title");
    if (j == 0) {
        info = "行程二";
    }
    else if (j == 1) {
        info = "行程三";
    }
    else if (j == 2) {
        info = "行程四";
    }
    else if (j == 3) {
        info = "行程五";
    }
    else if (j == 4) {
        info = "行程六";
    }
    else if (j == 5) {
        info = "行程七";
    }
    else if (j == 6) {
        info = "行程八";
    }
    else if (j == 7) {
        info = "行程九";
    }
    else if (j == 8) {
        info = "行程十";
    }
    else if (j == 9) {
        info = "行程十一";
    }
    else if (j == 10) {
        info = "行程十二";
    }
    else if (j == 11) {
        info = "行程十三";
    }
    else if (j == 12) {
        info = "行程十四";
    }
    else if (j == 13) {
        info = "行程十五";
    }
    else if (j == 14) {
        info = "行程十六";
    }
    else if (j == 15) {
        info = "行程十七";
    }
    else if (j == 16) {
        info = "行程十八";
    }
    else if (j == 17) {
        info = "行程十九";
    }
    else if (j == 18) {
        info = "行程二十";
    }

}

//获取当前时间
function get_time() {
    var myDate = new Date();
    var yy = myDate.getFullYear();
    var mm = myDate.getMonth() + 1;
    var dd = myDate.getDate();
    var arrDate = [];
    arrDate.push(yy, mm, dd);

    return arrDate;
}

//时间格式化
function ChangeDateFormat(time) {
    if (time != null) {
        var date = new Date(time);
        Y = date.getFullYear() + '年';
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var h = date.getHours();
        var minute = date.getMinutes();
        minute = minute < 10 ? ('0' + minute) : minute;

        //return date.getFullYear() + "/" + month + "/" + currentDate + " " + h + ":" + minute;
        return date.getFullYear() + "年" + month + "月" + currentDate + "日";

    }
    return "";
}



//验证
var msgText_V = function () {

    var flight = $("input[name='txtFlight_Train" + j + "']").val();
    if (flight != undefined) {
        if (flight == null || flight.trim().length <= 0) {
            layer.msg('请填写航班号/车次/轮船班次', { offset: 0 });
            $("input[name='txtFlight_Train" + j + "']").focus();
            return false;
        }
    }

    var dtime = $("#txtDepartureTime" + j + "").text();
    if (dtime != undefined) {
        if (dtime == null || dtime.trim().length <= 0) {
            layer.msg('请填写出发日期', { offset: 0 });
            $("#txtDepartureTime" + j + "").focus();
            return false;
        }
    }

    var atime = $("#txtArrivalTime" + j + "").text();
    if (atime != undefined) {
        if (atime == null || atime.trim().length <= 0) {
            layer.msg('请填写到达日期', { offset: 0 });
            ("#txtArrivalTime" + j + "").focus();
            return false;
        }
    }


    var etime = $("#txtExpectArrivalTime" + j + "").text();
    if (etime != undefined) {
        if (etime == null || etime.trim().length <= 0) {
            layer.msg('请填写预计到达时间(12小时制)', { offset: 0 });
            $("#txtExpectArrivalTime" + j + "").focus();
            return false;
        }
    }


    return {
        flight: flight,
        dtime: dtime,
        atime: atime,
        etime: etime
    };
};

var textJson = function (_text) {
    var arry = [];
    var travelArray = [];
    var glength = $(".travel_item").length;
    for (i = 0; i < glength; i++) {
        var count = $(".travel_item").eq(i).attr("data-tag");
        arry = {
            Mode: $("#txtMode" + count + "").text(), Flight_Train: $("input[name='txtFlight_Train" + count + "']").val().trim(), DepartureTime: $("#txtDepartureTime" + count + "").text(),
            ArrivalTime: $("#txtArrivalTime" + count + "").text(), ExpectArrivalTime: $("#txtExpectArrivalTime" + count + "").text(), Type: "travel" + count + ""
        };

        travelArray.push(arry);
    }

    var _main = [];
    _main = [{
        MeetingTravelInfoViews: travelArray, MeetingId: $("#meet").val()
    }];
    return _main;

};


//提交并保存行程信息
function sub_btn() {
    var _text = msgText_V();
    var _textJson = textJson(_text);
    if (_text) {
        $.ajax({
            url: "/WeChatMeeting/MeetingInvitation/AddTravelInfo?wechatid=" + $("#hid_appid").val(),
            type: "post",
            data: JSON.stringify(_textJson),
            contentType: "application/json",
            success: function success(data) {
                if (data.results.Data == 200) {
                    weui.alert('点击确定返回上一页', {
                        title: '补充行程信息成功',
                        buttons: [{
                            label: '确定',
                            type: 'primary',
                            onClick: function () { window.location.href = document.referrer }
                        }]
                    });
                }
                else {
                    weui.alert(data.results.Message)
                }
                
                //window.location.href = "Travel?meetingid=" + $("#meet").val() + "&AppID=" + $("#hid_appid").val() + "&wechatid=" + $("#hid_appid").val();
            },
            complete: function complete(data) {

            }
        });

    }
}

