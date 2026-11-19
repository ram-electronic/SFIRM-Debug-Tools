Import-Module GroupPolicy

$GpoName = "GPO-X-SFIRM-Crash-Dumps"
$Exes    = @(
    "SEPA XML Checker.exe",
    "SFAutomat.exe",
    "SFAutomatService.exe",
    "SfAutosend.exe",
    "SFDatabaseService.exe",
    "SFDatabaseSetup.exe",
    "SFEbicsWorker.exe",
    "SFirm.exe",
    "SFSupport.exe",
    "SFUpdateProvider.exe",
    "SFUpdateRepair.exe",
    "SFUpdateService.exe",
    "sfwinstartupinfo.exe"
)

$gpo = Get-GPO -Name $GpoName -ErrorAction SilentlyContinue
if (-not $gpo) { $gpo = New-GPO -Name $GpoName }

foreach ($exe in $Exes) {
    $key = "HKLM\SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps\$exe"
    $common = @{ Name = $GpoName; Context = "Computer"; Action = "Update"; Key = $key }

    Set-GPPrefRegistryValue @common -ValueName "DumpFolder" -Type ExpandString -Value '%PROGRAMDATA%\Star Finanz\CrashDumps'
    Set-GPPrefRegistryValue @common -ValueName "DumpCount"  -Type DWord        -Value 10
    Set-GPPrefRegistryValue @common -ValueName "DumpType"   -Type DWord        -Value 2
}