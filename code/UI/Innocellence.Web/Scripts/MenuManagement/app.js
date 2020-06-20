'use strict';

$(function() {
    console.log('a is loaded');
    //variable define
    var KeyData = null;
    var treeList = null;
    var $root = $('#root_ul');

    var config = {
        getTreeListUrl: '/WeChatMain/appmanage/GetListTree?appid=',
        getTypeListUrl: '/WeChatMain/AutoReply/GetList',
        postResult: '/WeChatMain/AppMenu/ModifyCategory'
    };

    var getUrlParm = function getUrlParm(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        return r != null ? decodeURI(r[2]) : null;
    };

    var synchronize = function synchronize() {
        console.log('sync');
        var $core_root = $('.core_preview');
        var data = getData();
        $core_root.html('');
        var length = data.length;
        for (var _i = 0; _i < length; _i++) {
            if (data[_i].children.length > 0) {
                $core_root.append('<div class="core_block block_' + length + '"><span class="glyphicon glyphicon-th"></span>' + data[_i].name + '</div>');
                var $block = $core_root.find('.core_block').eq($core_root.find('.core_block').length - 1);
                $block.append('<div class="core_popup_wrap hide"></div>');
                var $popup = $block.find('.core_popup_wrap');
                for (var j in data[_i].children) {
                    var item = data[_i].children[j];
                    //if (item.view === 'view') {
                    //    $popup.append('<a class="core_popup" target="_blank" href="' + item.url + '">' + item.name + '</a>');
                    //} else {
                    //    $popup.append('<a class="core_popup" href="javascript:;">' + item.name + '</a>');
                    //}
                    $popup.append('<a class="core_popup" href="javascript:;">' + item.name + '</a>');
                }
            } else {
                //if (data[_i].type === 'view') {
                //    $core_root.append('<div class="core_block block_' + length + '"><a target="_blank" href="' + data[_i].url + '">' + data[_i].name + '</a></div>');
                //} else {
                //    $core_root.append('<div class="core_block block_' + length + '"><a href="javascript:;">' + data[_i].name + '</a></div>');
                //}
                $core_root.append('<div class="core_block block_' + length + '"><a href="javascript:;">' + data[_i].name + '</a></div>');
            }
        }
        checkButtonAvailable();

        checkAddedFather();
        $root.find('ul').each(function(index, item) {
            checkAddedChild(item);
        });
    };

    var getData = function getData() {
        var result = [];
        $root.children('li').each(function(index, item) {
            var getData = getD($(item));
            //有子节点
            if ($(item).find('li:not(.add_child)').length > 0) {
                var father = {
                    id: getData('id'),
                    type: '',
                    url: '',
                    key: '',
                    name: getData('name'),
                    children: []
                };
                $(item).find('li:not(.add_child)').each(function(index, child) {
                    var getChildData = getD($(child));
                    var children = {
                        id: getChildData('id'),
                        type: getChildData('type'),
                        url: getChildData('url'),
                        key: getChildData('key'),
                        name: getChildData('name'),
                        children: []
                    };
                    father.children.push(children);
                });
                result.push(father);
            } else {
                var father = {
                    id: getData('id'),
                    type: getData('type'),
                    url: getData('url'),
                    key: getData('key'),
                    name: getData('name'),
                    children: []
                };
                result.push(father);
            }
        });
        return result;
    };

    var renderData = function renderData(data) {
        $root.empty();
        for (var j in data) {
            var item = data[j];
            var $father = $(getFatherTemplate(item));
            $root.append($father);
            if (item.children.length > 0) {
                item.children.forEach(function(child) {
                    $father.find('.add_child').before(getChildTemplate(child));
                });
                checkAddedChild($father);
            }
            checkAddedFather();
        }
    };
    //reset modal
    var resetModal = function resetModal() {
        $('#modal_id').val('');
        $('#modal_select').val('null');
        $('#key_select').val('null');
        $('#url_input').val('');
        $('#menu_title').val('');
        $('.modal_select').addClass('hide');
    };

    var getFatherTemplate = function getFatherTemplate(item) {
        return '<li class="list-group-item father" data-id="' + item.Id + '" data-type="' + item.type + '" data-url="' + item.url + '" data-key="' + item.key + '" data-name="' + item.name + '"> <span class="content_text father-title">' + item.name + '</span> <a class="icon action hide"><span class="glyphicon glyphicon-pencil"></span> </a><a class="icon action hide"><span class="glyphicon glyphicon-chevron-up"></span> </a><a class="icon action hide"><span class="glyphicon glyphicon-chevron-down"></span> </a><button class="btn float-right menu_property">\u4FEE\u6539\u83DC\u5355</button> <a class="icon float-right action hide"><span class="glyphicon glyphicon-trash"></span></a> </div><ul><li class="list-group-item add_child"><span class="add_child_button"><i class="ace-icon glyphicon glyphicon-plus"></i>&nbsp;\u6DFB\u52A0\u4E8C\u7EA7\u83DC\u5355</span></li></ul></li>';
    };
    var getChildTemplate = function getChildTemplate(item) {
        return '<li class="list-group-item child" data-id="' + item.Id + '" data-type="' + item.type + '" data-url="' + item.url + '" data-key="' + item.key + '" data-name="' + item.name + '"> <span class="content_text">' + item.name + '</span> <a class="icon action hide"><span class="glyphicon glyphicon-pencil"></span> </a><a class="icon action hide"><span class="glyphicon glyphicon-chevron-up"></span> </a><a class="icon action hide"><span class="glyphicon glyphicon-chevron-down"></span> </a><button class="btn menu_property float-right">\u4FEE\u6539\u83DC\u5355</button> <a class="icon float-right action hide"><span class="glyphicon glyphicon-trash"></span></a> </div></li>';
    };

    var getD = function getD(el) {
        return function(params) {
            return el.data(params) || '';
        };
    };
    var setD = function setD(el) {
        return function(params, value) {
            return el.data(params, value);
        };
    };

    //修改
    var bindEdit = function bindEdit() {
        $root.off('click', '.menu_property').on('click', '.menu_property', function() {
            var $li = $(this).closest('li');
            var re = getLiData($li);
            var $modal = $('#editModal');
            resetModal();
            SetModalData(re);
            var selection = re.type;
            var selects = $modal.find('.modal_select');
            switch (selection) {
                case 'null':

                    selects.addClass('hide');
                    break;
                case 'click':
                    selects.addClass('hide').eq(0).removeClass('hide');
                    break;
                case 'view':
                    selects.addClass('hide').eq(1).removeClass('hide');
                    break;
                case 'view-news-list':
                    selects.addClass('hide');
                    break;
            }
            $modal.modal({ backdrop: 'static' });
            $('#modal_submit').off('click').on('click', function() {
                var _getModalData = getModalData();

                var id = _getModalData.id;
                var type = _getModalData.type;
                var key = _getModalData.key;
                var url = _getModalData.url;
                if (!validataUrl(_getModalData)) {
                    return false;
                }
                var setData = setD($li);
                setData('id', id);
                setData('type', type);
                setData('key', key);
                setData('url', url);
                $('#editModal').modal('hide');
                synchronize();
            });
        });
    };

    var getModalData = function getModalData() {
        return {
            id: $('#modal_id').val(),
            type: $('#modal_select').val(),
            key: $('#key_select').val(),
            url: $('#url_input').val()

        };
    };
    var SetModalData = function SetModalData(re) {
        var id = re.id == "" ? 0 : re.id;
        var type = re.type || 'null';
        var key = re.key || '';
        var url = re.url || '';
        $('#modal_id').val(id);
        $('#modal_select').val(type);
        console.log(key);
        if (key) {
            $('#key_select').val(key).chosen().trigger("chosen:updated");
        }
        if (url) {
            $('#url_input').val(url);
        }
    };
    var getLiData = function getLiData(li) {
        var getData = getD($(li));
        return {
            id: getData('id'),
            type: getData('type'),
            key: getData('key'),
            url: getData('url')

        };
    };

    var getTreeList = function getTreeList(callback) {
        $.get(config.getTreeListUrl + _appId + '&time=' + Util.getTimestamp(), callback);
    };
    var getKeyList = function getKeyList(callback) {
        var $modal = $('#editModal');
        $.post(config.getTypeListUrl, { appid: _appId, 'queryAll': true, type: 2 }, function(data) {
            window.keyData = data;
            var $key_select = $modal.find('#key_select');
            for (var j in keyData.aaData) {
                var item = keyData.aaData[j];
                var key = item.Id + ":::" + item.Name;
                $key_select.append('<option value="' + item.Id + '">' + item.Name + '</option>');
            }
            callback();
        });
    };

    var bindChangeTypeSelection = function bindChangeTypeSelection() {
        var $modal = $('#editModal');
        $modal.on('change', '#modal_select', function() {
            var selection = $(this).val();
            var selects = $modal.find('.modal_select');
            switch (selection) {
                case 'null':

                    selects.addClass('hide');
                    break;
                case 'click':
                    var option = $('#key_select').find('option:first').val();
                    console.log(option)
                    $('#key_select').val(option);
                    selects.addClass('hide').eq(0).removeClass('hide');
                    break;
                case 'view':
                    selects.addClass('hide').eq(1).removeClass('hide');
                    break;
                case 'view-news-list':
                    selects.addClass('hide');
                    break;
            }
        });
    };

    var moveOrder = function moveOrder() {
        $root.off('click', '.glyphicon-chevron-up').on('click', '.glyphicon-chevron-up', function() {
            var $li = $(this).closest('li');
            if ($li.prev()) {
                $li.prev().before($li);
            }
            synchronize();
            return false;
        });
        $root.off('click', '.glyphicon-chevron-down').on('click', '.glyphicon-chevron-down', function() {
            var $li = $(this).closest('li');

            if ($li.next() && $li.next().attr('class') != 'list-group-item add_child') {
                $li.next().after($li);
            }
            synchronize();
            return false;
        });
    };

    //新增
    var addFather = function addFather() {
        //一级菜单
        $('#add_menu_button').on('click', function() {
            var li_father_temp = ' <li class="list-group-item father" data-id="0" data-type="" data-url="" data-key="" data-name="New"> <span class="content_text father-title">New</span> <a class="icon action hide"><span class="glyphicon glyphicon-pencil"></span> </a><a class="icon action"><span class="glyphicon glyphicon-chevron-up"></span> </a><a class="icon action"><span class="glyphicon glyphicon-chevron-down"></span> </a><button class="btn float-right menu_property">\u4FEE\u6539\u83DC\u5355</button> <a class="icon float-right action hide"><span class="glyphicon glyphicon-trash"></span></a> </div><ul><li class="list-group-item add_child"><span class="add_child_button"><i class="ace-icon glyphicon glyphicon-plus"></i>&nbsp;\u6DFB\u52A0\u4E8C\u7EA7\u83DC\u5355</span></li></ul></li>';
            $root.append(li_father_temp);
            checkAddedFather();
            synchronize();
        });
        //二级菜单
        $root.on('click', '.add_child_button', function() {
            var $ul = $(this).closest('ul');
            var $li = $(this).parent();

            $li.html('<input type="text" id="add_child_button"  />');
            $('#add_child_button').focus();
            var addChild = function addChild() {
                var val = $(this).val();
                if (val.trim() !== "" && val.trim().length > 0) {

                    $li.before('<li class="list-group-item child" data-id="0" data-type="" data-url="" data-key="" data-name="' + val + '"> <span class="content_text">' + val + '</span> <a class="icon action hide"><span class="glyphicon glyphicon-pencil"></span> </a><a class="icon action hide"><span class="glyphicon glyphicon-chevron-up"></span> </a><a class="icon action hide"><span class="glyphicon glyphicon-chevron-down"></span> </a><button class="btn menu_property float-right">修改菜单</button> <a class="icon float-right action hide"><span class="glyphicon glyphicon-trash"></span></a> </div></li>');
                    $li.html('<span class="add_child_button"><i class="ace-icon glyphicon glyphicon-plus"></i>&nbsp;添加二级菜单</span>');
                } else {
                    $li.html('<span class="add_child_button"><i class="ace-icon glyphicon glyphicon-plus"></i>&nbsp;添加二级菜单</span>');
                }
                checkAddedChild($ul);
                checkMenuProperty($ul.parent('li'));
                synchronize();
            };
            $li.off('blur', '#add_child_button').on('blur', '#add_child_button', addChild);
            $li.off('keypress', '#add_child_button').on('keypress', '#add_child_button', function(e) {

                if (e.keyCode === 13) {
                    $(this).blur();
                }
            });
            synchronize();
        });
    };
    var validataUrl = function(data) {
        var reg = "[a-zA-z]+://[^\s]*";
        var re = new RegExp(reg);
        if (data.type == 'view') {
            if (!re.test(data.url)) {
                layer.msg("URL不合法!");
                return false;
            }
        }
        return true;
    };
    var validate = function validate() {
        var validate = true;
        var message = '';
        $root.find('button:visible').each(function(index, item) {
            var $li = $(item).closest('li');
            if ($(item).hasClass('btn-danger')) {
                validate = false;
                message = '请填写菜单类型';
            };
        });
        return {
            validate: validate,
            message: message
        };
    };

    var checkButtonAvailable = function checkButtonAvailable() {
        $root.find('li:not(.add_child)').each(function(index, item) {
            var type = $(item).data('type');
            if (type == '' || type == null || type == 'null' || type == 'undefined') {
                $(item).find('button').removeClass('btn-success').addClass('btn-danger');
            } else {
                $(item).find('button').removeClass('btn-danger').addClass('btn-success');
            }
        });
    };
    var checkAddedFather = function checkAddedFather() {
        var len = $root.children('.father').length;
        if (len >= 3) {
            $('.bar_right').fadeOut();
        } else {
            $('.bar_right').fadeIn();
        }
    };
    var checkAddedChild = function checkAddedChild(ul) {
        var len = $(ul).children('.child').length;
        if (len >= 5) {
            $(ul).children('.add_child').fadeOut();
        } else {
            $(ul).children('.add_child').fadeIn();
        }
    };
    var checkMenuButton = function checkMenuButton() {
        var $li = $root.children('li');
        $li.each(function(index, item) {
            if ($(item).find('li:not(.add_child)').length > 0) {
                $(item).children('.menu_property').hide();
            } else {
                $(item).children('.menu_property').show();
            }
        });
    };

    var checkMenuProperty = function checkMenuProperty(li) {
        if ($(li).find('li:not(.add_child)').length > 0) {
            $(li).children('.menu_property').fadeOut();
        } else {
            $(li).children('.menu_property').fadeIn();
        }
    };

    var bindOthers = function bindOthers() {
        $root.on('mouseover', '.list-group-item', function(event) {
            $(this).children('.action').removeClass('hide');

            if ($(this).prev().length == 0) {

                $(this).find('.glyphicon-chevron-up').parent().addClass('hide');
            }
            if ($(this).next().length == 0 || $(this).next().attr('class') == 'list-group-item add_child') {

                $(this).find('.glyphicon-chevron-down').parent().addClass('hide');
            }

            event.stopPropagation();
            return false;
        });
        $root.on('mouseout', '.list-group-item', function(event) {
            $(this).children('.action').addClass('hide');
            event.stopPropagation();
            return false;
        });

        //修改title
        $root.on('click', '.glyphicon-pencil', function() {
            var $this_text = $(this).parent().siblings('.content_text');
            var $li = $(this).closest('li');
            if ($this_text.find('input').length > 0) {
                var $input_text_val = $this_text.find('input').val();
                if ($input_text_val && $input_text_val.length > 0) {
                    $this_text.html($input_text_val);
                    $li.data('name', $input_text_val);
                } else {
                    $this_text.html('Null');
                    $li.data('name', $input_text_val);
                }
                synchronize();
            } else {
                var $this_text_val = $this_text.text().trim();
                $this_text.html('<input class="edit_text" type="text" value="' + $this_text_val + '" />');
                $this_text.find('input').focus();
            }

            $this_text.off('blur', 'input').on('blur', 'input', function() {
                var $input_val = $(this).val().trim() == "" ? "Null" : $(this).val();
                $this_text.html($input_val);
                $li.data('name', $input_val);
                synchronize();
            });
            return false;
        });

        //删除
        $root.on('click', '.glyphicon-trash', function() {
            if (!confirm('确认删除?')) {
                return false;
            }
            var $ul = $(this).closest('ul');
            var $li = $(this).closest('li');
            $li.remove();
            if ($ul.attr('id') === 'root_ul') {
                //父节点删除
                checkAddedFather();
            } else {
                checkAddedChild($ul);
                checkMenuProperty($ul.parent('li'));
            }
            synchronize();
        });
        //提交
        $('#menu_submit').on('click', function() {
            var result = getData();
            var valid = validate();
            if (!valid.validate) {
                layer.msg(valid.message);
                return false;
            }
            $.post(config.postResult, { buttons: result, appId: getUrlParm('appid') }, function(data) {
                if (data.Message.Text === 'Success') {
                    layer.msg('成功');

                }

            });
        });

        var $core_preview = $('.core_preview');
        //二级窗口
        $core_preview.on('click', '.core_block', function(e) {
            //            e.stopPropagation();
            var $wrap = $(this).find('.core_popup_wrap');
            if ($wrap.length > 0) {

                if ($wrap.hasClass('hide')) {
                    $core_preview.find('.core_popup_wrap').addClass('hide');
                    $wrap.removeClass('hide');
                } else {
                    $core_preview.find('.core_popup_wrap').addClass('hide');
                }
            }
            // return false;
        });
        //搜索select

       
        $('#key_select').chosen({
            search_contains:true
        });
        $('#key_select_chosen').width('70%');
    };

    //------------------------run---
    var loading = layer.msg('加载中', { icon: 16 });
    var _appId = getUrlParm("appid") || 0;
    getTreeList(function(data) {
        getKeyList(function() {
            treeList = data.menu;
            //获取tree结构
            renderData(treeList);
            bindChangeTypeSelection();
            //绑定新增
            addFather();
            //更新preview栏目
            synchronize();
            //绑定property
            checkMenuButton();
            //绑定移动
            moveOrder();
            //绑定修改
            bindEdit();

            bindOthers();
            layer.close(loading);
        });
    });
});