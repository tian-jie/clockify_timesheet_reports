@ECHO ON

set path=%path%;C:\Windows\Microsoft.NET\Framework\v4.0.30319;C:\Program Files (x86)\Git\bin


c:
cd\
rd xyz /s /q
md xyz
cd xyz
@ECHO Fetching code...
git clone -b SnowWhite --single-branch https://code.innocellence.com/icdp/git/jie.tian/LillyChinaCAWeChat.git LillyChinaCAWeChat
cd LillyChinaCAWeChat\source


set TARGET=C:\LillyChinaCAWechat
@ECHO Cleaning up Target Folder...
rd %TARGET% /s /q



set PROJECT=Innocellence.Web
@ECHO Publishing %PROJECT%
msbuild UI\%PROJECT%\%PROJECT%.csproj /t:ResolveReferences;Compile /t:_WPPCopyWebApplication /p:Configuration=Release /t:Clean,Build /p:WebProjectOutputDir=%TARGET% >NUL
@ECHO Deploying Publishing %PROJECT%
del %TARGET%\bin\*.pdb /f /s

set PROJECT=Innocellence.CA
@ECHO Publishing %PROJECT%
msbuild Plugins\%PROJECT%\%PROJECT%.csproj /t:_WPPCopyWebApplication /p:Configuration=Release /t:Clean,Build /p:WebProjectOutputDir=%TARGET%\Plugins\%PROJECT% >NUL
@ECHO Deploying %PROJECT%
ren %TARGET%\Plugins\%PROJECT%\bin bin1
md %TARGET%\Plugins\%PROJECT%\bin
copy %TARGET%\Plugins\%PROJECT%\bin1\%PROJECT%.dll %TARGET%\Plugins\%PROJECT%\bin\
rd %TARGET%\Plugins\%PROJECT%\bin1 /s /q

set PROJECT=Innocellence.CA.Admin
@ECHO Publishing %PROJECT%
msbuild Plugins\%PROJECT%\%PROJECT%.csproj /t:_WPPCopyWebApplication /p:Configuration=Release /t:Clean,Build /p:WebProjectOutputDir=%TARGET%\Plugins\%PROJECT%  >NUL
@ECHO Deploying %PROJECT%
ren %TARGET%\Plugins\%PROJECT%\bin bin1
md %TARGET%\Plugins\%PROJECT%\bin
copy %TARGET%\Plugins\%PROJECT%\bin1\%PROJECT%.dll %TARGET%\Plugins\%PROJECT%\bin\
rd %TARGET%\Plugins\%PROJECT%\bin1 /s /q

set PROJECT=Innocellence.Finance
@ECHO Publishing %PROJECT%
msbuild Plugins\%PROJECT%\%PROJECT%.csproj /t:_WPPCopyWebApplication /p:Configuration=Release /t:Clean,Build /p:WebProjectOutputDir=%TARGET%\Plugins\%PROJECT% >NUL
@ECHO Deploying %PROJECT%
ren %TARGET%\Plugins\%PROJECT%\bin bin1
md %TARGET%\Plugins\%PROJECT%\bin
copy %TARGET%\Plugins\%PROJECT%\bin1\%PROJECT%.dll %TARGET%\Plugins\%PROJECT%\bin\
rd %TARGET%\Plugins\%PROJECT%\bin1 /s /q

set PROJECT=Innocellence.WeChatMain
@ECHO Publishing %PROJECT%
msbuild Plugins\%PROJECT%\%PROJECT%.csproj /t:ResolveReferences;Compile /t:_WPPCopyWebApplication /p:Configuration=Release /t:Clean,Build /p:WebProjectOutputDir=%TARGET%\Plugins\%PROJECT% >NUL
@ECHO Deploying  %PROJECT%
ren %TARGET%\Plugins\%PROJECT%\bin bin1
md %TARGET%\Plugins\%PROJECT%\bin
copy %TARGET%\Plugins\%PROJECT%\bin1\Innocellence.* %TARGET%\Plugins\%PROJECT%\bin\
rd %TARGET%\Plugins\%PROJECT%\bin1 /s /q


pause
