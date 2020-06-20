"use strict";

(function ($) {
    $.extend({
        WeChatAddressList: function WeChatAddressList() {
            var $list = $("#list_ul");
            var $list_tags = $("#list_ul_tags");
            var config = {
                AjaxUrl: $(window.parent.document).find('#AjaxUrl').val(),
                AppId: $(window.parent.document).find('#hiddenAppId').val(),
                mainGroup: $(window.parent.document).find('#mainGroup'),
                Showspan: $(window.parent.document).find('.Showspan'),
                ShowTag: $(window.parent.document).find('#tagShow').val()
            };

            var methods = {
                getConfig: function getConfig(callback) {
                    var config = $(window.parent.document).find("#config").val();
                    $.get(config, callback);

                },
                init: function init() {
                    this.getConfig(function(conf){
                        config = $.extend(config, conf);
                        if (config.ShowTag === "true") {
                            $("#showTag").show();
                        }
                        $(config.showAllButtonId).on('click', methods.showAll);
                        $('#groupArr').val(config.mainGroup.val());
                        $(".check_out").html(config.Showspan.html());
                        $(".check_out").find('input').remove();
                        methods.showAll();
                        //定义group
                        config.groupArr = config.mainGroup.val().length == 0 ? [] : JSON.parse($("#groupArr").val());
                        $("#check_out").on('click', 'i', methods._del_gropeo);
                        $list.on('click', ".list_person", methods._lineClick);
                        $list.on('click', ".list_do", methods._listdo);
                        $list.on('change', "input[type=checkbox]", methods.check);
                        $('.list_show_bt').on('click', ".groupId", methods.show_this);
                        $list_tags.on('change', "input[type=checkbox]", methods.check);
                        $list_tags.on('click', '.list_do', methods._lineClick);
                        methods.switchTab();
                    });
                },
                switchTab: function switchTab() {
                    $("#switchTab").on('click', 'a', function () {
                        $(this).addClass('active').siblings('a').removeClass('active');
                        $($(this).data('tab')).addClass('active').siblings('.wechatabTabs').removeClass('active');
                    });
                },
                showAll: function showAll() {
                    $(this).nextAll().remove();
                    $.get(config.AjaxUrl + "?" + config.param + "=" + config.AppId, methods.ajaxSuccessFun(false));
                },
                ajaxSuccessFun: function ajaxSuccessFun(isCheckedGroup) {
                    return function (data) {
                        var myhtml = ""; //请求到的组和人的html
                        var tagshtml = "";
                        var tag = 0;
                        //var selectedGroup = $(window.parent.document).find("#mainGroup").val();
                        var selectedGroup = $("#groupArr").val();
                        selectedGroup = selectedGroup == "" ? "" : JSON.parse(selectedGroup);
                        for (var item in data.PersonGroup) {
                            var _gisSellected = false;
                            var _tagSellected=false;
                            $.each(selectedGroup, function (i) {
                                    if (selectedGroup[i].Type == "Group" && selectedGroup[i].WeixinId == data.PersonGroup[item].WeixinId) {
                                        _gisSellected = true;
                                    }
                                    if(selectedGroup[i].Type == "Tag" && selectedGroup[i].WeixinId==data.PersonGroup[item].WeixinId){
                                        _tagSellected=true;
                                    }
                                });
                            if (data.PersonGroup[item].Type == "Group") {
                               
                                if (isCheckedGroup) {
                                    myhtml += '<li data-gptype="Group"><div class="list_do" ><img src="' + config.image_1 + '"><span>' + data.PersonGroup[item].WeixinName + '</span></div><input data-groupid="' + data.PersonGroup[item].WeixinId + '" id="' + data.PersonGroup[item].WeixinId + '" type="checkbox" checked="true" disabled ></li>';
                                } else if (_gisSellected) {

                                    myhtml += '<li class="Group"  data-gptype="Group" ><div class="list_do" ><img src="' + config.image_1 + '"><span>' + data.PersonGroup[item].WeixinName + '</span></div><input data-groupid="' + data.PersonGroup[item].WeixinId + '" id="' + data.PersonGroup[item].WeixinId + '" type="checkbox" checked="true"></li>';
                                } else {
                                    myhtml += '<li class="Group" data-gptype="Group"><div class="list_do" ><img src="' + config.image_1 + '"><span>' + data.PersonGroup[item].WeixinName + '</span></div><input data-groupid="' + data.PersonGroup[item].WeixinId + '" id="' + data.PersonGroup[item].WeixinId + '" type="checkbox" ></li>';
                                }
                            } else if (data.PersonGroup[item].Type == "Tag") {
                                tag = 1;
                                if(_tagSellected){
                                    tagshtml += '<li class="Tag" data-gptype="Tag"><div class="list_do" ><img src="' + config.image_3 + '"><span>' + data.PersonGroup[item].WeixinName + '</span></div><input data-groupid="' + data.PersonGroup[item].WeixinId + '" id="' + data.PersonGroup[item].WeixinId + '"  type="checkbox" checked="true"></li>';
                                }else{
                                    tagshtml += '<li class="Tag" data-gptype="Tag"><div class="list_do" ><img src="' + config.image_3 + '"><span>' + data.PersonGroup[item].WeixinName + '</span></div><input data-groupid="' + data.PersonGroup[item].WeixinId + '" id="' + data.PersonGroup[item].WeixinId + '" type="checkbox" ></li>';
                                }
                                
                            } else {
                                var _pisSellected = false;
                                var headerImage = "";
                                headerImage = data.PersonGroup[item].Avatar == null ? "/Scripts/wechatab/assets/images/icon2.jpg" : data.PersonGroup[item].Avatar;
                                $.each(selectedGroup, function (i) {
                                    if (selectedGroup[i].Type == "Person" && selectedGroup[i].WeixinId == data.PersonGroup[item].WeixinId) {
                                        _pisSellected = true;
                                    }
                                });
                                if (isCheckedGroup) {
                                    myhtml += '<li data-gptype="Person"><div class="list_person"><img src="' + headerImage + '"><span>' + data.PersonGroup[item].WeixinName + '</span></div><input data-groupid="' + data.PersonGroup[item].WeixinId + '" id="' + data.PersonGroup[item].WeixinId + '" type="checkbox" checked="true" disabled ></li>';
                                } else if (_pisSellected) {
                                    myhtml += '<li class="Group" data-gptype="Person"><div class="list_person"><img src="' + headerImage + '"><span>' + data.PersonGroup[item].WeixinName + '</span></div><input data-groupid="' + data.PersonGroup[item].WeixinId + '" id="' + data.PersonGroup[item].WeixinId + '" type="checkbox" checked="true" ></li>';
                                } else {
                                    myhtml += '<li class="Group" data-gptype="Person" ><div class="list_person"><img src="' + headerImage + '"><span>' + data.PersonGroup[item].WeixinName + '</span></div><input data-groupid="' + data.PersonGroup[item].WeixinId + '" id="' + data.PersonGroup[item].WeixinId + '" type="checkbox" ></li>';
                                }
                            }
                        }
                        if (tag === 1) {
                            $list_tags.html(tagshtml);
                        }

                        $list.html(myhtml);
                    };
                },

                groupPop: function groupPop() {
                    var breadCre = "";
                    var outSide = "";
                    $.each(config.groupArr, function (i) {
                        var image;
                        switch (config.groupArr[i].Type) {
                            case "Group":
                                image = config.image_1;
                                break;
                            case "Person":
                                image = config.image_2;
                                break;
                            case "Tag":
                                image = config.image_3;
                                break;
                            default:
                                image = config.image_1;
                        }
                        breadCre += "<span data-gtype=" + config.groupArr[i].Type + " data-gid=" + config.groupArr[i].WeixinId + "><img src='" + config.groupArr[i].Avatar + "'>" + config.groupArr[i].WeixinName + "<i class='" + config.Xlogo + "'></i></span>";
                        outSide += "<span data-gtype=" + config.groupArr[i].Type + " data-gid=" + config.groupArr[i].WeixinId + "><img src='" + config.groupArr[i].Avatar + "'>" + config.groupArr[i].WeixinName + "<i class='" + config.Xlogo + "'></i></span>";
                    });
                    var $checkOut = $("#check_out");
                    $checkOut.html("");
                    $checkOut.html(breadCre);
                    $("#groupArr").val(JSON.stringify(config.groupArr));
                    config.mainGroup.val(JSON.stringify(config.groupArr)); //把值赋给主页面上的选人框
                    config.Showspan.children().not("input").each(function () { 
                        $(this).remove();
                    });
                    config.Showspan.prepend(outSide);
                   
                    //行的点击事件，相当于间接的点击checkbox
                },
                show_this: function show_this() {
                    $(this).nextAll().remove();
                    config.goback = true;
                    methods._listdo($(this).data("groupid"));
                },

                //选中事件
                check: function check() {
                    var checkedName = $(this).prev().find("span").text();
                    var checkedType = $(this).parent().data("gptype");
                    var checkedImage = $(this).prev().find("img")[0].src;
                    var groupId = $(this).data('groupid') + '';
                    config.groupArr = $("#groupArr").val(); //取出已经选中的组或者人
                    if (config.groupArr != "") {
                        config.groupArr = JSON.parse(config.groupArr);
                    } else {
                        config.groupArr = [];
                    }
                    if ($(this).is(":checked")) {
                        config.groupArr.push({ WeixinName: checkedName, Type: checkedType, WeixinId: groupId, Avatar: checkedImage });
                        methods.groupPop();
                    } else {
                        var tempArr = [];
                        $.each(config.groupArr, function (i) {
                            if (config.groupArr[i].Type!=checkedType) {
                                tempArr.push(config.groupArr[i]);
                            }
                            else if(config.groupArr[i].WeixinId != groupId){
                                tempArr.push(config.groupArr[i]);
                            }
                        });

                        config.groupArr = tempArr;
                        methods.groupPop();
                    }
                    return false;
                },
                _listdo: function _listdo(e) {
                    var groupId = null;
                    var checkGroup = false;
                    if (!config.goback) {
                        //区分返回上级组和请求当前组下级
                        $("#list_1").val($(".list_ul").html());
                        var text = $(this).find("span").text(); //获取用户或者组的名称
                        groupId = $(this).next().data('groupid');
                        checkGroup = $(this).next().is(":checked");
                        var text_location = "<b>></b> <a class='groupId' data-groupId='" + groupId + "' href='javascript:void(0)'>" + text + "</a>";
                        $(".list_show_bt").append(text_location); //面包屑增加
                        $("#list_ul").html('');
                    } else {
                        //var selectedGroup = $(window.parent.document).find("#mainGroup").val();
                        var selectedGroup = $("#groupArr").val();
                        if (selectedGroup != "") {
                            selectedGroup = JSON.parse(selectedGroup);
                        }
                        $.each(selectedGroup, function (i) {
                            if (selectedGroup[i].Type == "Group" && selectedGroup[i].WeixinId == e) {
                                checkGroup = true;
                            }
                        });
                        groupId = e;
                        config.goback = false;
                    }

                    $.get(config.AjaxUrl + "?" + config.param + "=" + config.AppId + "&groupId=" + groupId, methods.ajaxSuccessFun(checkGroup));
                    return false;
                },
                _del_gropeo: function _del_gropeo() {
                    var _del_target = $(this).parent();
                    var gt_type = _del_target.data('gtype');
                    var gt_id = _del_target.data('gid');
                    var gt_img = $(this).parent().find("img")[0].src;
                    var _del_groupData = JSON.parse($('#groupArr').val());
                    var _index = "";
                    $.each(_del_groupData, function (i, value) {
                        if (value.Type == gt_type && value.WeixinId == gt_id) {
                            _index = i;
                        }
                    });
                    _del_groupData.splice(_index, 1); //返回值是当前删除的对象
                    $('#groupArr').val(JSON.stringify(_del_groupData));
                    $(window.parent.document).find("#mainGroup").val(JSON.stringify(_del_groupData));

                    _del_target.remove();

                    //重绘parent层 group人员显示
                    var breadCre = "";
                    $.each(_del_groupData, function (i) {
                        if (_del_groupData[i].Type == "Group") {
                            breadCre += "<span data-gtype=" + _del_groupData[i].Type + " data-gid=" + _del_groupData[i].WeixinId + "><img src='" + config.image_1 + "'>" + _del_groupData[i].WeixinName + "<i class='" + config.Xlogo + "'></i></span>";
                        } else {
                            breadCre += "<span data-gtype=" + _del_groupData[i].Type + " data-gid=" + _del_groupData[i].WeixinId + "><img src='" + _del_groupData[i].Avatar + "'>" + _del_groupData[i].WeixinName + "<i class='" + config.Xlogo + "'></i></span>";
                        }
                    });
                    $(".check_out").html(breadCre);
                    //$(".check_out").on('click', 'i', methods._del_gropeo);
               
                    $(window.parent.document).find(".forShowName .Showspan").children().not("input").each(function () { // "*"表示div.content下的所有元素
                        $(this).remove();
                    });
                    $(window.parent.document).find(".forShowName .Showspan").prepend(breadCre);
                    $('.list_show_bt a').last().click();
                },
                _lineClick: function _lineClick() {
                    $(this).parent().find('input').click();
                },
                __lineClick: function __lineClick() {
                    $(this).find('input').click();
                }
            };

            //Init
            methods.init();
            return methods;
        }

    });
})($);

//# sourceMappingURL=app_bk-compiled.js.map