cls
@echo **** 1.3.0-Beta5 : UPDATED THE VERSION NUMBER IN THE PROJECT *AND* BATCH FILE? ****
pause

cls
@call BuildAndTest.bat

@echo ======================

set /p ShouldPublish=Publish 1.3.0-Beta5 [yes]?
@if "%ShouldPublish%" == "yes" (
	@echo PUBLISHING
	dotnet nuget push .\Source\Lib\Morris.Moxy\bin\Release\Morris.Moxy.1.3.0-Beta5.nupkg -k %MORRIS.NUGET.KEY% -s https://api.nuget.org/v3/index.json
)

