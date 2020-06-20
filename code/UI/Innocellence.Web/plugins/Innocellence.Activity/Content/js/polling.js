$(function () {
    var pollingView = new Object();
    var isInital = true;

    $.addTemplateFormatter({
        upperCaseFormatter: function (value, options) {
            return value;
        },
        lowerCaseFormatter: function (value, options) {
            return value.toLowerCase();
        },
        toLetterFormatter: function (value, options) {
            return turnLetter(value);
        }
    });

    //initial Datetime picker
    $('.form_datatime').datetimepicker({
        format: 'yyyy-mm-dd',
        weekStart: 1,
        autoclose: true,
        startView: 2,
        minView: 2,
        startDate: new Date().toLocaleDateString(),
        todayBtn: 'linked',
        clearBtn: true,
        language: 'zh-CN',
    });

    //initial time picker
    $('#StartTime').timepicker({
        maxHours: 24,
        minuteStep: 1,
        snapToStep: true,
        showSeconds: false,
        defaultTime: '00:00',
        showMeridian: false
    });

    $('#EndTime').timepicker({
        maxHours: 24,
        minuteStep: 1,
        showSeconds: false,
        snapToStep: true,
        defaultTime: '23:59',
        showMeridian: false
    });
    //验证日期
    function checkdate(start, end) {
        if (start == null || start.length == 0) {
            artDialog.alert("请填写开始时间");
            return false;
        }
        if (end == null || end.length == 0) {
            artDialog.alert("请填写结束时间");
            return false;
        }
        var startime = new Date(start + " " + $('#StartTime').val());
        var endtime = new Date(end + " " + $('#EndTime').val());
        if (startime.getTime() >= endtime.getTime()) {
            artDialog.alert("结束时间不能早于开始时间。");
            return false;
        }
        return true;
    }
    //clear time when create
    if ($('#ID').val() < 1) {
        //init template when create
        addQuestion(true);
        $('#StartDateTime').val('');
        $('#EndDateTime').val('');
    }

    //init collapse
    if (isInital) {
        var target = $('a.collapsed:eq(0)');
        var headingNode = target.parents('.panel-heading');
        target.trigger('click');
        headingNode.css("backgroundColor", "#f5f5f5");
        target.text("收起");

        var type = $('#Type').val();
        if (type == 1) {
            $('.award-score').removeClass('hidden');
            $('.award-message').removeClass('hidden');
        } else {
            $('.award-score').addClass('hidden');
            $('.award-message').addClass('hidden');
        }

        isInital = false;
    }

    var validator = $('#polling_form').validate({
        errorPlacement: function (error, element) {
            if (element.parent().hasClass("input-daterange")) {
                error.appendTo(element.parent().next('.errorMsg'));
            } else {
                error.appendTo(element.parent());
            }
        }
    });

    //点编辑切换背景色
    $('.panel-group').on('show.bs.collapse', '.panel-collapse', function (e) {
        var headingNode = $(e.target).prev('.panel-heading');
        var questionSpan = headingNode.find('.polling-question');
        headingNode.css("backgroundColor", "#f5f5f5");
        headingNode.find('a[data-toggle="collapse"]').text("收起");
        if (questionSpan.text() != "") {
            questionSpan.text("");
        }
    }).on('hide.bs.collapse', '.panel-collapse', function (e) {
        var headingNode = $(e.target).prev('.panel-heading');
        var qTitle = $(e.target).find('input[name^="Title"]').val();
        headingNode.css("backgroundColor", "#fff");
        headingNode.find('a[data-toggle="collapse"]').text("编辑");
        if (qTitle != "") {
            headingNode.find('.polling-question').text(qTitle);
        }
    });

    //添加和删除选项
    $('.panel-group').on('click', 'a.js-add-item', function (e) {
        addOption(e.target);
        //initLimitInput();
    }).on('click', 'a.js-delete-item', function (e) {
        delOption(e.target);
    }).on('click', 'a.js-polling-upload-btn', function (e) {
        $(e.target).next('input[name=file]').trigger('click');
    }).on('click', 'a.link-dele', function (e) {
        var imgParentNode = $(e.target).parents('.img-container');
        imgParentNode.find('img.preview').attr("src", "");
        imgParentNode.hide();
        $(e.target).parents('.js-polling-option').find('.js-polling-upload-btn').text("上传图片");
    }).on('click', 'a.js-remove-qusetion', function (e) {
        delQuestion(e.target);
    }).on('click', 'a.js-copy-qusetion', function (e) {
        copyQuestion(e.target);
    }).on('click', '.js-polling-radio', function (e) {
        var _targetVal = $(e.target).val();
        var parentNode = $(e.target).parents('.panel-body');
        var mNode = parentNode.find('.js-multiple-control');
        var opNode = parentNode.find('.js-polling-option');
        var addNode = parentNode.find('.polling_meta_detail');
        var qTypeNode = parentNode.find('#qType');
        var opAddNode = parentNode.find('.js-add-item');
        var qScoreNode = parentNode.find('.question-score');
        var qAnswersNode = parentNode.find('.question-answers');
        if (_targetVal == "99") {
            qTypeNode.val(_targetVal);
            //隐藏选项相关
            mNode.addClass('hidden'); opNode.addClass('hidden');
            opNode.find('input[type="text"]').attr('disabled', 'disabled');
            addNode.addClass('hidden');
            qScoreNode.hide(); qAnswersNode.hide();
        } else if (_targetVal >= "2") {
            //编辑页默认添加3个Option
            if (opNode.length == 0) {
                var addOp = addOption(opAddNode);
                addOp.done(function () {
                    addOption(opAddNode).done(function () {
                        addOption(opAddNode);
                    });
                });
            }
            qTypeNode.val(mNode.find('.js-multiple-num').val());
            mNode.removeClass('hidden');
            opNode.removeClass('hidden');
            opNode.find('input[type="text"]').attr('disabled', false);
            addNode.removeClass('hidden');
            qScoreNode.hide(); qAnswersNode.hide();
        } else {
            //编辑页默认添加3个Option
            if (opNode.length == 0) {
                var addOp = addOption(opAddNode);
                addOp.done(function () {
                    addOption(opAddNode).done(function () {
                        addOption(opAddNode);
                    });
                });
            }
            qTypeNode.val(_targetVal);
            mNode.addClass('hidden');
            opNode.removeClass('hidden');
            opNode.find('input[type="text"]').attr('disabled', false);
            addNode.removeClass('hidden');
            if ($('#Type').val() == 1) { qScoreNode.show(); qAnswersNode.show(); }
        }
    }).on('change', '.js-multiple-num', function (e) {
        $(e.target).parents('.polling-meta-radio').find('#qType').val($(e.target).val());
    }).on('click', '.js-option-type', function (e) {
        var parentNode = $(e.target).parents('.js-option-type');
        var inputNode = parentNode.find('input[name="isAddInput"]');
        if (inputNode.is(":checked")) {
            inputNode.val("1");
            inputNode.prop("checked", "checked");
            parentNode.next('.js-option-input-required').removeClass('hidden');
        } else {
            inputNode.val("0");
            inputNode.prop("checked", false);
            parentNode.next('.js-option-input-required').addClass('hidden');
        }
    }).on('click', '.js-question-required', function (e) {
        var parentNode = $(e.target).parents('.js-question-required');
        var inputNode = parentNode.find('input[name="isRequired"]');
        if (inputNode.is(":checked")) {
            inputNode.val("0");
            inputNode.prop("checked", "checked");
        } else {
            inputNode.val("1");
            inputNode.prop("checked", false);
        }
    }).on('click', '.js-option-input-required', function (e) {
        var parentNode = $(e.target).parents('.js-option-input-required');
        var inputNode = parentNode.find('input[name="isRequired"]');
        if (inputNode.is(":checked")) {
            inputNode.val("1");
            inputNode.prop("checked", "checked");
        } else {
            inputNode.val("0");
            inputNode.prop("checked", false);
        }
    });

    //切换有奖问答--显示新字段
    $('#Type').on('change', function () {
        var typeNum = $(this).val();
        if (typeNum == 1) {
            $('.award-score').removeClass('hidden');
            $('.award-message').removeClass('hidden');
            $('.question-score').show();
            $('.question-answers').show();
            var len = $('.js-polling-radio:checked').length;
            for (var i = 0; i < len; i++) {
                var currentNode = $('.js-polling-radio:checked').eq(i);
                if (currentNode.val() == "99" || currentNode.val() >= "2") {
                    currentNode.parents('.panel-body').find('.question-score').hide();
                    currentNode.parents('.panel-body').find('.question-answers').hide();
                }
            }
        } else {
            $('.award-score').addClass('hidden');
            $('.award-message').addClass('hidden');
            $('.question-score').hide();
            $('.question-answers').hide();
        }
    });

    //切换为有奖问答--由本checkbox控制回复文字的显示与否
    $('#isReply').on('click', function () {
        if ($(this).is(':checked')) {
            $('.award-reply').removeClass('hidden');
        } else {
            $('#ReplyMessage').val("");//清空回复文字
            $('.award-reply').addClass('hidden');
        }
    });

    //添加和删除问题
    $('#js_add_question').on('click', function (e) {
        addQuestion();
        //如果为有奖问答则显示新字段
        if ($('#Type').val() == "1") {
            $('.question-score').show();
            $('.question-answers').show();
            var len = $('.js-polling-radio:checked').length;
            for (var i = 0; i < len; i++) {
                var currentNode = $('.js-polling-radio:checked').eq(i);
                if (currentNode.val() == "99" || currentNode.val() >= "2") {
                    currentNode.parents('.panel-body').find('.question-score').hide();
                    currentNode.parents('.panel-body').find('.question-answers').hide();
                }
            }
        } else {
            $('.question-score').hide();
            $('.question-answers').hide();
        }
    });

    $('#btnComplete').click(function () {
        if (!checkdate($("#StartDateTime").val(), $("#EndDateTime").val())) {
            return false;
        }
        if (!validator.form()) {
            artDialog.alert("操作失败，请检查页面是否有漏掉的错误项");
            return false;
        }
        constructView();
        $.ajax({
            url: 'Post',
            type: 'post',
            data: { objModal: pollingView, Id: pollingView.Id },
            success: function (data) {
                if (ReturnValueFilter(data)) {
                    artDialog.alert("操作成功");
                    window.location.href = 'index?appid=' + $('#AppId').val();
                }
            }
        });
    });

    function addQuestion(isFirst) {
        if (!isFirst && !validator.form()) {
            artDialog.alert("请填写完整已创建的问题再添加下一个");
            return false;
        }
        var qTotal = $('.panel-group').find('.question').length;
        var timestamp = new Date().getTime();
        $('.polling-add-question').before('<div id="questions_block_' + timestamp + '" class="question"></div>');
        var currentQNode = $('#questions_block_' + timestamp);
        var data = {
            id: "heading_" + timestamp, href: "#collapse_" + timestamp, tabid: "collapse_" + timestamp,
            qnum: (qTotal + 1), isMlt: "isMlt_" + timestamp, Title: "Title_" + timestamp, checkboxid: "checkbox_" + timestamp,
            Score: "Score_" + timestamp, RightAnswers: "RightAnswers_" + timestamp
        };

        currentQNode.loadTemplate($("#question_template"), data, { bindingOptions: { "ignoreUndefined": true, "ignoreNull": true } });

        if (!isFirst) {
            currentQNode.find('.js-remove-qusetion').removeClass('hidden');
            currentQNode.find('.panel-collapse').collapse('show');
            currentQNode.siblings('.question').find('.panel-collapse').collapse('hide');
        } else {
            //第一次初始化模板 有奖问答的话则显示俩新字段
            if ($('#Type').val() == "1") {
                currentQNode.find('.question-score').show();
                currentQNode.find('.question-answers').show();
            }
        }
        var addTrriger = currentQNode.find('.js-add-item');
        var addNode = addOption(addTrriger);
        addNode.done(function () {
            addOption(addTrriger).done(function () {
                addOption(addTrriger);
                //initLimitInput();
            });
        });
    }

    function addOption(target) {
        var num;
        var timestamp = new Date().getTime(), def = $.Deferred();
        var i = $(target).data("tag");
        var length = $(target).parents('.panel-body').find('.js-polling-option').length;
        num = turnLetter(length + 1);

        var optionTemplate = '<div class="col-lg-12 polling-option js-polling-option nopd-lr">' +
            '<input type="hidden" name="optionId_' + timestamp + '" value="0" />' +
            '<label class="col-lg-2 control-label text-left">选项 ' + num + '</label>' +
            '<div class="col-lg-5 no-pl">' +
            '<input type="text" id="OptionName' + timestamp + '" class="form-control limited" name="OptionName_' + timestamp + '" ' + 'placeholder=""' +
            ' validate="{required:true,maxlength:200,messages:{required:' + '\'请输入选项名称。\',maxlength:' + '\'请确保选项名称不超过200个字。\'' + '}}" />' +
            '</div>' +
            '<div class="col-lg-1 nopd-lr">' +
            '<div class="upload_area webuploader-container">' +
            '<a class="btn btn-upload js-polling-upload-btn" id="js_upload_' + timestamp + '">上传图片</a>' +
            '<input id="upload-input" type="file" name="file" accept="image/*" onchange="return uploadImage(this);" />' +
            '</div>' +
            '</div>' +
            '<div class="col-lg-2 nopd-lr ml20 js-option-setting"><label class="js-option-type" id="option_type_"' + timestamp + '><input type="checkbox" name="isAddInput" class="ace ace-checkbox-1" value="0" /><span class="lbl"> &nbsp;带文本框</span></label>' +
            '<label class="js-option-input-required hidden"><input type="checkbox" name="isRequired" class="ace ace-checkbox-1" value="1" checked="checked" /><span class="lbl"> &nbsp;必填</span></label></div>' +
            (length >= 2 ? ('<div class="col-lg-1 nopd-lr"><a href="javascript:;" class="link-delete js-delete-item" data-tag="' + i + '">删除</a></div>') : '') +
            '<div class="col-lg-4 img-container nopd-lr mt10" style="display:none;">' +
            '<span class="img-panel"><img class="preview" src="" width="75" height="75"></span>' +
            '<a href="javascript:;" class="link-dele">删除</a>' +
            '</div>' +
            '</div>';

        if (length + 1 > 10) {
            artDialog.alert('每个问题最多允许添加10个选项.');
        } else {
            $(target).closest('.polling_meta_detail').before(optionTemplate);
            if (length >= 2) $(target).parents('.panel-body').find('.js-multiple-num').append('<option value="' + (length + 1) + '">' + (length + 1) + '</option>');
        };

        setTimeout(function () { def.resolve(); }, 60);
        return def;
    };

    function delOption(target) {
        var parentNode = $(target).parents('.panel-body');
        var length = parentNode.find('.js-polling-option').length;
        var optionNode = $(target).parents('.js-polling-option');
        //如果删除的不是最后一个要更新选项值 -2是因为同胞子元素多了2个
        if (optionNode.index() - 2 != length) {
            var afterNodes = optionNode.nextAll('.js-polling-option');
            for (var i = 0; i < afterNodes.length; i++) {
                var num = $(afterNodes).eq(i).index() - 2;
                $(afterNodes).eq(i).find('label.control-label').text('选项' + turnLetter(num - 1));
            }
        }
        optionNode.remove();
        //删除选项后移除js-multiple-num的最后一项
        parentNode.find('.js-multiple-num option:last').remove();
    }

    function delQuestion(target) {
        var qTotal = $(target).parents('.panel-group').find('.question').length;
        var questionNode = $(target).parents('.question');
        //如果删除的不是最后一个要更新选项值
        if (questionNode.index() != qTotal) {
            var afterNodes = questionNode.nextAll('.question');
            for (var i = 0; i < afterNodes.length; i++) {
                var num = $(afterNodes).eq(i).index();
                $(afterNodes).eq(i).find('.polling-num').text('问题' + num);
            }
        }
        questionNode.remove();
    }

    function copyQuestion(target) {
        var cloneNode = $(target).parent().parent().parent().parent().clone();
        console.log(cloneNode.find(".limited").val());
        if (cloneNode.find(".limited").val() == "") {
            artDialog.alert("请填写完整已创建的问题再复制下一个");
            return false;
        }
        var timestamp = new Date().getTime();
        var qTotal = $(target).parents('.panel-group').find('.question').length;
       
        cloneNode.attr("id", "questions_block_" + timestamp);
        cloneNode.find('.js-remove-qusetion').removeClass("hidden");
        cloneNode.find('.panel-heading').attr("id", "heading_" + timestamp);
        cloneNode.find('.panel-collapse').attr("id", "collapse_" + timestamp);
        cloneNode.find('.js-polling-radio').attr("name", "isMlt_" + timestamp);
        cloneNode.find('.js-polling-radio').attr("id", "checkbox_" + timestamp);
        cloneNode.find('.title').attr("id", "Title_" + timestamp);
        cloneNode.find('.title').attr("name", "Title_" + timestamp);
        cloneNode.find('#Score').attr("name", "Score_" + timestamp);
        cloneNode.find('a[data-toggle="collapse"]').attr("href", "#collapse_" + timestamp);
        cloneNode.find('#RightAnswers').attr('name', 'RightAnswers_' + timestamp);
        cloneNode.find('.optionId-hidden').attr('name', "optionId_" + timestamp);
        cloneNode.find('.optionName-input').attr('name', "OptionName_" + timestamp);
        cloneNode.find('.optionName-input').attr('id', "OptionName_" + timestamp);
        cloneNode.find('.js-polling-upload-btn').attr('id', 'js_upload_' + timestamp);
        cloneNode.find('.js-option-type').attr('id', "option_type_" + timestamp);
        cloneNode.find('.questionId-hidden').attr('name', "questionId_" + timestamp);
        //cloneNode.find('input[type=hidden]').attr('value', "0");
        cloneNode.find('.keyid').attr('value', "0");
        $(cloneNode).find('.polling-num').text('问题' + (qTotal+1));
        $(".polling-add-question").before(cloneNode);
    }

    function constructView() {
        pollingView.Id = $('#ID').val();
        pollingView.AppId = $('#AppId').val();
        pollingView.Name = $('#Name').val();
        pollingView.Type = $('#Type').val() == "" ? "1" : $('#Type').val();
        pollingView.StandardScore = $('#StandardScore').val();
        pollingView.AwardNumber = $('#AwardNumber').val();
        pollingView.ReplyMessage = $('#ReplyMessage').val();
        pollingView.Status = "";
        pollingView.StartDateTime = $('#StartDateTime').val() + " " + $('#StartTime').val();
        pollingView.EndDateTime = $('#EndDateTime').val() + " " + $('#EndTime').val();
        pollingView.PollingQuestions = new Array();

        var qTotal = $('.panel-group > .question').length;
        for (var i = 0; i < qTotal; i++) {
            var qNode = $('.question').eq(i);
            var qType = qNode.find('#qType').val();
            var qId = qNode.find('input[name^=questionId]').val();
            var pollingQuestion = new Object();
            pollingQuestion.Id = qId == "0" ? 0 : qId;
            pollingQuestion.PollingId = $('#ID').val();
            pollingQuestion.Title = qNode.find('input[name^=Title]').val();
            pollingQuestion.Type = qType;
            pollingQuestion.OrderIndex = (i + 1);
            pollingQuestion.Score = qNode.find('input[name^=Score]').val();
            pollingQuestion.RightAnswers = qNode.find('input[name^=RightAnswers]').val();
            pollingQuestion.IsRequired = qNode.find('.js-question-required').find('input[name=isRequired]').is(":checked") ? "False" : "True";
            pollingQuestion.PollingOptionEntities = new Array();

            var optionNode = qNode.find('.js-polling-option');
            var optionTotal = optionNode.length;

            if (qType != "99") {
                for (var j = 0; j < optionTotal; j++) {
                    var pollingOption = new Object();
                    var optionType = new Object();
                    var optionId = optionNode.eq(j).find('input[name^=optionId]').val();
                    pollingOption.Id = optionId == "0" ? 0 : optionId;
                    pollingOption.QuestionId = qId == "0" ? 0 : qId;
                    pollingOption.OrderIndex = (j + 1);
                    pollingOption.OptionName = optionNode.eq(j).find('input[name^="OptionName"]').val();
                    pollingOption.Picture = optionNode.eq(j).find('img.preview').attr('src');
                    optionType.type = optionNode.eq(j).find('.js-option-type').find('input[name="isAddInput"]').val();
                    optionType.isRequired = optionNode.eq(j).find('.js-option-input-required').find('input[name="isRequired"]').val();
                    pollingOption.Type = JSON.stringify(optionType);
                    pollingOption.optiontype = optionType;
                    pollingQuestion.PollingOptionEntities.push(pollingOption);
                }
            }

            pollingView.PollingQuestions.push(pollingQuestion);
        }
    }

    function initLimitInput() {
        $('.panel-group input.limited').inputlimiter({
            remText: '%n',//character%s remaining...
            limitText: '/%n.'
        });
    }

    function turnLetter(num) {
        var letter = "";
        switch (num) {
            case 1:
                letter = "A";
                break;
            case 2:
                letter = "B";
                break;
            case 3:
                letter = "C";
                break;
            case 4:
                letter = "D";
                break;
            case 5:
                letter = "E";
                break;
            case 6:
                letter = "F";
                break;
            case 7:
                letter = "G";
                break;
            case 8:
                letter = "H";
                break;
            case 9:
                letter = "I";
                break;
            case 10:
                letter = "J";
                break;
        }
        return letter;
    }

});

function uploadImage(obj) {
    //如果取消上传，则不执行上传事件
    if ($(obj).val() == "") {
        artDialog.alert("请选择文件!");
        return false;
    } else {
        $(obj).FileUpload({
            self: obj,
            maxsize: 200 * 1024,
            url: "/Common/PostImage",
            allowExtension: '.png,.jpg,.jpeg,.JPG,.PNG,.JPEG',
            rewriteDocumentId: '',
            processBarId: 'progress-bar1',
            callBack: function (data) {
                $(obj).parents('.js-polling-option').find('.img-container').show()
                    .find('img.preview').prop('src', "/Common/File?id=" + data.id + "&filename=" + data.result.Src);
                $(obj).prev('.js-polling-upload-btn').text("重新上传");
            }
        });
    }
}
