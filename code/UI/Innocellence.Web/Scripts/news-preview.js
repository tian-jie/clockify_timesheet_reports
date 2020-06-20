var app = angular.module('newsEdit', []);
var swapItems = function(arr, index1, index2) {
    arr[index1] = arr.splice(index2, 1, arr[index1])[0];
    return arr;
};
var arrayDel = function(arr, index) {
    arr.splice(index, 1);
    return arr;
}
app.controller('NewsEdit', function($scope, $http) {
    $scope.IsUseComment = true;

    $scope.check = function(val) {
        //!val ? alert('选中') : alert('未选中');
        $scope.IsUseComment = !val;
    }

    $scope.news = [];
    $scope.firstitem = {};
    // model first item
    $scope.forreset_firstName = {
        id: 0,
        msgtitle: "",
        msgfileImg: "",
        msgtips: "",
        msgmaintext: "",
        btntext: "Add Comment",
        IsUseComment: true,
        showLike: "true",
        showReadCount: "true",
        articleNoShare: "true",
        showComment: "false",
        securityLevel:"0",
        newMaterialId: 0,
        msgarticleurl: ""
    };
    $scope.firstDown = function($event) {
        $event.stopPropagation();
        var first = $scope.news[0];
        $scope.news[0] = Object.assign({}, $scope.firstitem);
        $scope.firstitem = Object.assign({}, first);
    }
    $scope.deleteFirst = function() {
        if ($scope.news.length == 1) {
            $scope.firstitem = {};
            
            $('.default-item,#firstItem').show();
            $('.first-item,#secondItem').hide();
        }
        var cache = $scope.firstitem;
        $scope.firstitem = Object.assign({}, $scope.news[0]);
        $scope.news = arrayDel($scope.news, 0);


    }
    $scope.reset_firstitem = function() {
        $scope.firstitem = angular.copy($scope.forreset_firstName);
    };
    $scope.reset_firstitem();

    //model other common item ---
    //$scope.news = [{ msgtitle: '@占位', msgfileImg: '', msgtips: '占位', msgmaintext: '占位' }];//不写这个，后面的watch会提示字段未定义
    $scope.id = 0;
    $scope.activeId = 0;
    $scope.activeTitle = "";
    $scope.msgtitle = "";
    $scope.msgfileImg = "";
    $scope.msgtips = "";
    $scope.msgmaintext = "";
    $scope.btntext = "";
    $scope.showLike = "true";
    $scope.showReadCount = "true";
    $scope.articleNoShare = "true";
    $scope.showComment = "false";
    $scope.isWatermark = "true";
    $scope.securityLevel = "0";
    $scope.newMaterialId = 0;
    $scope.msgarticleurl = "";
    //model type3 msg-img ---
    $scope.type3_msgImg = "";

    //model type4 msg-video ---
    $scope.type4_msgVideoSrc = "";
    $scope.type4_msgVideoTitle = "";
    $scope.type4_msgVideoDesc = "";
    $scope.type4_msgVideoImg = "";
    //model type3 msg-audio ---
    $scope.type5_SoundSrc = "";
    $scope.type5_duration = "";
    //model type6 msg-file ---
    $scope.type6_msgFileSrc = "";

    /* --- functions beloww ---*/

    $scope.LikeBoxClick = function (htmlitem,isfirst) {
        if ($(htmlitem.target).is(":checked")) {
            $(htmlitem.target).parent().find('.IsLike').val("true");
            if (isfirst) {
                $scope.firstitem.showLike = $(htmlitem.target).parent().find('.IsLike').val()
            }
            else {
                $scope.showLike = $(htmlitem.target).parent().find('.IsLike').val()
            }                    
            $(htmlitem.target).prop('checked', true);
        } else {
            $(htmlitem.target).parent().find('.IsLike').val("false");
            if (isfirst) {
                $scope.firstitem.showLike = $(htmlitem.target).parent().find('.IsLike').val()
            }
            else {
                $scope.showLike = $(htmlitem.target).parent().find('.IsLike').val()
            }
            $(htmlitem.target).prop('checked', false);
        }
    };
    $scope.ShowReadCountBoxClick = function (htmlitem,isfirst) {
        if ($(htmlitem.target).is(":checked")) {
            $(htmlitem.target).parent().find('.ShowReadCount').val("true");
            if (isfirst) {
                $scope.firstitem.showReadCount = $(htmlitem.target).parent().find('.ShowReadCount').val()
            }
            else {
                $scope.showReadCount = $(htmlitem.target).parent().find('.ShowReadCount').val()
            }                
            $(htmlitem.target).prop('checked', true);
        } else {
            $(htmlitem.target).parent().find('.ShowReadCount').val("false");
            if (isfirst) {
                $scope.firstitem.showReadCount = $(htmlitem.target).parent().find('.ShowReadCount').val()
            }
            else {
                $scope.showReadCount = $(htmlitem.target).parent().find('.ShowReadCount').val()
            }
            $(htmlitem.target).prop('checked', false);
        }
    };
    $scope.ShowCommentBoxClick = function (htmlitem,isfirst) {
        if ($(htmlitem.target).is(":checked")) {
            $(htmlitem.target).parent().find('.ShowComment').val("true");
            if (isfirst) {
                $scope.firstitem.showComment = $(htmlitem.target).parent().find('.ShowComment').val()
            }
            else {
                $scope.showComment = $(htmlitem.target).parent().find('.ShowComment').val()
            }                   
            $(htmlitem.target).prop('checked', true);
        } else {
            $(htmlitem.target).parent().find('.ShowComment').val("false");
            if (isfirst) {
                $scope.firstitem.showComment = $(htmlitem.target).parent().find('.ShowComment').val()
            }
            else {
                $scope.showComment = $(htmlitem.target).parent().find('.ShowComment').val()
            }
            $(htmlitem.target).prop('checked', false);
        }
    };
    $scope.IsWatermarkBoxClick = function (htmlitem,isfirst) {
        if ($(htmlitem.target).is(":checked")) {
            $(htmlitem.target).parent().find('.IsWatermark').val("true");
            if (isfirst) {
                $scope.firstitem.isWatermark = $(htmlitem.target).parent().find('.IsWatermark').val()
            }
            else {
                $scope.isWatermark = $(htmlitem.target).parent().find('.IsWatermark').val()
            }           
            $(htmlitem.target).prop('checked', true);
        } else {
            $(htmlitem.target).parent().find('.IsWatermark').val("false");
            if (isfirst) {
                $scope.firstitem.isWatermark = $(htmlitem.target).parent().find('.IsWatermark').val()
            }
            else {
                $scope.isWatermark = $(htmlitem.target).parent().find('.IsWatermark').val()
            }
            $(htmlitem.target).prop('checked', false);
        }
    };
    $scope.ArticleNoShareBoxClick = function (htmlitem, isfirst) {
        if ($(htmlitem.target).is(":checked")) {
            $(htmlitem.target).parent().find('.ArticleNoShare').val("true");
            if (isfirst) {
                $scope.firstitem.articleNoShare = $(htmlitem.target).parent().find('.ArticleNoShare').val()
            }
            else {
                $scope.articleNoShare = $(htmlitem.target).parent().find('.ArticleNoShare').val()
            }
            $(htmlitem.target).prop('checked', true);
        } else {
            $(htmlitem.target).parent().find('.ArticleNoShare').val("false");
            if (isfirst) {
                $scope.firstitem.articleNoShare = $(htmlitem.target).parent().find('.ArticleNoShare').val()
            }
            else {
                $scope.articleNoShare = $(htmlitem.target).parent().find('.ArticleNoShare').val()
            }
            $(htmlitem.target).prop('checked', false);
        }
    };
    $scope.ForNewDataWatch = function () {

        $scope.firstitem.showLike = $('.firstShowLikeBox').parent().find('.IsLike').val();
        $scope.firstitem.showReadCount = $('.firstShowReadCountBox').parent().find('.ShowReadCount').val();
        $scope.firstitem.showComment = $('.firstShowCommentBox').parent().find('.ShowComment').val();
        $scope.firstitem.isWatermark = $('.firstIsWatermark').parent().find('.IsWatermark').val();
        $scope.firstitem.securityLevel = $('#firstSecurityLevel').val()
        $scope.firstitem.newMaterialId = $('#firstMaterialId').val()
        $scope.firstitem.articleNoShare = $('.firstArticleNoShare').parent().find('.ArticleNoShare').val();


        // $scope.$apply()
        $scope.showLike = $('.secondShowLikeBox').parent().find('.IsLike').val();
        $scope.showReadCount = $('.secondShowReadCountBox').parent().find('.ShowReadCount').val();
        $scope.showComment = $('.secondShowCommentBox').parent().find('.ShowComment').val();
        $scope.isWatermark = $('.secondIsWatermark').parent().find('.IsWatermark').val();
        $scope.securityLevel = $('#secondSecurityLevel').val();
        $scope.newMaterialId = $('#secondMaterialId').val();
        $scope.articleNoShare = $('.secondArticleNoShare').parent().find('.ArticleNoShare').val();
    }



    //reset form
    $scope._ng_reset = function() {
        $scope.reset_firstitem();
        $scope.news = [];

        $("#uploader1 #thelist").html("");
        $("#uploader2 #thelist").html("");

        $('#firstItem .bar').css({ width: '0%' });
        $('#Img1-progress-bar').text('');
        editor1.setContent("");
        $('.default-item,#firstItem').show();
        $('.first-item,#secondItem').hide();
    };
    $scope._ng_resetImg = function() {
        $scope.type3_msgImg = "";
        $("#uploader #thelist").html("");
        //$('#Img3-progress-bar').css({ width: '0%' }).text('');
    };
    $scope._ng_resetVideo = function() {
        $scope.type4_msgVideoSrc = "";
        $scope.type4_msgVideoTitle = "";
        $scope.type4_msgVideoDesc = "";
        $scope.type4_msgVideoImg = "";
        $("#uploader4 #thelist").html("");
    };

    $scope.checkImg = function(s) {

        if ($scope.firstitem.msgfileImg == "") {
            return s == 0 ? 1 : 0;
        } else {
            return s == 1 ? 1 : 0;
        }
    };
    $scope.checkIImg = function(s) {

        if ($scope.firstitem.IfileImg == null || $scope.firstitem.IfileImg == "") {
            return s == 0 ? 1 : 0;
        } else {
            return s == 1 ? 1 : 0;
        }
    };
    $scope.checkType3img = function() {
        if ($scope.type3_msgImg == "") {
            return 0;
        } else {
            return 1;
        }
    };
    $scope.checkType6file = function() {
        if ($scope.type6_msgFileSrc == "") {
            return 0;
        } else {
            return 1;
        }
    };
    $scope.checkType4video = function(s) {
        if ($scope.type4_msgVideoSrc == "") {
            return s == 0 ? 1 : 0;
        } else {
            $("#checkType4video").prop('src', '/uploadfile/upload/image/' + $scope.type4_msgVideoSrc);
            return s == 1 ? 1 : 0;
        }
    };
    $scope.checktype5Sound = function() {
        if ($scope.type5_SoundSrc == "") {
            return 0;
        } else {
            return 1;
        }
    };

    //activeFirst func
    $scope.activeFirst = function() {
        $('#firstItem').show();
        $('#secondItem').hide();
        //var _curLi = $('.repeat-item')[index];
        $('#fi-item').addClass("newsActived").siblings().removeClass('newsActived');
        if (!$scope.firstitem.msgfileImg) {
            $('#firstItem').find('.preview').remove();
        } else {
            $('#firstItem').find('.preview').html('<div style="float:left"><img style="width: 100px;height:100px;" src="' + $scope.firstitem.msgfileImg + '"></div><div style="clear:both;"></div>')
        };
        $('.firstShowReadCountBox').prop("checked", eval($scope.firstitem.showReadCount))
        $('.firstShowLikeBox').prop("checked", eval($scope.firstitem.showLike))
        $('.firstShowCommentBox').prop("checked", eval($scope.firstitem.showComment))
        $('.firstIsWatermarkBox').prop("checked", eval($scope.firstitem.isWatermark))
        $('.firstArticleNoShareBox').prop("checked", eval($scope.firstitem.articleNoShare))


        $('.firstShowLikeBox').parent().find('.IsLike').val($scope.firstitem.showLike);
        $('.firstShowReadCountBox').parent().find('.ShowReadCount').val($scope.firstitem.showReadCount);
        $('.firstShowCommentBox').parent().find('.ShowComment').val($scope.firstitem.showComment);
        $('.firstIsWatermarkBox').parent().find('.IsWatermark').val($scope.firstitem.isWatermark);
        $('.firstArticleNoShareBox').parent().find('.ArticleNoShare').val($scope.firstitem.articleNoShare);

        var firstSelect = $('#firstSecurityLevel').find('option')

        for (var i = 0; i < firstSelect.length ; i++) {
            if ($(firstSelect[i]).val() === $scope.firstitem.securityLevel) {
                $(firstSelect[i]).prop("selected", "selected")
            }
        }



    };

    $scope.refresh = function() {

    };
    //add func
    $scope.isAddFunGo = false;

    $scope.AddNewItem = function() {

        $scope.isAddFunGo = true; //for item repeat done;
        $scope.id++;

        $scope.itemIsUseComment = true;

        $scope.checkItem = function(e) {
            //!val ? alert('选中') : alert('未选中');
            //$scope.IsUseComment = !val;
            $scope.itemIsUseComment = !e.IsUseComment;
        }

        $scope.news.push({ id: $scope.id, msgtitle: '', msgfileImg: '', msgtips: '', msgmaintext: '', btntext: 'Add Comment', IsUseComment: true, showLike: "true", showReadCount: "true", showComment: "false", isWatermark: "true", securityLevel: "0", newMaterialId: 0, articleNoShare: "true", msgarticleurl:"" });

        $('.default-item,#firstItem').hide();
        $('.first-item,#secondItem').show();

        if ($scope.news.length >= 9) {
            $('.last-item').hide(); //max 8 news
        }
        return true;
    };




    // init ueditor
    var editor1 = new UE.ui.Editor({
        initialFrameWidth: 500,
        toolbars: [
            [
                'source', //源代码
                'undo', //撤销
                'redo', //重做
                'bold', //加粗
                'indent', //首行缩进
                'italic', //斜体
                'underline', //下划线
                'subscript', //下标
                'fontborder', //字符边框
                'superscript', //上标
                'unlink', //取消链接
                'link', //超链接
                'cleardoc', //清空文档
                'fontfamily', //字体
                'fontsize', //字号
                'paragraph', //段落格式
                //'simpleupload', //单图上传
                'insertimage', //多图上传
                //'emotion', //表情
                'spechars', //特殊字符
                //'insertvideo', //视频
                'help', //帮助
                'justifyleft', //居左对齐
                'justifyright', //居右对齐
                'justifycenter', //居中对齐
                'justifyjustify', //两端对齐
                'forecolor', //字体颜色
                'backcolor', //背景色
                'insertorderedlist', //有序列表
                'insertunorderedlist', //无序列表
                'fullscreen', //全屏
                'insertpolling'//投票
            ]
        ],
        initialFrameHeight: 180,
        autoHeightEnabled: false,
        autoFloatEnabled: true,
        enableAutoSave: false,
        wordCountMsg: '{#count}',
        //maximumWords:2000,       //允许的最大字符数
        //wordCountMsg: '{#count}/剩余{#leave}',   //当前已输入 {#count} 个字符，您还可以输入 个字符
        //wordOverFlowMsg: '<span style="color:red;">你输入的字符个数已经超出最大允许值，服务器可能会拒绝保存！</span>',    //<span style="color:red;">你输入的字符个数已经超出最大允许值，服务器可能会拒绝保存！</span>
        elementPathEnabled: false

    });
    //事件 重写ctrl + x change content
    editor1.addListener("selectionChange", function() {
        $scope.firstitem.msgmaintext = editor1.getContent();
        $scope.$apply(function() {
            jQuery('#editorvalue1').val(editor1.getContent());
        });
    });


    //事件

    editor1.addListener("contentChange oncut", function() {
        $scope.firstitem.msgmaintext = editor1.getContent();
        $scope.$apply(function() {
            jQuery('#editorvalue1').val(editor1.getContent());
        });

        //$scope.$apply(function () {
        //    jQuery('#editorvalue1').val(editor1.getContent());
        //});
    });




    var editor2 = new UE.ui.Editor({
        initialFrameWidth: 500,
        toolbars: [
            [
                'source', //源代码
                'undo', //撤销
                'redo', //重做
                'bold', //加粗
                'indent', //首行缩进
                'italic', //斜体
                'underline', //下划线
                'subscript', //下标
                'fontborder', //字符边框
                'superscript', //上标
                'unlink', //取消链接
                'link', //超链接
                'cleardoc', //清空文档
                'fontfamily', //字体
                'fontsize', //字号
                'paragraph', //段落格式
                //'simpleupload', //单图上传
                'insertimage', //多图上传
                //'emotion', //表情
                'spechars', //特殊字符
                //'insertvideo', //视频
                'help', //帮助
                'justifyleft', //居左对齐
                'justifyright', //居右对齐
                'justifycenter', //居中对齐
                'justifyjustify', //两端对齐
                'forecolor', //字体颜色
                'backcolor', //背景色
                'insertorderedlist', //有序列表
                'insertunorderedlist', //无序列表
                'fullscreen', //全屏
                'insertpolling'//投票
            ]
        ],
        initialFrameHeight: 180,
        autoHeightEnabled: false,
        autoFloatEnabled: true,
        enableAutoSave: false,
        wordCountMsg: '{#count}',
        //maximumWords: 2000,       //允许的最大字符数
        //wordCountMsg: '{#count}/剩余{#leave}',   //当前已输入 {#count} 个字符，您还可以输入 个字符
        //wordOverFlowMsg: '<span style="color:red;">你输入的字符个数已经超出最大允许值，服务器可能会拒绝保存！</span>',    //<span style="color:red;">你输入的字符个数已经超出最大允许值，服务器可能会拒绝保存！</span>
        elementPathEnabled: false
    });

    //事件 重写ctrl + x change content
    editor2.addListener("selectionChange", function() {
        $scope.msgmaintext = editor2.getContent();
        $scope.$apply(function() {
            jQuery('#editorvalue2').val(editor2.getContent());
        });
    });

    editor2.addListener("contentChange", function() {
        $scope.msgmaintext = editor2.getContent();
        $scope.$apply(function() {
            jQuery('#editorvalue2').val(editor2.getContent());
        });
        //$scope.$apply(function() {
        //    jQuery('#editorvalue2').val(editor2.getContent());
        //});
        //$('#editorvalue2').val(editor2.getContent());
    });
    editor1.render("editor1");
    editor2.render("editor2");
    $scope.scop_editor2 = editor2; //editor2对象赋给scope,便于指令调用
});


//item 循环结束执行
app.directive('repeatDone', function() {
    return {
        link: function(scope, element, attrs) {
            if (scope.$last) { // 这个判断意味着最后一个 OK
                scope.$eval(attrs.repeatDone); // 执行绑定的表达式
            }
        }
    };
});



app.directive('item', function() {
    var repeatItemHtml = "<li class='repeat-item' ng-repeat='new in news'><div class='order' style='display:none;'><b class='arrow fa fa-angle-up' ng-click='upRecord($index)'></b>&nbsp;&nbsp;<b class='arrow fa fa-angle-down' ng-click='downRecord($index)'></b></div><a href='javascript:void(0)' repeat-done='active($index,$last)' ng-click='active($index,$last)'>" +
        "<div class='repeat-img-box' ng-if='checkImg2(1,$index)' ><img ng-src='{{new.msgfileImg}}'/></div>" +
        "<p class='repeat-item-placeholder' ng-if='checkImg2(0,$index)'  ></p>" +
        "<p class='title' ng-bind='new.msgtitle'></p>" + "</a><button class='rep-del' ng-click='delete($index)'><i class='ace-icon glyphicon glyphicon-remove'></i></button></li>";

    return {
        restrict: 'EA',
        replace: true,
        transclude: true,
        template: repeatItemHtml,
        link: function(scope, element, attrs) {

            scope.checkImg2 = function(s, index) {

                if (scope.news[index].msgfileImg == "") {
                    return s == 0 ? 1 : 0;
                } else {
                    return s == 1 ? 1 : 0;
                }


            };


            // 上移
            scope.upRecord = function($index) {

                if ($index == 0) {
                    var first = scope.news[0];
                    scope.news[0] = Object.assign({}, scope.firstitem);
                    scope.firstitem = Object.assign({}, first);
                    return;
                }
                swapItems(scope.news, $index, $index - 1);
            };

            // 下移
            scope.downRecord = function($index) {
                if ($index == scope.news.length - 1) {
                    return;
                }
                swapItems(scope.news, $index, $index + 1);
            };

            scope.delete = function(index) {

                $('.last-item').show(); //max 8 news

                if (scope.news.length == 1) {
                    $('.default-item,#firstItem').show();
                    $('.first-item,#secondItem').hide();
                }
                var newsTemp = [];
                for (var i = 0; i < scope.news.length; i++) {
                    if (i == index) {
                        continue;
                    } else if (i > index) {
                        scope.news[i].id--;
                        newsTemp.push(scope.news[i]);
                    } else {
                        newsTemp.push(scope.news[i]);
                    }
                }
                //if (newsTemp.length == 0) {
                //    newsTemp = [{ msgtitle: '@占位', msgfileImg: '占位', msgtips: '占位', msgmaintext: '占位' }];
                //}
                scope.news = newsTemp;
                //scope.news.splice(index, 1);
                scope.activeFirst(); //when click add always focus first item

            };
            scope.repeatCallBack = function() {
                // same to active
            };
            scope.active = function(index, last) {

                $('.default-item,#firstItem').hide();
                $('.first-item,#secondItem').show();

                scope.activeId = index;
                var activeItem = scope.news[index];
                scope.msgtitle = activeItem.msgtitle;
                scope.msgfileImg = activeItem.msgfileImg;
                scope.msgtips = activeItem.msgtips;
                scope.msgmaintext = activeItem.msgmaintext;
                scope.btntext = activeItem.btntext;
                scope.IsUseComment = activeItem.IsUseComment;
                scope.msgarticleurl = activeItem.msgarticleurl;
                
                scope.showLike = activeItem.showLike;
                scope.showReadCount = activeItem.showReadCount
                scope.showComment = activeItem.showComment
                scope.isWatermark = activeItem.isWatermark
                scope.securityLevel = activeItem.securityLevel
                scope.articleNoShare = activeItem.articleNoShare

                $('.secondShowReadCountBox').prop("checked", eval(activeItem.showReadCount))
                $('.secondShowLikeBox').prop("checked", eval(activeItem.showLike))
                $('.secondShowCommentBox').prop("checked", eval(activeItem.showComment))
                $('.secondIsWatermarkBox').prop("checked", eval(activeItem.isWatermark))
                $('.secondArticleNoShareBox').prop("checked", eval(activeItem.articleNoShare))


                $('.secondShowLikeBox').parent().find('.IsLike').val(activeItem.showLike);
                $('.secondShowReadCountBox').parent().find('.ShowReadCount').val(activeItem.showReadCount);
                $('.secondShowCommentBox').parent().find('.ShowComment').val(activeItem.showComment);
                $('.secondIsWatermarkBox').parent().find('.IsWatermark').val(activeItem.isWatermark);
                $('.secondArticleNoShareBox').parent().find('.ArticleNoShare').val(activeItem.articleNoShare);

                var secondSelect = $('#secondtSecurityLevel').find('option')

                for (var i = 0; i < secondSelect.length ; i++) {
                    if ($(secondSelect[i]).val() === activeItem.securityLevel) {
                        $(secondSelect[i]).prop("selected", "selected")
                    }
                }

                
                if (scope.msgmaintext == "") {
                    scope.scop_editor2.setContent("");
                } else {
                    scope.scop_editor2.setContent(scope.msgmaintext);
                }
                if (scope.msgfileImg == "") {
                    //$('#Img2-progress-bar').css({ width: '0%' });
                    //$('#Img2-progress-bar').text('');
                    $("#uploader2 #thelist").html("");
                    $('#image2-src').siblings('.preview').remove();

                } else {
                    if ($('#image2-src').siblings('.preview').length == 0) {
                        $('#image2-src').parent().append('<div class="preview" style="margin-top:10px;"><div style="float:left"><img style="width: 100px;height:100px;" src="' + scope.msgfileImg + '"></div></div><div style="clear:both;"></div>')
                    }
                    console.log('else');
                    $('#image2-src').parent().find('.preview').html('<div style="float:left"><img style="width: 100px;height:100px;" src="' + scope.msgfileImg + '"></div><div style="clear:both;"></div>')
                }
                var _curLi = $('.repeat-item')[index];
                jQuery(_curLi).addClass("newsActived").siblings().removeClass('newsActived');
                if (scope.isAddFunGo && last) { //循环结束给每个新闻添加默认标题
                    jQuery(_curLi).find('.title').text('标题');
                    scope.isAddFunGo = false;
                }

            };
            scope.$watch('msgtitle', function(newValue, oldValue) {
                scope.news[scope.activeId].msgtitle = newValue;
            });
            scope.$watch('msgfileImg', function(newValue, oldValue) {
                scope.news[scope.activeId].msgfileImg = newValue;
            });
            scope.$watch('msgtips', function(newValue, oldValue) {
                scope.news[scope.activeId].msgtips = newValue;
            });
            scope.$watch('msgmaintext', function(newValue, oldValue) {
                scope.news[scope.activeId].msgmaintext = newValue;
            });

            scope.$watch('btntext', function(newValue, oldValue) {
                scope.news[scope.activeId].btntext = newValue;
            });
            scope.$watch('msgarticleurl', function (newValue, oldValue) {
                scope.news[scope.activeId].msgarticleurl = newValue;
            });
            scope.$watch('IsUseComment', function(newValue, oldValue) {
                scope.news[scope.activeId].IsUseComment = newValue;
            });
            scope.$watch('showLike', function (newValue, oldValue) {
                scope.news[scope.activeId].showLike = newValue;
            });
            scope.$watch('showReadCount', function (newValue, oldValue) {
                scope.news[scope.activeId].showReadCount = newValue;
            });
            scope.$watch('showComment', function (newValue, oldValue) {
                scope.news[scope.activeId].showComment = newValue;
            });
            scope.$watch('isWatermark', function (newValue, oldValue) {
                scope.news[scope.activeId].isWatermark = newValue;
            });
            scope.$watch('articleNoShare', function (newValue, oldValue) {
                scope.news[scope.activeId].articleNoShare = newValue;
            });
            scope.$watch('securityLevel', function (newValue, oldValue) {
                scope.news[scope.activeId].securityLevel = newValue;
            });
            scope.$watch('newMaterialId', function (newValue, oldValue) {
                scope.news[scope.activeId].newMaterialId = newValue;
            });
        }
    };
});