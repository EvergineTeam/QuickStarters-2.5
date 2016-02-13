#r @"FAKE.3.26.1/tools/FakeLib.dll"

open Fake
open Fake
open Fake.XamarinHelper
open System
open System.Collections.Generic
open System.IO
open System.Linq
open Fake.AssemblyInfoFile

let configuration = "Release"
let architecture = "Any CPU"
let rootFolder = "../"
let WaveToolDirectory = "WaveEngine.WindowsTools"

let getFolder solutionFile= Path.GetDirectoryName(solutionFile)

let Exec command args =
    let result = Shell.Exec(command, args)

    if result <> 0 then failwithf "%s exited with error %d" command result

let RestorePackages solutionFile =
    RestoreMSSolutionPackages (fun p -> 
        { p with
            Sources = "https://www.myget.org/F/waveengine-nightly/api/v2" :: p.Sources
            Retries = 5 
            OutputPath = Path.Combine(getFolder solutionFile, "packages") }) solutionFile

type status =
    | Success
    | Failed

type quickstarterReport = 
    {
        Result : status
        Path : string
    }

let items = new List<quickstarterReport>()

let processResults (path : string) (flag : bool) =
    let report : quickstarterReport = 
        {
            Result = if flag then status.Success else status.Failed
            Path = path
        }

    items.Add(report)

let mutable OkProjects = 0;

let printReport (l : List<quickstarterReport>) =
    printfn ""
    traceImportant "---------------------------------------------------------------------"
    traceImportant "Quickstarters Report:"
    traceImportant "---------------------------------------------------------------------"
    l |> Seq.iteri (fun index item ->
        if l.[index].Result = status.Success then
            trace (index.ToString() + "-    Success " + l.[index].Path)
            OkProjects <- OkProjects + 1
        else
            traceError (index.ToString() + "-    Failed " + l.[index].Path))

    printfn ""
    printfn "   Projects success: %i / %i" OkProjects l.Count
    traceImportant "---------------------------------------------------------------------"
    printfn ""

let buildquickstarter (platform: string, configuration : string, architecture : string, quickstarter : string) = 
    match platform with
    | "Windows" -> MSBuild null "Build" [("Configuration", configuration); ("Platform", architecture)] [quickstarter] |> ignore
    | "Linux" -> Exec "xbuild" ("/p:Configuration=" + configuration + " " + quickstarter)
    | "MacOS" -> Exec "/Applications/Xamarin Studio.app/Contents/MacOS/mdtool" ("-v build -t:Build -c:" + configuration + " " + quickstarter)
    | _-> ()

let buildquickstarters(platform: string) =
    for quickstarter in Directory.GetFiles(rootFolder, ("*" + platform + ".sln"), SearchOption.AllDirectories) do
        traceImportant ("Project " + quickstarter)

        let mutable flag = true

        try
            traceImportant ("restoring..")
            RestorePackages quickstarter

            traceImportant ("Building...")
            

            buildquickstarter (platform, configuration, architecture, quickstarter)
        with
            | _ -> flag <- false
        
        processResults quickstarter flag

    printReport items

Target "environment-var" (fun () ->
    let variablePath = System.IO.Path.GetFullPath("WaveEngine.WindowsTools/");
    trace variablePath
    let variableName = "WaveEngine"

    setEnvironVar variableName variablePath
    trace "Environment Variable created"
)

Target "restore-windowstools" (fun() ->
    traceImportant "Clear tools directory"
    DeleteDirs [WaveToolDirectory]

    traceImportant "Get WaveEngine.WindowsTools nuget packages"
    let nugetArgs = " install " + WaveToolDirectory + " -ExcludeVersion -ConfigFile NuGet\NuGet.config"
    trace nugetArgs
    Exec "NuGet/nuget.exe" nugetArgs

    traceImportant "Generate waveengine installation path"
    let target = WaveToolDirectory + "/v2.0/Tools/VisualEditor/"
    !! (WaveToolDirectory + "/tools/*.*")
        |> CopyFiles target
)

Target "update-nightlypackages" (fun() ->
    traceImportant "Update to nightly nuget packages"
    Exec "WaveTools/UpdateToNightlyPackages.exe" rootFolder
)

Target "windows-quickstarters" (fun () ->
    buildquickstarters("Windows")
)

Target "macos-quickstarters" (fun () ->
    buildquickstarters("MacOS")
)

Target "linux-quickstarters" (fun () ->
    buildquickstarters("Linux")
)

"restore-windowstools"
    ==> "environment-var"
    ==> "update-nightlypackages"
    ==> "windows-quickstarters"
 