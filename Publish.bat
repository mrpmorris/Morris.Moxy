cls
@echo **** 1.2.0 : UPDATED THE VERSION NUMBER IN THE PROJECT *AND* BATCH FILE? ****
pause

cls
@call BuildAndTest.bat

@echo
@echo
@echo
@echo ======================

set /p ShouldPublish=Publish 1.2.0 [yes]?
@if "%ShouldPublish%" == "yes" (
	@echo PUBLISHING
	dotnet nuget push .\Source\Lib\Morris.Moxy\bin\Release\Morris.Moxy.1.2.0.nupkg -k %MORRIS.NUGET.KEY% -s https://api.nuget.org/v3/index.json
)

