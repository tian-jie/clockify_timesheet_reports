(function () {
    var polligview = {};
    polligview.AnswerResults = [];
    var artDialog = {};
    artDialog.alert = function (content, title, callback) {
        return dialog({
            title: title ? title : '提示',
            id: 'Alert',
            icon: 'warning',
            fixed: true,
            lock: true,
            zIndex: 9999,
            content: content,
            ok: true,
            okValue : '确定',
            time: 1500,
            close: callback,
            width:200,  
            height:50 
        }).showModal();
    };
    var judgePollingNull = function putValue(pollingId) {
        combineData(pollingId);
        if (polligview.AnswerResults.length == 0) {
            artDialog.alert("请至少填写一个问题！" + str);
            
            return false;
        }
        return true;
    };

    var str = '';
    var signal = '';
    var optioncount = 0; //已选数量

    var judgeNull = function checkoption(pollingId) {
        var qTotal = $('#VoteForm_' + pollingId + ' .question').length;

        for (var m = 0; m < qTotal; m++) {
            var qNode = $('#VoteForm_' + pollingId + ' .question').eq(m);
            //必填
            var optionNode = qNode.find('.option');
            var answer = optionNode.find(".questionrequired");
            if (answer.length != 0) {

                if ($("#AnswerText_" + m).val() != "") {
                    signal += "yes,";
                } else {
                    signal += "no,";
                }
                if ($("#AnswerText_" + m).val().length > 500) {
                    str += '问题答案应少于500个字。<br/>';
                    signal += "no,";
                }
            }
            if (optionNode.length > 0) {
                var optionInput = optionNode.eq(0).find('input[name^=Box]');
                optioncount = 0;
                for (var k = 0; k < optionInput.length; k++) {

                    if (optionInput.eq(k).is(":checked")) {
                        optioncount += 1;
                        //文本类型
                        var answer = optionInput.eq(k).parents(".row").find(".answertextrequired");
                        if (answer.length != 0) {
                            if (answer.find('textarea[name=OptionAnswerText]').val() != "") {
                                signal += "yes,";
                            } else {
                                signal += "no,";
                            }
                            if (answer.find('textarea[name=OptionAnswerText]').val().length > 200) {
                                str += '选项答案应少于200个字。<br/>';
                                signal += "no,";
                            }
                        }
                    }

                }
                if (optioncount > 0) {
                    signal += "yes,";
                } else if (optioncount == 0 && qNode.find('input[class=QuestionId]').attr('questionrequired') == 'True' && qNode.find(".questionrequired").length == 0) {
                    signal += "no,";
                }

            }
        }

        if (signal.indexOf("no") > -1 && signal.length > 1) {
            signal = '';
            artDialog.alert("请完成所有的选项，方可提交，谢谢!" + str);
            
            str = '';
            $("#btnSend_" + pollingId).removeAttr("disabled");
            return false;
        }

        return true;
    };

    $('input[type=checkbox]').click(function () {
        if ($(this).attr("typevalue") != 0) {
            if ($(this).parents(".question").find($("input[name^='Box']:checked")).length == $(this).attr("typevalue")) {

                $(this).parents(".question").find($("input[name^='Box']:unchecked")).attr('disabled', 'disabled');

            } else {
                $(this).parents(".question").find($("input[name^='Box']")).attr('disabled', false);
            }
        }
        if ($(this).attr("hasinput") == 1 && $(this).is(":checked")) {
            $(this).parents(".row").find($(".optionanswertext")).removeClass("hidden");
        }
        else {
            $(this).parents(".row").find($(".optionanswertext")).addClass("hidden");
        }

    });
    $('input[type=radio]').click(function () {
        $(this).parents(".question").find($(".optionanswertext")).addClass("hidden");
        if ($(this).attr("hasinput") == 1 && $(this).is(":checked")) {
            $(this).parents(".row").find($(".optionanswertext")).removeClass("hidden");
        }
    });
    $('.js-btn-submit').click(function () {
        var pollingId = $(this).attr('id').replace('btnSend_', '');
        $("#btnSend_" + pollingId).attr("disabled", "disabled");

        //判断值是否选择
        if (!judgeNull(pollingId)) {
            $("#btnSend_" + pollingId).removeAttr("disabled");

            return false;
        };


        //赋值
        if (!judgePollingNull(pollingId)) {
            $("#btnSend_" + pollingId).removeAttr("disabled");

            return false;
        };
        //putValue(pollingId);
        $.post('/Activity/polling/InputData', { objModal: polligview }, function (data) {
            if (data.Status != 200) {
                var d = dialog({
                    title: '提示',
                    content: data.Message,
                    okValue: '确定',
                    ok: function () {
                        refresh();
                    }
                });
                d.show();
                return;
            }

            if (data != null) {
                var voteType = $("#Type_" + polligview.PollingId).val();
                if (voteType == "1") {

                    $("#VoteForm_" + polligview.PollingId).addClass("hidden");
                    $("#wxVoteDetail_" + polligview.PollingId).addClass("hidden");
                    $("#wxQADetail_" + polligview.PollingId).removeClass("hidden");
                    refresh();

                } else if (voteType == "2") {

                    $("#VoteForm_" + polligview.PollingId).addClass("hidden");
                    $("#wxQADetail_" + polligview.PollingId).addClass("hidden");
                    $("#wxVoteDetail_" + polligview.PollingId).removeClass("hidden");
                    // window.location.search += '&' + (new Date()).getTime();
                    refresh();

                }
                else if (voteType == "3") {

                    $("#VoteForm_" + polligview.PollingId).addClass("hidden");
                    $("#wxQADetail_" + polligview.PollingId).addClass("hidden");
                    $('#polling_modal').modal('hide');

                    if ($('#EventId').val() == 0) {
                        $("#wxVoteDetail_" + polligview.PollingId).removeClass("hidden");
                        refresh();
                    }
                } else {
                    $("#btnSend_" + polligview.PollingId).removeAttr("disabled");
                    
                    
                }
            }

        }).error(function () {
            $("#btnSend_" + polligview.PollingId).removeAttr("disabled");
            artDialog.alert("对不起，提交失败。");
        });

    });
    $('.js-btn-save').click(function () {
        var pollingId = $(this).attr('id').replace('btnSave_', '');
        //赋值
        if (!judgePollingNull(pollingId)) {
            $("#btnSend_" + pollingId).removeAttr("disabled");

            return false;
        };
        combineData(pollingId);
        //putValue(pollingId);
        $.post('/Activity/polling/InputDataTemp', { objModal: polligview }, function (data) {
            if (data.Status == 200) {
                artDialog.alert("暂存成功，可以关闭页面也可以继续作答，点击提交后答题结果生效。");
                return;
            }

        })
    });
    function refresh() {
        if (window.location.search) {
            window.location.href = window.location.href + "&v=" + (new Date()).getTime();
        } else {
            window.location.href = window.location.href + "?v=" + (new Date()).getTime();
        }
    }
    function combineData(pollingId) {
        polligview.AnswerResults = [];
        polligview.PollingId = pollingId;
        var qTotal = $('#VoteForm_' + pollingId + ' .question').length;

        for (var i = 0; i < qTotal; i++) {
            var item = {};
            var qNode = $('#VoteForm_' + pollingId + ' .question').eq(i);

            item.QuestionId = parseInt(qNode.find('input[class=QuestionId]').val());
            item.QuestionName = qNode.find('input[class=QuestionId]').attr('questionname');

            var optionNode = qNode.find('.option');
            if (optionNode.length > 0) {
                //文本类型
                var answer = optionNode.find(".answertext");
                if (answer.length != 0 && $("#AnswerText_" + i).val() != '') {
                    item.Answer = "";
                    item.AnswerText = $("#AnswerText_" + i).val();
                    var target = {};
                    $.extend(true, target, item);
                    polligview.AnswerResults.push(target);
                }
                var optionTotal = optionNode.length;
                for (var j = 0; j < optionTotal; j++) {
                    var optionInput = optionNode.eq(j).find('input[name^=Box]');
                    for (var k = 0; k < optionInput.length; k++) {
                        if (optionInput.eq(k).is(":checked")) {

                            item.Answer = parseInt(optionInput.eq(k).attr("boxvalue"));
                            item.AnswerText = optionInput.eq(k).attr("answervalue");
                            //文本类型
                            var answerOption = optionInput.eq(k).parents(".row").find(".optionanswertext");
                            if (answerOption.length != 0) {
                                item.AnswerText += '：' + optionInput.eq(k).parents(".row").find('textarea[name=OptionAnswerText]').val();
                            }
                            var target = {};
                            $.extend(true, target, item);
                            polligview.AnswerResults.push(target);
                        }
                    }
                }
            }
            //}
        }
    }
    window.onload = function () {
        if ($("input[id ^= 'PollingId']").attr('id') != null) {

            var pollingId = $("input[id ^= 'PollingId']").attr('id').replace('PollingId_', '');
            $.post('/Activity/polling/ResultTemp', { id: pollingId }, function (result) {
                if (result != null) {
                    if (result.Status == '200') {
                        if (result.Data != null && result.Data.length > 0) {
                            for (var i = 0; i < result.Data.length; i++) {
                                var AnswerResult = result.Data[i];
                                if (AnswerResult.AnswerResults[0] != null) {
                                    if (AnswerResult.AnswerResults[0].Answer == "0") {
                                        $('.question').find("input[value=" + "'" + AnswerResult.QuestionId + "']")
                                            .parents('.question').find('textarea[name=AnswerText]').val(AnswerResult.AnswerResults[0].AnswerText);
                                    }
                                    else {
                                        if (AnswerResult.AnswerResults[0].AnswerText.indexOf("：") > -1) {
                                            ErgodicTree(pollingId, AnswerResult.AnswerResults[0].Answer, 1, AnswerResult.AnswerResults[0].AnswerText.split("：")[1]);

                                        }
                                        else {
                                            ErgodicTree(pollingId, AnswerResult.AnswerResults[0].Answer, 0, 0);

                                        }
                                    }
                                }
                            }
                        }
                    };
                };
            });
        }
    };
    //遍历树，根据boxvalue值匹配option
    function ErgodicTree(pollingId, value1, count, value2) {
        polligview.PollingId = pollingId;
        var qTotal = $('#VoteForm_' + pollingId + ' .question').length;

        for (var i = 0; i < qTotal; i++) {

            var qNode = $('#VoteForm_' + pollingId + ' .question').eq(i);

            var optionNode = qNode.find('.option');
            if (optionNode.length > 0) {

                var optionTotal = optionNode.length;
                for (var j = 0; j < optionTotal; j++) {
                    var optionInput = optionNode.eq(j).find('input[name^=Box]');
                    for (var k = 0; k < optionInput.length; k++) {

                        if (optionInput.eq(k).attr("boxvalue") == value1) {
                            optionInput.eq(k).trigger('click');

                            //文本类型
                            if (count > 0) {
                                var answerOption = optionInput.eq(k).parents(".row").find(".optionanswertext");
                                answerOption.removeClass("hidden").addClass("show");
                                optionInput.eq(k).parents(".row").find('textarea[name=OptionAnswerText]').val(value2);
                            }

                        }
                    }
                }
            }

        }

    }

})();