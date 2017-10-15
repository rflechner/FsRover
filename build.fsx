#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open System
open System.Text
open ProcessHelper
open EnvironmentHelper

let buildDir = "./release/"
ensureDirectory buildDir

Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "Build" (fun _ ->
    trace "Building default target"
    RestorePackages()
    !! "**/*.sln" |> MSBuildRelease buildDir "Build" |> Log "BuildLib-Output: "
)

"Clean"
 ==> "Build"

RunTargetOrDefault "Build"
