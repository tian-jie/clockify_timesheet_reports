// click add button
function Add() {
    var appId = $('#hiddenAppId').val();
    window.location.href = "Edit?appid=" + appId;
}

function Delete(id) {
    var appId = $('#hiddenAppId').val();
    $.get(
        'CheckUsed?appid=' + appId + '&id=' + id,
        {},
        function (data) {
            if (data.hasUsed == true) {
                var d = dialog({
                    title: '提示',
                    content: '该口令正在被菜单使用, 无法删除.',
                    okValue: '确认',
                    ok: function () { },
                });
                d.showModal();
            } else {
                LEAP.Common.MainPop.RowClick(id, 2);
            }
        },
        "json"
    );
}

$(document).ready(function () {
    $("#SearchForm").bind("keydown", function (e) {
        // 兼容FF和IE和Opera    
        var theEvent = e || window.event;
        var code = theEvent.keyCode || theEvent.which || theEvent.charCode;
        if (code == 13) {
            theEvent.preventDefault();
        }
    });
    var appId = $('#hiddenAppId').val();
    $('[data-toggle="tooltip"]').tooltip({ trigger: 'hover' });

    // 自动回复一览
    LEAP.Common.MainPop.options.dataTable = $('.data-table').dataTable(jQuery.extend(true, datatableSetting, {
        "ajax": {
            "url": "GetList",
            "data": function (d) {
                // alert(arguments.callee.caller );
                ajaxData(d, datatableSetting.SearchForm, LEAP.Common.MainPop == null ? null : LEAP.Common.MainPop.options.dataTable);
                if (sessionStorage.autoReplayToIndexpage == "false" || sessionStorage.autoReplayToIndexpage == undefined) {
                    sessionStorage.autoReplay = d.start
                }
            }
        },
        "iDisplayStart": sessionStorage.autoReplayToIndexpage == "true" ? sessionStorage.autoReplay : 0,
        "paging": true,
        "info": true,
        "aoColumns": [
            { "mData": 'Name' },
            { "mData": 'Description' },
            { "mData": 'KeywordTypeName' },
            { "mData": 'UpdatedUserName' },
            { "mData": 'CreatedDate' },
            { "mData": "Operation" }
        ],
        "columnDefs": [
            {
                "targets": 3,
                "render": function (data, type, full, meta) {
                    if (data == '' || data == null) {
                        return full.CreatedUserID;
                    } else {
                        return data;
                    }
                }
            },
            {
                "targets": 5,
                "render": function (data, type, full, meta) {
                    return '<a href="Edit?appid=' + appId + '&id=' + full.Id + '" class="artDailog btn btn-info  btn-xs" style="margin-right:4px;" data-toggle="tooltip" data-placement="top" title="编辑"><i class="fa fa-pencil"></i></a>' +
                        '<a href="#" onclick="Delete(\'' + full.Id + '\');return false;" class="btn btn-danger btn-xs" data-toggle="tooltip" data-placement="top" title="删除">' +
                        '<i class="fa fa-trash-o"></i></a>';
                }
            }
        ],
        fnDrawCallback: function () {

            $('[data-toggle="tooltip"]').tooltip({ trigger: 'hover' });
            $('ul.pagination').append("<li class='paginate_button'><input type='text' style='width:30px;float:left' id='go_page'></li><li class='paginate_button'><a class='gotopage'>Go</a></li>")
            $('.gotopage').click(function () {
                var gopage = (/^[0-9]+$/).test($('#go_page').val()) ? $('#go_page').val() : "1";
                $('.data-table').dataTable().api().page(parseInt(gopage) - 1).draw(false)
            })
            sessionStorage.autoReplayToIndexpage = "false";
        }
    }));
});

