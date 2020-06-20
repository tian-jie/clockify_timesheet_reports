'use strict';

(function($) {
    function getUrlParam(parameters) {
        var reg = new RegExp("(^|&)" + parameters + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]);
        return null;
    }
    
    $.fn.extend({
        materialControl: function materialControl(options) {
            var element = this;
            var defaults = {
                bind: '',
                category: 'image',

                layerConfig: {
                    type: 2,
                    closeBtn: false, //不显示关闭按钮
                    btn: ['确定', '取消'],
                    title: '选择素材',
                    shadeClose: true,
                    offset: [$(window).height()/2 - 300 ],
                    shade: [0.8, '#393D49'],
                    maxmin: false, //开启最大化最小化按钮
                    area: ['800px', '600px'],
                    scrollbar: true,
                    content: ['/Scripts/materialControl/html/GroupSelect.html?' + new Date().getTime().toString() + '&appid=' + getUrlParam('appid') + '&category=', 'no']
                }
            };
            var conf = {
                'IMAGE_1': { id: 1, el: '#image1-src', upload: '#uploader1', father: '#firstItem' },
                'IMAGE_2': { id: 1, el: '#image2-src', upload: '#uploader2', father: '#secondItem' },
                'IMAGE_3': { id: 1, el: '#image3-src', upload: '#uploader', father: '#msgType-pic' },
                'AUDIO': { id: 2, el: '#image5-src', upload: '#uploader5', father: '#msgType-sound' },
                'VIDEO': { id: 3, el: '#image4-src', upload: '#uploader4', father: '#msgType-video' },
                'FILE': { id: 4, el: '#image6-src', upload: '#uploader6', father: '#msgType-file' },
                'AUTO_NEWS': { id: 5, father: '#msgType-wordpic' },
            };
            options = $.extend(defaults, options);
            options.layerConfig.content[0] += options.category;
            var methods = {
                setUp: function setUp(ele) {
                    $(ele).parent().html('<a href="javascript:void(0);" ><span class="message_edition_title_link btn btn-white btn-primary"><i class="ace-icon fa fa-folder-open"></i>选择素材</span></a>');
                },
                bindButton: function bindButton() {
                    $(conf[options.category.toUpperCase()].father).on('click', '.message_edition_title_link', this.layerOpen);
                },
                layerOpen: function layerOpen() {
                    var layerYes = function layerYes(index) {};
                    var layerCancel = function layerCancel(index) {
                        layer.close(index);
                    };
                    var layerEnd = function layerEnd() {
                        var el = conf[options.category.toUpperCase()].el;

                        if ($(el).val()) {
                            $(el).change();
                        }
                        var father = conf[options.category.toUpperCase()].father;
                        $(father).find('input').change();
                        $(father).find('textarea').change();
                        $('input.form-control').change();

                    };
                    options.layerConfig.yes = layerYes;
                    options.layerConfig.cancel = layerCancel;
                    options.layerConfig.end = layerEnd;
                    console.log(options.layerConfig);
                    layer.open(options.layerConfig);
                    return false;
                }
            };

            (function init(options) {

                methods.bindButton();
                methods.setUp(element);
            })(options);
        }
    });
})($);

//# sourceMappingURL=materialControl-compiled.js.map

//# sourceMappingURL=materialControl_bk-compiled.js.map