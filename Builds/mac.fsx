#r @"FAKE.4.63/tools/FakeLib.dll"

#load "common.fsx"

open Fake
open Common

let WaveToolDirectory = "WaveEngine.MacTools"

Target "mac-restore-tools" (fun() ->
    traceImportant "Clear tools directory"
    DeleteDirs [WaveToolDirectory]

    traceImportant "Get WaveEngine.MacTools nuget packages"
    let nugetArgs = " install " + WaveToolDirectory + " -ExcludeVersion -PreRelease -ConfigFile NuGet/NuGet.config"
    trace nugetArgs
    Exec "NuGet/nuget.exe" nugetArgs

    //let chmodArgs = "+x " + WaveToolDirectory + "/tools/sox"
    //Exec "chmod" chmodArgs

    traceImportant "Generate waveengine installation path"
    let target = "/Library/Frameworks/WaveEngine.framework/v2.0/Tools/VisualEditor/"
    !! (WaveToolDirectory + "/tools/*.*")
        |> CopyFiles target
)

Target "mac-update-nightlypackages" (fun() ->
    traceImportant "Update to nightly nuget packages"
    let args = "WaveTools/UpdateToNightlyPackages.exe " + rootFolder
    Exec "mono" args
)

Target "mac-quickstarters" (fun () ->
    buildquickstarters("MacOS")
    reportartifact()
)

"mac-restore-tools"    
    ==> "mac-update-nightlypackages"
    ==> "mac-quickstarters"