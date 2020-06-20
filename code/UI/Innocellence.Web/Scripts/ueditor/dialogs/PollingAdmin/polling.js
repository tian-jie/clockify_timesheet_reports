    // plugins/polling.js
    /**
     * video����� ΪUEditor�ṩ��Ƶ����֧��
     * @file
     * @since 1.2.6.1
     */

    UE.plugins['polling'] = function () {
        var me = this;

        /**
         * ����������Ƶ�ַ���
         * @param url ��Ƶ��ַ
         * @param width ��Ƶ���
         * @param height ��Ƶ�߶�
         * @param align ��Ƶ����
         * @param toEmbed �Ƿ���flash������ʾ
         * @param addParagraph  �Ƿ���Ҫ���P ��ǩ
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
         * ������Ƶ
         * @command insertvideo
         * @method execCommand
         * @param { String } cmd �����ַ���
         * @param { Object } videoAttr ��ֵ�Զ��� ����һ����Ƶ����������
         * @example
         * ```javascript
         *
         * var videoAttr = {
         *      //��Ƶ��ַ
         *      url: 'http://www.youku.com/xxx',
         *      //��Ƶ���ֵ�� ��λpx
         *      width: 200,
         *      height: 100
         * };
         *
         * //editor �Ǳ༭��ʵ��
         * //��༭�����뵥����Ƶ
         * editor.execCommand( 'insertvideo', videoAttr );
         * ```
         */

        /**
         * ������Ƶ
         * @command insertvideo
         * @method execCommand
         * @param { String } cmd �����ַ���
         * @param { Array } videoArr ��Ҫ�������Ƶ�����飬 ���е�ÿһ��Ԫ�ض���һ����ֵ�Զ��� ������һ����Ƶ����������
         * @example
         * ```javascript
         *
         * var videoAttr1 = {
         *      //��Ƶ��ַ
         *      url: 'http://www.youku.com/xxx',
         *      //��Ƶ���ֵ�� ��λpx
         *      width: 200,
         *      height: 100
         * },
         * videoAttr2 = {
         *      //��Ƶ��ַ
         *      url: 'http://www.youku.com/xxx',
         *      //��Ƶ���ֵ�� ��λpx
         *      width: 200,
         *      height: 100
         * }
         *
         * //editor �Ǳ༭��ʵ��
         * //�÷���������༭���ڲ���������Ƶ
         * editor.execCommand( 'insertvideo', [ videoAttr1, videoAttr2 ] );
         * ```
         */

        /**
         * ��ѯ��ǰ������ڴ��Ƿ���һ����Ƶ
         * @command insertvideo
         * @method queryCommandState
         * @param { String } cmd ��Ҫ��ѯ�������ַ���
         * @return { int } �����ǰ������ڴ���Ԫ����һ����Ƶ���� �򷵻�1�����򷵻�0
         * @example
         * ```javascript
         *
         * //editor �Ǳ༭��ʵ��
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
