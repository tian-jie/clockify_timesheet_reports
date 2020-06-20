/*!
 * jquery.scrollLoading.js
 * by zhangxinxu  http://www.zhangxinxu.com
 * 2010-11-19 v1.0
 * 2012-01-13 v1.1 偏移值计算修改 position → offset
 * 2012-09-25 v1.2 增加滚动容器参数, 回调参数
*/
(function($) {
    $.fn.scrollLoading = function (options) {
        var iPageSize = 10;
        var _PageIndex = 1;
        var bolProcess = false;

		var defaults = {
			attr: "data-url",
			container: $(window),
			callback: $.noop
		};
		var params = $.extend({}, defaults, options || {});
		params.cache = [];
		$(this).each(function() {
		    var node = this.nodeName.toLowerCase(), url = $(this).attr(params["attr"]);
		    if ($(this).data("search") == true)
		    {
		        url = url + "?searchword=" + $('#searchword').val();
		    }
			//重组
			var data = {
				obj: $(this),
				tag: node,
				url: url
			};
			params.cache.push(data);
		});
		
		var callback = function(call,data) {
			if ($.isFunction(params.callback)) {
			 return	params.callback(call.get(0),data);
			}
		};
		//动态显示数据
		var loading = function () {
		    $(".scrollLoading .loading").show();
		    console.log('before1 loading' + bolProcess);

		    //if ($('.container .row').length < 10) { return; }

		    if (bolProcess) {
		        $(".scrollLoading .loading").hide();
		        //$('.scrollLoading .no-result').show();
		        //setTimeout(function () { $('.scrollLoading .no-result').hide(); }, 1500);
		       
		        return;
		    }
		    bolProcess = true;
		   
			var contHeight = params.container.height();
			if ($(window).get(0) === window) {
				contop = $(window).scrollTop();
			} else {
				contop = params.container.offset().top;
			}

			console.log('before loading' + bolProcess);
			
			$.each(params.cache, function(i, data) {
				var o = data.obj, tag = data.tag, url = data.url, post, posb;

				if (o) {
					post = o.offset().top - contop, post + o.height();
	
					if ((post >= 0 && post < contHeight) || (posb > 0 && posb <= contHeight)) {


					    //page_list(p, settings);
						if (url) {
							//在浏览器窗口内
							if (tag === "img") {
								//图片，改变src
								callback(o.attr("src", url));		
							} else {
							    console.log('start loading');
							    var postPara = { start: _PageIndex * iPageSize, length: iPageSize };
							    if (options.postData != null) {
							        postPara = $.extend(postPara, options.postData);
							    }
							    $.getJSON(url, postPara, function (data) {
							        $(".scrollLoading .loading").hide();
							        var d = callback(o, data.aaData);
							        if (d == false || (data == null || data.aaData == null || data.aaData.length==0)) {
							            params.container.unbind("scroll");
							            $('.scrollLoading .no-result').show();
							            setTimeout(function () { $('.scrollLoading .no-result').hide(); }, 1500);
							        }

							        var t = setTimeout(function () { bolProcess = false; $('.scrollLoading .no-result').hide(); }, 1500);

							        //bolProcess = false;
							        console.log('loading sucess' + d);
							    });
							    _PageIndex++;
							}		
						} else {
							// 无地址，直接触发回调
							callback(o);
						}
						//data.obj = null;	
					}
				}
			});
			//var t = setTimeout(function () { bolProcess = false; }, 100);
		};
		
		//事件触发
		//加载完毕即执行
		//loading();
		//滚动执行
		params.container.bind("scroll", loading);
	};
})(jQuery);