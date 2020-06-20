'use strict';

$(function () {
    var id = function () {
        return window.location.search.split('&')[1].split('=')[1];
    }();
    console.log(id);

    $.ajax({
        url: '/WechatMain/SendMessageLog/GetItem?id=' + id,
        type: 'get',
        success: parseResult
    });
});

var parseResult = function parseResult(data) {
    var item = data;
    var $father = $('.mc_content');
    var html = singleTemplate(item);

    $father.append(html);

    //绑定视频,图片preview
    bindPreview();
    //绑定语音
    bindAudio();
    //绑定close button
    bindCLose();
   
};

var bindCLose = function bindCLose() {
    var index = parent.layer.getFrameIndex(window.name);
    $('.js_conClose').on('click', function () {
        parent.layer.close(index);
    });
};

var bindAudio = function bindAudio() {
    $('.item_sound').on('click', '.mc_text_wrap', function () {
        console.log(1);
        var that = this;
        var src = $(that).find('.icon_sound').data('src') || '';
        var exitsMedia = $('#palyAudio');
        if (exitsMedia.length > 0) {
            exitsMedia[0].pause();
            exitsMedia.parents('.mc_text_wrap').removeClass('item_sound_active');
            exitsMedia.parent().html('');
        }

        $(this).find('.icon_sound_active').html('<audio id="palyAudio" src="' + src + '">');
        var newMedia = $('#palyAudio');
        newMedia[0].play();
        newMedia.parents('.mc_text_wrap').addClass('item_sound_active');
        newMedia.on('ended', function () {
            newMedia.parents('.mc_text_wrap').removeClass('item_sound_active');
            newMedia.parent().html('');
        });
    });
};

var bindPreview = function bindPreview() {
    $(".fancybox").fancybox();
    $('.fancybox-media').fancybox({
        'padding': 0,
        'autoScale': false,
        'transitionIn': 'none',
        'transitionOut': 'none',
        'hideOnOverlayClick': false
    });
};



var specialForNews = function specialForNews(item) {
    var content = $.parseJSON(item.ArticleContentEdit);
    console.log(item);
    var html = '';
    //多图文
    if (content.length > 1) {
        html = '<div class="video_unit"><a class="video_image" href="/WechatMain/Message/GetNews?id=' + item.Id + '&subId=' + content[0].Id + "&code=" + item.ArticleCode + '" target="_blank"><div class="video_image_wrap"><img src="' + content[0].ImageContent + '"><span class="image_vertical"></span></div><p class="video_sub_title">' + content[0].NewsTitle + '</p></a> ';
        for (var i = 1, len = content.length; i < len; i++) {
            var htm = '<a class="video_artical" href="/WechatMain/Message/GetNews?id=' + item.Id + '&subId=' + content[i].Id + "&code=" + item.ArticleCode + '"  target="_blank"><img src="' + content[i].ImageContent + '" class="video_artical_img"><p class="video_artical_title">' + content[i].NewsTitle + '</p><span class="video_vertical"></span></a>';
            html += htm;
        }
        html += '</div>';
    }
    //单图文
    else {
        html = '<div class="video_unit"><p class="video_title">' + content[0].NewsTitle + '</p><a class="video_image" href="/WechatMain/Message/GetNews?id=' + item.Id + '&subId=' + content[0].Id + "&code=" + item.ArticleCode + '" target="_blank"><div class="video_image_wrap"><img src="' + content[0].ImageContent + '"><span class="image_vertical"></span></div></a><p class="video_detail">' + (content[0].NewsComment == null ? "" : content[0].NewsComment) + '</p><a class="video_more" href="/WechatMain/Message/GetNews?id=' + item.Id + '&subId=' + content[0].Id + '" target="_blank">查看详细</a></div>';
        }
    return html;
};


var chargeContent = function chargeContent(item) {
    var html = '';
    var content = $.parseJSON(item.ArticleContentEdit);
    switch (content[0].NewsCate) {
        case 'text':
            html = '<div class="mc_text_wrap mc_text_wrap_green float-left"><div class="mc_text_middle"><div class="mc_text_inner"><div class="mc_detail">' + Util.parseFaceFromStrToDisplay(content[0].NewsContent) + '</div><span class="mc_vertical"></span> </div></div></div>';
            break;
        case 'image':
            html = '<div class="pic_unit"><div class="image_wrap"><div class="image_inner"><a  href="' + content[0].ImageContent + '" target="_blank" ><img src="' + content[0].ImageContent + '" class="pic_image"></a></div></div><p class="pic_name"></p></div>';
            break;
        case 'voice':
            html = '<div class="js_audio mc_text_wrap mc_text_wrap_green"><div class="mc_text_middle"><div class="mc_text_inner mc_text_inner_sound" style="width:140px"><span class="icon_sound" data-src="' + content[0].SoundSrc + '"></span> <span class="icon_sound_active"></span> <span class="mc_vertical"></span> </div></div></div>';
            break;
        case 'video':
            html = '<div class="video_unit"><p class="video_title">' + content[0].NewsTitle + '</p><a target="_blank" class="fancybox-media" href="' + content[0].VideoContent + '"><div class="video_image_warp js_video_container"><div class="video-js vjs-default-skin vjs-paused vjs-controls-enabled vjs-user-inactive" style="width:336px;height:190px"><video class="vjs-tech" src="' + content[0].VideoContent + '" preload="auto"></video><div></div></div></a><p class="video_detail">' + (content[0].NewsComment == null ? "" : content[0].NewsComment) + '</p></div>';
            break;
        case 'file':
            var ext = content[0].FileSrc.split('.')[1];
            html = '<div class="item_file"><a class="mc_text_wrap mc_text_wrap_green" target="_blank" href="/' + content[0].FileSrc + '"><div class="mc_text_middle"><div class="mc_text_inner"><div class="mc_detail"><span class="icon_filetype filetype_' + ext + '"></span><div class="file_detail_info"><span class="name">' + content[0].RealFileName + '</span> <span class="gray size">' + content[0].Size + '</span></div></div></div></div></a></div>';
            break;
        case 'news':
            html = specialForNews(item);
            break;
        default:
            html = 'test';
    }
    return html;
};
var showTime = function showTime(timeStamp) {

    var text = timeStamp.format('M月D日 HH:mm A');
    return '<div class="mc_content_item time"><span class="mc_time">' + text + '</span></div>';
};
var singleTemplate = function singleTemplate(item) {
    var content = chargeContent(item);
    var additonal = null;
    switch (item.ContentType) {
        case 4:
            additonal = 'item_sound';
            break;
        default:
            additonal = null;
    }
    var templateSend = '<div class="mc_content_item"><div class="mc_card_wrap clearfix">' + content + '</div>';

    return templateSend;
};


//# sourceMappingURL=app_bk-compiled.js.map