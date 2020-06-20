"use strict";

$(function () {
    //get appid
    var getUrlParm = function getUrlParm(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        return r != null ? decodeURI(r[2]) : null;
    };
    var _AppIdUrl = getUrlParm("appid") || 0;
    $('#hiddenAppId').val(_AppIdUrl);
    $('.yulanModal').WeChatPersonSelection();
    $('#wechatab').WeChatAddressBook();
    $('#wechatab-v2').WeChatAddressSelection();
    /*** tab partial ***/
    $('#msgType li').click(function (e) {
        e.preventDefault();
        if ($(this).hasClass('active')) {
            return false;
        }
        $(this).addClass('active').siblings().removeClass('active');
        var tabConts = $('#myTabContent .tab-pane').eq($(this).index());
        tabConts.addClass('active').siblings().removeClass('active');
        $('.preview-box').eq($(this).index()).show().siblings('.preview-box').hide();
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


    function parseDate() {
        return {
            date: $('#pickDate').val(),
            hour: $('#date_hour').val(),
            minute: $('#date_minutes').val(),
        }

    }
    /*** create json and validate partial ***/
    //validate1
    var msgText_V = function msgText_V(isyulan) {
        var selectedGroup = $('#realTImeCount').text();
        if (!isyulan&&(selectedGroup == "" || selectedGroup < 2)) { //update by tx 2016/10/20  (修改为至少选择一个发送人)
            layer.msg('至少选择2个发送人');
            return false;
        }
        var mtvalue = $('#saytext').val();
        if (mtvalue == null || mtvalue.trim().length <= 0) {
            //$('.type1ErrorText').show();
            layer.msg('请填写消息内容');
            $('#saytext').focus();
            return false;
        }
        if (mtvalue.length > 600) {
            layer.msg('文本超长');
            $('#saytext').focus();
            return false;
        }
        return mtvalue;
    };
    //validate2
    var msgPicText_V = function msgPicText_V(isyulan) {
        var selectedGroup = $('#realTImeCount').text();
        if (!isyulan&&(selectedGroup == "" || selectedGroup < 2)) { //update by tx 2016/10/20  (修改为至少选择一个发送人)
            layer.msg('至少选择2个发送人');
            return false;
        }
        var _oldObjv = JSON.parse($('.obj_news').text());
        var _firstNewsv = JSON.parse($('.obj_firstItem').text());
        var validate = true;
        var firstNewValidate = true;
        $.each(_firstNewsv, function () {
            if (_firstNewsv.msgtitle == "") {
                layer.msg('请填写标题');
                firstNewValidate = false;
                return false;
            }
            if (_firstNewsv.msgfileImg == "") {
                layer.msg('请上传图片');
                firstNewValidate = false;
                return false;
            } //多条新闻下，不验证摘要
            if (_oldObjv.length <= 0 && _firstNewsv.msgtips == "") {
                layer.msg('请填写摘要');
                firstNewValidate = false;
                return false;
            }
            if (_firstNewsv.msgtips.length > 120) {
                layer.msg('摘要大于120字符');
                firstNewValidate = false;
                return false;
            }
            if (_firstNewsv.msgmaintext == "") {
                layer.msg('请填写正文');
                firstNewValidate = false;
                return false;
            }
            if (_firstNewsv.btntext == "") {
                layer.msg('请填写回复按钮内容');
                firstNewValidate = false;
                return false;
            }
        });
        if (!firstNewValidate) {
            $('.first-item').click();
            $('.first-item').addClass('newsActived').siblings().removeClass('newsActived');
            return false;
        }
        var curLi_Index = "";
        $.each(_oldObjv, function (i) {
            curLi_Index = i;
            if (_oldObjv[i].msgtitle == "") {
                layer.msg('请填写标题');
                validate = false;
                return false;
            }
            if (_oldObjv[i].msgfileImg == "") {
                layer.msg('请上传图片');
                validate = false;
                return false;
            }
            //if (_oldObjv[i].msgtips == "") {
            //    layer.msg('摘要不能为空');
            //    validate = false;
            //    return false;
            //}
            if (_oldObjv[i].msgtips.length > 120) {
                layer.msg('摘要不能大于120字符');
                validate = false;
                return false;
            }
            if (_oldObjv[i].msgmaintext == "") {
                layer.msg('请填写正文');
                validate = false;
                return false;
            }
            if (_oldObjv[i].btntext == "") {
                layer.msg('请填写回复按钮内容');
                validate = false;
                return false;
            }
        });
        if (validate) {
            return true;
        } else {
            var _curLi = $('.repeat-item')[curLi_Index];
            $(_curLi).find("a").click();
            $(_curLi).addClass("newsActived").siblings().removeClass('newsActived');
            return false;
        }
    };
    //validate3
    var msgPic_V = function msgPic_V(isyulan) {
        var selectedGroup = $('#realTImeCount').text();
        if (!isyulan&&(selectedGroup == "" || selectedGroup < 2)) { //update by tx 2016/10/20  (修改为至少选择一个发送人)
            layer.msg('至少选择2个发送人');
            return false;
        }
        var mpicsrc = $('#image3-src').val();
        if (mpicsrc == null || mpicsrc.trim().length <= 0) {
            //$('.type1ErrorText').show();
            layer.msg('请选择发送的图片');
            return false;
        }
        return mpicsrc;
    };
    //validate4
    var msgVideo_V = function msgVideo_V(isyulan) {
        var selectedGroup = $('#realTImeCount').text();
        if (!isyulan&&(selectedGroup == "" || selectedGroup < 2)) { //update by tx 2016/10/20  (修改为至少选择一个发送人)
            layer.msg('至少选择2个发送人');
            return false;
        }
        var mvideosrc = $('#image4-src').val().replace("/uploads/", "");
        if (mvideosrc == null || mvideosrc.trim().length <= 0) {
            layer.msg('请上传发送的视频');
            return false;
        }
        var mvideotitle = $('#msgVideoTitle').val();
        if (mvideotitle == null || mvideotitle.trim().length <= 0) {
            layer.msg('请填写标题');
            return false;
        }
        var mvideodesc = $('#msgVideoDesc').val();
        if (mvideodesc == null || mvideodesc.trim().length <= 0) {
            //layer.msg('请填写视频介绍');
            //return false;
            mvideodesc = "";
        }
        if (mvideodesc.length > 120) {
            layer.msg('视频介绍超过120字符');
            return false;
        }
        //var mvideoposter = $('#image7-src').val();
        //if (mvideoposter == null || mvideoposter.trim().length <= 0) {
        //    layer.msg('请上传封面图片');
        //    return false;
        //}
        return {
            mvideotitle: mvideotitle,
            mvideodesc: mvideodesc,
            mvideosrc: mvideosrc,
            //mvideoposter: mvideoposter
        };
    };
    //validate5
    var msgSound_I = function msgSound_I(isyulan) {
        var selectedGroup = $('#realTImeCount').text();
        if (!isyulan&&(selectedGroup == "" || selectedGroup < 2)) { //update by tx 2016/10/20  (修改为至少选择一个发送人)
            layer.msg('至少选择2个发送人');
            return false;
        }
        var val = $('#image5-src').val();
        if (!val || val.length == 0) {
            layer.msg('请上传语音');
            return false;
        }
        return true;
    };


    //json for PeopleGroup
    var groupOrPeople = function groupOrPeople() {
        var _group = JSON.parse($('#mainGroup').val());
        var _peo = [];
        var _gru = [];
        var _peoName = [];
        var _gruName = [];
        var _tag = [];
        var _tagName = [];
        $.each(_group, function (item) {
            if (_group[item].Type == "Group") {
                _gru.push(_group[item].WeixinId);
                _gruName.push(_group[item].WeixinName);
            } else if (_group[item].Type == "Person") {
                _peo.push(_group[item].WeixinId);
                _peoName.push(_group[item].WeixinName);
            } else {
                _tag.push(_group[item].WeixinId);
                _tagName.push(_group[item].WeixinName);
            }
        });
        return {
            groups: {
                group: _gru,
                groupName: _gruName
            },
            persons: {
                person: _peo,
                personName: _peoName
            },
            tags: {
                tag: _tag,
                tagName: _tagName
            }
        };
    };
    //json1 text
    var textJson = function textJson(_text, postType) {
        //var _groupArr = groupOrPeople(); //返回people和group的数组
        var _main = [];
        debugger;
        var group = $("#group").val();
        var Sex = $("#sex").val();
        var Province = $("#province").val();
        var City = $("#cities").val();
        var msgText = { NewsTitle: _text };
        return [{
            AppId: _AppIdUrl,
            NewsCate: 'text',
            SendToPerson: $(".multiple-select-yulan").select2("val"),
            //SendToGroup: _groupArr.groups.group,
            //SendToPersonName: _groupArr.persons.personName,
            //SendToGroupName: _groupArr.groups.groupName,
            //SendToTag: _groupArr.tags.tag,
            //SendToTagName: _groupArr.tags.tagName,
            MaterialId: $('#materialId').val(),
            isSecurityPost: Util.checkIsSecurityPost('isSecurityPost'),
            SecurityLevel: $('#SecurityLevel').val(),
            ScheduleSendTime: parseDate(),
            PostType: postType, //1 normal post 2: defer post
            NewsTitle: _text,
            NewsContent: _text,
            //user filter info 
            Group: $("#group").val(),
            Sex: $("#sex").val(),
            Province: $("#province").val(),
            City: $("#cities").val(),
            //SendToPerson: [$("#userid").val()]
        }];

    };
    //json3 img
    var imgJson = function imgJson(_imgsrc, postType) {
        //var _groupArr = groupOrPeople(); //返回people和group的数组
        var _realFileName = $('#image5-src').data('realFileName');

        var _imgMain = [{
            AppId: _AppIdUrl,
            NewsCate: 'image',
            MaterialId: $('#materialId').val(),
            isSecurityPost: Util.checkIsSecurityPost('isSecurityPost'),
            SecurityLevel: $('#SecurityLevel').val(),
            ScheduleSendTime: parseDate(),
            PostType: postType, //1 normal post 2: defer post
            //user filter info 
            Group: $("#group").val(),
            Sex: $("#sex").val(),
            Province: $("#province").val(),
            City: $("#cities").val(),
            //SendToPerson: [$("#userid").val()],
            SendToPerson: $(".multiple-select-yulan").select2("val"),
            NewsTitle: window.image_original_url || '图片',
            ImageContent: _imgsrc
        }];
        return _imgMain;
    };
    //json4 video
    var videoJson = function videoJson(sendRequest, postType, newSrc) {
        //var _groupArr = groupOrPeople(); //返回people和group的数组
        var _realFileName = $('#image4-src').data('realFileName');
        var _videoMain = [{
            AppId: _AppIdUrl,
            NewsCate: 'video',
            realFileName: _realFileName,
            MaterialId: $('#materialId').val(),
            isSecurityPost: Util.checkIsSecurityPost('isSecurityPost'),
            SecurityLevel: $('#SecurityLevel').val(),
            ScheduleSendTime: parseDate(),
            PostType: postType, //1 normal post 2: defer post
            //user filter info 
            Group: $("#group").val(),
            Sex: $("#sex").val(),
            Province: $("#province").val(),
            City: $("#cities").val(),
            //SendToPerson: [$("#userid").val()],
            SendToPerson: $(".multiple-select-yulan").select2("val"),
            NewsTitle: sendRequest.mvideotitle,
            NewsComment: sendRequest.mvideodesc,
            videoContent: sendRequest.mvideosrc,
            imageContent: newSrc
        }];
        return _videoMain;
    };
    //json2 text-img
    var textImgJson = function textImgJson(postType) {
        //var _groupArr = groupOrPeople(); //返回people和group的数组
        var _newsObj = [];
        var _oldObj = JSON.parse($('.obj_news').text());
        var _firstNews = JSON.parse($('.obj_firstItem').text());
        //var _feedBackBtnContent = $("#FeedBackBtnContent").val();
        //if ($.trim(_feedBackBtnContent.length) == 0) {
        //    _feedBackBtnContent = "Add Comment";
        //}
        var _newFirst = {

            AppId: _AppIdUrl,
            NewsCate: 'news', //msgtype 图文消息
            NewsTitle: _firstNews.msgtitle,
            MaterialId: $('#materialId').val(),
            isSecurityPost: Util.checkIsSecurityPost('isSecurityPost'),
            SecurityLevel: $('#SecurityLevel').val(),
            ScheduleSendTime: parseDate(),
            PostType: postType, //1 normal post 2: defer post
            //user filter info 
            Group: $("#group").val(),
            Sex: $("#sex").val(),
            Province: $("#province").val(),
            City: $("#cities").val(),
            //SendToPerson: [$("#userid").val()],
            SendToPerson: $(".multiple-select-yulan").select2("val"),
            ImageContent: _firstNews.msgfileImg,
            NewsComment: _firstNews.msgtips,
            NewsContent: _firstNews.msgmaintext,
            FeedBackBtnContent: _firstNews.btntext,
            IsUseComment: _firstNews.IsUseComment
        };
        _newsObj.push(_newFirst);
        $.each(_oldObj, function (idx, obj) {
            var _curr = {
                AppId: _AppIdUrl,
                NewsCate: 'news', //msgtype 图文消息
                MaterialId: $('#materialId').val(),
                isSecurityPost: Util.checkIsSecurityPost('isSecurityPost'),
                SecurityLevel: $('#SecurityLevel').val(),
                ScheduleSendTime: parseDate(),
                PostType: postType, //1 normal post 2: defer post
                //user filter info 
                Group: $("#group").val(),
                Sex: $("#sex").val(),
                Province: $("#province").val(),
                City: $("#cities").val(),
                //SendToPerson: [$("#userid").val()],
                SendToPerson: $(".multiple-select-yulan").select2("val"),
                NewsTitle: obj.msgtitle,
                ImageContent: obj.msgfileImg,
                NewsComment: obj.msgtips,
                NewsContent: obj.msgmaintext,
                FeedBackBtnContent: obj.btntext, //$("#FeedBackBtnContent").val()
                IsUseComment: obj.IsUseComment
            };
            _newsObj.push(_curr);
        });
        return _newsObj;
    };
    var SoundIJson = function SoundIJson(postType) {
        //var _groupArr = groupOrPeople(); //返回people和group的数组
        var _soundSrc = $('#image5-src').val();
        var _realFileName = $('#image5-src').data('realFileName');
        var _newFirst = [{
            AppId: _AppIdUrl,
            NewsCate: 'voice', //msgtype 图文消息
            MaterialId: $('#materialId').val(),
            isSecurityPost: Util.checkIsSecurityPost('isSecurityPost'),
            SecurityLevel: $('#SecurityLevel').val(),
            ScheduleSendTime: parseDate(),
            PostType: postType, //1 normal post 2: defer post
            soundSrc: _soundSrc,
            realFileName: _realFileName,
            //user filter info 
            Group: $("#group").val(),
            Sex: $("#sex").val(),
            Province: $("#province").val(),
            City: $("#cities").val(),
            //SendToPerson: [$("#userid").val()]
            SendToPerson: $(".multiple-select-yulan").select2("val"),

        }];

        return _newFirst;
    };

    var fileIJson = function fileIJson(postType) {
        //var _groupArr = groupOrPeople(); //返回people和group的数组
        var _fileSrc = $('#image6-src').val();
        var _realFileName = $('#image6-src').data('realFileName');
        var _size = $('#image6-src').data('size');

        var _newFirst = [{
            AppId: _AppIdUrl,
            NewsCate: 'file', //msgtype 图文消息
            fileSrc: _fileSrc,
            MaterialId: $('#materialId').val(),
            isSecurityPost: Util.checkIsSecurityPost('isSecurityPost'),
            SecurityLevel: $('#SecurityLevel').val(),
            ScheduleSendTime: parseDate(),
            PostType: postType, //1 normal post 2: defer post
            size: _size,
            realFileName: _realFileName,
            //user filter info 
            Group: $("#group").val(),
            Sex: $("#sex").val(),
            Province: $("#province").val(),
            City: $("#cities").val(),
            //SendToPerson: [$("#userid").val()]
            SendToPerson: $(".multiple-select-yulan").select2("val"),
        }];

        return _newFirst;
    };

    /*** msg send partial ***/
    //submitfun
    function subyulan(isyulan, defer) {
        var postType = 1;
        if (defer) {
            postType = 2;
            if ($('#pickDate').val() == '') {
                layer.msg('请选择时间!');
                return;
            }
        }
        var msgtype = $('#hiddenType').val();
        //user filter info 
        var test = {
            Group: $("#group").val(),
            Sex: $("#sex").val(),
            Province: $("#province").val(),
            City: $("#cities").val(),
        }

        var url = isyulan == true ? 'WechatServiceSendMessage?isPreview=true' : 'WechatServiceSendMessage';

        switch (msgtype) {
            case "1":
                //发消息
                var _text = msgText_V(isyulan); //validate
                if (_text) {
                    $("#submitBtn").text('发送中...');
                    var _textJson = textJson(_text, postType); //json1
                    $.ajax({
                        url: url,
                        type: "post",
                        data: JSON.stringify(_textJson),
                        contentType: "application/json",
                        success: function success(data) {
                            if (data.results.Data == "200") {
                                layer.msg("发送成功");
                                //reset form
                                if (!isyulan) {
                                    $('#mainGroup').val("");
                                    $('.Showspan').html('');
                                    $('#saytext').val("");
                                    $('#for-msgType-text').text("");
                                    $('#count').html('您还可以输入600字');
                                    $('#Timed').modal('hide');
                                } //非预览发送成功reset当前新闻
                                else {
                                    $(".multiple-select-yulan").select2("val", null)
                                }

                            } else {
                                layer.msg("发送失败");
                            }
                            isfirst = true;
                        },
                        complete: function complete(data) {
                            $('#submitBtn').text('推送');
                        }
                    });
                } else {
                    isfirst = true;
                }
                break;
            case "2":
                //发图文
                var _valid = msgPicText_V(isyulan);
                if (_valid) {
                    $("#submitBtn").text('发送中...');
                    var _News = textImgJson(postType);
                    $.ajax({
                        url: url,
                        type: "post",
                        data: JSON.stringify(_News, postType),
                        contentType: "application/json",
                        success: function success(data) {
                            if (data.results.Data == "200") {
                                layer.msg("发送成功");
                                if (!isyulan) {
                                    $('#mainGroup').val("");
                                    $('.Showspan').html('');
                                    $('.go-ng-reset').click();
                                    $('#Timed').modal('hide');
                                } //非预览发送成功reset当前新闻
                                else {
                                    $(".multiple-select-yulan").select2("val", null)
                                }
                            } else {
                                layer.msg("发送失败");
                            }
                            isfirst = true;
                        },
                        complete: function complete(data) {
                            $('#submitBtn').text('推送');
                        }
                    });
                } else {
                    isfirst = true;
                }
                break;
            case "3":
                //发图
                var _msgImg = msgPic_V(isyulan);
                if (_msgImg) {
                    $("#submitBtn").text('发送中...');
                    var _imgJson = imgJson(_msgImg, postType); //json1
                    $.ajax({
                        url: url,
                        type: "post",
                        data: JSON.stringify(_imgJson),
                        contentType: "application/json",
                        success: function success(data) {
                            if (data.results.Data == "200") {
                                layer.msg("发送成功");
                                if (!isyulan) {
                                    $('#mainGroup').val("");
                                    $('.Showspan').html('');
                                    $('.go-ng-resetImg').click();
                                    $('#Timed').modal('hide');
                                }
                                else {
                                    $(".multiple-select-yulan").select2("val", null)
                                }
                            } else {
                                layer.msg("发送失败");
                            }
                            isfirst = true;
                        },
                        complete: function complete(data) {
                            $('#submitBtn').text('推送');
                        }
                    });
                } else {
                    isfirst = true;
                }
                break;
            case "4":
                //发视频
                var _msgVideo = msgVideo_V(isyulan);
                var photpSrc = null;
                if ($('#materialId').val() == "null") {
                    var theVideo = $('.default-img-box video');
                    var canvas = document.createElement("canvas");
                    var ctx = canvas.getContext('2d').drawImage(theVideo.get(0), 0, 0);
                    var photpSrc = canvas.toDataURL();
                }
                if (_msgVideo) {
                    $("#submitBtn").text('发送中...').after('<span id="submitBtnAfter">视频发送过程较慢，请耐心等待</span>');
                    var _videoJson = videoJson(_msgVideo, postType, photpSrc);
                    $.ajax({
                        url: url,
                        type: "post",
                        data: JSON.stringify(_videoJson),
                        contentType: "application/json",
                        success: function success(data) {
                            if (data.results.Data == "200") {
                                layer.msg("发送成功");
                                if (!isyulan) {
                                    $('#mainGroup').val("");
                                    $('.Showspan').html('');
                                    $('.go-ng-resetVideo').click();
                                    $('#Timed').modal('hide');
                                }
                                else {
                                    $(".multiple-select-yulan").select2("val", null)
                                }
                            } else if (data.results.Data == "500") {
                                layer.msg("发送失败");
                            } else {
                                layer.msg(data.results.Data, { time: 5000 });
                            }
                            isfirst = true;
                        },
                        complete: function complete(data) {
                            $('#submitBtn').text('推送');
                            $('#submitBtnAfter').remove();
                        }
                    });
                } else {
                    isfirst = true;
                }
                break;
            case "5":
                //发语音
                var _valid = msgSound_I(isyulan);
                if (_valid) {
                    $("#submitBtn").text('发送中...');
                    var _Sound = SoundIJson(postType);
                    $.ajax({
                        url: url,
                        type: "post",
                        data: JSON.stringify(_Sound),
                        contentType: "application/json",
                        success: function success(data) {
                            if (data.results.Data == "200") {
                                layer.msg("发送成功");
                                if (!isyulan) {
                                    $('#mainGroup').val("");
                                    $('#Timed').modal('hide');

                                    // $('#uploader5').find('#thelist').remove();
                                } //非预览发送成功reset当前新闻
                                else {
                                    $(".multiple-select-yulan").select2("val", null)
                                }
                            } else {
                                layer.msg("发送失败");
                            }
                            isfirst = true;
                        },
                        complete: function complete(data) {
                            $('#submitBtn').text('推送');
                        }
                    });
                } else {
                    isfirst = true;
                }
                break;
            default:
        }
    }

    //发送
    var isfirst = true; //第一次点击发送
    $('#submitBtn').click(function () {
        isfirst = false;
        subyulan(false);
        if (isfirst) {

        }
    });
    //预览fun
    $('#yulanJustLook1').click(function () {
        if (isfirst) {
            isfirst = false;
            subyulan(true);
            setTimeout(function () { isfirst = true; }, 2000)
        }
    });
    //延时推送
    $('#deferSubmitBtn').on('click', function () {
        $('#Timed').modal();
    })
    $('#deferPostSubmit').on('click', function () {
        subyulan(false, true);
    });
    $.fn.datepicker.dates['cn'] = {
        days: ["周日", "周一", "周二", "周三", "周四", "周五", "周六", "周日"],
        daysShort: ["日", "一", "二", "三", "四", "五", "六", "七"],
        daysMin: ["日", "一", "二", "三", "四", "五", "六", "七"],
        months: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
        monthsShort: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
        today: "今天",
        clear: "清除"
    };
    var dateEnd = new Date();
    dateEnd.setDate(dateEnd.getDate() + 7);
    $('#pickDate').datepicker({
        language: "cn",
        startDate: new Date(),
        endDate: dateEnd,
        autoclose: true, //选中之后自动隐藏日期选择框
        todayBtn: 'linked', //今日按钮
        format: "yyyy-mm-dd" //日期格式
    });
    $('#msgType').on('click', 'li', function () {
        if ($(this).find('a').attr('href') === '#msgType-wordpic') {
            $('.visible-range').show();
        } else {
            $('.visible-range').hide();
        }
    })

});

//表情按钮
$(function () {
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
    $(".em_nr a").click(function () {
        var a = $("#saytext").val();
        var data_image = $(this).attr("data-image");
        $("#saytext").val(a + "[" + this.title + "]");
        $("#for-msgType-text").append("<img src='/images/QQexpression/" + data_image + ".gif'>");
        if ($("#for-msgType-text").html() != "") {
            $("#for-msgType-text").css("padding", "2px 8px");
        } else {
            $("#for-msgType-text").css("padding", "0px");
        }
    });

    $('#saytext').bind('input propertychange', function () {
        var str = $("#saytext").val();
        $("#for-msgType-text").html(replace_img(str));
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

    var emoji_cn = ['[微笑]', '[撇嘴]', '[色]', '[发呆]', '[得意]', '[流泪]', '[害羞]', '[闭嘴]', '[睡]', '[大哭]', '[尴尬]', '[发怒]', '[调皮]', '[呲牙]', '[惊讶]', '[难过]', '[酷]', '[冷汗]', '[折磨]', '[吐]', '[偷笑]', '[愉快]', '[白眼]', '[傲慢]', '[饥饿]', '[困]', '[惊恐]', '[流汗]', '[憨笑]', '[悠闲]', '[奋斗]', '[咒骂]', '[疑问]', '[嘘]', '[晕]', '[抓狂]', '[衰]', '[骷髅]', '[敲打]', '[再见]', '[擦汗]', '[抠鼻]', '[鼓掌]', '[溴大了]', '[坏笑]', '[左哼哼]', '[右哼哼]', '[哈欠]', '[鄙视]', '[委屈]', '[快哭了]', '[阴险]', '[亲亲]', '[吓]', '[可怜]', '[菜刀]', '[西瓜]', '[啤酒]', '[篮球]', '[乒乓]', '[咖啡]', '[饭]', '[猪头]', '[玫瑰]', '[凋谢]', '[嘴唇]', '[爱心]', '[心碎]', '[蛋糕]', '[闪电]', '[炸弹]', '[刀]', '[足球]', '[瓢虫]', '[便便]', '[月亮]', '[太阳]', '[礼物]', '[拥抱]', '[强]', '[弱]', '[握手]', '[胜利]', '[抱拳]', '[勾引]', '[拳头]', '[差劲]', '[爱你]', '[NO]', '[YES]', '[爱情]', '[飞吻]', '[跳跳]', '[发抖]', '[怄火]', '[转圈]', '[磕头]', '[回头]', '[跳绳]', '[投降]', '[激动]', '[乱舞]', '[献吻]', '[左太极]', '[右太极]'];
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
        console.log(1);
        var str = $("#saytext").val();
        $("#for-msgType-text").html(replace_img(str));
    });

    //ordermanager------------------
    $('.main-preview').on('mouseover', '.repeat-item', function () {
        $(this).find(".order").css("display", "block");
        console.log($(this).next('.repeat-item').length);
        if ($(this).next('.repeat-item').length == 0) {
            $(this).find('.fa-angle-down').hide();
        } else {
            $(this).find('.fa-angle-down').show();
        }
    });
    $('.main-preview').on('mouseout', '.repeat-item', function () {
        $(this).find(".order").css("display", "none");
    });

    $('.main-preview').on('mouseover', '#fi-item', function () {
        $(this).find(".order").css("display", "block");
    });
    $('.main-preview').on('mouseout', '#fi-item', function () {
        $(this).find(".order").css("display", "none");
    });


    //upload------------


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
            case '.uploader_news_cover':
                return $(ele).fileupload({
                    dataType: 'json',
                    url: '/upload/uploadfile?type=image&appid=' + appId + '&isNewsCover=true',
                    type: 'post',

                    add: function add(e, data) {
                        var that = this;
                        var validateImage = ['jpeg', 'png', 'jpg'];
                        if (validateImage.indexOf(validate(data).toLowerCase()) == -1) {
                            layer.msg("不支持的格式!");
                            return false;
                        }
                        if (parseInt(data.originalFiles[0].size) > 1 * 1024 * 1024 * 2) {
                            layer.msg("请上传少于2M以下的图片");
                            return false;
                        }
                        var ajax = data.submit();
                        $(this).parent().parent().find('.progress1 .bar').css('display', 'block');
                        ajax.success(function (res) {
                            console.log(data);
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
            case '.uploader_image':
                return $(ele).fileupload({
                    dataType: 'json',
                    url: '/upload/uploadfile?type=image&appid=' + appId,
                    type: 'post',

                    add: function add(e, data) {
                        var that = this;
                        var validateImage = ['jpeg', 'png', 'jpg'];
                        if (validateImage.indexOf(validate(data).toLowerCase()) == -1) {
                            layer.msg("不支持的格式!");
                            return false;
                        }
                        if (parseInt(data.originalFiles[0].size) > 1 * 1024 * 1024 * 2) {
                            layer.msg("请上传少于2M以下的图片");
                            return false;
                        }
                        var ajax = data.submit();
                        $(this).parent().parent().find('.progress1 .bar').css('display', 'block');
                        ajax.success(function (res) {
                            console.log(data);
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

                        var ext = data.originalFiles[0].name.split('.')[1];
                        if (validateFile.indexOf(validate(data).toLowerCase()) == -1) {
                            layer.msg("不支持文件格式!");
                            return false;
                        }
                        if (parseInt(data.originalFiles[0].size) > 1 * 1024 * 1024 * 20) {
                            layer.msg("请上传少于20M以下的文件");
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
                            inputHidden.data('realFileName', res.realFileName);
                            inputHidden.data('size', size);
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
                        console.log(validate(data).toLowerCase());
                        if (validateVideo.indexOf(validate(data).toLowerCase()) == -1) {
                            layer.msg("不支持视频格式!");
                            return false;
                        }
                        if (parseInt(data.originalFiles[0].size) > 1 * 1024 * 1024 * 10) {
                            layer.msg("请上传少于10M以下的视频");
                            return false;
                        }
                        var that = this;
                        //data.context = $('<p/>').text('Uploading...').appendTo($(this).parent());
                        var ajax = data.submit();
                        $(this).parent().parent().find('.progress1 .bar').css('display', 'block');
                        ajax.success(function (res) {

                            var inputHidden = $(that).parent().siblings('#image4-src');
                            inputHidden.val(res.targetFilePath + '\\' + res.serverFileName);
                            inputHidden.data('realFileName', res.realFileName);
                            inputHidden.change();
                            var father = $(that).parent().parent().find('.preview');
                            if (father.length === 0) {
                                $(that).parent().parent().append('<div class="preview" style="margin-top:10px;"><div>' + res.realFileName + '</div><div">上传成功</div></div>');
                            } else {

                                father.html('<div>' + res.serverFileName + '"</div><div>上传成功</div>');
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
                        if (validateVideo.indexOf(validate(data).toLowerCase()) == -1) {
                            layer.msg("不支持音频格式!");
                            return false;
                        }
                        if (parseInt(data.originalFiles[0].size) > 1 * 1024 * 1024 * 2) {
                            layer.msg("请上传少于2M以下的语音");
                            return false;
                        }
                        var that = this;
                        //data.context = $('<p/>').text('Uploading...').appendTo($(this).parent());
                        var ajax = data.submit();
                        $(this).parent().parent().find('.progress1 .bar').css('display', 'block');
                        ajax.success(function (res) {
                            if (+res.duration > 60) {
                                $.get('Delete?id=' + res.Id, function () {
                                    layer.msg("音频不允许超过60秒!");
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
    uploadFunc('.uploader_news_cover');
    uploadFunc('.uploader_image');
    uploadFunc('.uploader_video');
    uploadFunc('.uploader_file');
    uploadFunc('.uploader_audio');
});

//# sourceMappingURL=app-compiled.js.map