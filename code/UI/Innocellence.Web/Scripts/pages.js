var nullData = "暂无相关数据";
(function () {
    var EventBase = function () {
        this.addListener = function (type, listener) {
            getListener(this, type, true).push(listener);
        }
        this.removeListener = function (type, listener) {
            var listeners = getListener(this, type);
            for (var i = 0; i < listeners.length; i++) {
                if (listeners[i] == listener) {
                    listeners.splice(i, 1);
                    return;
                }
            }
        }

        this.fireEvent = function (type) {
            var listeners = getListener(this, type),
                r, t, k;
            if (listeners) {
                k = listeners.length;
                while (k--) {
                    t = listeners[k].apply(this, arguments);
                    if (t !== undefined) {
                        r = t;
                    }
                }
            }
            if (t = this['on' + type.toLowerCase()]) {
                r = t.apply(this, arguments);
            }
            return r;
        }
    }

    function getListener(obj, type, force) {
        var allListeners;
        type = type.toLowerCase();
        return ((allListeners = (obj.__allListeners || force && (obj.__allListeners = {}))) &&
            (allListeners[type] || force && (allListeners[type] = [])));
    }

    window['EventBase'] = EventBase;


})();
//???
var Page = function (pageCanvas) {
    this.recordCount;
    this.pageSize;
    this.numericButtonCount;
    this.pageCanvas = pageCanvas;
    this.pageIndex = 1;
}
Page.prototype = new EventBase();
Page.prototype.getPageHtml = function () {
    this.pageCount = Math.ceil(this.recordCount / this.pageSize);
    //var prev = this.pageIndex == 1 ? " <li class='previous disabled'><span><i class='fa fa-angle-left'></i>&#19978;&#19968;&#39029;</span></li>" : " <li class='previous'><a href='javascript:;' pageindex='"+ (this.pageIndex - 1) +"'><i class='fa fa-angle-left'></i>&#19978;&#19968;&#39029;</a></li> ";
    //var next = this.pageCount <= this.pageIndex ? " <li class='next disabled'><span>&#19979;&#19968;&#39029;<i class='fa fa-angle-right'></i></span></li>" : " <li class='next'><a href='javascript:;' pageIndex='"+ (this.pageIndex + 1) +"'>&#19979;&#19968;&#39029;<i class='fa fa-angle-right'></i></a></li>";  
    //var first = this.pageIndex == 1 ? "<span class='onthis'>1</span>..." : "<span><a href='javascript:;' pageindex='1'>1</a></span>...";
    //var last = this.pageCount <= this.pageIndex ? "...<span class='onthis'>" + this.pageCount + "</span>" : "...<span><a href='javascript:;' pageindex='"+ (this.pageCount) +"'>"+ this.pageCount +"</a></span>";  
    var pageStr = ""
    var pageMathIndex = Math.floor(this.numericButtonCount / 2);
    var pageStartIndex;
    var pageEndIndex;
    var lastpage = this.pageCount;
    if (this.pageCount < this.numericButtonCount) {
        pageStartIndex = 1
        pageEndIndex = this.pageCount;
    } else {
        if (this.pageCount - pageMathIndex < this.pageIndex) {
            // pageStartIndex = this.pageCount - this.numericButtonCount + 1;
            pageStartIndex = this.pageCount - this.numericButtonCount + 1
            pageEndIndex = this.pageCount;
        } else {
            if (this.pageIndex - pageMathIndex < 1) {
                pageStartIndex = 1;
                pageEndIndex = this.numericButtonCount;
            } else {
                pageStartIndex = this.pageIndex - pageMathIndex;
                pageEndIndex = this.pageIndex + pageMathIndex;
            }
        }

    }
    pageStr += "<ul class='pagination-psp'>";
    if (this.pageIndex > 1) {
        pageStr += "<li><a href='javascript:;' pageindex='" + (this.pageIndex - 1) + "'>< 上一页</a></li>";
    } else {
        pageStr += "<li><a class='none'>< 上一页</a></li>";
    }
    if (this.pageIndex == 1) pageStr += " <li><a class='active'>" + 1 + "</a></li>"
        //else if (this.pageIndex == pageEndIndex - pageMathIndex) "";
    else pageStr += "<li><a href='javascript:;' pageindex='" + 1 + "'>" + 1 + "</a></li>";

    //pageStr += "<li><a href='javascript:;' pageindex='" + 1 + "'>" + 1 + "</a></li>";   
    if (this.pageCount > this.numericButtonCount && this.pageIndex > pageMathIndex + 1) {

        pageStr += "<li><a class='none'>..</a></li>";
    }
    for (var i = pageStartIndex + 1; i < pageEndIndex; i++) {

        if (this.pageIndex == i) pageStr += " <li><a class='active'>" + i + "</a></li>"
            //else if (this.pageIndex == pageEndIndex - pageMathIndex) "";
        else pageStr += " <li><a href='javascript:;' pageindex='" + i + "'>" + i + "</a></li>";
    }
    if (this.pageCount > this.numericButtonCount && this.pageIndex < lastpage - pageMathIndex) {
        pageStr += "<li><a class='none'>..</a></li>";
    }

    if (this.pageIndex == lastpage) pageStr += " <li><a class='active'>" + lastpage + "</a></li>"
        //else if (this.pageIndex == pageEndIndex - pageMathIndex) "";
    else pageStr += "<li><a href='javascript:;' pageindex='" + lastpage + "'>" + lastpage + "</a></li>";


    if (this.pageIndex < pageEndIndex) {
        pageStr += "<li><a href='javascript:;' pageindex='" + (this.pageIndex + 1) + "'>下一页 ></a></li>";
    } else {
        pageStr += "<li><a class='none'>下一页 ></a></li>";
    }
    pageStr += "<li><input type='text' style='width:30px' id='go_page'></li><li><a href='javascript:;' class='gotopage'>Go</a></li>"
    pageStr += "</ul>";
    //	 if(pageStartIndex == 1) first = '';
    //	 if(pageEndIndex == this.pageCount) last = '';
    if (this.pageCount == 1 || this.pageCount == 0) {
        pageStr = '';
    }
    //pageStr = first + prev + pageStr + next + last;
    //pageStr = prev  + pageStr  + next;
    return pageStr;
}
Page.prototype.onPageChanged = function (pageIndex) {
    this.pageIndex = pageIndex;
    this.fireEvent('pageChanged');
}
Page.prototype.pageEvent = function (page) {
    this.onclick = function (e) {
        e = e || window.event;
        t = e.target || e.srcElement;
        console.log(t);
        if (t.className == "gotopage") {
            var gopagenew = document.getElementById("go_page").value == "" ? "1" : document.getElementById("go_page").value
            page.onPageChanged(parseInt(gopagenew));
        }
        else {
            if (t.tagName == "A" && t.className != "none" && t.className != "active") {
                page.onPageChanged(parseInt(t.getAttribute("pageindex")));
            }
        }

    }
}
Page.prototype.render = function () {
    var pageCanvas = document.getElementById(this.pageCanvas);
    if (pageCanvas != null) {
        pageCanvas.innerHTML = this.getPageHtml();
        this.pageEvent.call(pageCanvas, this);
    }
}
Page.prototype.initialize = function () {
    this.onPageChanged(this.pageIndex);
}