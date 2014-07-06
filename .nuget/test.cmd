set p=..\AonWeb.FluentHttp.Hal\bin\Release\
for /f "tokens=*" %%a in ('dir %p%*.nupkg /b /od') do set newest=%%a
echo %p%%newest%
pause
