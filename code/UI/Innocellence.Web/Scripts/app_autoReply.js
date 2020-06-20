"use strict";

$(function () {
    //get appid
    var getUrlParm = function getUrlParm(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        return r != null ? decodeURI(r[2]) : null;
    };
    var _AppIdUrl = getUrlParm("appid") || 0;
    var id = getUrlParm("id");
    $('#hiddenAppId').val(_AppIdUrl);

    /*** tab partial ***/
    $('#msgType li').click(function (e) {
        e.preventDefault();
        if ($(this).hasClass('active')) {
            return false;
        }
        $(this).addClass('active').siblings().removeClass('active');
        var tabConts = $('#myTabContent .tab-pane').eq($(this).index());
        tabConts.addClass('active').siblings().removeClass('active');
        // $('.preview-box').eq($(this).index()).show().siblings('.preview-box').hide();
        $('#hiddenType').val(tabConts.data('msg-type'));
        switch (tabConts.data('msg-type')) {
            case 2:
                $('.filename').text("上传");
                break;
            case 3:
                $('.filename').text("上传");
                break;
            default:
        }
    });
    /*** create json and validate partial ***/
    var msgLink_V = function msgLink_V() {
        var mtvalue = $('#msgType-link').val();
        if (mtvalue == null || mtvalue.trim().length <= 0) {
            layer.msg('请填写链接地址', { offset: 0 });
            $('#msgType-link').focus();
            return false;
        } else {
            var regex = "^((https|http)?:\/\/)[^\s]+";
            var re = new RegExp(regex);
            if (!re.test(mtvalue)) {
                layer.msg('请正确填写链接地址, 以http://或https://开头', { offset: 0 });
                $('#msgType-link').focus();
                return false;
            }
        }
        var mtvalue = $('#msgType-link-title').val();
        if (mtvalue == null || mtvalue.trim().length <= 0) {
            layer.msg('请填写文本内容', { offset: 0 });
            $('#msgType-link-title').focus();
            return false;
        }
        return mtvalue;
    }
    //validate1
    var msgText_V = function msgText_V() {

        var mtvalue = $('#saytext').val();
        if (mtvalue == null || mtvalue.trim().length <= 0) {
            //$('.type1ErrorText').show();
            layer.msg('请填写消息内容', { offset: 0 });
            $('#saytext').focus();
            return false;
        }
        if (mtvalue.length > 600) {
            layer.msg('文本超长', { offset: 0 });
            $('#saytext').focus();
            return false;
        }
        return mtvalue;
    };
    //validate2
    var msgPicText_V = function msgPicText_V() {
        var list = JSON.parse($('#news_content').val());
        if (list.length == 0) {
            layer.msg('请选择素材', { offset: 0 });
            return false;
        }
        return list;
    };
    //validate3
    var msgPic_V = function msgPic_V() {

        var mpicsrc = $('#image3-src').val();
        if (mpicsrc == null || mpicsrc.trim().length <= 0) {
            //$('.type1ErrorText').show();
            layer.msg('请选择发送的图片', { offset: 0 });
            return false;
        }
        return mpicsrc;
    };
    //validate4
    var msgVideo_V = function msgVideo_V() {
        var mvideotitle = $('#msgVideoTitle').val();
        if (mvideotitle == null || mvideotitle.trim().length <= 0) {
            layer.msg('请选择素材', { offset: 0 });
            return false;
        }
        var mvideosrc = $('#image4-src').val().replace("/uploads/", "");
        if (mvideosrc == null || mvideosrc.trim().length <= 0) {
            layer.msg('请选择素材', { offset: 0 });
            return false;
        }

        var mvideodesc = $('#msgVideoDesc').val();
        if (mvideodesc == null || mvideodesc.trim().length <= 0) {
            //layer.msg('请填写视频介绍', { offset: 0 });
            //return false;
            mvideodesc = "";
        }
        if (mvideodesc.length > 120) {
            layer.msg('请选择素材', { offset: 0 });
            return false;
        }

        var mvideocover = $('#image7-src').val();

        return {
            mvideotitle: mvideotitle,
            mvideodesc: mvideodesc,
            mvideosrc: mvideosrc,
            mvideocover: mvideocover,
        };
    };
    //validate5
    var msgSound_I = function msgSound_I() {

        var val = $('#image5-src').val();
        if (!val || val.length == 0) {
            layer.msg('请选择素材', { offset: 0 });
            return false;
        }
        return true;
    };
    //validate file
    var msgFile_I = function msgFile_I() {

        var val = $('#image6-src').val();
        if (!val || val.length == 0) {
            layer.msg('请选择素材', { offset: 0 });
            return false;
        }
        return true;
    };

    var linkJson = function linkJson() {
        return [{
            NewsCate: 'link',
            NewsContent: $('#msgType-link').val(),
            NewsTitle: $('#msgType-link-title').val(),
        }];
    }

    //json1 text
    var textJson = function textJson(_text) {
        return [{

            NewsCate: 'text',
            NewsContent: _text
        }];
    };
    //json3 img
    var imgJson = function imgJson(_imgsrc) {

        var _imgMain = [{

            NewsCate: 'image',
            materialId: $('#materialId').val(),
            NewsTitle: $('#image5-src').data('realFileName') || window.image_original_url || '图片',
            ImageContent: _imgsrc
        }];
        return _imgMain;
    };
    //json4 video
    var videoJson = function videoJson(sendRequest) {

        var _videoMain = [{
            AppId: _AppIdUrl,
            NewsCate: 'video',
            materialId: $('#materialId').val(),
            NewsTitle: sendRequest.mvideotitle,
            realFileName: window.video_original_url || '视频',
            ImageContent: sendRequest.mvideocover,
            NewsComment: sendRequest.mvideodesc,
            videoContent: sendRequest.mvideosrc,
        }];
        return _videoMain;
    };
    //json2 text-img
    var textImgJson = function textImgJson(data) {
        var re = data.map(function (item) {
            return item.Id
        }).join(',')
        return [{
            NewsCate: 'news',
            NewsId: re
        }];
    };
    var SoundIJson = function SoundIJson() {
        var _soundSrc = $('#image5-src').val();
        var _newFirst = [{

            NewsCate: 'voice', //msgtype 图文消息
            materialId: $('#materialId').val(),
            realFileName: window.audio_original_url || '音频',
            soundSrc: _soundSrc,
        }];

        return _newFirst;
    };

    var fileIJson = function fileIJson() {

        var _fileSrc = $('#image6-src').val();
        var _realFileName = $('#image6-src_title').val();
        var _size = $('#image6-src_size').val();

        var _newFirst = [{
            AppId: _AppIdUrl,
            materialId: $('#materialId').val(),
            NewsCate: 'file', //msgtype 图文消息
            fileSrc: _fileSrc,
            size: _size,
            realFileName: _realFileName

        }];

        return _newFirst;
    };

    /*** msg send partial ***/

    //submitfun
    function sendReq() {

        var msgtype = $('#hiddenType').val();
        var data = {};
        switch (msgtype) {
            case "0":
                var _link = msgLink_V();
                if (_link) {
                    var _linkJson = linkJson();
                    data.send = _linkJson;
                } else {
                    return false;
                }
                break;
            case "1":
                //发消息
                var _text = msgText_V(); //validate

                if (_text) {

                    var _textJson = textJson(_text); //json1
                    data.send = _textJson;
                } else {
                    return false;
                }
                break;
            case "2":
                //发图文
                var _valid = msgPicText_V();
                if (_valid) {
                    var _News = textImgJson(_valid);
                    data.send = _News;
                } else {
                    return false;
                }
                break;
            case "3":
                //发图
                var _msgImg = msgPic_V();
                if (_msgImg) {

                    var _imgJson = imgJson(_msgImg); //json1
                    data.send = _imgJson;
                } else {
                    return false;
                }
                break;
            case "4":
                //发视频
                var _msgVideo = msgVideo_V();
                if (_msgVideo) {
                    var _videoJson = videoJson(_msgVideo);
                    data.send = _videoJson;
                } else {
                    return false;
                }
                break;
            case "5":
                //发语音
                var _valid = msgSound_I();
                if (_valid) {
                    var _Sound = SoundIJson();
                    data.send = _Sound;
                } else {
                    return false;
                }
                break;
            case "6":
                //file
                var _valid = msgFile_I();
                if (_valid) {
                    $("#submitBtn").text('发送中...');
                    var _News = fileIJson();
                    data.send = _News;
                } else {
                    return false;
                }
                break;
            default:
        }

        function createRequestForm(data) {
            debugger
            data.main = {};
            data.main.id = id || 0;
            data.main.appId = _AppIdUrl;
            data.main.name = $('#main_name').val();
            if (!data.main.name || data.main.name.length === 0) {
                layer.msg('请填写口令名称', { offset: 0 });

                return false;
            }
            if (data.main.name.length > 20) {
                layer.msg('口令名称不能超过20字符', { offset: 0 });

                return false;
            }
            data.main.materialId = $('#materialId').val();
            data.main.description = $('#main_decription').val();
            console.log(data.send);
            data.main.isSecurityPost = Util.checkIsSecurityPost('isSecurityPost');
            data.send[0].isSecurityPost = data.main.isSecurityPost;
            data.main.keywordType = $('#main_keywordType').val();
            var blank = 1,
                toolong = 1;
            switch (data.main.keywordType) {
                case "1":
                    data.main.matchType = [];
                    var _matchType = $('.main_MatchType');
                    $.each(_matchType, function (index, item) {
                        if ($(item).val() != '') {
                            data.main.matchType.push($(item).val());
                        }

                    });
                    var textKeyword = $('.main_textMatchType');
                    var _textKeyword = [];


                    $.each(textKeyword, function (index, item) {
                        if (!$(item).val() || $(item).val().length === 0) {
                            blank = 0;
                            return false;
                        };
                        if ($(item).val().length > 20) {
                            toolong = 0;
                            return false;
                        };
                        _textKeyword.push($(item).val());
                    });
                    if (!blank) {
                        layer.msg('请填写关键词', { offset: 0 });

                        return false;
                    }
                    if (!toolong) {
                        layer.msg('关键词太长', { offset: 0 });

                        return false;
                    }
                    data.main.textKeyword = _textKeyword;
                    break;
                case "2":
                    data.main.matchType = [$('#menuTypes').val()];

                    data.main.textKeyword = [$('#divMenuKeyword').val()];
                    if (!data.main.textKeyword || data.main.textKeyword.length == 0) {
                        layer.msg('请输入关键词', { offset: 0 });
                        blank = 0;
                        return false;
                    }
                    break;
                case "313":
                    console.log($('#cropSubscribe').val());
                    data.main.textKeyword = [$('#cropSubscribe').val()];
                    if (!data.main.textKeyword || data.main.textKeyword.length == 0) {
                        layer.msg('请输入关键词', { offset: 0 });
                        blank = 0;
                        return false;
                    }
                    break;
                case "50":
                case "362":
                    data.main.textKeyword = [$('#scanCode').val()];
                    if (!data.main.textKeyword || data.main.textKeyword.length == 0) {
                        layer.msg('请输入关键词', { offset: 0 });
                        blank = 0;
                        return false;
                    }
                    break;
                default:
            }

            //-----main
            data.InterfaceLink = '';
            data.UserTags = -1;
            data.UserGroups = -1;
            data.MessageTags = -1;
            for (var i = 1; i <= 4; i++) {
                var $content = $("#drop_" + i + "_content");
                if ($("#drop_" + i).parent().hasClass('hidden')) {
                    //页面激活
                    if (i === 2) {
                        var val = $content.find('#message_link').val();
                        console.log(data);
                        data.InterfaceLink = val;
                    } else if (i === 1) {
                        var name = $content.find('select').attr('name');
                        var $li = $content.find('li.active');
                        if ($li.length === 0) {
                            data[name] = -1;
                        } else {
                            data[name] = [];
                            $li.each(function () {
                                data[name].push(+$(this).find('input').val());
                            });
                        }
                    } else {
                        var name = $content.find('select').attr('name');
                        data[name] = +$content.find('select').val();
                    }
                }

            }
            return {
                data: data,
                blank: blank,
                toolong: toolong,
            };
        }

        var re = createRequestForm(data);
        if (!re.blank || !re.toolong) {
            return false;
        }
        $.ajax({
            url: "EditPost",
            type: "post",
            data: JSON.stringify(re.data),
            contentType: "application/json",
            success: function success(data) {
                if (data.Message.Status == 200) {
                    layer.msg('保存成功', { offset: 0 });
                    sessionStorage.autoReplayToIndexpage = "true"
                    setTimeout(function () {
                        window.location.href = "/WeChatMain/AutoReply/Index?appid=" + _AppIdUrl;
                    }, 2000);
                }
            }

        });
    }

    $('#autoReply_submit').on('click', sendReq);

    //表情按钮

    $(".em_nr a").each(function (e) {
        e++;
        $(this).attr("data-image", e);
    });
    $(document).click(function (e) {
        if (e.target.className != $(".com_form p span").attr("class")) {
            $(".em_wrap").fadeOut(500);
        }
    });
    $(".emotion").click(function () {
        if ($(".em_wrap").is(":visible")) {
            $(".em_wrap").fadeOut(500);
        } else {
            $(".em_wrap").fadeIn(500);
        }
    });

    function getCursortPosition(ctrl) { //获取光标位置函数
        var CaretPos = 0; // IE Support
        if (document.selection) {
            ctrl.focus();
            var Sel = document.selection.createRange();
            Sel.moveStart('character', -ctrl.value.length);
            CaretPos = Sel.text.length;
        }
            // Firefox support
        else if (ctrl.selectionStart || ctrl.selectionStart == '0')
            CaretPos = ctrl.selectionStart;
        return (CaretPos);
    }

    function addFace(obj, pos, html) {
        var s = obj.value;
        obj.value = s.substring(0, pos) + html + s.substring(pos);
    }
    $(".em_nr a").click(function () {
        var saytext = document.getElementById('saytext');
        var postion = getCursortPosition(saytext);
        addFace(saytext, postion, "[" + this.title + "]");
        var data_image = $(this).attr("data-image");
        //$("#saytext").val(a + "[" + this.title + "]");

        $("#for-msgType-text").html(Util.parseFaceFromStrToDisplay(saytext.value));
        if ($("#for-msgType-text").html() != "") {
            $("#for-msgType-text").css("padding", "2px 8px");
        } else {
            $("#for-msgType-text").css("padding", "0px");
        }
        var remain = $('#saytext').val().length;
        console.log(remain);
        if (remain > 600) {
            pattern = $('字数超过限制，请适当删除部分内容');
        } else {
            var result = limitNum - remain;
            pattern = '您还可以输入' + result + '字';
        }
        $('#count').html(pattern);
    });



    //max word length
    var limitNum = 600;
    var pattern = '您还可以输入' + limitNum + '字';
    $('#count').html(pattern);
    $('#saytext').keyup(function () {
        var remain = $(this).val().length;
        if (remain > 600) {
            pattern = $('字数超过限制，请适当删除部分内容');
        } else {
            var result = limitNum - remain;
            pattern = '您还可以输入' + result + '字';
        }
        $('#count').html(pattern);
    });

    var emoji_cn = ['[微笑]', '[撇嘴]', '[色]', '[发呆]', '[得意]', '[流泪]', '[害羞]', '[闭嘴]', '[睡]', '[大哭]', '[尴尬]', '[发怒]', '[调皮]', '[呲牙]', '[惊讶]', '[难过]', '[酷]', '[冷汗]', '[抓狂]', '[吐]', '[偷笑]', '[愉快]', '[白眼]', '[傲慢]', '[饥饿]', '[困]', '[惊恐]', '[流汗]', '[憨笑]', '[悠闲]', '[奋斗]', '[咒骂]', '[疑问]', '[嘘]', '[晕]', '[抓狂]', '[衰]', '[骷髅]', '[敲打]', '[再见]', '[擦汗]', '[抠鼻]', '[鼓掌]', '[溴大了]', '[坏笑]', '[左哼哼]', '[右哼哼]', '[哈欠]', '[鄙视]', '[委屈]', '[快哭了]', '[阴险]', '[亲亲]', '[吓]', '[可怜]', '[菜刀]', '[西瓜]', '[啤酒]', '[篮球]', '[乒乓]', '[咖啡]', '[饭]', '[猪头]', '[玫瑰]', '[凋谢]', '[嘴唇]', '[爱心]', '[心碎]', '[蛋糕]', '[闪电]', '[炸弹]', '[刀]', '[足球]', '[瓢虫]', '[便便]', '[月亮]', '[太阳]', '[礼物]', '[拥抱]', '[强]', '[弱]', '[握手]', '[胜利]', '[抱拳]', '[勾引]', '[拳头]', '[差劲]', '[爱你]', '[NO]', '[YES]', '[爱情]', '[飞吻]', '[跳跳]', '[发抖]', '[怄火]', '[转圈]', '[磕头]', '[回头]', '[跳绳]', '[投降]', '[激动]', '[乱舞]', '[献吻]', '[左太极]', '[右太极]'];
    //匹配方法
    function replace_img(str) {
        str = str.replace(/\</g, '&lt;');
        str = str.replace(/\>/g, '&gt;');
        str = str.replace(/\n/g, '<br/>');
        for (var i = 0; i < emoji_cn.length; i++) {

            str = str.replace(eval('/\\' + emoji_cn[i] + '/g'), '<img src="/images/QQexpression/' + (i + 1) + '.gif" border="0" />');
        }
        return str;
    }

    $("#multiText").on('keyup', 'input', function () {

        var length = $(this).val().length;
        var $span = $(this).siblings('span');
        var num = +$span.text().split('/')[1];
        $span.text(length + "/" + num);
        $span.removeClass('red');
        $(this).parent().parent().find('.message_edition__text_length_warn').css('display', 'none');

        if (length >= num) {
            $span.addClass('red');
            $(this).parent().parent().find('.message_edition__text_length_warn').css('display', 'block');
        }
    });
    $('#saytext').on('input propertychange', function () {
        var str = $("#saytext").val();
        $("#for-msgType-text").html(replace_img(str));
    });

    //upload------------
    var validateMinSize = function (size) {
        if (size < 5) {
            return {
                valid: false,
                message: '上传文件不能小于5字节',
            }
        }
        return {
            valid: true,
            message: '',
        }

    }

    var uploadFunc = function uploadFunc(ele) {
        var appId = $('#hiddenAppId').val();
        var validate = function validate(data) {
            return data.originalFiles[0].name.split('.')[1];
        };
        var $material = $('#materialId');
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
        switch (ele) {
            case '.uploader_image':
                return $(ele).fileupload({
                    dataType: 'json',
                    url: '/upload/uploadfile?type=image&appid=' + appId,
                    type: 'post',

                    add: function add(e, data) {
                        var that = this;
                        var validateImage = ['jpeg', 'gif', 'png', 'jpg', 'bmp'];
                        if (validateImage.indexOf(validate(data)) == -1) {
                            layer.msg("不支持的格式!", { offset: 0 });
                            return false;
                        }
                        var valid = validateMinSize(parseInt(data.originalFiles[0].size));
                        if (!valid.valid) {
                            layer.msg(valid.message);
                            return false;
                        }
                        if (parseInt(data.originalFiles[0].size) > 1 * 1024 * 1024 * 2) {
                            layer.msg("请上传少于2M以下的图片", { offset: 0 });
                            return false;
                        }
                        var ajax = data.submit();
                        $(this).parent().parent().find('.progress1 .bar').css('display', 'block');
                        ajax.success(function (res) {
                            window.image_original_url = data.originalFiles[0].name;
                            var inputHidden = $(that).parent().siblings('input');
                            inputHidden.val(res.targetFilePath + '\\' + res.serverFileName);
                            inputHidden.change();
                            var father = $(that).parent().parent().find('.preview');
                            console.log(father);
                            if (father.length === 0) {
                                $(that).parent().parent().append('<div class="preview" style="margin-top:10px;"><div style="float:left"><img style="width: 100px;height:100px;" src="' + res.targetFilePath + '\\' + res.serverFileName + '" /></div><div style="float:left;margin-left:10px;line-height: 100px;color:#14a97f;"><i class="ace-icon glyphicon glyphicon-ok"></i>&nbsp;已上传</div></div><div style="clear:both"></div>');
                            } else {
                                father.html('<div style="float:left"><img style="width: 100px;height:100px;" src="' + res.targetFilePath + '\\' + res.serverFileName + '" /></div><div style="float:left;margin-left:10px;line-height: 100px;color:#14a97f;"><i class="ace-icon glyphicon glyphicon-ok"></i>&nbsp;已上传</div>');
                            }
                            $material.val('null');
                        });
                    },
                    progressall: function progressall(e, data) {
                        var progress = parseInt(data.loaded / data.total * 100, 10);
                        $('.progress1 .bar').css('width', progress + '%');
                        if (progress == 100) {
                            $(this).parent().parent().find('.progress1 .bar').css('display', 'none');
                        }
                    },
                    done: function done(e, data) { }

                });
                break;
                //file
            case '.uploader_file':
                return $(ele).fileupload({
                    dataType: 'json',
                    url: '/upload/uploadfile?type=file&appid=' + appId,
                    type: 'post',

                    add: function add(e, data) {
                        var validateFile = ['text', 'txt', 'doc', 'docx', 'xml', 'pdf', 'zip', 'ppt', 'xls', 'pptx', 'xlsx'];
                        console.log(data);
                        var ext = data.originalFiles[0].name.split('.')[1];
                        if (validateFile.indexOf(validate(data)) == -1) {
                            layer.msg("不支持文件格式!", { offset: 0 });
                            return false;
                        }
                        var valid = validateMinSize(parseInt(data.originalFiles[0].size));
                        if (!valid.valid) {
                            layer.msg(valid.message);
                            return false;
                        }
                        if (parseInt(data.originalFiles[0].size) > 1 * 1024 * 1024 * 20) {
                            layer.msg("请上传少于20M以下的文件", { offset: 0 });
                            return false;
                        }
                        var that = this;
                        //data.context = $('<p/>').text('Uploading...').appendTo($(this).parent());
                        var ajax = data.submit();
                        $(this).parent().parent().find('.progress1 .bar').css('display', 'block');
                        ajax.success(function (res) {
                            var size = renderSize(data.originalFiles[0].size);
                            var inputHidden = $(that).parent().siblings('#image6-src');
                            inputHidden.val(res.targetFilePath + '\\' + res.serverFileName);
                            inputHidden.change();

                            var inputHiddenType = $(that).parent().siblings('#image6-src_type');
                            var inputHiddenTitle = $(that).parent().siblings('#image6-src_title');
                            var inputHiddenSize = $(that).parent().siblings('#image6-src_size');
                            inputHiddenType.val(ext);
                            inputHiddenType.change();
                            inputHiddenTitle.val(res.realFileName);
                            inputHiddenTitle.change();
                            inputHiddenSize.val(size);
                            inputHiddenSize.change();
                            var father = $(that).parent().parent().find('.mod-send-message_contentwrap');
                            if (father.length === 0) {
                                $(that).parent().parent().append('<div class="mod-send-message_contentwrap"> <span class="icon_filetype filetype_' + ext + '"></span> <span class="mod-send-message_process-name">' + res.realFileName + ' </span> <span class="mod-send-message_process-size">(' + size + ')</span><input type=hidden value="' + res.targetFilePath + '\\' + res.serverFileName + '"/></div>');
                            } else {
                                father.html('<span class="icon_filetype filetype_' + ext + '"></span> <span class="mod-send-message_process-name">' + res.realFileName + ' </span><span class="mod-send-message_process-size">(' + size + ')</span>');
                            }
                            $material.val('null');
                            //$('.mc_detail').html('<span class="icon_filetype filetype_'+ext+'"></span><div class="file_detail_info"><span class="name">'+res.realFileName+'</span><span class="gray size">('+size+')</span></div>')
                        });
                    },
                    progressall: function progressall(e, data) {
                        var progress = parseInt(data.loaded / data.total * 100, 10);
                        $('.progress1 .bar').css('width', progress + '%');
                        if (progress == 100) {
                            $(this).parent().parent().find('.progress1 .bar').css('display', 'none');
                        }
                    },
                    done: function done(e, data) { },
                    change: function change() {
                        console.log('change');
                    }

                });
                break;
                //video
            case '.uploader_video':
                return $(ele).fileupload({
                    dataType: 'json',
                    url: '/upload/uploadfile?type=video&appid=' + appId,
                    type: 'post',

                    add: function add(e, data) {
                        var validateVideo = ['mp4'];
                        console.log(data);
                        if (validateVideo.indexOf(validate(data)) == -1) {
                            layer.msg("不支持视频格式!", { offset: 0 });
                            return false;
                        }
                        var valid = validateMinSize(parseInt(data.originalFiles[0].size));
                        if (!valid.valid) {
                            layer.msg(valid.message);
                            return false;
                        }
                        if (parseInt(data.originalFiles[0].size) > 1 * 1024 * 1024 * 10) {
                            layer.msg("请上传少于10M以下的视频", { offset: 0 });
                            return false;
                        }
                        var that = this;
                        //data.context = $('<p/>').text('Uploading...').appendTo($(this).parent());
                        var ajax = data.submit();
                        $(this).parent().parent().find('.progress1 .bar').css('display', 'block');
                        ajax.success(function (res) {
                            window.video_original_url = data.originalFiles[0].name;
                            var inputHidden = $(that).parent().siblings('#image4-src');
                            inputHidden.val(res.targetFilePath + '\\' + res.serverFileName);
                            inputHidden.data('realFileName', res.realFileName);
                            inputHidden.change();
                            var father = $(that).parent().parent().find('.preview');
                            if (father.length === 0) {
                                $(that).parent().parent().append('<div class="preview" style="margin-top:10px;"><div>' + res.realFileName + '</div><div">上传成功</div></div>');
                            } else {

                                father.html('<div>' + res.realFileName + '"</div><div>上传成功</div>');
                            }
                            $material.val('null');
                        });
                    },
                    progressall: function progressall(e, data) {
                        var progress = parseInt(data.loaded / data.total * 100, 10);
                        $('.progress1 .bar').css('width', progress + '%');
                        if (progress == 100) {
                            $(this).parent().parent().find('.progress1 .bar').css('display', 'none');
                        }
                    },
                    done: function done(e, data) { },
                    change: function change() {
                        console.log('change');
                    }

                });
                break;
            case '.uploader_audio':
                return $(ele).fileupload({
                    dataType: 'json',
                    url: '/upload/uploadfile?type=audio&appid=' + appId,
                    type: 'post',

                    add: function add(e, data) {
                        var validateVideo = ['mp3'];
                        console.log(data);
                        if (validateVideo.indexOf(validate(data)) == -1) {
                            layer.msg("不支持音频格式!", { offset: 0 });
                            return false;
                        }
                        var valid = validateMinSize(parseInt(data.originalFiles[0].size));
                        if (!valid.valid) {
                            layer.msg(valid.message);
                            return false;
                        }
                        if (parseInt(data.originalFiles[0].size) > 1 * 1024 * 1024 * 2) {
                            layer.msg("请上传少于2M以下的语音", { offset: 0 });
                            return false;
                        }
                        var that = this;
                        //data.context = $('<p/>').text('Uploading...').appendTo($(this).parent());
                        var ajax = data.submit();
                        $(this).parent().parent().find('.progress1 .bar').css('display', 'block');
                        ajax.success(function (res) {
                            window.audio_original_url = data.originalFiles[0].name;
                            if (+res.duration > 60) {
                                $.get('Delete?id=' + res.Id, function () {
                                    layer.msg("音频不允许超过60秒!", { offset: 0 });
                                });
                            } else {
                                var inputHiddenSrc = $(that).parent().siblings('#image5-src');
                                inputHiddenSrc.val(res.targetFilePath + '\\' + res.serverFileName);
                                inputHiddenSrc.data('realFileName', res.realFileName);
                                inputHiddenSrc.change();
                                var inputHiddenDuration = $(that).parent().siblings('#image5-src_duration');
                                inputHiddenDuration.val(res.duration);
                                inputHiddenDuration.change();
                                var father = $(that).parent().parent().find('.preview');
                                if (father.length === 0) {
                                    $(that).parent().parent().append('<div class="preview" style="margin-top:10px;"><img src="/images/mod-icon-audio_6cb9e065.png"> <span class="mod-send-message_process-name-audio" data-realFileName="' + res.realFileName + '">' + res.duration.trim() + '“&nbsp;&nbsp;&nbsp;' + res.realFileName + '</span></div><div style="clear:both"></div>');
                                } else {

                                    father.html('<img src="/images/mod-icon-audio_6cb9e065.png""> <span class="mod-send-message_process-name-audio" >' + res.duration.trim() + '“&nbsp;&nbsp;&nbsp;' + res.realFileName + '</span>');
                                }
                            }
                            $material.val('null');
                        });
                    },
                    progressall: function progressall(e, data) {
                        var progress = parseInt(data.loaded / data.total * 100, 10);
                        $('.progress1 .bar').css('width', progress + '%');
                        if (progress == 100) {
                            $(this).parent().parent().find('.progress1 .bar').css('display', 'none');
                        }
                    },
                    done: function done(e, data) { },
                    change: function change() {
                        console.log('change');
                    }

                });
                break;
        }
    };
    uploadFunc('.uploader_image');
    uploadFunc('.uploader_video');
    uploadFunc('.uploader_file');
    uploadFunc('.uploader_audio');

    //tab
    //显示
    $('.dropdown-menu').on('click', 'a', function () {
        $(this).parent().addClass('hidden');
        var index = $(this).parent().index();
        $('.nav-tabs').find('li').eq(index).removeClass('hidden').find('a').click();
        $('.tab-content').find('div').eq(index).find('select').eq(0).val([]).multiselect('refresh');
        if ($(this).parent().siblings('li:not(.hidden)').length == 0) {
            $(this).closest('.dropdown').fadeOut();
        }
    });

    //删除
    $('.nav-tabs').on('click', '.glyphicon', function () {
        $(this).parents('li').addClass('hidden');
        var index = $(this).parents('li').index();
        $(this).parents('li').siblings('.dropdown').find('.dropdown-menu').find('li').eq(index).removeClass('hidden');
        $('.nav-tabs').find('li').eq(0).find('a').click();
        $(this).closest('li').siblings('.dropdown').fadeIn();
        return false;
    });

    //init
    $('.tab-content').find('#drop_1_content_selection').multiselect();
    if ($('#hiddenIsCorp').val() == "1") {

        $('.dropdown-hidden-crop').addClass('hidden');
        $('.dropdown').hide();
    }
    $('#replyForm').submit(function () {
        var tabId = $('#myTabContent').children('.active').attr('id');
        $('#hiddenTab').val(tabId);
    });



    //update
    function upadteRequestForm() {
        var msg = layer.msg('加载中', { icon: 16, time: 0 });
        $.get('GetContent?id=' + id, function (data) {
            layer.close(msg);
            var main = data.Main;

            parseSend(data);
            parseTabs(data);
        });

        var parseSend = function parseSend(data) {
            var send = data.Send[0];
            if (send.isSecurityPost) {
                $('#isSecurityPost').attr("checked", 'true');
            }

            switch (send.NewsCate) {
                case 'link':
                    $('#msgType').children('li').eq(0).click();
                    $('#msgType-link').val(send.NewsContent);
                    $('#msgType-link-title').val(send.NewsTitle);
                    break;
                case 'text':
                    $('#msgType').children('li').eq(1).click();
                    $('#saytext').val(send.NewsContent);
                    var remain = $('#saytext').val().length;
                    if (remain > 600) {
                        pattern = $('字数超过限制，请适当删除部分内容');
                    } else {
                        var result = limitNum - remain;
                        pattern = '您还可以输入' + result + '字';
                    }
                    $('#count').html(pattern);
                    break;
                case 'news':
                    console.log(data);
                    var result = data.Send.map(function (item) {
                        return {
                            NewsCate: item.NewsCate,
                            ArticleTitle: item.NewsTitle,
                            ImageCoverUrl: item.ImageSrc,
                            Id: item.Id,
                            ArticleComment: item.NewsComment,

                        }
                    })
                    $('#msgType').children('li').eq(2).click();
                    $('#news_content').val(JSON.stringify(result));
                    $('#msgType-wordpic').find('.preview-box').show();
                    $('#news_list').find('.first_title').text(result[0].ArticleTitle);
                    $('#news_list').find('.first_img').attr('src', result[0].ImageCoverUrl);
                    $('#news_list').find('.first_comment').text(result[0].ArticleComment);

                    if (result.length > 1) {
                        $('#fi-item').show();
                        $('#fi-item').addClass('newsActived')
                        $('#msgType-wordpic').find('.default-item').hide();
                        for (var i = 1; i < result.length; i++) {
                            var html = "<li class='repeat-item' ><div class='order' style='display:none;'><b class='arrow fa fa-angle-up'></b>&nbsp;&nbsp;<b class='arrow fa fa-angle-down' ></b></div><a href='javascript:void(0)' >" +
                                "<div class='repeat-img-box' ><img /></div>" +
                                "<p class='title' ></p>" + "</a><button class='rep-del' ><i class='ace-icon glyphicon glyphicon-remove'></i></button></li>";
                            $('.last-item').before(html);
                            $('#news_list').find('.repeat-item').eq(i - 1).find('.title').text(result[i].ArticleTitle);
                            $('#news_list').find('.repeat-item').eq(i - 1).find('img').attr('src', result[i].ImageCoverUrl);
                        }

                    }
                    break;
                case "image":
                    $('#msgType').children('li').eq(3).click();
                    $('#image3-src').val(send.ImageContent);
                    $('#image3-src').parent().append('<div class="preview" style="margin-top:10px;"><div style="float:left"><img style="width: 100px;height:100px;" src="' + send.ImageContent + '" /></div><div style="float:left;margin-left:10px;line-height: 100px;">' + send.NewsTitle + '</div></div><div style="clear:both"></div>');
                    $('#image5-src').data('realFileName', send.NewsTitle);
                    break;
                case 'file':
                    $('#msgType').children('li').eq(4).click();
                    $('#image6-src').val(send.FileSrc);
                    $('#image6-src').parent().append('<div class="mod-send-message_contentwrap"> <span class="icon_filetype filetype_' + send.RealFileName.split('.')[1] + '"></span> <span class="mod-send-message_process-name">' + send.realFileName + ' </span> <span class="mod-send-message_process-size">(' + send.Size + ')</span><input type=hidden value="' + send.FileSrc + '"/></div>');
                    break;
                case 'video':
                    $('#msgType').children('li').eq(5).click();
                    $('#image4-src').val(send.VideoContent);
                    $('#image4-src').data('realFileName', send.RealFileName);
                    $('#image4-src').parent().append('<div class="preview" style="margin-top:10px;"><div style="float:left"><img style="width: 100px;height:100px;" src="' + send.ImageContent + '" /></div><div style="float:left;margin-left:10px;line-height: 100px;"></div>' + send.NewsTitle + '</div><div style="clear:both"></div>');
                    $('#image7-src').val(send.ImageContent);
                    $('#msgVideoTitle').val(send.NewsTitle);
                    $('#msgVideoDesc').val(send.NewsComment);
                    break;
                case 'voice':
                    window.audio_original_url = send.RealFileName;
                    $('#msgType').children('li').eq(6).click();
                    $('#image5-src').val(send.SoundSrc);
                    $('#image5-src').parent().append('<div class="preview" style="margin-top:10px;"><img src="/images/mod-icon-audio_6cb9e065.png"> <span class="mod-send-message_process-name-audio" data-realFileName="' + send.RealFileName + '">&nbsp;&nbsp;&nbsp;' + send.NewsTitle + '</span></div><div style="clear:both"></div>');
                    break;
            }
        };
        var parseTabs = function parseTabs(_ref) {
            var MessageTags = _ref.MessageTags;
            var UserGroups = _ref.UserGroups;
            var UserTags = _ref.UserTags;
            var InterfaceLink = _ref.InterfaceLink;
            console.log(_ref);
            if (MessageTags > 0) {
                $('#drop_4').click();
                $('#drop_4_content_selection').val(MessageTags);
            }
            if (InterfaceLink) {
                $('#drop_2').click();
                $('#message_link').val(InterfaceLink);
            }
            if (UserGroups > 0) {
                $('#drop_3').click();
                $('#drop_3_content_selection').val(UserGroups);
            }
            if (UserTags.length > 0) {
                $('#drop_1').click();
                $('#drop_1_content_selection').multiselect('select', UserTags);

            }
            $('#drop_0').click();
        };
    }

    if (id && id.length > 0) {
        //执行ajax
        console.log('ajax');
        upadteRequestForm();
    }

    //hide  保密checkbox
    getUrlParm()



    //news
    console.log($('#news_content').val());
    var list = JSON.parse($('#news_content').val());
    var deleteArray = function (arr, index) {
        arr.splice(index, 1);
        return arr;
    }
    //first add

    $('#msgType-wordpic').on('click', '#fi-item', function () {
        $('#news_list').find('li.repeat-item').removeClass('newsActived');
        $(this).addClass('newsActived');

    });
    //add
    $('#msgType-wordpic').on('click', 'li.last-item', function () {
        var html = "<li class='repeat-item newsActived' ><div class='order' style='display:none;'><b class='arrow fa fa-angle-up'></b>&nbsp;&nbsp;<b class='arrow fa fa-angle-down' ></b></div><a href='javascript:void(0)' >" +
            "<div class='repeat-img-box' ><img /></div>" +
            "<p class='title' ></p>" + "</a><button class='rep-del' ><i class='ace-icon glyphicon glyphicon-remove'></i></button></li>";
        if ($('#news_list').find('li.repeat-item').length >= 9) {
            layer.msg('最多10条新闻');
            return;
        }
        if ($('#msgType-wordpic').find('li.repeat-item').length == 0) {
            changeFirstDisplay();
        }
        $('#msgType-wordpic').find('li').removeClass('newsActived');
        $('.last-item').before(html);
        var list = JSON.parse($('#news_content').val());
        list.push({
            Id: '',
            ArticleTitle: '',
            ArticleComment: '',
            ImageCoverUrl: '',
        })
        $('#news_content').val(JSON.stringify(list));

    })
    var changeFirstDisplay = function () {
        if ($('.default-item').is(':visible')) {
            $('.default-item').hide();
            $('#fi-item').show();
        } else {
            $('.default-item').show();
            $('#fi-item').hide();
        }

    }
    var deleteFirst = function () {
        var list = JSON.parse($('#news_content').val());

        list = arrayDel(list, 0);
        renderFirst(list);
        if (lsit.length === 1) {

        }
        $('#news_content').val(JSON.stringify(list));
    }

    //select
    $('#msgType-wordpic').off('click', 'li.repeat-item').on('click', 'li.repeat-item', function () {
        $('#news_list').find('#fi-item').removeClass('newsActived');
        $(this).siblings('li.repeat-item').removeClass('newsActived');
        $(this).addClass('newsActived');
    });
    //delete
    $('#msgType-wordpic').off('click', '.rep-del').on('click', '.rep-del', function () {
        var list = JSON.parse($('#news_content').val());
        if ($(this).index('.rep-del') == 0) {
            list = deleteArray(list, 0);
            renderFirst(list);
            if (list.length === 1) {
                changeFirstDisplay();
            }
            $('#news_list').find('li.repeat-item').eq(0).remove();
        } else {
            if ($('#msgType-wordpic').find('li.repeat-item').length == 1) {
                changeFirstDisplay();
            }
            var index = $(this).parent().index('.repeat-item');

            $(this).parent().remove();
            deleteArray(list, index + 1);
            $('#news_content').val(JSON.stringify(list));
        }
        $('#news_content').val(JSON.stringify(list));

    });

    //order
    $('#msgType-wordpic').on('mouseover', '#fi-item', function () {
        $(this).find(".order").css("display", "block");
    });
    $('#msgType-wordpic').on('mouseout', '#fi-item', function () {
        $(this).find(".order").css("display", "none");
    });
    $('#msgType-wordpic').on('mouseover', '.repeat-item', function () {
        $(this).find(".order").css("display", "block");
        if ($(this).next('.repeat-item').length == 0) {
            $(this).find('.fa-angle-down').hide();
        } else {
            $(this).find('.fa-angle-down').show();
        }
    });
    $('#msgType-wordpic').on('mouseout', '.repeat-item', function () {
        $(this).find(".order").css("display", "none");
    });
    var swapItems = function (arr, index1, index2) {
        arr[index1] = arr.splice(index2, 1, arr[index1])[0];
        return arr;
    };
    var renderFirst = function (list) {
        $('#news_list').find('.first_title').text(list[0].ArticleTitle);
        $('#news_list').find('.first_img').attr('src', list[0].ImageCoverUrl);
        $('#news_list').find('.first_comment').text(list[0].ArticleComment);
    }
    var renderRepeat = function (list, index) {
        var $repeat = $('#news_list').find('li.repeat-item').eq(index);
        $repeat.find('.title').text(list[index + 1].ArticleTitle);
        $repeat.find('img').attr('src', list[index + 1].ImageCoverUrl);

    }
    var arrayDel = function (arr, index) {
        arr.splice(index, 1);
        return arr;
    }

    //上箭头
    $('#msgType-wordpic').off('click', '.fa-angle-up').on('click', '.fa-angle-up', function (e) {
        e.stopPropagation();
        var list = JSON.parse($('#news_content').val());
        var index = $(this).closest('li.repeat-item').index('.repeat-item');
        list = swapItems(list, index + 1, index);
        if (index == 0) {
            renderFirst(list);
            renderRepeat(list, index);


        } else {
            renderRepeat(list, index);
            renderRepeat(list, index - 1);
        }

        $('#news_content').val(JSON.stringify(list));

    });

    //下箭头
    $('#msgType-wordpic').off('click', '.fa-angle-down').on('click', '.fa-angle-down', function (e) {
        e.stopPropagation();
        var list = JSON.parse($('#news_content').val());
        var index = $(this).closest('li.repeat-item').index('.repeat-item');

        if (index == -1) {
            list = swapItems(list, 1, 0);
            renderFirst(list);
            renderRepeat(list, 0);


        } else {
            list = swapItems(list, index + 2, index + 1);
            renderRepeat(list, index);
            renderRepeat(list, index + 1);
        }

        $('#news_content').val(JSON.stringify(list));

    });



});