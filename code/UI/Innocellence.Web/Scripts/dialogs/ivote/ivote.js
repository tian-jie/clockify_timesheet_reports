/**
 * Created by Innocellence Wechat Group.
 * User: andrew.zheng
 * Date: 16-3-22
 * Time: ����14:56
 * To build this template use File | Settings | File Templates.
 */
//Complete UE Componet Register.
UE.registerUI('vote', function (editor, uiName) {

    //����dialog
    var dialog = new UE.ui.Dialog({
        //ָ����������ҳ���·��������ֻ��֧��ҳ��,��Ϊ��addCustomizeDialog.js��ͬĿ¼�����������·��
        iframeUrl: '/scripts/dialogs/ivote/ivote.html',
        //��Ҫָ����ǰ�ı༭��ʵ��
        editor: editor,
        //ָ��dialog������
        name: uiName,
        //dialog�ı���
        title: "insert " + uiName,

        //ָ��dialog����Χ��ʽ
        cssRules: "width:600px;height:300px;",

        //���������buttons�ʹ���dialog��ȷ����ȡ��
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

    //�ο�addCustomizeButton.js
    var btn = new UE.ui.Button({
        name: uiName,
        title: uiName,
        //��Ҫ��ӵĶ�����ʽ��ָ��iconͼ�꣬����Ĭ��ʹ��һ���ظ���icon
        cssRules: 'background-position: -500px 0;',
        onclick: function () {
            //��Ⱦdialog
            dialog.render();
            dialog.open();
        }
    });

    //���㵽�༭������ʱ����ťҪ����״̬����
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
}/*index ָ����ӵ��������ϵ��Ǹ�λ�ã�Ĭ��ʱ׷�ӵ����,editorId ָ�����UI���Ǹ��༭��ʵ���ϵģ�Ĭ����ҳ�������еı༭��������������ť*/);


