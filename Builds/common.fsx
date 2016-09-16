module Common
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
    
    let projects = Directory.GetFiles(rootFolder, String.Format("*{0}*.sln", platform), SearchOption.AllDirectories)           

    for quickstarter in projects do
    
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