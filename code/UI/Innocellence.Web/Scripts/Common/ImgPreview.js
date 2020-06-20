//-----------------------------------------------
//added by andrew 2016/2/26   基于微信图片预览接口
//-----------------------------------------------
(function () {
    var imgsSrc = [];
    //获取主机地址
    var pathName = window.document.location.href;
    var pos = pathName.indexOf('/', 10);

    var hostPath = pathName.substring(0, pos);

    function reviewImage(src) {
        //alert("reviewImage: " + src);
        if (typeof window.WeixinJSBridge != 'undefined') {
            WeixinJSBridge.invoke('imagePreview', {
                'current': src,
                'urls': imgsSrc
            });
        }
    }
    function onImgLoad() {
        var imgs = document.getElementsByTagName('img');
        for (var i = 0, l = imgs.length; i < l; i++) {
            var img = imgs.item(i);
            var src = img.getAttribute('src'); // || img.getAttribute('data-src');

            if (src) {

                if (src.indexOf('http') < 0) {
                    src = hostPath + src;
                    imgsSrc.push(src);
                } else { continue; }

                (function (src) {
                    if (img.addEventListener) {
                        //alert("adding event: addEventListener");
                        img.addEventListener('click', function (event) {
                            reviewImage(src);
                            event.stopPropagation();//阻止事件冒泡
                        });
                    } else if (img.attachEvent) {
                        //alert("adding event: attachEvent");
                        img.attachEvent('click', function () {
                            reviewImage(src);
                            window.event.cancelBubble = true;//ie 阻止事件冒泡
                        });
                    }
                })(src);
            }
        }
        return true;
    }

    function load() {
        $('#articleDetail').on('click.img.preview', 'img', function () {
            var $this=$(this), isFindA = findAtag($this);

            if (isFindA === false) {
                reviewImage($this.attr('src'));
            }
        });

        $('#articleDetail img').each(function () {
            var $this = $(this), isFindA = findAtag($this), imgUrl = $this.attr('src');

            if (isFindA === false) {
                if (imgUrl.indexOf('http') < 0) {
                    imgUrl = hostPath + imgUrl;
                    imgsSrc.push(imgUrl);
                }
                else
                {
                    imgsSrc.push(imgUrl);
                }
            }
        });

        function findAtag($img) {
            var $atag = $img.closest('a');

            if ($atag && $atag.attr('href') && $atag.attr('href') != '#') {
                return true;
            }

            return false;
        }
    }

    $(function () {
        //onImgLoad();
        load();
    });
})();