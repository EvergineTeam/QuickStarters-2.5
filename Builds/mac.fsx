#r @"FAKE.3.26.1/tools/FakeLib.dll"

#load "common.fsx"

open Fake
open Common

let WaveToolDirectory = "WaveEngine.MacTools"

Target "mac-restore-tools" (fun() ->
    traceImportant "Clear tools directory"
    DeleteDirs [WaveToolDirectory]

    traceImportant "Get WaveEngine.MacTools nuget packages"
    let nugetArgs = " install " + WaveToolDirectory + " -ExcludeVersion -ConfigFile NuGet/NuGet.config"
    trace nugetArgs
    Exec "NuGet/nuget.exe" nugetArgs

    traceImportant "Generate waveengine installation path"
    let target = "/Library/Frameworks/WaveEngine.framework/v2.0/Tools/VisualEditor/"
    !! (WaveToolDirectory + "/Tools/*.*")
        |> CopyFiles target
)

Target "mac-update-nightlypackages" (fun() ->
    traceImportant "Update to nightly nuget packages"
    let args = "WaveTools/UpdateToNightlyPackages.exe " + rootFolder
    Exec "mono" args
)

Target "mac-quickstarters" (fun () ->
    buildquickstarters("MacOS")
)

"mac-restore-tools"    
    ==> "mac-update-nightlypackages"
    ==> "mac-quickstarters"