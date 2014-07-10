@echo on
echo "start pack"
..\..\.nuget\nuget.exe pack ..\XFramework.csproj
echo "start upload'"
..\..\.nuget\nuget.exe setApiKey cf555ff7-a9c6-4720-966a-6f7f1b36d29f
..\..\.nuget\nuget.exe push XFramework.1.0.2.2.nupkg
pause