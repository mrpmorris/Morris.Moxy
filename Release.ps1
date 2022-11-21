dotnet tool restore
Push-Location .\Source
dotnet cleanup -y
Pop-Location
dotnet build .\Source\Lib\Morris.Moxy\Morris.Moxy.csproj -c Release
dotnet build .\Source\Morris.Moxy.sln -c Release