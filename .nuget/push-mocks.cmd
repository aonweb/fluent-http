set p=..\AonWeb.FluentHttp.Mocks\bin\Release\
for /f "tokens=*" %%a in ('dir %p%*.nupkg /b /od') do set newest=%%a
echo 
NuGet.exe push %p%%newest%  -source http://devweb.homeoffice.amc.corp:8912/
