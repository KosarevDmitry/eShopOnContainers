param (
    [string]$solution = "eShopOnContainers-ServicesAndWebApps.sln"
)
$outfile = "b.txt"
Write-Output "COPY ""$solution"" ""$solution""" > $outfile
Add-Content -Path $outfile ""
#Select-String -Path $solution -Pattern ', "(.*?\.csproj)"'
Select-String -Path $solution -Pattern ', "(.*?\.csproj)"' |  ForEach-Object { $_.Matches.Groups[1].Value} #|Out-File -FilePath $outfile -Append
#Select-String -Path $solution -Pattern ', "(.*?\.csproj)"' | ForEach-Object { $_.Matches.Groups[1].Value.Replace("\", "/") } | Sort-Object | ForEach-Object {"COPY ""$_"" ""$_"""} | Out-File -FilePath $outfile -Append