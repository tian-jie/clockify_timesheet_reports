$(function () {
    //获取url 参数值
    $.getUrlParam = function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }

    var myClientId = $('#currentUserId').val();

    var appid = $.getUrlParam('wechatid');
    // Proxy created on the fly
    var chat = $.connection.chat;
    // Declare a function on the chat hub so the server can invoke it
    chat.client.sendMessage = function (clientId, name, message, headerimg) {
        if (clientId == myClientId) {
            writeEventAfter(name, message, 'self', 'text', headerimg);
        }
        else {
            writeEventAfter(name, message, 'other', 'text', headerimg);
        }
        $('#messages').scrollTop($('#wrapper').height());
    };
    chat.client.receiveImage = function (clientId, name, message, headerimg) {
        // 向页面添加消息
        if (clientId == myClientId) {
            writeEventAfter(name, message, 'self', 'image', headerimg);
        }
        else {
            writeEventAfter(name, message, 'other', 'image', headerimg);
        }
        $('#messages').scrollTop($('#wrapper').height());
    };

    // Start the connection
    $.connection.hub.start().done(function () {
        chat.server.group(appid);
        $("#broadcast").click(function () {
            // Call the chat method on the server
            if ($('#msg').val() !== null && $('#msg').val() !== "") {
                chat.server.send(myClientId, $('#msg').val(), appid)
                               .done(function (data) {
                                   $('#messages').scrollTop($('#wrapper').height());
                                   $('#msg').val("")
                                   $('#messages').scrollTop($('#messages').height());
                               })
                               .fail(function (e) {

                               });

            }
           
        });
        $('.site-chat__plus--button').on("click", function () {
            var scope = this;
            wx.chooseImage({
                count: 1, // 默认9
                sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
                sourceType: ['album', 'camera'], // 可以指定来源是相册还是相机，默认二者都有
                success: function (res) {
                    var localIds = res.localIds; // 返回选定照片的本地ID列表，localId可以作为img标签的src属性显示图片
                    imageUpload(localIds[0], function (res) {                            // 发送图片测试
                        chat.server.sendImage(myClientId, res.serverId, appid)
                           .done(function (data) {
                               $('#messages').scrollTop($('#wrapper').height());
                           })
                           .fail(function (e) {
                           });;

                    });
                }
            });
        })   
    });;
    

   
    
    //绑定回车事件
    //$('#msg').bind('keyup', function (event) {
    //    if (event.keyCode == "13") {
    //        //回车执行查询
    //        $("#broadcast").click();
    //    }
    //});
    
    //图片上载借口调用
    var imageUpload = function (localId, callback) {
        wx.uploadImage({
            localId: localId, // 需要上传的图片的本地ID，由chooseImage接口获得
            isShowProgressTips: 1,// 默认为1，显示进度提示
            success: function (res) {
                var serverId = res.serverId; // 返回图片的服务器端ID
                callback && callback(res);

            }
        });
    }
    
    //下拉获取更多历史聊天记录
    
    var counter = 10;
    var page =2
    // 每页展示4个
    // dropload
    $('#messages').dropload({
        domUp: {
            domClass: 'dropload-up',
            domRefresh: '<div class="dropload-refresh">↓下拉加载历史记录</div>',
            domUpdate: '<div class="dropload-update">↑释放更新</div>',
            domLoad: '<div class="dropload-load"><span class="loading"></span>内容加载中...</div>'
        },
        loadUpFn: function (me) {
            console.log(me);
            runAjax(page, me);
            page = page + 1;
        },     
        distance: 50,
        threshold:50
    });
    runAjax(page,null);
    
    function runAjax(page, me) {

        var newpage = page
        if (me == null) {
            newpage = 1;
        }

        $.ajax({
            type: 'GET',
            url: '/wechattalk/multitalk/gethistorytalk',
            dataType: 'json',
            data: { "wechatid": appid, "pageindex": newpage, "pagesize": counter, "enterdate": $("#enterTime").val() },
            success: function (data) {
                //me.noData(true);
                var result = '';
                for (var i = 0; i < data.data.length; i++) {
                    if (data.data[i].OpenId == myClientId) {
                        result += writeEventBefore(data.data[i].Name, data.data[i].TextContent, 'self', data.data[i].MsgType, data.data[i].ImgHeadUrl);
                    }
                    else {
                        result += writeEventBefore(data.data[i].Name, data.data[i].TextContent, 'other', data.data[i].MsgType, data.data[i].ImgHeadUrl);
                    }

                }
                if (me !== null) {
                    $('#wrapper').prepend(result);
                    // 每次数据加载完，必须重置
                    me.resetload();
                    // 重置索引值，重新拼接more.json数据
                    // 解锁
                    me.unlock();
                }
                else {
                    $('#wrapper').prepend(result);
                    $('#messages').scrollTop($('#wrapper').height());
                }
                
            },
            error: function (xhr, type) {
                alert('Ajax error!');
                // 即使加载出错，也得重置
                if (newpage !== 1) {
                    me.resetload();
                }               
            }
        });

    }

    
   
});
function writeEventAfter(name, msg, fromWho, textImage, imgurl) {
    if (textImage == "image") {
        mainMessage = "<div class='site-chat__content site-chat__content--" + fromWho + "'>" +
           "<div class='site-chat__plain site-chat__plain--" + textImage + "'><span><img src='" + msg + "' onclick='preViewImage(this)' /></span>" +
           "</div></div>"
    }
    else {
        mainMessage = "<div class='site-chat__content site-chat__content--" + fromWho + "'>" +
               "<div class='site-chat__plain site-chat__plain--" + textImage + "'><span>" + msg + "</span>" +
               "</div></div>"
    }

    $('#wrapper').append(
        "<div class='clearfix site-chat__message site-chat__message--text site-chat__message--" + fromWho + "'>" +
        "<div class='site-chat__wrap site-chat__wrap--" + fromWho + "'>" +
        "<img src='" + imgurl + "' class='site-chat__avatar site-chat__avatar--" + fromWho + "'>" +
        "<div class='site-chat__username site-chat__username--" + fromWho + "'>" + name + "</div>" + mainMessage +
       "</div></div>");
}
//向页面顶部添加数据 
function writeEventBefore(name, msg, fromWho, textImage, imgurl) {
    if (textImage == "image") {
        mainMessage = "<div class='site-chat__content site-chat__content--" + fromWho + "'>" +
           "<div class='site-chat__plain site-chat__plain--" + textImage + "'><span><img src='" + msg + "'onclick='preViewImage(this)' /></span>" +
           "</div></div>"
    }
    else {
        mainMessage = "<div class='site-chat__content site-chat__content--" + fromWho + "'>" +
               "<div class='site-chat__plain site-chat__plain--" + textImage + "'><span>" + msg + "</span>" +
               "</div></div>"
    }

    return  "<div class='clearfix site-chat__message site-chat__message--text site-chat__message--" + fromWho + "'>" +
        "<div class='site-chat__wrap site-chat__wrap--" + fromWho + "'>" +
        "<img src='" + imgurl + "' class='site-chat__avatar site-chat__avatar--" + fromWho + "'>" +
        "<div class='site-chat__username site-chat__username--" + fromWho + "'>" + name + "</div>" + mainMessage +
       "</div></div>";
}

//点击图片获取url 调用 preview
var preViewImage = function (t) {
    var urllist = [];
    var thisurl = window.location.protocol + "//" + window.location.host + $(t).attr("src");
    $('#messages').find(".site-chat__plain img").each(function () {
        urllist.push(window.location.protocol + "//" + window.location.host + $(this).attr("src"));
    });
    wx.previewImage({
        current: thisurl, // 当前显示图片的http链接
        urls: urllist // 需要预览的图片http链接列表
    });
}
