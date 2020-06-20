/**
 * Created by Innocellence Wechat Group.
 * User: andrew.zheng
 * Date: 16-5-09
 * Time: 下午13:55
 * To build this template use File | Settings | File Templates.
 */
//Complete UE Componet Register.
UE.registerUI('uploadimage', function (editor, uiName) {

    //创建dialog
    var dialog = new UE.ui.Dialog({
        //指定弹出层中页面的路径，这里只能支持页面,因为跟addCustomizeDialog.js相同目录，所以无需加路径
        iframeUrl: '/scripts/dialogs/uploadimage/uploadimage.html',
        //需要指定当前的编辑器实例
        editor: editor,
        //指定dialog的名字
        name: uiName,
        //dialog的标题
        title: "Insert Image with options",

        //指定dialog的外围样式
        cssRules: "width:650px;height:430px;",

        //如果给出了buttons就代表dialog有确定和取消
        buttons: [
            {
                className: 'edui-okbutton',
                label: 'Confirm',
                onclick: function () {
                    //do my thing here
                    dialog.close(true);

                }
            },
            {
                className: 'edui-cancelbutton',
                label: 'Cancel',
                onclick: function () {
                    dialog.close(false);
                }
            }
        ]
    });

    //参考addCustomizeButton.js
    var btn = new UE.ui.Button({
        name: uiName,
        title: uiName,
        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
        cssRules: 'background-position: -726px -77px;',
        onclick: function () {
            //渲染dialog
            dialog.render();
            dialog.open();
        }
    });

    //当点到编辑内容上时，按钮要做的状态反射
    editor.addListener('selectionchange', function () {
        var state = editor.queryCommandState(uiName);
        if (state == -1) {
            btn.setDisabled(true);
            btn.setChecked(false);
        } else {
            btn.setDisabled(false);
            btn.setChecked(state);
        }
    });

    return btn;
}/*index 指定添加到工具栏上的那个位置，默认时追加到最后,editorId 指定这个UI是那个编辑器实例上的，默认是页面上所有的编辑器都会添加这个按钮*/);


UE.commands['imagefloat'] = {
    execCommand: function (cmd, align) {
        var me = this,
            range = me.selection.getRange();
        if (!range.collapsed) {
            var img = range.getClosedNode();
            if (img && img.tagName == 'IMG') {
                switch (align) {
                    case 'left':
                    case 'right':
                    case 'none':
                        var pN = img.parentNode, tmpNode, pre, next;
                        while (dtd.$inline[pN.tagName] || pN.tagName == 'A') {
                            pN = pN.parentNode;
                        }
                        tmpNode = pN;
                        if (tmpNode.tagName == 'P' && domUtils.getStyle(tmpNode, 'text-align') == 'center') {
                            if (!domUtils.isBody(tmpNode) && domUtils.getChildCount(tmpNode, function (node) {
                                return !domUtils.isBr(node) && !domUtils.isWhitespace(node);
                            }) == 1) {
                                pre = tmpNode.previousSibling;
                                next = tmpNode.nextSibling;
                                if (pre && next && pre.nodeType == 1 && next.nodeType == 1 && pre.tagName == next.tagName && domUtils.isBlockElm(pre)) {
                                    pre.appendChild(tmpNode.firstChild);
                                    while (next.firstChild) {
                                        pre.appendChild(next.firstChild);
                                    }
                                    domUtils.remove(tmpNode);
                                    domUtils.remove(next);
                                } else {
                                    domUtils.setStyle(tmpNode, 'text-align', '');
                                }


                            }

                            range.selectNode(img).select();
                        }
                        domUtils.setStyle(img, 'float', align == 'none' ? '' : align);
                        if (align == 'none') {
                            domUtils.removeAttributes(img, 'align');
                        }

                        break;
                    case 'center':
                        if (me.queryCommandValue('imagefloat') != 'center') {
                            pN = img.parentNode;
                            domUtils.setStyle(img, 'float', '');
                            domUtils.removeAttributes(img, 'align');
                            tmpNode = img;
                            while (pN && domUtils.getChildCount(pN, function (node) {
                                return !domUtils.isBr(node) && !domUtils.isWhitespace(node);
                            }) == 1
                                && (dtd.$inline[pN.tagName] || pN.tagName == 'A')) {
                                tmpNode = pN;
                                pN = pN.parentNode;
                            }
                            range.setStartBefore(tmpNode).setCursor(false);
                            pN = me.document.createElement('div');
                            pN.appendChild(tmpNode);
                            domUtils.setStyle(tmpNode, 'float', '');

                            me.execCommand('insertHtml', '<p id="_img_parent_tmp" style="text-align:center">' + pN.innerHTML + '</p>');

                            tmpNode = me.document.getElementById('_img_parent_tmp');
                            tmpNode.removeAttribute('id');
                            tmpNode = tmpNode.firstChild;
                            range.selectNode(tmpNode).select();
                            //去掉后边多余的元素
                            next = tmpNode.parentNode.nextSibling;
                            if (next && domUtils.isEmptyNode(next)) {
                                domUtils.remove(next);
                            }

                        }

                        break;
                }

            }
        }
    },
    queryCommandValue: function () {
        var range = this.selection.getRange(),
            startNode, floatStyle;
        if (range.collapsed) {
            return 'none';
        }
        startNode = range.getClosedNode();
        if (startNode && startNode.nodeType == 1 && startNode.tagName == 'IMG') {
            floatStyle = domUtils.getComputedStyle(startNode, 'float') || startNode.getAttribute('align');

            if (floatStyle == 'none') {
                floatStyle = domUtils.getComputedStyle(startNode.parentNode, 'text-align') == 'center' ? 'center' : floatStyle;
            }
            return {
                left: 1,
                right: 1,
                center: 1
            }[floatStyle] ? floatStyle : 'none';
        }
        return 'none';


    },
    queryCommandState: function () {
        var range = this.selection.getRange(),
            startNode;

        if (range.collapsed) return -1;

        startNode = range.getClosedNode();
        if (startNode && startNode.nodeType == 1 && startNode.tagName == 'IMG') {
            return 0;
        }
        return -1;
    }
};


/**
 * 插入图片
 * @command insertimage
 * @method execCommand
 * @param { String } cmd 命令字符串
 * @param { Object } opt 属性键值对，这些属性都将被复制到当前插入图片
 * @remind 该命令第二个参数可接受一个图片配置项对象的数组，可以插入多张图片，
 * 此时数组的每一个元素都是一个Object类型的图片属性集合。
 * @example
 * ```javascript
 * editor.execCommand( 'insertimage', {
 *     src:'a/b/c.jpg',
 *     width:'100',
 *     height:'100'
 * } );
 * ```
 * @example
 * ```javascript
 * editor.execCommand( 'insertimage', [{
 *     src:'a/b/c.jpg',
 *     width:'100',
 *     height:'100'
 * },{
 *     src:'a/b/d.jpg',
 *     width:'100',
 *     height:'100'
 * }] );
 * ```
 */

UE.commands['insertimage'] = {
    execCommand: function (cmd, opt) {
        opt = utils.isArray(opt) ? opt : [opt];
        if (!opt.length) {
            return;
        }
        var me = this,
            range = me.selection.getRange(),
            img = range.getClosedNode();

        if (me.fireEvent('beforeinsertimage', opt) === true) {
            return;
        }

        if (img && /img/i.test(img.tagName) && (img.className != "edui-faked-video" || img.className.indexOf("edui-upload-video") != -1) && !img.getAttribute("word_img")) {
            var first = opt.shift();
            var floatStyle = first['floatStyle'];
            delete first['floatStyle'];
            ////                img.style.border = (first.border||0) +"px solid #000";
            ////                img.style.margin = (first.margin||0) +"px";
            //                img.style.cssText += ';margin:' + (first.margin||0) +"px;" + 'border:' + (first.border||0) +"px solid #000";
            domUtils.setAttributes(img, first);
            me.execCommand('imagefloat', floatStyle);
            if (opt.length > 0) {
                range.setStartAfter(img).setCursor(false, true);
                me.execCommand('insertimage', opt);
            }

        } else {
            var html = [], str = '', ci;
            ci = opt[0];
            if (opt.length == 1) {
                str = '<img src="' + ci.src + '" ' + (ci._src ? ' _src="' + ci._src + '" ' : '') +
                    (ci.width ? ' width="' + ci.width + '" ' : '100%') +
                    (ci.height ? ' height="' + ci.height + '" ' : '') +
                    (ci['floatStyle'] == 'left' || ci['floatStyle'] == 'right' ? ' style="float:' + ci['floatStyle'] + ';"' : '') +
                    (ci.title && ci.title != "" ? ' title="' + ci.title + '"' : '') +
                    (ci.border && ci.border != "0" ? ' border="' + ci.border + '"' : '') +
                    (ci.alt && ci.alt != "" ? ' alt="' + ci.alt + '"' : '') +
                    (ci.hspace && ci.hspace != "0" ? ' hspace = "' + ci.hspace + '"' : '') +
                    (ci.vspace && ci.vspace != "0" ? ' vspace = "' + ci.vspace + '"' : '') + '/>';
                if (ci['floatStyle'] == 'center') {
                    str = '<p style="text-align: center">' + str + '</p>';
                }
                html.push(str);

            } else {
                for (var i = 0; ci = opt[i++];) {
                    str = '<p ' + (ci['floatStyle'] == 'center' ? 'style="text-align: center" ' : '') + '><img src="' + ci.src + '" ' +
                        (ci.width ? 'width="' + ci.width + '" ' : '100%') + (ci._src ? ' _src="' + ci._src + '" ' : '') +
                        (ci.height ? ' height="' + ci.height + '" ' : '') +
                        ' style="' + (ci['floatStyle'] && ci['floatStyle'] != 'center' ? 'float:' + ci['floatStyle'] + ';' : '') +
                        (ci.border || '') + '" ' +
                        (ci.title ? ' title="' + ci.title + '"' : '') + ' /></p>';
                    html.push(str);
                }
            }
            function onSuccess(rs) {
                try {
                    rs = eval("(" + rs + ")");
                    if (rs.state == 'SUCCESS') {
                        var opt = me.options;
                        me.execCommand('insertimage', {
                            src: opt.snapscreenUrlPrefix + rs.url,
                            _src: opt.snapscreenUrlPrefix + rs.url,
                            alt: rs.title || '',
                            floatStyle: opt.snapscreenImgAlign
                        });
                    } else {
                        alert(rs.state);
                    }
                } catch (e) {
                    alert(lang.callBackErrorMsg);
                }
            }
            me.execCommand('insertHtml', html.join(''));
        }

        me.fireEvent('afterinsertimage', opt);
    }
};