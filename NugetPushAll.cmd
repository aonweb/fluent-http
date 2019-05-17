@ECHO OFF
SET THIS_SCRIPTS_DIRECTORY=%~dp0

PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Serialization\bin\Release\""
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp\bin\Release\""
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.HAL.Serialization\bin\Release\""
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Hal\bin\Release\""
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Mocks\bin\Release\""
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Autofac\bin\Release\""
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Full\bin\Release\""
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.GraphQL\bin\Release\""
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Xamarin\bin\Release\""
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Xamarin.HttpClient\bin\Release\""

pause