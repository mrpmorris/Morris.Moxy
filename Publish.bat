cls
@echo **** 1.6.0-Beta2 : UPDATED THE VERSION NUMBER IN THE PROJECT *AND* BATCH FILE? ****
pause

cls
@call BuildAndTest.bat

@echo ======================

set /p ShouldPublish=Publish 1.6.0-Beta2 [yes]?
@if "%ShouldPublish%" == "yes" (
	@echo PUBLISHING
	dotnet nuget push .\Source\Lib\Morris.Moxy\bin\Release\Morris.Moxy.1.6.0-Beta2.nupkg -k %MORRIS.NUGET.KEY% -s https://api.nuget.org/v3/index.json
)

