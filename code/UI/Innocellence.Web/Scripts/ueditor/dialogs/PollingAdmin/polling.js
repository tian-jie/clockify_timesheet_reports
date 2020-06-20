    // plugins/polling.js
    /**
     * video插件， 为UEditor提供视频插入支持
     * @file
     * @since 1.2.6.1
     */

    UE.plugins['polling'] = function () {
        var me = this;

        /**
         * 创建插入视频字符窜
         * @param url 视频地址
         * @param width 视频宽度
         * @param height 视频高度
         * @param align 视频对齐
         * @param toEmbed 是否以flash代替显示
         * @param addParagraph  是否需要添加P 标签
         */
        function creatInsertStr(pollingname, pollingid, classname, type) {
            var str;
            switch (type) {
                case 'image':
                    str = '<span>' + pollingname + '</span>' + '<img src="/Scripts/dialogs/polling/polling.jpg" width="80%" ';
                    break;
                case 'polling':
                    str = '<polling' + (pollingid ? ' pollingid="' + pollingid + '"' : '') + ' class="' + classname + ' video-js" ' + 'pollingname=' + pollingname + '/>';
                    break;
            }
            return str;
        }

        function switchImgAndPolling(root, img2Polling) {
            utils.each(root.getNodesByTagName(img2Polling ? 'img' : 'polling'), function (node) {
                var className = node.getAttr('class');

                if (className && className.indexOf('edui-faked-polling') != -1) {
                    var html = creatInsertStr(node.getAttr('_name'), node.getAttr('_id'), clasName, img2Polling ? 'polling' : 'image');
                    node.parentNode.replaceChild(UE.uNode.createElement(html), node);
                }

            });
        }

        me.addOutputRule(function (root) {
            switchImgAndPolling(root, true);
        });
        me.addInputRule(function (root) {
            switchImgAndPolling(root);
        });

        /**
         * 插入视频
         * @command insertvideo
         * @method execCommand
         * @param { String } cmd 命令字符串
         * @param { Object } videoAttr 键值对对象， 描述一个视频的所有属性
         * @example
         * ```javascript
         *
         * var videoAttr = {
         *      //视频地址
         *      url: 'http://www.youku.com/xxx',
         *      //视频宽高值， 单位px
         *      width: 200,
         *      height: 100
         * };
         *
         * //editor 是编辑器实例
         * //向编辑器插入单个视频
         * editor.execCommand( 'insertvideo', videoAttr );
         * ```
         */

        /**
         * 插入视频
         * @command insertvideo
         * @method execCommand
         * @param { String } cmd 命令字符串
         * @param { Array } videoArr 需要插入的视频的数组， 其中的每一个元素都是一个键值对对象， 描述了一个视频的所有属性
         * @example
         * ```javascript
         *
         * var videoAttr1 = {
         *      //视频地址
         *      url: 'http://www.youku.com/xxx',
         *      //视频宽高值， 单位px
         *      width: 200,
         *      height: 100
         * },
         * videoAttr2 = {
         *      //视频地址
         *      url: 'http://www.youku.com/xxx',
         *      //视频宽高值， 单位px
         *      width: 200,
         *      height: 100
         * }
         *
         * //editor 是编辑器实例
         * //该方法将会向编辑器内插入两个视频
         * editor.execCommand( 'insertvideo', [ videoAttr1, videoAttr2 ] );
         * ```
         */

        /**
         * 查询当前光标所在处是否是一个视频
         * @command insertvideo
         * @method queryCommandState
         * @param { String } cmd 需要查询的命令字符串
         * @return { int } 如果当前光标所在处的元素是一个视频对象， 则返回1，否则返回0
         * @example
         * ```javascript
         *
         * //editor 是编辑器实例
         * editor.queryCommandState( 'insertvideo' );
         * ```
         */
        me.commands["insertpolling"] = {
            execCommand: function (cmd, pollingObjs) {
                var html = [], id = 'tmpPolling', cl;
                cl = 'edui-faked-polling';
                html.push(creatInsertStr(pollingObjs.name, pollingObjs.id, cl, 'image'));


                me.execCommand("inserthtml", html.join(""), true);
                var rng = this.selection.getRange();


                var img = this.document.getElementById('tmpVideo' + i);
                domUtils.removeAttributes(img, 'id');
                rng.selectNode(img).select();
                me.execCommand('imagefloat', pollingObjs[i].align);

            },
            queryCommandState: function () {
                var img = me.selection.getRange().getClosedNode(),
                    flag = img && img.className == "edui-faked-polling" ;
                return flag ? 1 : 0;
            }
        };
    };
