param (
    [Parameter(Mandatory=$true)]
    [string]$Configuration
)

$PublishDir = "..\Build\$Configuration"
$BuildDir = "..\Build\$Configuration\ERHMS Info Manager"
$ExecutableFileName = "ERHMS Info Manager.exe"
$ExecutableFileId = "ERHMS_Info_Manager.exe"
$TargetFileName = "ERHMS_Info_Manager.msi"
$VersionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$BuildDir\$ExecutableFileName")

pushd $PSScriptRoot
heat `
    dir "$BuildDir" `
    -cg FileComponentGroup `
    -dr INSTALLDIR `
    -gg `
    -indent 2 `
    -ke `
    -nologo `
    -sfrag `
    -srd `
    -sreg `
    -suid `
    -var var.BuildDir `
    -out FileComponentGroup.wxs
candle `
    -dBuildDir="$BuildDir" `
    -dExecutableFileName="$ExecutableFileName" `
    -dExecutableFileId="$ExecutableFileId" `
    -dProductName="$($VersionInfo.ProductName)" `
    -dProductVersion="$($VersionInfo.ProductVersion)" `
    -dFileVersion="$($VersionInfo.FileVersion)" `
    -dCompanyName="$($VersionInfo.CompanyName)" `
    -nologo `
    -out obj\ `
    Product.wxs FileComponentGroup.wxs
light `
    -ext WixUIExtension `
    -nologo `
    -out "bin\$TargetFileName" `
    obj\Product.wixobj obj\FileComponentGroup.wixobj
copy `
    "bin\$TargetFileName" `
    "$PublishDir\$TargetFileName"
popd
