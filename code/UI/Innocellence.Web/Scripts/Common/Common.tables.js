
var LEAP = {};
LEAP.Common = {};
LEAP.Common.Controller = '.';

var datatableSetting = {
    "serverSide": true,
    "Paginat": true,
    bAutoWidth: false,
    "bLengthChange": false,
    "searching": false,
    //"sDom": '<"H"lfr>t<"F"ip>',
    "ordering": false,
    "SearchForm": "SearchForm",
    "ajax": {
        "url": LEAP.Common.Controller + "/GetList",
        "type": 'post',
        "data": function (d) {
            // alert(arguments.callee.caller );
            ajaxData(d, datatableSetting.SearchForm, LEAP.Common.MainPop == null ? null : LEAP.Common.MainPop.options.dataTable);

        },
        error: function (xhr, error, thrown) {
            artDialog.alert("Server Error!Code:" + error + " Message:" + thrown);
        }
    },

    "columnDefs": [
    //    {
    //    "targets": 1,
    //    "render": function (data, type, full, meta) {
    //        return '<a href="#" onclick="LEAP.Common.MainPop.ShowUpdateInfo(' + full.Id + ');return false;" class="artDailog">' + data + '</a>';
    //    }
    //},
    {
        "targets": 0,
        "render": function (data, type, full, meta) {
            return '<input type="checkbox" value="' + data.Id + '" title="' + data + '" id="checkbox" />';
        }
    },
    ],
    language: {
        "sProcessing": "处理中...",
        "sLengthMenu": "显示 _MENU_ 项结果",
        "sZeroRecords": "没有匹配结果",
        "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项",
        "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项",
        "sInfoFiltered": "(由 _MAX_ 项结果过滤)",
        "sInfoPostFix": "",
        "sSearch": "搜索:",
        "sUrl": "",
        "sEmptyTable": "表中数据为空",
        "sLoadingRecords": "载入中...",
        "sInfoThousands": ",",
        "oPaginate": {
            "sFirst": "首页",
            "sPrevious": "< 上一页",
            "sNext": "下一页 >",
            "sLast": "末页"
        },
        "oAria": {
            "sSortAscending": ": 以升序排列此列",
            "sSortDescending": ": 以降序排列此列"
        }
    }
};

//支持一个页面多个table绑定
function ajaxData(d, SearchForm, dTable) {
    // search condition
    // d.searchTxt = $('#search_value').val();
    var dSearch = $("#" + SearchForm).serializeArray();
    // jQuery.merge(d, dSearch);
    $.each(dSearch, function (key, val) {
        d[val.name.toUpperCase()] = val.value;
    });

    // alert(LEAP.Common.MainPop.updateFlag);

    //var dTable = LEAP.Common.MainPop.options.dataTable;

    //增加的时候需要将iRecordsTotal清零，否则不会重新计算
    if (dTable == undefined || dTable == null || !LEAP.Common.MainPop.updateFlag) {
        d.iRecordsTotal = 0;
        LEAP.Common.MainPop.updateFlag = true;
    } else {
        d.iRecordsTotal = dTable.fnSettings().fnRecordsTotal();
    }

    if (window.location.href.indexOf("?") > 0) {
        var arr = window.location.search.slice(1).replace(/\+/g, ' ').split('&');
        var result = undefined;
        for (var i = 0; i < arr.length; i++) {
            var arrHref = arr[i].split('=');
            if (arrHref[0] != '') {
                d[arrHref[0].toUpperCase()] = arrHref[1];
            }
        }
    }
}




$(document).ready(function () {


    //$("span.icon input:checkbox, th input:checkbox").click(function() {
    //	var checkedStatus = this.checked;
    //	var checkbox = $(this).parents('.widget-box').find('tr td:first-child input:checkbox');		
    //	checkbox.each(function() {
    //		this.checked = checkedStatus;
    //		if (checkedStatus == this.checked) {
    //			$(this).closest('.checker > span').removeClass('checked');
    //		}
    //		if (this.checked) {
    //			$(this).closest('.checker > span').addClass('checked');
    //		}
    //	});
    //});

    //$('.modal select[data-ajax--url]').each(function () {
    //    var url = $(this).attr('data-ajax--url');
    //    $(this).removeAttr('data-ajax--url');
    //    $(this).prop('data-url', url);
    //});

    //fix a bug when a select2 in modal(search input cannot focus)
    $.fn.modal.Constructor.prototype.enforceFocus = function () { };

    //  $('select[class!=hidden]').not('[class*=easyui]').select2({ minimumResultsForSearch: Infinity });

    $('[data-rel=tooltip]').tooltip();


    //$('.date-picker').datepicker({
    //    autoclose: true,
    //    todayHighlight: true
    //}).next().on(ace.click_event, function () {
    //    $(this).prev().focus();//show datepicker when clicking on the icon
    //});

    //$('.datetime-picker').datetimepicker({
    //    autoclose: true,
    //    todayHighlight: true
    //}).next().on(ace.click_event, function () {
    //    $(this).prev().focus();//show datepicker when clicking on the icon
    //});


    $(document).on('click', 'th input:checkbox', function () {
        var that = this;
        $(this).closest('table').find('tr > td:first-child input:checkbox')
        .each(function () {
            this.checked = that.checked;
            $(this).closest('tr').toggleClass('selected');
        });
    });


    $('[data-rel="tooltip"]').tooltip({ placement: tooltip_placement });
    function tooltip_placement(context, source) {
        var $source = $(source);
        var $parent = $source.closest('table');
        var off1 = $parent.offset();
        var w1 = $parent.width();

        var off2 = $source.offset();
        //var w2 = $source.width();

        if (parseInt(off2.left) < parseInt(off1.left) + parseInt(w1 / 2)) return 'right';
        return 'left';
    }

    $.metadata.setType("attr", "validate");

    LEAP.Common.MainPop = $("#ModalTable").formPopup();

    $('#btnAdd').on('click', function () {
        return LEAP.Common.MainPop.TableButtonClick(0, 0);
    });
    $('#btnUpdate').on('click', function () {
        return LEAP.Common.MainPop.TableButtonClick(0, 1);
    });
    $('#btnDelete').on('click', function () {
        return LEAP.Common.MainPop.TableButtonClick(0, 2);
    });

    $('#btnSearch').on('click', function () {
        if (!BeforeSearch()) {
            return;
        }
        return LEAP.Common.MainPop.TableSearchClick();
    });

    $('#btnExport').on('click', function () {

        var para = '';

        var dSearch = $("#" + datatableSetting.SearchForm).serializeArray();
        if (!BeforeExport(dSearch)) {
            return false;
        }

        $.each(dSearch, function (key, val) {
            para += val.name + '=' + val.value + '&';
        });

        $.download("Export", para + "t=" + (new Date()).getTime());
        return true;
    });

    $('#btnPublish').on('click', function () {
        //点击批量publish 获取当前所有选择的列 
        $('#nestable .dd-list').html("");
        var tableId = LEAP.Common.MainPop.options.dataTable;
        var selectedRows = GetSelectedRows(tableId, null);

        if (selectedRows.length <= 0) {
            artDialog.alert("请至少选择一个数据.");
            return false;
        }

        if (selectedRows.length > 10) {
            artDialog.alert("一次最多选择10条新闻.");
            return false;
        }

        for (var i = 0; i < selectedRows.length; i++) {
            if (selectedRows[i].ArticleStatus != "Saved") {
                artDialog.alert("不能选择已发布的新闻, 请先取消发布!");
                return false;
            }
            $('#nestable .dd-list').append('<li class="dd-item item-blue2" data-id="' + selectedRows[i].Id + '"><div class="dd-handle">'
	            + selectedRows[i].ArticleTitle
	            + '</div></li>');
        }
    });

    $('#' + datatableSetting.SearchForm + ',body #' + datatableSetting.SearchForm).on('keydown', function (envent) {
        if (event.which == 13) {
            $(this).find('#btnSearch').trigger('click');
            return false;
        }
    });

});

function BeforeSearch() {
    return true;
}

function BeforeExport(objForms) {
    return true;
}

function GetSelectedRows(tableId, id) {

    var aReturn = new Array();
    var i = 0; var iIndex = 0;
    var checkbox = $(tableId).find('tr td:first-child input:checkbox');
    checkbox.each(function () {

        if (id != null) {
            if (tableId.fnSettings().aoData[iIndex]._aData.Id == id) {
                aReturn[0] = tableId.fnSettings().aoData[iIndex]._aData;
                return aReturn;
            }
        } else if (this.checked) {
            aReturn[i++] = tableId.fnSettings().aoData[iIndex]._aData;
        }
        iIndex++;

    });

    return aReturn;
}

function ChangeStatus(id, appid, obj, url, appname, needDialog) {

    var bol = false;
    needDialog = false;
    if ($(obj).prev('span').html() == "已激活") {
        bol = true;
    }
    if (needDialog) {
        var d = dialog({
            title: '提示',
            content: '你是否确认' + (bol ? '撤消' : '') + '激活?'
                      + (bol ? '' : '<br/><br/><br/><input type="checkbox" name="ispush" style="margin-top:2px;"/>&nbsp;是否推送至<b>' + appname + '</b>所有用户？'),
            okValue: '确认',
            ok: function () {
                var ispush;
                ispush = $('input[name="ispush"]').is(":checked") ? true : false;

                $.ajax({
                    type: 'Get',
                    url: url + '?Id=' + id + '&ispush=' + ispush + '&appid=' + appid,
                    cache: false,
                    success: function (data) {
                        if (ReturnValueFilter(data)) {
                            $(obj).toggleClass('btn-success');
                            $(obj).toggleClass('btn-danger');

                            if ($(obj).html() == '<i class="fa fa-cloud-download"></i>') {
                                $(obj).html('<i class="fa fa-cloud-upload"></i>');
                                $(obj).prev().html('已保存');
                            } else {
                                $(obj).html('<i class="fa fa-cloud-download"></i>');
                                $(obj).prev().html('已激活');
                            }
                            if (LEAP.Common.MainPop && LEAP.Common.MainPop.options.dataTable) {
                                LEAP.Common.MainPop.options.dataTable.fnDraw(true);
                            }
                        }
                    }
                });
                return true;
            },
            cancelValue: '取消',
            cancel: function () { }
        });
        d.showModal();
    } else {
        var tableId = LEAP.Common.MainPop.options.dataTable;
        var selectedRows = GetSelectedRows(tableId, null);
        if (selectedRows.length > 0) {
            var arrayList = new Array();
            for (var i = 0; i < selectedRows.length; i++) {
                arrayList.push(selectedRows[i].Id);
            }
            $.ajax({
                type: 'Post',
                url: 'ChangeStatusBatch',
                data: { "Ids": arrayList, isNotification: false },
                cache: false,
                success: function (data) {
                    if (ReturnValueFilter(data)) {
                        if (LEAP.Common.MainPop && LEAP.Common.MainPop.options.dataTable) {
                            LEAP.Common.MainPop.options.dataTable.fnDraw(true);
                        }
                    }
                }
            });
        } else {
            $.ajax({
                type: 'Get',
                url: url + '?Id=' + id + '&ispush=' + false + '&appid=' + appid,
                cache: false,
                success: function (data) {
                    if (ReturnValueFilter(data)) {
                        $(obj).toggleClass('btn-success');
                        $(obj).toggleClass('btn-danger');

                        if ($(obj).html() == '<i class="fa fa-cloud-download"></i>') {
                            $(obj).html('<i class="fa fa-cloud-upload"></i>');
                            $(obj).prev().html('已保存');
                        } else {
                            $(obj).html('<i class="fa fa-cloud-download"></i>');
                            $(obj).prev().html('已激活');
                        }
                        if (LEAP.Common.MainPop && LEAP.Common.MainPop.options.dataTable) {
                            LEAP.Common.MainPop.options.dataTable.fnDraw(true);
                        }
                    }
                }
            });
        }
    }

}

function ChangeStatusCheckSchedule(id, appid, obj, url, scheduleTime) {

    var bol = false;
    var needDialog = false;
    var nowTime = new Date();
    if (scheduleTime != "")
    {
        //var scheduleTimeToDate = eval('new ' + (scheduleTime.replace(/\//g, '')));
        var scheduleTimeToDate = new Date(scheduleTime);
        needDialog = scheduleTimeToDate > nowTime ? true:false
    }  
    if ($(obj).prev('span').html() == "已激活") {
        bol = true;
    }
    if (needDialog) {
        var d = dialog({
            title: '提示',
            content: '该条图文正处于定时推送中，推送时间为' + scheduleTime 
                      + ('<br/><br/>将此图文设置为激活状态将取消定时推送，请确认是否激活'),
            okValue: '确认',
            ok: function () {
                var ispush;
                ispush = $('input[name="ispush"]').is(":checked") ? true : false;

                $.ajax({
                    type: 'Get',
                    url: url + '?Id=' + id + '&ispush=' + ispush + '&appid=' + appid,
                    cache: false,
                    success: function (data) {
                        if (ReturnValueFilter(data)) {
                            $(obj).toggleClass('btn-success');
                            $(obj).toggleClass('btn-danger');

                            if ($(obj).html() == '<i class="fa fa-cloud-download"></i>') {
                                $(obj).html('<i class="fa fa-cloud-upload"></i>');
                                $(obj).prev().html('已保存');
                            } else {
                                $(obj).html('<i class="fa fa-cloud-download"></i>');
                                $(obj).prev().html('已激活');
                            }
                            if (LEAP.Common.MainPop && LEAP.Common.MainPop.options.dataTable) {
                                LEAP.Common.MainPop.options.dataTable.fnDraw(true);
                            }
                        }
                    }
                });
                return true;
            },
            cancelValue: '取消',
            cancel: function () { }
        });
        d.showModal();
    }
    else {
        //var tableId = LEAP.Common.MainPop.options.dataTable;
        //var selectedRows = GetSelectedRows(tableId, null);
        $.ajax({
            type: 'Get',
            url: url + '?Id=' + id + '&ispush=' + false + '&appid=' + appid,
            cache: false,
            success: function (data) {
                if (ReturnValueFilter(data)) {
                    $(obj).toggleClass('btn-success');
                    $(obj).toggleClass('btn-danger');

                    if ($(obj).html() == '<i class="fa fa-cloud-download"></i>') {
                        $(obj).html('<i class="fa fa-cloud-upload"></i>');
                        $(obj).prev().html('已保存');
                    } else {
                        $(obj).html('<i class="fa fa-cloud-download"></i>');
                        $(obj).prev().html('已激活');
                    }
                    if (LEAP.Common.MainPop && LEAP.Common.MainPop.options.dataTable) {
                        LEAP.Common.MainPop.options.dataTable.fnDraw(true);
                    }
                }
            }
        });

    }

}