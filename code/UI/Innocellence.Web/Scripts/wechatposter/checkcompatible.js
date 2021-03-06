// 检查浏览器的兼容性
(function () {
    /* 检测是否支持某个css属性
     用法：checkCssKey("Animation", true)
     */
    function checkCssKey( key, hasPrefixes ) {
        // 目前手工初页仅支持webkit前缀的css属性，如果手工初页在这方面做了拓展，则需要修改cssPrefixes,添加被支持浏览器的css前缀
        var cssPrefixes = ["webkit"];
        var styles = document.createElement( "div" ).style;
        var isSupport = false;
        if ( hasPrefixes ) {
            for ( var i = 0; i < cssPrefixes.length; i++ ) {
                if ( styles[cssPrefixes[i] + key] != undefined ) {
                    isSupport = true;
                    break;
                }
            }
        }
        else {
            isSupport = styles[key] != undefined;
        }
        return isSupport;
    }

    function checkCanvas() {
        var canvas = document.createElement( "canvas" );
        return (canvas.getContext && canvas.getContext( "2d" )) != undefined;
    }

    function checkFileReader() {
        return window.FileReader != undefined;
    }

    function checKernel() {
        // 要有AppleWebKit，不能有Maxthon、QQBrowser、baidubrowser、MetaSr、TheWorld
        var agent = navigator.userAgent;
        return agent.indexOf( "AppleWebKit" ) != -1 && agent.indexOf( "Maxthon" ) == -1
            && agent.indexOf( "QQBrowser" ) == -1 && agent.indexOf( "baidubrowser" ) == -1
            && agent.indexOf( "MetaSr" ) == -1 && agent.indexOf( "TheWorld" ) == -1;
    }

    // 检查浏览器兼容性
    function checkCompatible() {
        return checkCanvas() && checkFileReader() && checKernel();
    }

    function generateNotSupportPage() {
        var page = document.createElement( "div" );
        page.className = "not-support-page";
        document.getElementsByTagName( "body" )[0].appendChild( page );
        var word = document.createElement( "div" );
        word.className = "not-support-page-word";
        page.appendChild(word);
    }

    !checkCompatible() && generateNotSupportPage();
})();