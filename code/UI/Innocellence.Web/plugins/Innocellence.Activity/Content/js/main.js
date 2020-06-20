
$(function () {
    var stopSignal = false;
    document.onkeydown = keyevent;
    var basePlace = $('.needPlace');
    var allPlace = basePlace.length; 
    var baseImgeUrl = "";

    //获取随机数
    function randomNum(min, max) {
        return parseInt(Math.random() * (max - min + 1) + min);
    }

    //弹出头像的基本代码
    function newDialogPeople(currentPeople) {
        var peopleHtml = '<div class="animatePeople"><div class="animatWrap">' +
            '<img src="' + baseImgeUrl + currentPeople.ImgUrl + '" onerror="imgError(this)" />' +
            '<div class="wrap-sign-name"><div class="sign-name">' + currentPeople.UserName + '</div></div>' +
            '</div></div>';
        $('body').append(peopleHtml);
    }

    //图像消失
    function outStart(currentPeople, count1) {
        var outpeoplePlace = $('#ready' + count1);
        var targetTop = $(outpeoplePlace).offset().top;
        var targetLeft = $(outpeoplePlace).offset().left;

        $('.animatePeople').css({
            "top": targetTop + $(outpeoplePlace).height() / 2,
            "left": targetLeft + $(outpeoplePlace).width() / 2,
            "transform": "translate(-50%,-50%) scale(0)",
        });

        //动画完事后,高亮当前用户。同时移除当前的用户html
        outpeoplePlace.addClass("activePeople");
        outpeoplePlace.find('img').attr('src', baseImgeUrl + currentPeople.ImgUrl);

        setTimeout(function () {
            $('.animatePeople').remove();
            ajaxData();
        }, 500);
    }

    //图像出现	
    function raiseStart(currentPeople, count1) {
        //新建一个新的头像
        newDialogPeople(currentPeople);

        setTimeout(function () {
            $('.animatePeople').css({
                "top": "50%",
                "transform": "translate(-50%,-50%) scale(3)",
            });
        }, 1000);

        setTimeout(function () {
            outStart(currentPeople, count1);
        }, 2000);
    }

    //初始化当前people各参数
    function getPosition(res1) {
        var currentActive = $('.activePeople');
        var count1 = $(currentActive).length + 1;
        if (res1 != null) {
            if (count1 <= allPlace) {
                raiseStart(res1, count1);
            } else {
                //位置放满后，随机取位置放
                var count2 = randomNum(1, allPlace);
                raiseStart(res1, count2);
            }
        } else {
            setTimeout(function () {
                ajaxData();
            }, 1000);
        }
    }

    //每次请求一个checkIn用户的主函数
    function ajaxData() {
        if (!stopSignal) {
            $.ajax({
                url: "/EventManage/GetEmployeeForSignIn",
                type: "get",
                cache: false,
                data: { eventId: $('#EventId').val(), screenType: $('#ScreenType').val() },
                contentType: "application/json",
                success: function (data) {
                    getPosition(data.Employee);
                }
                , error: function () {
                    ajaxData();
                }
            });
        }
    }

    //页面加载的时候，检测已经签到的人，然后点亮他们的位置
    function pagePreLoad() {
        $.ajax({
            url: "/EventManage/GetInitializedEmployeeList",
            type: "get",
            cache: false,
            data: { eventId: $('#EventId').val(), screenType: $('#ScreenType').val() },
            contentType: "application/json",
            success: function (data) {
                if (data.EmployeeList != null) {
                    var employeeList = data.EmployeeList;
                    $.each(employeeList, function (t) {
                        $('#ready' + (t + 1)).addClass("activePeople");
                        $('#ready' + (t + 1)).find('img').attr('src', baseImgeUrl + employeeList[t].ImgUrl);
                    });
                }
                ajaxData();
            },
            error: function () {
                //pagePreLoad();
            }
        });
    }

    //init 开始
    pagePreLoad();

    //仅仅是暂停
    $("#stop").click(function () {
        $(this).text() == "stop" ? $(this).text("show") : $(this).text("stop");
        stopSignal == true ? stopSignal = false : stopSignal = true;
        ajaxData();
    }); 

    $("#ready").click(function () {
        var readyArr = ["r", "e", "a", "d", "y"];
        readyArr.forEach(function (item) {
            $('.ready' + item).addClass('activePeople');
        });
    });

    function showWhite() {
        var timer = 1;
        function go() {
            if (timer > allPlace) {
                clearInterval(tt);
                $(".sign-title").fadeOut(100);
                $(".sign-ready").fadeOut(100);
                $('.whiteoverlay').fadeIn(1000);
                $('.whiteoverlay').fadeOut(500);

                setTimeout(' $("body").addClass("body--ready")', 100);
                setTimeout(' $(".sign-bg").remove()', 100);
                setTimeout(function () {
                    resetShow();
                }, 500);
                return false;
            }
            $('#ready' + timer).addClass('hideImg');
            timer++;
        }

        var tt = setInterval(function () { go(); }, 1);
    }

    function keyevent() {
        var p = window.event.keyCode;
        if ((p == 80) && (event.ctrlKey) && (event.altKey)) {

            if ($(".sign-bg").css("display") == "block") {
                stopSignal = true;//暂停checkIn
                $('.firstVideo')[0].play();
                showWhite();

                setTimeout(function () {
                    bigTitleshow();
                }, 14000);
                setTimeout(function () {
                    djs();
                }, 6000);
                setTimeout(function () {
                    gogogo();
                }, 10000);
            }
            else {
                $(".sign-bg").css("display", "block");
                $(".sign-ready").css("opacity", "1");
                $(".sign-title").css("opacity", "1");
                $(".sign-title").fadeIn(100);
                $(".sign-ready").fadeIn(100);
                $(".ready-video").css("display", "none");
                $("body").removeClass("body--ready");
                $(".last-title").removeClass("show-last-title");
                $('.needPlace').parent().removeClass('hideImg');
                $('.firstVideo')[0].pause();
                $('.firstVideo')[0].currentTime = 0;
            }
        }
    }

    function djs() {
        $('.djsVideo')[0].play();
    }

    function gogogo() {
        $('.firstVideo')[0].pause();
        $('.firstVideo')[0].currentTime = 0;
        $('.goVideo')[0].play();
    }

    function bigTitleshow() {
        $(".last-title").addClass('show-last-title');
        $('.firstVideo')[0].pause();
        $('.firstVideo')[0].currentTime = 0;
    }

    function resetShow() {
        $(".ready-video").css("display", "block");
        S.init();
    }

}); //end main


//辅助function,没用
function getreadyIds(o1o) {
    var allids = [];
    $.each(basePlace, function (i) {
        var r1 = $(basePlace[i]).attr('data-' + o1o);
        if (typeof r1 != "undefined") {
            allids.push(r1);
        }
    });
    return allids;
}

function getreadyIdsa() {
    var allids = $('.needPlace');
    $.each($('.needPlace'), function (i) {
        var r1 = $(allids[i]).attr('data-readyr');
        if (typeof r1 != "undefined") {
            $(allids[i]).parent().addClass('readyr');
        }
    });
    return allids;
}

function test2() {
    var ss = $('.readyr');
    $.each(ss, function (i) {
        var t = i + 1;
        $(ss[i]).attr('id', 'ready' + t);
    });
    $('.readyr');
}