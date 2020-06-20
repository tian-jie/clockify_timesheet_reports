@ECHO OFF
@ECHO "C:\Program Files (x86)\Git\bin\git" fetch --all
@ECHO "C:\Program Files (x86)\Git\bin\git" reset --hard origin/master

@ECHO "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv" "UI\Innocellence.Web\Innocellence.Web.csproj" /deploy
@ECHO "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv" "Plugins\Innocellece.CA\Innocellence.CA.csproj" /deploy
@ECHO "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv" "Plugins\Innocellece.CA.Admin\Innocellence.CA.Admin.csproj" /deploy
@ECHO "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv" "Plugins\Innocellece.WeChatMain\Innocellence.WeChatMain.csproj" /deploy

