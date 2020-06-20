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
			//重组
			var data = {
				obj: $(this),
				tag: node,
				url: url
			};
			params.cache.push(data);
		});
		
		var callback = function(call) {
			if ($.isFunction(params.callback)) {
			 return	params.callback.call(call.get(0));
			}
		};
		//动态显示数据
		var loading = function () {

		    console.log('before1 loading' + bolProcess);

		    if ($('.container .row').length < 10) { return; }

		    if (bolProcess) {
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
						if (url) {
							//在浏览器窗口内
							if (tag === "img") {
								//图片，改变src
								callback(o.attr("src", url));		
							} else {
							    console.log('start loading');
							    $('<div></div>').insertBefore(o).load(url, { start: _PageIndex * iPageSize, length: iPageSize, searchword: $('#searchword').val() }, function () {

							        var d = callback(o);
							        if(d==false){
							            params.container.unbind("scroll");
							        }

							        var t = setTimeout(function () { bolProcess = false;}, 100);

							       // bolProcess = false;
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