'use strict';
//获取当前窗口链接的参数

var newsData = [];
var insertArray = function (arr, index, item) {
    arr.splice(index, 0, item);
    return arr;
}
var deleteArray = function (arr, index) {
    arr.splice(index, 1);
    return arr;
}

var config = {
    url: '/WeChatMain/FileManage/Getlist',
    newsUrl: '/WeChatMain/ArticleInfo/GetNewsList',
    category: function () {
        return window.location.search.split('&')[2].split('=')[1];
    }(),
    appid: function () {
        return window.location.search.split('&')[1].split('=')[1];
    }(),
};
var parseExt = function (str) {
    var result = '';
    if (str == null || str == '') { return ''; }
    if (str && str !== "") {
        var parseArr = str.split('.');
        for (var i in parseArr) {
            if (i == parseArr.length - 2) {
                result += parseArr[i] + '\r\n.';
            } else if (i == parseArr.length - 1) {
                result += parseArr[i]
            } else {
                result += parseArr[i] + '.';
            }
        }
    }
    return result;
}
var conf = {
    'IMAGE_1': { id: 5, el: '#image1-src', edit: '#firstItem' },
    'IMAGE_2': { id: 5, el: '#image2-src', edit: '#secondItem' },
    'IMAGE_3': { id: 1, el: '#image3-src', edit: '#msgType-pic' },
    'AUDIO': { id: 2, el: '#image5-src', edit: '#msgType-sound' },
    'VIDEO': { id: 3, el: '#image4-src', edit: '#msgType-video' },
    'FILE': { id: 4, el: '#image6-src', edit: '#msgType-file' },
    'AUTO_NEWS': { id: 5, edit: '#msgType-wordpic' },
};
var ajaxPost = function ajaxPost(searchKeyword, pageconfig) {
    searchKeyword = searchKeyword || '';
    layer.load();
    $.ajax({
        url: config.url + '?appid=' + config.appid + '&pageIndex=' + pageconfig.pageIndex + '&pageSize=' + pageconfig.pageSize + '&start=' + pageconfig.start + '&length=' + pageconfig.length,
        type: 'post',
        data: { type: +conf[config.category.toUpperCase()].id, searchKeyword: searchKeyword },
        success: function (data) {
            render(data, pageconfig)
        }
    });
};
var ajaxNews = function (searchKeyword, pageconfig) {
    searchKeyword = searchKeyword || '';
    layer.load();
    $.ajax({
        url: config.newsUrl + '?AppId=' + config.appid + '&pageIndex=' + pageconfig.pageIndex + '&pageSize=' + pageconfig.pageSize,
        type: 'post',
        data: { type: +conf[config.category.toUpperCase()].id, searchKeyword: searchKeyword },
        success: function (data) {
            render(data, pageconfig)
        }
    });
};

var switchWrapper = function switchWrapper(category) {
    var wrapper = {
        1: ' <div class="js_list mod-dialog_material-image"> <p class="js_list_lru_title mod-send-message__dialog-title" style="">&nbsp;<span><input id="materialSearchText" type="text" maxLength="20" placeholder="输入查询名称" />&nbsp;&nbsp;<button id="materialSearchButton" class="btn"><i class="ace-icon fa fa-search bigger-140 align-top"></i></button></span></p><div class="js_list_lru js_list_items clearfix ui-ml-large"> </div> </div>',
        2: '<div class="js_list mod-dialog_material-audio"> <p class="js_list_lru_title mod-send-message__dialog-title" style="">&nbsp;<span><input id="materialSearchText" type="text" maxLength="20" placeholder="输入查询名称" />&nbsp;&nbsp;<button id="materialSearchButton" class="btn"><i class="ace-icon fa fa-search bigger-140 align-top"></i></button></span></p> <ul class="js_list_lru js_list_items clearfix"> </ul>  </div>',
        3: '<div class="js_list mod-dialog_material-imagetxt"><p class="js_list_lru_title mod-send-message__dialog-title" style="">&nbsp;<span><input id="materialSearchText" type="text" maxLength="20" placeholder="输入查询名称" />&nbsp;&nbsp;<button id="materialSearchButton" class="btn"><i class="ace-icon fa fa-search bigger-140 align-top"></i></button></span></p><div class="js_list_lru js_list_items clearfix ui-ml-large" "></div></div>'
    };
    var id = conf[category.toUpperCase()].id;
    if (id == 2) {
        return wrapper[2];
    } else if (id == 5) {
        return wrapper[3];
    } else {
        return wrapper[1];
    }
};

var switchHtml = function switchHtml(category, item) {
    var html = '';
    switch (conf[category.toUpperCase()].id) {
        case 1:
            html = '<div data-type="image" data-id="' + item.Id + '" class="js_list_item mod-account-list__item-index ui-mr-large"><div class="mod-account-list__div"><img class="mod-account-list__img" data-id="' + item.Id + '" src="' + parseExt(item.ThumbUrl) + '" alt="" height="80" width="80"></div><div class="mod-account-list__name-index"></div></div>';
            break;
        case 2:
            html = '<li data-type="audio" data-id="' + item.Id + '" class="js_list_item mod-send-message__dialog-single"><span class="mod-send-message__audio-timeimage-wrap"><span class="mod-send-message__audio-timeimage"></span></span><span class="mod-send-message__audio-time"></span><span class="mod-send-message__dialog-audio-content" data-id="' + item.Id + '" data-title="' + item.AttachmentTitle + '" data-src="' + item.AttachmentUrl + '" data-duration="' + item.Duration + '">' + parseExt(item.AttachmentTitle) + '</span></li>';
            break;
        case 3:
            html = '<div data-type="video" data-id="' + item.Id + '" class="js_list_item mod-account-list__item-index ui-mr-large"><div class="mod-account-list__div"><span class="mod-send-message__video-logo"></span><img data-id="' + item.Id + '" data-url="/' + item.AttachmentUrl + '" data-thumb="' + item.ThumbUrl + '" data-title="' + item.AttachmentTitle + '"data-description="' + item.Description + '" class="mod-account-list__img" height="100" width="160" src="' + item.ThumbUrl + '"></div><p class="mod-account-list__name-index-80" >' + parseExt(item.AttachmentTitle) + '</p></div>';
            break;
        case 4:
            html = '<div data-type="file" data-id="' + item.Id + '" class="js_list_item mod-account-list__item-index ui-mr-large"><div class="mod-account-list__div"><span class="icon_filetype filetype_' + item.Extension.split('.')[1] + '" style="width:80px;height:80px"></span></div><p class="mod-account-list__name-index-80" data-id="' + item.Id + '" data-src="' + item.AttachmentUrl + '" data-ext="' + item.Extension + '" data-title="' + item.AttachmentTitle + '" data-size="' + item.FileSize + '">' + parseExt(item.AttachmentTitle) + '</p></div>';
            break;
        case 5:
            html = '<div data-type="text" data-id="' + item.Id + '" class="js_list_item mod-account-list__item-index ui-mr-large margin_button"><div class="mod-account-list__div news"><p class="video_title">' + parseExt(item.ArticleTitle) + '</p> <div class="video_image_wrap"> <img src="' + item.ImageCoverUrl + '" data-id="' + item.Id + '" ><span class="image_vertical"></span></div><p class="video_detail">' + (item.ArticleComment == null ? "" : item.ArticleComment) + '</p></div>';
            break;
    }
    return html;
};

var bindSelect = function bindSelect() {
    $('.js_box').on('click', '.mod-account-list__div', function () {
        if ($(this).hasClass('mod-account-list__div-active')) {
            $(this).removeClass('mod-account-list__div-active');
        }
        $('.mod-account-list__div').removeClass('mod-account-list__div-active');
        $(this).addClass('mod-account-list__div-active');
    });

    $('.js_box').on('click', '.js_list_item.mod-send-message__dialog-single', function () {
        if ($(this).hasClass('mod-send-message__dialog-single-active')) {
            $(this).removeClass('mod-send-message__dialog-single-active');
        }
        $('.js_list_item.mod-send-message__dialog-single').removeClass('mod-send-message__dialog-single-active');
        $(this).addClass('mod-send-message__dialog-single-active');
    });
};
var renderWrapper = function () {
    var wrapper = switchWrapper(config.category);
    $('.js_box').append(wrapper);
}

var render = function render(data, page) {
    var datas = data.aaData;
    newsData = datas;
    page.recordCount = data.iTotalRecords;

    page.render();
    if (data.Message) {
        alert(data.Message.Text);
        return;
    }
    $('.js_list_items').empty();
    if (datas.length === 0) {
        $('.js_list_items').html('<span>没有相关数据</span>');
    } else {
        datas.forEach(function (item) {
            var html = switchHtml(config.category, item);
            $('.js_list_items').append(html);
        });
    }

    bindSelect();
    materialSearch(page);
    layer.closeAll('loading');
};
var renderSize = function renderSize(fileByte) {
    var fileSizeByte = fileByte;
    var fileSizeMsg = "";
    if (fileSizeByte < 1024) {
        fileSizeMsg = fileSizeByte + "B";
    } else if (fileSizeByte < 1048576) fileSizeMsg = parseInt(fileSizeByte / 1024) + "KB";
    else if (fileSizeByte == 1048576) fileSizeMsg = "1MB";
    else if (fileSizeByte > 1048576 && fileSizeByte < 1073741824) fileSizeMsg = parseInt(fileSizeByte / (1024 * 1024)) + "MB";
    else if (fileSizeByte > 1048576 && fileSizeByte == 1073741824) fileSizeMsg = "1GB";
    else if (fileSizeByte > 1073741824 && fileSizeByte < 1099511627776) fileSizeMsg = (fileSizeByte / (1024 * 1024 * 1024)).toFixed(2) + "GB";
    else fileSizeMsg = "文件超过1TB";
    return fileSizeMsg;
};

var materialSearch = function (page) {
    $('#materialSearchText').on('keypress', function (e) {
        var theEvent = e || window.event;
        var code = theEvent.keyCode || theEvent.which || theEvent.charCode;
        if (code == 13) {
            e.preventDefault();
            //回车执行查询  
            var text = $('#materialSearchText').val();
            layer.load();

            if (config.category.toUpperCase() == 'IMAGE_1' || config.category.toUpperCase() == 'IMAGE_2' || config.category.toUpperCase() == 'AUTO_NEWS') {
                ajaxNews(text.trim(),page);
            } else {
                ajaxPost(text.trim(),page);
            }
        }
    })
    $('#materialSearchButton').off('click').on('click', function () {
        var text = $('#materialSearchText').val();
        layer.load();

        if (config.category.toUpperCase() == 'IMAGE_1' || config.category.toUpperCase() == 'IMAGE_2' || config.category.toUpperCase() == 'AUTO_NEWS') {
            ajaxNews(text.trim(),page);
        } else {
            ajaxPost(text.trim(),page);
        }
    })
}

$(function () {
    renderWrapper();
    var p = new Page("page");
    p.numericButtonCount = 5;
    p.pageSize = 6;
    p.start = (p.pageIndex - 1) * p.pageSize;
    p.length = p.pageSize;
    p.initialize();
    var text = $('#materialSearchText').val();
    if (config.category.toUpperCase() == 'IMAGE_1' || config.category.toUpperCase() == 'IMAGE_2' || config.category.toUpperCase() == 'AUTO_NEWS') {
        ajaxNews(text.trim(), p);
        p.addListener('pageChanged', function () {
            ajaxNews(text.trim(), p);
        });
    } else {
        ajaxPost(text.trim(), p);
        p.addListener('pageChanged', function () {
            p.start = (p.pageIndex - 1) * p.pageSize;
            p.length = p.pageSize;
            ajaxPost(text.trim(), p);
        });
    }
    var index = parent.layer.getFrameIndex(window.name);
    var _parent = parent;
    var $parent = $(window.parent.document);
    var targetShow = conf[config.category.toUpperCase()].el;
    var $materialId = $parent.find('#materialId');
    $parent.on('click', '.layui-layer-btn0', function () {
        var $list = $parent.find(targetShow).parent();
        if ($(".mod-account-list__div-active").length > 0) {
            if (config.category.toUpperCase() == "VIDEO") {
                var videoAttribute = $(".mod-account-list__div-active").find('img');
                var src = videoAttribute.data('url');
                var thumbSrc = videoAttribute.data('thumb');
                var title = videoAttribute.data('title');
                var desc = videoAttribute.data('description');
                if ($list.find('.preview').length > 0) {
                    $list.find('.preview').html('<div><div style="float:left"><img style="width: 100px;height:100px;" src="' + thumbSrc + '"></div></div><div>已选择素材</div><div class="clear"></div>');
                } else {
                    $list.append('<div class="preview" style="margin-top:10px;"><img style="width: 100px;height:100px;" src="' + thumbSrc + '"><div">已选择素材</div></div><div class="clear"></div>');
                }
                $parent.find('#msgVideoTitle').val(title);
                $parent.find('#msgVideoDesc').val(desc);
                $parent.find(targetShow).val(src);
                $materialId.val(videoAttribute.data('id'));
            }
                //图文走的这
            else if (config.category.toUpperCase() == "IMAGE_1" || config.category.toUpperCase() == "IMAGE_2") {
                var newsId = $(".mod-account-list__div-active").parent().data('id');
                var selected;
                    for (var i in newsData) {
                        if (newsData[i].Id == newsId) {
                            selected = newsData[i];
                            break;
                        }
                    }
                var tab = conf[config.category.toUpperCase()].edit;
                var $tab = $parent.find(tab);
                var uid = config.category.toUpperCase() == "IMAGE_1" ? "editor1" : "editor2";
                var commit = config.category.toUpperCase() == "IMAGE_1" ? "msgtips" : "msg-tips";

                $tab.find('input[name="msgPictitle"]').val(selected.ArticleTitle);
                $tab.find('textarea[name="' + commit + '"]').val(selected.ArticleComment);

                $tab.find('input[name="msgArticleURL"]').val(selected.ArticleURL);

                $tab.find('input[name=IsLikeBox]').prop("checked", selected.IsLike)
                $tab.find('input[name=ShowReadCountBox]').prop("checked", selected.ShowReadCount)
                $tab.find('input[name=ShowCommentBox]').prop("checked", selected.ShowComment)
                $tab.find('input[name=IsWatermarkBox]').prop("checked", selected.IsWatermark)
                $tab.find('input[name=ArticleNoShareBox]').prop("checked", selected.NoShare)

                $tab.find('.new-materialId').val(selected.Id);

                $tab.find('input[name=IsLikeBox]').parent().find('.IsLike').val(selected.IsLike);
                $tab.find('input[name=ShowReadCountBox]').parent().find('.ShowReadCount').val(selected.ShowReadCount);
                $tab.find('input[name=ShowCommentBox]').parent().find('.ShowComment').val(selected.ShowComment);
                $tab.find('input[name=IsWatermarkBox]').parent().find('.IsWatermark').val(selected.IsWatermark);
                $tab.find('input[name=ArticleNoShareBox]').parent().find('.ArticleNoShare').val(selected.NoShare);

                var alloptions = $tab.find('.securityLevelClass').find('option')

                for (var i = 0; i < alloptions.length ; i++) {
                    if (parseInt($(alloptions[i]).val()) === selected.SecurityLevel) {
                        $(alloptions[i]).prop("selected", "selected")
                    }
                }


                $parent.find("#angular_watch").click();


                var ue = window.parent.UE.getEditor(uid);
                var content = selected.ArticleContent == null ? '' : selected.ArticleContent.toString();
                ue.setContent(content);
                if ($list.find('.preview').length > 0) {
                    $list.find('.preview').html('<div style="float:left"><img style="width: 100px;height:100px;" src="' + selected.ImageCoverUrl + '"></div><div style="float:left;margin-left:10px;line-height: 100px;">已选择素材</div><div style="clear:both;"></div>');
                } else {
                    $list.append('<div class="preview" style="margin-top:10px;"><div style="float:left"><img style="width: 100px;height:100px;" src="' + selected.ImageCoverUrl + '"></div><div style="float:left;margin-left:10px;line-height: 100px;">已选择素材</div></div><div style="clear:both;"></div>');
                }


                $parent.find(targetShow).val(selected.ImageCoverUrl);
                $materialId.val(newsId);

            }
            else if (config.category.toUpperCase() == "FILE") {
                var fileAttribute = $(".mod-account-list__div-active").siblings('.mod-account-list__name-index-80');

                var _src = fileAttribute.data('src');

                var _title = fileAttribute.data('title');

                var size = fileAttribute.data('size');
                var ext = fileAttribute.data('ext');
                $parent.find(targetShow).val(_src);
                $parent.find('#image6-src_type').val(ext.split('.')[1]);
                $parent.find('#image6-src_title').val(_title);
                $parent.find('#image6-src_size').val(renderSize(size));

                if ($list.find('.mod-send-message_contentwrap').length == 0) {
                    $list.append('<div class="mod-send-message_contentwrap"> <span class="icon_filetype filetype_' + ext.split('.')[1] + '"></span> <span class="mod-send-message_process-name">' + _title + ' </span><span class="mod-send-message_process-size">(' + renderSize(size) + ')</span></div>');
                } else {
                    $list.find('.mod-send-message_contentwrap').html('<span class="icon_filetype filetype_' + ext.split('.')[1] + '"></span> <span class="mod-send-message_process-name">' + _title + ' </span><span class="mod-send-message_process-size">(' + renderSize(size) + ')</span></div>');
                }
                $materialId.val(fileAttribute.data('id'));
            }
                //这不是图文 目前看好像没走 走的图片的逻辑
            else if (config.category.toUpperCase() == "AUTO_NEWS") {
                var content_list = JSON.parse(window.parent.$('#news_content').val());
                var li_index = window.parent.$('.repeat-item.newsActived').index('.repeat-item');
                var newsId = $(".mod-account-list__div-active").parent().data('id');
                var selected;
                for (var i in newsData) {
                    if (newsData[i].Id == newsId) {
                        selected = newsData[i];
                        break;
                    }
                }

                console.log(content_list);
                console.log(li_index);
                if (li_index == -1) {
                    if (content_list.length == 0) {
                        window.parent.$('.preview-box').show();
                        window.parent.$('#news_list').find('.first_title').text(selected.ArticleTitle);
                        window.parent.$('#news_list').find('.first_img').attr('src', selected.ImageCoverUrl);
                        window.parent.$('#news_list').find('.first_comment').text(selected.ArticleComment);
                        content_list.push({
                            Id: selected.Id,
                            ArticleTitle: selected.ArticleTitle,
                            ArticleComment: selected.ArticleComment,
                            ImageCoverUrl: selected.ImageCoverUrl,
                            IsLike: selected.IsLike,
                            ShowReadCountBox: selected.ShowReadCountBox,
                            ShowCommentBox: selected.ShowCommentBox
                        });

                    } else if (content_list.length == 1) {
                        window.parent.$('#news_list').find('.first_title').text(selected.ArticleTitle);
                        window.parent.$('#news_list').find('.first_img').attr('src', selected.ImageCoverUrl);
                        window.parent.$('#news_list').find('.first_comment').text(selected.ArticleComment);
                        content_list[0] = {
                            Id: selected.Id,
                            ArticleTitle: selected.ArticleTitle,
                            ArticleComment: selected.ArticleComment,
                            ImageCoverUrl: selected.ImageCoverUrl,
                            IsLike: selected.IsLike,
                            ShowReadCountBox: selected.ShowReadCountBox,
                            ShowCommentBox: selected.ShowCommentBox
                        };
                    } else {
                        if (window.parent.$('#fi-item').is(':visible')) {
                            window.parent.$('#news_list').find('.first_title').text(selected.ArticleTitle);
                            window.parent.$('#news_list').find('.first_img').attr('src', selected.ImageCoverUrl);
                            window.parent.$('#news_list').find('.first_comment').text(selected.ArticleComment);
                            content_list[0] = {
                                Id: selected.Id,
                                ArticleTitle: selected.ArticleTitle,
                                ArticleComment: selected.ArticleComment,
                                ImageCoverUrl: selected.ImageCoverUrl,
                                IsLike: selected.IsLike,
                                ShowReadCountBox: selected.ShowReadCountBox,
                                ShowCommentBox: selected.ShowCommentBox
                            };
                        }
                    }

                }
                else {
                    content_list[li_index + 1] = {
                        Id: selected.Id,
                        ArticleTitle: selected.ArticleTitle,
                        ArticleComment: selected.ArticleComment,
                        ImageCoverUrl: selected.ImageCoverUrl,
                        IsLike: selected.IsLike,
                        ShowReadCountBox: selected.ShowReadCountBox,
                        ShowCommentBox: selected.ShowCommentBox
                    };
                    var $item = window.parent.$('li.repeat-item').eq(li_index);
                    if ($item.hasClass('newsActived')) {
                        $item.find('.title').text(selected.ArticleTitle);
                        $item.find('img').attr('src', selected.ImageCoverUrl);
                    }

                }

                window.parent.$('#news_content').val(JSON.stringify(content_list));

            }
            else {
                var _src2 = $(".mod-account-list__div-active").find('img').attr('src');
                if (_src2) {
                    if ($list.find('.preview').length > 0) {
                        $list.find('.preview').html('<div style="float:left"><img style="width: 100px;height:100px;" src="' + _src2 + '"></div><div style="float:left;margin-left:10px;line-height: 100px;">已选择素材</div><div style="clear:both;"></div>');
                    } else {
                        $list.append('<div class="preview" style="margin-top:10px;"><div style="float:left"><img style="width: 100px;height:100px;" src="' + _src2 + '"></div><div style="float:left;margin-left:10px;line-height: 100px;">已选择素材</div></div><div style="clear:both;"></div>');
                    }
                    $parent.find(targetShow).val(_src2);

                    $materialId.val($(".mod-account-list__div-active").find('img').data('id'));
                }
            }
        }
        else if ($(".mod-send-message__dialog-single-active").length) {

            var audioAttribute = $(".mod-send-message__dialog-single-active").find('.mod-send-message__dialog-audio-content');

            var _src3 = audioAttribute.data('src');

            var _title2 = audioAttribute.data('title');

            var duration = audioAttribute.data('duration');

            if ($list.find('.preview').length > 0) {
                $list.find('.preview').html('<img src="/images/mod-icon-audio_6cb9e065.png""> <span class="mod-send-message_process-name-audio"> ' /* + duration + '“    '*/ + _title2 + ' </span>');
            } else {
                $list.append('<div class="preview" style="margin-top:10px;"><img src="/images/mod-icon-audio_6cb9e065.png"> <span class="mod-send-message_process-name-audio"> ' /* + duration + '“    ' */ + _title2 + ' </span></div>');
            }
            $parent.find(targetShow).val(_src3);
            $materialId.val(audioAttribute.data('id'));
            //$parent.find('#image5-src_duration').val(duration + '“');
        }
            //audio
        

        _parent.layer.close(index);
    });
});

//# sourceMappingURL=app_bk-compiled.js.map