(function ($, window, document, undefined) {
    $.fn.LEAPDataBind = function (options) {
        var defaults = {
            pageSize: 10,
            pagerId: "page",
            renderHtml: "",
            renderId: 'leaplist',
            url: 'GetList',
            isPage: true,
            data: "",
            isTotal: "",
            noDataDo: function () { }, //ajax请求成功之后json为空后执行
            noListDataDo: function () { },
            renderSuccess: function () { },
            needNullData: true,
        };
        var settings = $.extend({}, defaults, options);

        if (!settings.isPage) {
            list(settings);
            return;
        }

        var p = new Page(settings.pagerId);
        p.numericButtonCount = 5;
        p.pageSize = settings.pageSize;
        p.addListener('pageChanged', function () {
            page_list(p, settings);
        });
        p.initialize();
    };

    var list = function (settings) {
        var para = "iRecordsTotal=" + 0 + "&length=" + settings.pageSize + "&start=" + 0 + "&" + settings.data;
        $.ajax({
            url: settings.url,
            type: "post",
            data: para,
            success: function (jsondata) {
                BindData(jsondata, settings);
                settings.renderSuccess();
                if ((jsondata.iTotalRecords == "" || jsondata.iTotalRecords == 0)) {
                    if (settings.needNullData) {
                        $("#" + settings.renderId).html("<p class='col-md-12' style='color:#999'>" + nullData + "</p>");
                    }
                    settings.noListDataDo();
                    return 0;
                }
            }
        });
    }

    var page_list = function (page, settings) {
        var para = "iRecordsTotal=" + page.recordCount + "&length=" + page.pageSize + "&start=" + ((page.pageIndex - 1) * page.pageSize) + "&" + settings.data;
        $("#page").css('display', 'inline');
        $.ajax({
            url: settings.url,
            type: "post",
            data: para,
            success: function (jsondata) {
                BindData(jsondata, settings);
                settings.renderSuccess();
                page.recordCount = jsondata.iTotalRecords;
                page.render();
                if (page.recordCount == "" || page.recordCount == 0) {
                    if (settings.needNullData) {
                        $("#" + settings.renderId).html("<p class='col-md-12' style='color:#999'>" + nullData + "</p>");
                    }
                    settings.noDataDo();
                    $("#" + settings.isTotal).text(page.recordCount);
                    $("#page").css('display', 'none');
                    return 0;
                }
                if (settings.isTotal != "") {
                    $("#" + settings.isTotal).text(page.recordCount);
                }

            }
        });

    };

    var BindData = function (data, settings) {
        var template = "<script type='text/x-jsrender' id='leap_list'>" + settings.renderHtml + "</script>";
        $('#' + settings.renderId).empty();
        $('#' + settings.renderId).append(template);
        var jsview = $.templates("#leap_list");
        jsview.link('#' + settings.renderId, data.aaData);
    }

})(jQuery, window, document);


