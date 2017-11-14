$versionFull = $OctopusParameters["Octopus.Release.Number"]
$iconUri = $OctopusParameters["SquirellIconPath"]

#$versionFull = "1.4.3.3"
#$iconUri = "http://exchange.qa.gis.by/SubscriptionEditor/app-ico.ico"

$packageId = "SubscriptionEditor"

$nuget = ".\tools\NuGet.CommandLine\nuget.exe"
$squirell = ".\tools\squirell\Squirrel.com"

$versionPart = $versionFull.split("-")[0]
$buildNumberParts = $versionPart.split(".")
$version = "$($buildNumberParts[0]).$($buildNumberParts[1]).$($buildNumberParts[2])"
echo $version

Remove-Item "$packageId.nuspec" -Force -ErrorAction SilentlyContinue

Copy-Item ".\.squirell\$packageId.cepsun" "$packageId.nuspec" -Force

Copy-Item ".\.squirell\index.htm" "index.htm" -Force

& "$nuget" pack "$packageId".nuspec -Version "$version" -Properties iconUrl="$iconUri"

$packaged = "$("$packageId").$("$version").nupkg"
echo "$packaged"

& "$squirell" --releasify "$packaged" --no-msi  -r ".\Releases"

$webConfigTemplate = @'
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
  <system.webServer>
    <staticContent>
      <mimeMap fileExtension=".nupkg" mimeType="application/zip" />
      <mimeMap fileExtension="." mimeType="text/plain" />
    </staticContent>
  </system.webServer>
</configuration>
'@
New-Item .\Releases\web.config -type file -force -value $webConfigTemplate

