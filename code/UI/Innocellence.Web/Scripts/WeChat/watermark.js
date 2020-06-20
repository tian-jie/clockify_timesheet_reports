/**
 * Created by YingNan.Xu on 2016/8/22.
 */

//window.onload = function () {
//    watermark({ watermark_txt: "�������ڲ���ѵ" });
//}

function watermark(settings) {

    //Ĭ������
    var defaultSettings = {
        watermark_txt: "",
        watermark_x: 20,//ˮӡ��ʼλ��x������
        watermark_y: 20,//ˮӡ��ʼλ��Y������
        watermark_rows: 1000,//ˮӡ����
        watermark_cols: 15,//ˮӡ����
        watermark_x_space: 100,//ˮӡx����
        watermark_y_space: 50,//ˮӡy����
        watermark_color: '#5C5C5C',//ˮӡ������ɫ
        watermark_alpha: 0.2,//ˮӡ͸����
        watermark_fontsize: '18px',//ˮӡ�����С
        watermark_font: '΢���ź�',//ˮӡ����
        watermark_width: 120,//ˮӡ���
        watermark_height: 35,//ˮӡ����
        watermark_angle: 30//ˮӡ��б����
    };
    //�����������滻Ĭ��ֵ����������jquery.extend
    if (arguments.length === 1 && typeof arguments[0] === "object") {
        var src = arguments[0] || {};
        for (key in src) {
            if (src[key] && defaultSettings[key] && src[key] === defaultSettings[key])
                continue;
            else if (src[key])
                defaultSettings[key] = src[key];
        }
    }

    var oTemp = document.createDocumentFragment();

    //��ȡҳ�������
    var page_width = Math.max(document.body.scrollWidth, document.body.clientWidth);
    //��ȡҳ����󳤶�
    var page_height = Math.max(document.body.scrollHeight, document.body.clientHeight);

    //�����ˮӡ��������Ϊ0����ˮӡ�������ù��󣬳���ҳ������ȣ������¼���ˮӡ������ˮӡx����
    if (defaultSettings.watermark_cols == 0 || (parseInt(defaultSettings.watermark_x + defaultSettings.watermark_width * defaultSettings.watermark_cols + defaultSettings.watermark_x_space * (defaultSettings.watermark_cols - 1)) > page_width)) {
        defaultSettings.watermark_cols = parseInt((page_width - defaultSettings.watermark_x + defaultSettings.watermark_x_space) / (defaultSettings.watermark_width + defaultSettings.watermark_x_space));
        defaultSettings.watermark_x_space = parseInt((page_width - defaultSettings.watermark_x - defaultSettings.watermark_width * defaultSettings.watermark_cols) / (defaultSettings.watermark_cols - 1));
    }
    //�����ˮӡ��������Ϊ0����ˮӡ�������ù��󣬳���ҳ����󳤶ȣ������¼���ˮӡ������ˮӡy����
    if (defaultSettings.watermark_rows == 0 || (parseInt(defaultSettings.watermark_y + defaultSettings.watermark_height * defaultSettings.watermark_rows + defaultSettings.watermark_y_space * (defaultSettings.watermark_rows - 1)) > page_height)) {
        defaultSettings.watermark_rows = parseInt((defaultSettings.watermark_y_space + page_height - defaultSettings.watermark_y) / (defaultSettings.watermark_height + defaultSettings.watermark_y_space));
        defaultSettings.watermark_y_space = parseInt(((page_height - defaultSettings.watermark_y) - defaultSettings.watermark_height * defaultSettings.watermark_rows) / (defaultSettings.watermark_rows - 1));
    }
    var x;
    var y;
    for (var i = 0; i < defaultSettings.watermark_rows; i++) {
        y = defaultSettings.watermark_y + (defaultSettings.watermark_y_space + defaultSettings.watermark_height) * i;
        for (var j = 0; j < defaultSettings.watermark_cols; j++) {
            x = defaultSettings.watermark_x + (defaultSettings.watermark_width + defaultSettings.watermark_x_space) * j;

            var mask_div = document.createElement('div');
            mask_div.id = 'mask_div' + i + j;
            mask_div.className  = 'mask_class';
            mask_div.appendChild(document.createTextNode(defaultSettings.watermark_txt));
            //����ˮӡdiv��б��ʾ
            mask_div.style.webkitTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
            mask_div.style.MozTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
            mask_div.style.msTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
            mask_div.style.OTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
            mask_div.style.transform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
            mask_div.style.visibility = "";
            mask_div.style.position = "absolute";
            mask_div.style.left = x + 'px';
            mask_div.style.top = y + 'px';
            mask_div.style.overflow = "hidden";
            mask_div.style.zIndex = "9999";
            //mask_div.style.border="solid #eee 1px";
            mask_div.style.opacity = defaultSettings.watermark_alpha;
            mask_div.style.fontSize = defaultSettings.watermark_fontsize;
            mask_div.style.fontFamily = defaultSettings.watermark_font;
            mask_div.style.color = defaultSettings.watermark_color;
            mask_div.style.textAlign = "center";
            mask_div.style.width = defaultSettings.watermark_width + 'px';
            mask_div.style.height = defaultSettings.watermark_height + 'px';
            mask_div.style.display = "block";
            mask_div.style.pointerEvents = 'none';
            oTemp.appendChild(mask_div);
        }
        ;
    }
    if ($(".mask_class").length > 0)
    {
        $(".mask_class").each(function(){
            $(this).remove();
        })
    } 
    document.body.appendChild(oTemp);
}