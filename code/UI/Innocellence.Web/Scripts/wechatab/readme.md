#Wechatab
##### A plugin for wechat address book.
###### By Osborn 2016.8.1
#Install wechatab
###Prepare:
1. input appId 
2. input uri.
3. ...

###Usage
1. select element to bind this plugin eg:
>`
$(".col-lg-8").WeChatAddressBook()
`

2. some options created by default:

>`
defaults = {
                configFile:"config1.json",
                configPath:"/wechatab/assets/config/",
                path: 'wechatab',
                scriptPath: '/lib/layer/layer.js',
                cssFiles: ['/assets/css/halflings.css', '/assets/css/style.css'],
                appId: $("#hiddenAppId").val(),
                urlGetAllUser: "/backoffice/GetVisiblePersonsGroups?appId=",
                AjaxUrl: "/CAAdmin/Message/GetAllVisiblePersonGroup?appId=",
                addButtonId: 'do_add',
                addButtonForAllId: 'do_all',
                Xlogo:"ace-icon glyphicon glyphicon-remove",
                image_1: "wechatab/assets/images/icon1.png",
                image_2: "wechatab/assets/images/icon2.jpg",
                layerConfig: {
                    type: 2,
                    closeBtn: false, //不显示关闭按钮
                    btn: ['确定', '取消'],
                    title: '选择发送范围',
                    shadeClose: true,
                    shade: [0.8, '#393D49'],
                    maxmin: false, //开启最大化最小化按钮
                    area: ['700px', '500px'],
                    scrollbar: true,
                    content: ['wechatab/assets/html/GroupSelect.html?' + (new Date().getTime().toString()), 'no']
                }
`

  you can overwrite easily it by:
>`
$(".col-lg-8").WeChatAddressBook({
    Xlogo:"ace-icon glyphicon glyphicon-remove",
    AjaxUrl: "/backoffice/GetVisiblePersonsGroups",    
});
`


3. "config/config1.json" the config of popup ,if u need to diy your version, please create new config.json just like:
>`
{
"groupArr": null,
"showAllButtonId": "#show_all",
"liListId": "#list_ul",
"isCheckedGroup": false,
"goback": false,
"image_1": "../images/icon1.png",
"image_2": "../images/icon2.jpg",
"param":"appId"
}`
***




