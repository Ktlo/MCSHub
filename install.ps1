[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True,Position=1)]
    [string]$System = "win",
    [switch]$Uninstall
)

$installutil = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe"

function Install-Win {
    #$path = $(Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0").MSBuildToolsPath
    #[System.IO.Path]::Combine($path, "MSBuild.exe")
    MSBuild -p:Configuration="Release" ".\MinecraftServerHub.NTService\MinecraftServerHub.NTService.csproj"
    $path = "$Env:ProgramFiles\MinecraftServerHub"
    New-Item $path -ItemType "Directory"
    Copy-Item ".\MinecraftServerHub.NTService\bin\Release\MinecraftServerHub.NTService.exe" "$path\MCSHub.exe"
    $save = Get-Location
    Set-Location $path
    & $installutil ".\MCSHub.exe"
    Set-Location $save
}

function Remove-Win {
    $path = "$Env:ProgramFiles\MinecraftServerHub"
    $save = Get-Location
    Set-Location $path
    & $installutil -u ".\MCSHub.exe"
    Set-Location $save
    Remove-Item -Recurse $path
}

function Install-Linux {
    dotnet msbuild -p:Configuration="Release" ".\MinecraftServerHub.App\MinecraftServerHub.App.csproj"
    $path = "/usr/share/mcshub"
    Copy-Item -Recurse ".\MinecraftServerHub.App\bin\Release\netcoreapp2.0" $path
    #TODO
}

switch ($System.ToLower()) {
    "win" {
        if ($Uninstall.ToBool()) {
            Remove-Win
        }
        else {
            Install-Win
        }
    }
    "linux" {
        Install-Linux
    }
    Default {
        throw New-Object System.NotImplementedException("No such platform")
    }
}
