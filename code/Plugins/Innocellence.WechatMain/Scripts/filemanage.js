var currentType;
$(document).ready(function () {
    $('.js_toolbar li').each(function () {
        this.onclick = function () {
            debugger
            $('#container').empty();
            var type = $(this).attr("data-type");
            currentType = $(this);
            switch (type) {
                case "image":
                    $('#UploadBtn').attr("title", "Upload Image");
                    $('#UploadBtn').attr("data-target", "#ImageUploadModal");
                    break;
                case "video":
                    $('#UploadBtn').attr("title", "Upload Video");
                    $('#UploadBtn').attr("data-target", "#VideoUploadModal");
                    break;
                default:
                    $('#UploadBtn').attr("title", "");
                    $('#UploadBtn').attr("data-target", "");
                    break;
            }
            $(".js_toolbar li").removeClass("active");
            $('.js_toolbar li[data-type="' + type + '"]').addClass("active");
            window.location = "GetListByType?type=" + type;
        }
    });
});
