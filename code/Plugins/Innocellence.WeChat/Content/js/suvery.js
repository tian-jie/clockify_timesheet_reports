$(document).ready(function () {

    $('.surbox').find('.star').click(function () {
        var box = $('.surbox').find('.star');
        var max = box.length;
        var index = box.index(this);
        var sval = index + 1;
        $("#Satisfied").val(sval);
        for (var i = 0; i <= index; i++) {
            box.eq(i).find('i').removeClass("glyphicon glyphicon-star-empty").addClass("glyphicon glyphicon-star");
        }
        for (var j = index+1; j <= 3; j++) {
            box.eq(j).find('i').removeClass("glyphicon glyphicon-star").addClass("glyphicon glyphicon-star-empty");
        }
        $.ajax({
            type: 'POST',
            url: "/hreservice/QuestionManage/Satisfied",
            data: {
                id: $("#id_survey").val(),
                satisfaction: $("#Satisfied").val()
            },
            success: function (data) {

                //artDialog.alert("您的反馈已提交，感谢您的参与");
                //window.location.href = window.location.href + "?v=" + (new Date()).getTime();
                var d = dialog({
                    title: '提示',
                    content: '您的反馈已提交，感谢您的参与~',
                    okValue: '确定',
                    ok: function() {
                        window.location.href = window.location.href + "?v=" + (new Date()).getTime();
                    }

                });
                d.show();
                $(".star").off('click');
            },
        });

    });
})



