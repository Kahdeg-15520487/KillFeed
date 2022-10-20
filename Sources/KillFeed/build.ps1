$rimworldVersion = (Get-Item ..\..\..\..\RimWorldWin64_Data\Managed\Assembly-CSharp.dll).VersionInfo
$majorVersion = $rimworldVersion.FileMajorPart
$minorVersion = $rimworldVersion.FileMinorPart
$buildConfig = "v$($majorVersion)$($minorVersion)"
$buildVerFolder = "v$($majorVersion).$($minorVersion)"
"Building KillFeed for Rimworld $buildVerFolder"
$buildLog = dotnet build --configuration $buildConfig
Copy-Item .\KillFeed\bin\$buildConfig\KillFeed.dll ..\..\$buildVerFolder\Assemblies\