dotnet tool restore
Push-Location .\Source
dotnet cleanup -y
Pop-Location
dotnet build .\Source\Morris.Moxy.sln -c Release