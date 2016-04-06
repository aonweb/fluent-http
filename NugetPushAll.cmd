@ECHO OFF
SET THIS_SCRIPTS_DIRECTORY=%~dp0

PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Serialization\bin\Release\" -NoPrompt -PushPackageToNuGetGallery"
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp\bin\Release\" -NoPrompt -PushPackageToNuGetGallery"
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.HAL.Serialization\bin\Release\" -NoPrompt -PushPackageToNuGetGallery"
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Hal\bin\Release\" -NoPrompt -PushPackageToNuGetGallery"
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Mocks\bin\Release\" -NoPrompt -PushPackageToNuGetGallery"
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Autofac\bin\Release\" -NoPrompt -PushPackageToNuGetGallery"
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Full\bin\Release\" -NoPrompt -PushPackageToNuGetGallery"
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1 -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.GraphQL\bin\Release\" -NoPrompt -PushPackageToNuGetGallery"
REM PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& \"%THIS_SCRIPTS_DIRECTORY%tools\_CreateNewNuGetPackage\DoNotModify\UploadNuGetPackage.ps1\"  -ProjectPath \"%THIS_SCRIPTS_DIRECTORY%AonWeb.FluentHttp.Xamarin\bin\Release\" -NoPrompt -PushPackageToNuGetGallery"

pause