'use strict';

var perPageCount = 20;
var lastRecord = moment();
var userInfo, isFirst = 0,
    isLast = 0;
var prev_count = 0,
    next_count = 0;
var contentLength = 0;
var type = function () {
    return window.location.search.split('&')[1].split('=')[1];
}();
var id = function () {
    return window.location.search.split('&')[2].split('=')[1];
}();
var appid = function () {
    return window.location.search.split('&')[3].split('=')[1];
}();
var hiddenAutoReply = function () {
    return window.location.search.split('&')[4].split('=')[1];
}();
$(function () {
    getData(id, 0, perPageCount, 'next');
});
var firstReply = true;
var getData = function (id, currentPage, perPageCount, location) {
    var loading = layer.msg('加载中', { icon: 16, time: 0 });
    $.ajax({
        url: '/WeChatMain/WechatUserRequestMessageLog/GetUserHistory',
        type: 'post',
        data: {
            pageNumber: currentPage,
            id: id,
            appid: appid,
            pageSize: perPageCount,
            hiddenAutoReply: hiddenAutoReply,
        },
        success: parseResult(location, loading)
    });
};
var parseResult = function parseResult(location, loading) {
    return function (data) {
        console.log(data)
        isFirst = data.isFirst == true ? 1 : 0;
        isLast = data.isLast == true ? 1 : 0;
        if (isFirst) {
            layer.close(loading);
            layer.msg('已经到最上');
        } else if (isLast) {
            layer.close(loading);
            layer.msg('已经到最下');
        }
        if (data.list.length > 0) {
            var insertTemp = '';
            var firstTime = false;
            var t = 1000 * 60 * 10; // 十分钟间隔 决定是否显示系统时间
            var result = data.list;
            var $father = $('.mc_content');

            for (var i = 0; i < result.length; i++) {
                if (!result[i]) {
                    break;
                }
                var item = result[i];
                var currentTime = moment(item.CreatedTime);
                var timeText = '';
                if (currentTime.valueOf() - lastRecord.valueOf() >= t) {
                    timeText = showTime(currentTime);
                }
                lastRecord = currentTime;
                if (item.Id == id) {
                    firstTime = true;
                    var html = singleTemplate(item, timeText, 'anchor');
                } else {
                    var html = singleTemplate(item, timeText);
                }

                insertTemp += html;

            }
            if (location === 'next') {
                $father.append(insertTemp);

            } else {
                $father.prepend(insertTemp);
            }

            if (firstTime) {
                $father.animate({ scrollTop: $('#anchor').offset().top }, 1000)
            }
            var divs = $father.children('div');
            var len = 0;
            divs.each(function (index, item) {
                len += $(item).height();

            });

            if (location === 'prev') {
                $('.mc_content').scrollTop(len - contentLength);
            }


            contentLength = len;

            //绑定点击user name
            $('.js_conUserName').text(data.list[0].UserName);
            //绑定popup
            bindPopup(result);
            //绑定视频,图片preview
            bindPreview();
            //绑定语音
            bindAudio();
            //绑定close button
            bindCLose();

            if (!data.list[0].IsCrop && firstReply) {
                $(".mc_header").append('<a class="btn float-right js_conReply" onclick=OpenSendMsg("' + data.list[0].UserID + '")>回复</a>')
                firstReply = false;
            }
        }
        //绑定scroll
        bindScroll();
        layer.close(loading);
    }
};
var chargeContent = function chargeContent(item) {
    var html = '';
    if (item.Content != null && item.Content != '') {
        switch (item.ContentType) {
            case 1:
                html = checkEmoji(item.Content);
                break;
            case 2:

                var content;
                if (item.Content.indexOf('X') >= 0) {
                    content = JSON.parse(item.Content);
                } else {
                    var d = item.Content.split(',');
                    content = { X: d[0], Y: d[1] };
                }

                html = '<a target="_blank" href="http://apis.map.qq.com/uri/v1/geocoder?coord=' + content.X + ',' + content.Y + '"><img src="http://st.map.qq.com/api?size=228*129&amp;center=' + content.X + ',' + content.Y + '&amp;zoom=16&amp;markers=' + content.X + ',' + content.Y + '" class="mc_image"><p class="mc_map_mask"></p><p class="mc_map_title"></p></a>';
                break;
            case 3:
            case 103:
                html = '<img src="' + item.Content + '" class="mc_image" onclick="openHiddenbox(this)">';
                break;
            case 4:
            case 104:
                html = '<span class="icon icon_sound" data-src="' + item.Content + '" data-duration="' + item.Duration + '"></span><span class="icon_sound_active"></span> <span class="mc_vertical"></span>';
                break;
            case 5:
            case 10:
                html = '<a target="_blank" href="' + item.Content + '" data-lightbox=""><video src="' + item.Content + '" class="mc_image"/></a>';
                break;
            case 6:
                html = specialForLink(item);
                break;
            case 7:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
                html = '<span>Client Request: ' + item.Content + '</span>';
                break;
            case 17:
                if (item.Content.indexOf('UserTags') >= 0) {
                    var qrCode = JSON.parse(item.Content);
                    html = '<span>扫码关注, 扫描的二维码为: ' + qrCode.QrCode + '</span>';
                } else {
                    html = '<span>扫码关注, 扫描的二维码为: ' + item.Content + '</span>';
                }
                break;
            case 105:
                html = '<a target="_blank" class="fancybox-media" href="' + item.Content + '"><video src="' + item.Content + '" class="mc_image"></video></a>';
                break;
            case 106:
                html = specialForNews(item);
                break;
            default:
                html = '<span>' + item.Content + '</span>';
        }
    }
    return html;
};

var openHiddenbox = function (e) {
    var nowsrc = $(e).attr("src");
    parent.$(".hiddenfancybox a.fancybox").attr("href", nowsrc);
    parent.$(".hiddenfancybox a.fancybox").fancybox({
        'scrolling': 'no',
        'arrows': false,
    });

    parent.$(".hiddenfancybox a.fancybox").click();
}

var showTime = function showTime(timeStamp) {

    var text = timeStamp.format('M月D日 HH:mm A');
    return '<div class="mc_content_item time"><span class="mc_time">' + text + '</span></div>';
};
var singleTemplate = function singleTemplate(item, timeText, anchor) {
    var content = chargeContent(item);
    var additonal = null;
    switch (item.ContentType) {
        case 4:
        case 104:
            additonal = 'item_sound';
            break;
        default:
            additonal = '';
    }
    if (anchor) {
        var templateReceive = '<div  id="anchor">' + timeText + '<div data-msgid="' + item.Id + '" class="mc_content_item item_left ' + additonal + '">' + '<div class="mc_content_wrap clearfix">' + '<div class="mc_avatar_wrap">' + '<img src="' + item.PhotoUrl + '" data-uid="' + item.UserID + '" class="border_alpha js_userCard">' + '</div>' + '<div class="mc_text_wrap">' + '<div class="mc_text_middle">' + '<div class="mc_text_inner">' + '<span class="mc_arrow">' + '<span class="mc_arrow_wrap"></span>' + '<span class="mc_arrow_middle"></span>' + '<span class="mc_arrow_inner"></span>' + '</span>' + '<div class="mc_detail">' + content + '</div>' + '<span class="mc_vertical"></span>' + '<div class="tips_loading userVideo_tips_loading hide"></div>' + '</div>' + '</div>' + '</div>' + '</div>' + '</div>' + '</div>';
        var templateSend = '<div  id="anchor">' + timeText + '<div class="mc_content_item item_right ' + additonal + '">' + '<div class="mc_content_wrap clearfix">' + '<div class="mc_avatar_wrap">' + '<img src="' + item.AppLogo + '" class="border_alpha"></div>' + '<div class="mc_user_name">Admin</div>' + '<div class="mc_text_wrap">' + '<div class="mc_text_middle">' + '<div class="mc_text_inner">' + '<span class="mc_arrow">' + '<span class="mc_arrow_wrap"></span>' + ' <span class="mc_arrow_middle"></span> ' + '<span class="mc_arrow_inner"></span>' + '</span>' + '<div class="mc_detail">' + content + '</div>' + '<span class="mc_vertical"></span>' + '<div class="tips_loading userVideo_tips_loading hide"></div>' + '</div></div></div><div class="clear"></div></div></div></div>';
    } else {
        var templateReceive = '<div>' + timeText + '<div data-msgid="' + item.Id + '" class="mc_content_item item_left ' + additonal + '">' + '<div class="mc_content_wrap clearfix">' + '<div class="mc_avatar_wrap">' + '<img src="' + item.PhotoUrl + '" data-uid="' + item.UserID + '" class="border_alpha js_userCard">' + '</div>' + '<div class="mc_text_wrap">' + '<div class="mc_text_middle">' + '<div class="mc_text_inner">' + '<span class="mc_arrow">' + '<span class="mc_arrow_wrap"></span>' + '<span class="mc_arrow_middle"></span>' + '<span class="mc_arrow_inner"></span>' + '</span>' + '<div class="mc_detail">' + content + '</div>' + '<span class="mc_vertical"></span>' + '<div class="tips_loading userVideo_tips_loading hide"></div>' + '</div>' + '</div>' + '</div>' + '</div>' + '</div>' + '</div>';
        var templateSend = '<div>' + timeText + '<div class="mc_content_item item_right ' + additonal + '">' + '<div class="mc_content_wrap clearfix">' + '<div class="mc_avatar_wrap">' + '<img src="' + item.AppLogo + '" class="border_alpha"></div>' + '<div class="mc_user_name"> </div>' + '<div class="mc_text_wrap">' + '<div class="mc_text_middle">' + '<div class="mc_text_inner">' + '<span class="mc_arrow">' + '<span class="mc_arrow_wrap"></span>' + ' <span class="mc_arrow_middle"></span> ' + '<span class="mc_arrow_inner"></span>' + '</span>' + '<div class="mc_detail">' + content + '</div>' + '<span class="mc_vertical"></span>' + '<div class="tips_loading userVideo_tips_loading hide"></div>' + '</div></div></div><div class="clear"></div></div></div></div>';
    }
    return item.ContentType >= 100 ? templateSend : templateReceive;
};

var specialForNews = function specialForNews(item) {
    var content = JSON.parse(item.Content);
    if (content.length == 0) {
        return '';
    }
    var html = '';
    //多图文
    if (content.length > 1) {
        html = '<div class="video_unit">' + '<a class="video_image" href="' + content[0].Url + '" target="_blank">' + '<div class="video_image_wrap">' + '<img src="' + content[0].PicUrl + '">' + '<span class="image_vertical"></span>' + '</div><p class="video_sub_title">' + (content[0].Title || content[0].title) + '</p></a> ';
        for (var i = 1, len = content.length; i < len; i++) {
            var htm = '<a class="video_artical" href="' + content[i].Url + '" target="_blank">' + '<img src="' + content[i].PicUrl + '" class="video_artical_img">' + '<p class="video_artical_title">' + (content[i].Title || content[i].title) + '</p>' + '<span class="video_vertical"></span>' + '</a>';
            html += htm;
        }
        html += '</div>';
    }
        //单图文
    else {
        var summary = content[0].summary || content[0].Description || '';
        html = '<div class="video_unit">' + '<p class="video_title">' + (content[0].Title || content[0].title) + '</p>' + '<a class="video_image" href="' + content[0].Url + '" target="_blank">' + '<div class="video_image_wrap">' + '<img src="' + content[0].PicUrl + '">' + '<span class="image_vertical"></span></div></a>' + '<p class="video_detail">' + summary + '</p>' + '<a class="video_more" href="' + content[0].Url + '" target="_blank">查看详细</a>' + '</div>';
    }
    return html;
};

var specialForLink = function specialForLink(item) {
    var html = '';
    if (item != null && item != undefined) {
        if (item.Content != null && item.Content != '') {
            var content = JSON.parse(item.Content);
            if (content != null && content != undefined) {
                html = '<ul class="imgText-list">' + '<li><a href="' + content.Url + '" target="_blank">' + '<h5 class="imgText-heading">' + content.Title + '</h5>' + '<div class="imgText-body">' + content.Description + '<div class="imgText-right" href="#">' + '<img src="" width="50px" height="50px">' + '</div></div></li></a></ul>';
            }
        }
    }
    return html;
}

var bindPreview = function bindPreview() {
    $(".fancybox").fancybox({
        'scrolling': 'no',
        'arrows': false,
    });
    $('.fancybox-media').fancybox({
        'padding': 0,
        'cyclic': 'false',
        'autoScale': false,
        'transitionIn': 'none',
        'transitionOut': 'none',
        'hideOnOverlayClick': false
    });
};

var bindPopup = function bindPopup(result) {
    if (result[0].IsCrop) {
        $('.border_alpha.js_userCard').mouseenter(function () {

            var html = '<div class="userCardBody" style=" color:black;display:block"><div style="padding: 10px;height: 81px;"><div class="userImg" style="float:left"><img height="80" width="80" src="' + result[0].PhotoUrl + '"></div><div class="userDetail" style="float:left"><div title="' + result[0].UserName + '" class="userName ellipsis" style="margin: 10px;width: 140px;font-size: 18px;max-height: 60px;text-overflow: ellipsis; -o-text-overflow: ellipsis;white-space: nowrap;overflow: hidden;">' + result[0].UserName + '</div><div class="userAccout ellipsis" style="margin: 10px;width: 140px;text-overflow: ellipsis; -o-text-overflow: ellipsis;white-space: nowrap;overflow: hidden;">' + result[0].EmployeeNo + '</div></div></div><div class="item" style="padding-left: 10px;padding-right: 10px;padding-bottom: 5px;width: 240px;text-overflow: ellipsis;-o-text-overflow: ellipsis;white-space: nowrap;overflow: hidden;" title="' + result[0].Mobile + '"><span>手机:</span> <span>' + result[0].Mobile + '</span></div><div class="item" style="padding-left: 10px;padding-right: 10px;padding-bottom: 5px;width: 240px;text-overflow: ellipsis;-o-text-overflow: ellipsis;white-space: nowrap;overflow: hidden;" title="' + result[0].Department + '"><span>部门:</span> <span>' + result[0].Department + '</span></div></div>'
            userInfo = layer.open({
                closeBtn: 0,
                type: 4,
                tips: [3, 'white'],
                content: [html, this]

            });
        });
        $('.border_alpha.js_userCard').mouseleave(function () {
            if (userInfo) {
                layer.close(userInfo);
            }
        });
    }
};
var flag_audio = '';
var bindAudio = function bindAudio() {
    RongIMLib.RongIMVoice.init();
    //stopAudio();
    $('.item_sound').off('click').on('click', '.mc_text_wrap', function () {
        var that = this;
        var src = $(that).find('.icon_sound').data('src') || '';
        var exitsMedia = $('#palyAudio');
        if (exitsMedia.length > 0) {
            exitsMedia[0].pause();
            exitsMedia.parents('.mc_text_wrap').removeClass('item_sound_active');
            exitsMedia.parent().html('');
        }

        $(that).find('.icon_sound_active').html('<audio id="palyAudio" preload="" type="audio/mpeg" src="' + src + '">');
        var newMedia = $('#palyAudio');
        var dataStr = '';
        dataStr = $(that).find('.icon_sound').attr('data-src');
        var duration = $(that).find('.icon_sound').attr('data-duration');
        newMedia[0].play();
        newMedia.parents('.mc_text_wrap').addClass('item_sound_active');
        newMedia.off('ended').on('ended', function () {
            newMedia.parents('.mc_text_wrap').removeClass('item_sound_active');
            newMedia.parent().html('');
        });
        if ($(that).find('.icon_sound').attr('data_flag') === "play") {
            stopAudio();
            newMedia.parents('.mc_text_wrap').removeClass('item_sound_active');
            $(that).find('.icon_sound').removeAttr('data_flag');
        } else {
            $.ajax({
                url: '/FileManage/ConvertToBase64',
                type: 'Get',
                data: { "filePath": dataStr },
                success: function (data) {
                    RongIMLib.RongIMVoice.play(data.path);
                    flag_audio = 1;
                    $(that).find('.icon_sound').attr('data_flag', "play");

                    setTimeout(function () {
                        newMedia.parents('.mc_text_wrap').removeClass('item_sound_active');
                    }, duration * 1000);
                }

            })
        }




        //if ($(that).find('.icon_sound').attr('data_flag') === "1") {
        //    $(that).on('click', function () {
        //        stopAudio();
        //        $(that).find('.icon_sound').removeAttr('data_flag');

        //    })
        //} else {
        //    $.ajax({
        //        url: '/FileManage/ConvertToBase64',
        //        type: 'Get',
        //        data: { "filePath": dataStr },
        //        success: function (data) {
        //            console.log(data.path); //base64String
        //            console.log(data.duration); //play time
        //            RongIMLib.RongIMVoice.play(data.path);
        //            flag_audio = 1;
        //            $(that).find('.icon_sound').attr('data_flag', "play");

        //            setTimeout(function () {
        //                newMedia.parents('.mc_text_wrap').removeClass('item_sound_active');
        //            }, data.duration * 1000);
        //        }

        //    })
        //}



    });
};
var bindCLose = function bindCLose() {
    var index = parent.layer.getFrameIndex(window.name);
    $('.js_conClose').off('click').on('click', function () {
        parent.layer.close(index);
    });
};
function stopAudio() {
    RongIMLib.RongIMVoice.stop();
}
var bindScroll = function bindScroll() {
    $('.mc_content').off('scroll').on('scroll', function () {
        var scrollTop = +$(this).scrollTop();
        var windowHeight = +$(window).height();

        if (scrollTop + windowHeight >= contentLength && scrollTop > 0) {
            //console.log('出发了下边');
            if (!isLast) {


                $('.mc_content').off('scroll')
                return getData(id, ++next_count, perPageCount, 'next');
            }

        }


        if (scrollTop == 0) {
            if (!isFirst) {

                //console.log('出发了上边');
                $('.mc_content').off('scroll')
                return getData(id, --prev_count, perPageCount, 'prev');
            }

        }


    });

};

var OpenSendMsg = function (openId) {
    $('#SendMsgOpenId', window.parent.document).val(openId);
    $('#openSendMsglayer', window.parent.document).click();
    $.ajax({
        url: "/WeChatMain/WechatUserRequestMessageLog/SetReaded?appid=" + appid + "&openId=" + openId + "&id=" + id,
    });
}