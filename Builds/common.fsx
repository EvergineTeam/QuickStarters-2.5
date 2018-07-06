module Common
#r @"FAKE.4.63/tools/FakeLib.dll"

open Fake
open Fake
open Fake.XamarinHelper
open System
open System.Collections.Generic
open System.IO
open System.Linq
open Fake.AssemblyInfoFile

exception BuildException of string

let configuration = "Debug"
let architecture = "Any CPU"
let rootFolder = "../"
let deployDir = "../Deploy/"
let artifactPath = deployDir + "report.txt"

let getFolder solutionFile= Path.GetDirectoryName(solutionFile)

let Exec command args =
    let result = Shell.Exec(command, args)

    if result <> 0 then failwithf "%s exited with error %d" command result

let RestorePackages solutionFile =
    RestoreMSSolutionPackages (fun p -> 
        { p with
            Sources = "https://www.myget.org/F/waveengine-nightly/api/v3/index.json" :: p.Sources
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
let mutable reportString = "\n";

let printReport (l : List<quickstarterReport>) =
               
    reportString <- sprintf "%s\n---------------------------------------------------------------------" reportString
    reportString <- sprintf "%s\nQuickstarters Report:" reportString
    reportString <- sprintf "%s\n---------------------------------------------------------------------" reportString
    l |> Seq.iteri (fun index item ->

        if l.[index].Result = status.Success then
            let r = String.Format("\n {0} -      Success - {1}", index.ToString(), l.[index].Path)   
            reportString <- sprintf "%s%s" reportString r               
            OkProjects <- OkProjects + 1
        else
            let r = String.Format("\n {0} -      Failed - {1}", index.ToString(), l.[index].Path)   
            reportString <- sprintf "%s%s" reportString r                    
        )

    reportString <- sprintf "%s\n" reportString
    let r = sprintf "\n   Projects success: %i / %i" OkProjects l.Count
    reportString <- sprintf "%s%s" reportString r
    reportString <- sprintf "%s\n---------------------------------------------------------------------" reportString
    reportString <- sprintf "%s\n" reportString

    // Print report
    printfn "%s" reportString

    // Create report file
    Fake.FileHelper.CreateDir deployDir    
    File.WriteAllText(artifactPath, reportString);   

    if (OkProjects < l.Count) then raise (BuildException("All starterkits not passed")) 

let buildquickstarter (platform: string, configuration : string, architecture : string, quickstarter : string) = 
    match platform with
    | "Linux" -> MSBuild null "Build" [("Configuration", configuration); ("Platform", "x86")] [quickstarter] |> ignore
    | _-> MSBuild null "Build" [("Configuration", configuration); ("Platform", architecture)] [quickstarter] |> ignore

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

// Publish reports
let reportartifact () = 
    let absoluteArtifactPath = System.IO.Path.GetFullPath(artifactPath)
    TeamCityHelper.PublishArtifact (absoluteArtifactPath)    

