$(document).ready(function () {

    //-------------------------------------变量区域 -------------------------------------//
    var appId = $('#hiddenAppId').val();
    var isCorp = $('#hiddenIsCorp').val();
    //var autoReplyId = $('#hiddenId').val();
    //var primaryType = $('#hiddenPrimaryType').val();
    var enumTextVal = $('#hiddenEnumTextVal').val();
    var enumMenuVal = $('#hiddenEnumMenuVal').val();
    var enumScanVal = $('#hiddenEnumScanVal').val();
    var enumCropSubscribeVal = $('#enumCropSubscribeVal').val();
    var enumFocusWithScanVal = $('#hiddenEnumFocusWithScanVal').val();
    var enumFocusVal = $('#hiddenEnumFocusVal').val();
    var eumNormalMenuVal = $('#hiddenEnumNormalMenuVal').val();
    var eumReplyLinkVal = $('#hiddenEnumReplyLinkVal').val();
    var eumReplyTextVal = $('#hiddenEnumReplyTextVal').val();
    var eumReplyArticleVal = $('#hiddenEnumReplyArticleVal').val();
    var enumReplyNewsLatestVal = $('#hiddenEnumReplyNewsLatestVal').val();
    var enumReplyNewsManualVal = $('#hiddenEnumReplyNewsManualVal').val();
    var jsonModel = JSON.parse($('#hiddenJsonModel').val());

    //-------------------------------------函数区域Start -------------------------------------//

    // 添加文本匹配规则
    var addMatchRule = function () {
        var divTxt = $('#divTxt');
        var lastItem = divTxt.children('div:last-child');
        lastItem.clone().appendTo(divTxt);
        lastItem = divTxt.children('div:last-child');

        // 初始化
        lastItem.find('label').html('');
        var newMatchSelect = lastItem.find('select');
        newMatchSelect.val(newMatchSelect.find('option:first').val());
        lastItem.find('input').val('');

        // 显示删除按钮并绑定事件
        lastItem.find('a:last-child').show().on('click', function () {
            removeMatchRule(this);
        });

        return lastItem;
    };

    // 移除文本匹配规则
    var removeMatchRule = function (obj) {
        var myRow = $(obj).parents('div.row');
        myRow.remove();
    };

    // 当口令类型是菜单时的初始化
    var initMenuKeyword = function (obj) {
        var type = obj.val();
        if (type == eumNormalMenuVal) {
            $('#divMenuKeyword').show();
        } else {
            $('#divMenuKeyword').hide();
        }
    };

    // 添加回复
    var addReplyContent = function () {
        var lastItem = $('#replyContentArea').find('div.reply-area:last');
        var newItem = lastItem.clone().insertAfter(lastItem);

        // 初始化数据
        var newReplySelect = newItem.find('select[name=replyType]');
        newReplySelect.val(newReplySelect.find('option:first').val());
        newItem.find('textarea').val('');
        newItem.find('input[type=text]').val('');
        newItem.find('input[name=isTextEncrypt]').attr('checked', false);
        triggerReplyTypeChange(newReplySelect);

        // 绑定回复类型变更事件
        newReplySelect.on('change', function () {
            triggerReplyTypeChange($(this));
        });

        // 事件绑定：图文消息Modal
        newItem.find('.divReplyArticle a').on('click', function () {
            triggerNewsModal($(this));
        });

        // 事件绑定：文件Modal
        newItem.find('.divReplyFile a').on('click', function () {
            triggerFileModal($(this));
        });

        // 事件绑定：图文回复类型改变
        newItem.find('select[name=replyNewsType]').on('change', function () {
            triggerReplyNewsTypeChange($(this));
        });

        // 事件绑定：是否加密的checkbox点击
        newItem.find('input[name=isTextEncrypt]').on('change', function () {
            var checked = $(this).prop('checked');
            if (checked) {
                $(this).prev().val('1');
            } else {
                $(this).prev().val('0');
            }
        });

        // 显示删除按钮，第一条回复无删除按钮，以保证至少有一条回复
        newItem.find('a.btn-del-content').show().on('click', function () {
            removeReplyContent(this);
        });
    };

    // 删除回复
    var removeReplyContent = function (obj) {
        var myRow = $(obj).parents('div.reply-area');
        myRow.remove();
    };

    // 触发回复类型变更
    var triggerReplyTypeChange = function (obj) {
        var type = obj.val();
        var divText = obj.parents('.row:first').next();
        var divArticle = divText.next();
        var divOther = divArticle.next();

        if (type == eumReplyTextVal || type == eumReplyLinkVal) {
            divText.show();
            divArticle.hide();
            divOther.hide();
        } else if (type == eumReplyArticleVal) {
            divText.hide();
            divArticle.show();
            divOther.hide();
        } else {
            divText.hide();
            divArticle.hide();
            divOther.show();
        }
    };

    // 触发回复图文类型变更
    var triggerReplyNewsTypeChange = function (obj) {
        var type = obj.val();
        var area1 = obj.parents('.row:first').find('.divNewsCount');
        var area2 = area1.next();

        if (type == enumReplyNewsLatestVal) {
            area1.show();
            area2.hide();
        } else if (type == enumReplyNewsManualVal) {
            area1.hide();
            area2.show();
        }
    };

    // 编辑模式下：初始化口令类型区域
    var initKeywordTypeZone = function () {
        // 初始化口令类型区域
        // 文本类型时
        if (jsonModel.PrimaryType == enumTextVal) {
            $('#divTxt').show();
            $('#divMenu').hide();
            $('#divScan').hide();
            // 菜单类型时
        } else if (jsonModel.PrimaryType == enumMenuVal) {
            $('#divTxt').hide();
            $('#divScan').hide();
            $('#divMenu').show();
            // 其它类型
        } else if (jsonModel.PrimaryType == enumScanVal || jsonModel.PrimaryType == enumFocusWithScanVal) {
            $('#divScan').show();
            $('#divTxt').hide();
            $('#divMenu').hide();
        } else if (jsonModel.PrimaryType == enumFocusVal) {
            $('#divTxt').hide();
            $('#divMenu').hide();
            $('#divScan').hide();
        } else {
            $('#divTxt').hide();
            $('#divMenu').hide();
            $('#divScan').hide();
        }

        initMenuKeyword($('#menuTypes'));

        // 显示第一个之外的匹配条件
        $('#divTxt').find('a.btn-default').not(':first').show();
    };

    // 编辑模式下：初始化口令回复区域
    var initReplyZone = function () {
        var contents = jsonModel.Contents;
        var maxIndex = contents.length - 1;
        $.each(contents, function (index, obj) {
            var primaryType = obj.PrimaryType;
            var secondaryType = obj.SecondaryType;
            var isEncrypt = obj.IsEncrypt;
            var content = obj.Content;
            var newsIds = obj.NewsID;
            var fileIds = obj.FileID;
            var lastItem = $('#replyContentArea').children('div.reply-area:last');

            // 设置回复类型
            var replyTypeSelect = lastItem.find('select[name=replyType]');
            replyTypeSelect.val(primaryType);

            var replyTypeOptions = replyTypeSelect.children();
            var i;
            // 设置选中状态
            for (i = 0; i < replyTypeOptions.length; i++) {
                if (replyTypeOptions[i].value == primaryType) {
                    $(replyTypeOptions[i]).attr('selected', 'selected');
                    break;
                }
            }

            // 设置可见
            triggerReplyTypeChange(replyTypeSelect);

            // 回复文本时
            if (primaryType == eumReplyTextVal || primaryType == eumReplyLinkVal) {
                lastItem.find('textarea[name=textReply]').val(content);

                // 设置加密
                if (isEncrypt) {
                    lastItem.find('input[name=isTextEncrypt]').attr('checked', 'checked');
                    lastItem.find('input[name=isTextEncryptReplace]').val('1');
                }

                // 回复图文时
            } else if (primaryType == eumReplyArticleVal) {
                var replyNewsTypeSelect = lastItem.find('select[name=replyNewsType]');
                replyNewsTypeSelect.val(secondaryType);
                var replyNewsTypeOptions = replyNewsTypeSelect.children();
                // 设置可见
                triggerReplyNewsTypeChange(replyNewsTypeSelect);
                // 设置选中状态
                for (i = 0; i < replyNewsTypeOptions.length; i++) {
                    if (replyNewsTypeOptions[i].value == secondaryType) {
                        $(replyNewsTypeOptions[i]).attr('selected', 'selected');
                        break;
                    }
                }

                // 最新消息数
                if (secondaryType == enumReplyNewsLatestVal) {
                    lastItem.find('input[name=replyNewsLatestCount]').val(content);
                    // 手动选择消息时，显示NewsID
                } else if (secondaryType == enumReplyNewsManualVal) {
                    lastItem.find('input[name=replyNewsList]').val(newsIds);
                }

                // 其它类型(图片，音频，视频，文件)
            } else {
                lastItem.find('input[name=textReplyFiles]').val(fileIds);
            }

            // 添加下一个回复
            if (maxIndex > index) {
                addReplyContent();
            }
        });
    };

    //-------------------------------------函数区域End -------------------------------------//


    //-------------------------------------初始化Start -------------------------------------//

    // 如果是编辑状态，需要初始化数据
    if (jsonModel.Id > 0) {
        // 初始化口令类型区域
        initKeywordTypeZone();

        // 初始化口令回复区域
        initReplyZone();

    }

    // 事件绑定：口令类型改变
    $('#main_keywordType').on('change', function () {
        var type = $(this).val();
        if (type == enumTextVal) {
            $('#divTxt').show();
            $('#divMenu').hide();
            $('#divCropSubscribe').hide();
            $('#divScan').hide();
        } else if (type == enumMenuVal) {
            $('#divTxt').hide();
            $('#divScan').hide();
            $('#divCropSubscribe').hide();
            $('#divMenu').show();
        } else if (type == enumScanVal || type == enumFocusWithScanVal) {
            $('#divTxt').hide();
            $('#divMenu').hide();
            $('#divCropSubscribe').hide();
            $('#divScan').show();
        } else if (type == enumFocusVal) {
            $('#divTxt').hide();
            $('#divMenu').hide();
            $('#divCropSubscribe').hide();
            $('#divScan').hide();
        } else if (type == enumCropSubscribeVal) {
            $('#divTxt').hide();
            $('#divMenu').hide();
            $('#divScan').hide();
            $('#divCropSubscribe').show();
        } else {
            $('#divTxt').hide();
            $('#divMenu').hide();
            $('#divScan').hide();
            $('#divCropSubscribe').hide();
        }
        if (Number(isCorp) == 1) {
            if (type == enumCropSubscribeVal) {
                $('.dropdown').show();
            } else {
                $('.dropdown').hide();
            }
        }
    });
    $('#main_keywordType').trigger('change');
    // 事件绑定：当口令类型是文本类型时，点击添加匹配规则按钮
    $('#divTxt').on('click', 'a.btn-success', addMatchRule);

    // 事件绑定：当口令类型是文本类型时，点击删除匹配规则按钮
    $('#divTxt a.btn-default').on('click', function () {
        removeMatchRule(this);
    });

    // 事件绑定：当口令类型为菜单类型，菜单子类型改变
    $('#menuTypes').on('change', function () {
        initMenuKeyword($(this));
    });

    // --------------------------------------回复相关初期化   --------------------------------------/

    // 事件绑定：添加回复
    $('#addReply').on('click', function () {
        addReplyContent();
    });

    // 事件绑定：删除回复
    $('#replyContentArea a.btn-del-content').on('click', function () {
        removeReplyContent(this);
    });

    // 事件绑定：回复类型改变
    $('select[name=replyType]').on('change', function () {
        triggerReplyTypeChange($(this));
    });

    // 事件绑定：图文回复类型改变
    $('select[name=replyNewsType]').on('change', function () {
        triggerReplyNewsTypeChange($(this));
    });

    // 事件绑定：是否加密的checkbox点击
    $('input[name=isTextEncrypt]').on('change', function () {
        var checked = $(this).prop('checked');
        if (checked) {
            $(this).prev().val('1');
        } else {
            $(this).prev().val('0');
        }
    });


    // --------------------------------------保存相关   --------------------------------------/
    // 保存（添加，编辑）成功后跳转到一览画面
    LEAP.Common.MainPop.options.fnAfterSuccess = function () {
        window.location.href = 'index?appid=' + appId;
    };

    // ----------------------Modal: 图文消息一览
    //$('.divReplyArticle a').on('click', function () {
    //    triggerNewsModal($(this));
    //});

    //var triggerNewsModal = function (obj) {
    //    $('#ModalArticleList').modal('show');
    //    //var url = "GetArticleList" + "?where=" + JSON.stringify({ Rules: [{ Field: 'AppId', value: appId }] });
    //    var url = "GetArticleList" + "?AppId=" + appId;
    //    var replyNewsList = obj.parents('div.row').find('input[name=replyNewsList]');

    //    // 只初始化一次
    //    if (LEAP.Common.MainPop.options.dataTable == null) {
    //        var table = $('#ModalArticleList .data-table');
    //        LEAP.Common.MainPop.options.dataTable = table.dataTable($.extend(true, datatableSetting, {
    //            "ajax": { "url": url },
    //            "aoColumns": [
    //                {
    //                    "mData": "checkbox",
    //                    "bSortable": false,
    //                    "sClass": "sTdCheckbox"
    //                },
    //                {
    //                    "mData": "Id",
    //                    "bSortable": false,
    //                    "sClass": "sTdCheckbox"
    //                },
    //                {
    //                    "mData": "ArticleTitle",
    //                    "bSearchable": false,
    //                    "bSortable": false
    //                },
    //                { "mData": "ArticleCateSub" },
    //                { "mData": "PublishDate" }
    //            ],
    //            "columnDefs": $.extend(true, datatableSetting.columnDefs, [
    //                {
    //                    "targets": 0,
    //                    "render": function (data, type, full, meta) {

    //                        return '<input type="checkbox" value="' + full.Id + '" data-title="' + full.ArticleTitle + '" id="checkbox" />';
    //                    }
    //                },
    //                {
    //                    "targets": 2,
    //                    "render": function (data, type, full, meta) {
    //                        return '<a href="' + '/News/ArticleInfo/wxdetail/' + full.Id + '" target="_blank"> ' + data + '</a>';
    //                    }
    //                }]),
    //            fnDrawCallback: function () {
    //                // 绑定checkbox事件
    //                table.find('input[type=checkbox]').on('change', function () {
    //                    var isChecked = $(this).prop('checked');
    //                    var id = $(this).val();
    //                    var orgin = replyNewsList.val();
    //                    var result = '';
    //                    if (isChecked) {
    //                        if (orgin.length > 0) {
    //                            orgin = orgin + ',';
    //                        }
    //                        result = orgin + id;
    //                    } else {
    //                        orgin = ',' + orgin;
    //                        result = orgin.replace(',' + id, '');
    //                        // 去掉第一个多余的逗号
    //                        result = result.substring(1);
    //                    }

    //                    replyNewsList.val(result);
    //                });

    //                $('[data-toggle="tooltip"]').tooltip({ trigger: 'hover' });
    //            }
    //        }));
    //    }
    //};

    // ----------------------Modal: 文件一览
    $('.divReplyFile a').on('click', function () {
        triggerFileModal($(this));
    });

    var triggerFileModal = function (obj) {
        debugger
        var replyType = obj.parents('div.reply-area').find('select[name=replyType]').val();
        var url = "GetFileList" + "?ReplyType=" + replyType;
        var replyFile = obj.parents('div.divReplyFile').find('input[name=textReplyFiles]');
        // 显示文件一览
        initJSView(replyType, replyFile);
    };

    var initJSView = function (replyType, replyFile) {
        var attachmentType;
        switch (replyType) {
            case "3":
                attachmentType = 'Image';
                break;
            case "4":
                attachmentType = 'Audio';
                break;
            case "5":
                attachmentType = 'Video';
                break;
            default:
                return;
        }
        var htmlStr = '<tr role="row" class="odd">'
                  + '<td class=" sTdCheckbox">{{: Id}}</td>'
                  + '<td>{{: AttachmentTitle}}</td>'
                  + '<td>{{: Type}}</td>'
                  + '<td>{{: AttachmentUrl}}</td>'
                  + '<td><a href="#" data-id="{{: Id}}" data-mediaId="{{: MediaId}}" class="btn btn-success btn-xs" data-toggle="tooltip" data-placement="top" title="Select"><i class="fa fa-check"></i></a></td>'
                  + '</tr>';

        $('#tbody_image').LEAPDataBind({
            renderId: "tbody_image",
            url: "/WeChatMain/FileManage/GetList",
            data: "type=" + attachmentType + "&appid=" + appId,
            pageSize: 5,
            pagerId: "page_image",
            isPage: true,
            renderHtml: htmlStr,
            renderSuccess: function () {
                debugger
                $('#ModalFileList').modal('show');
                // 绑定选中文件事件
                $('a.btn-success').on('click', function () {
                    var obj = $(this);
                    var id = obj.attr('data-id');
                    var mediaId = obj.attr('data-mediaId');
                    // 如果这个文件没上传到微信服务器，上传之
                    if (mediaId == '' || mediaId == 'null') {
                        $.ajax({
                            url: "/WeChatMain/AutoReply/UploadFileToWx?appId=" + appId + "&fileId=" + id + "&replyType=" + replyType,
                            success: function (data) {
                                // 如果没有error信息
                                if (data.Message == null) {
                                    mediaId = data.media_id;
                                    obj.attr('data-mediaId', mediaId);
                                    replyFile.val(id);
                                    $('#ModalFileList').modal('hide');
                                }
                            }
                        });
                    } else {
                        replyFile.val(id);
                        $('#ModalFileList').modal('hide');
                    }
                });
            }
        });
    };

});

//-------------------------------------初始化End -------------------------------------//