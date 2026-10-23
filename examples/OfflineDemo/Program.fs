open System
open System.IO
open ClaudeAgentSdk
open FSharp.Control
open Thoth.Json.Net

[<EntryPoint>]
let main args =
    let greet = Mcp.tool "greet" "Local greeting example"
                    (Mcp.Schema.object' [Mcp.Schema.required "name" Mcp.Schema.string])
                    (fun input -> task {
                        match Mcp.getString "name" input with
                        | Ok name -> return Mcp.textResult $"Hello, {name}!"
                        | Error e -> return Mcp.errorResult e
                    })
    match greet.Handler(Encode.object ["name", Encode.string "Example"]).GetAwaiter().GetResult() with
    | ToolSuccess [TextContent text] -> printfn "Local tool: %s" text
    | other -> failwithf "Unexpected tool output: %A" other
    match greet.Handler(Encode.object []).GetAwaiter().GetResult() with
    | ToolFailure _ -> printfn "Missing name: rejected by handler"
    | _ -> failwith "Missing name was accepted"
    if args.Length > 0 then
        let scratch = Path.Combine(Path.GetTempPath(), "sdk-offline-" + Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory scratch |> ignore
        let logPath = Path.Combine(scratch,"inputs.jsonl")
        try
            let options = { Options.defaults with CliPath = Some(Path.GetFullPath args[0]); Env = Map.ofList ["DEMO_INPUT_LOG",logPath] }
            let inputs = TaskSeq.ofSeq ["user","first"; "user","second"]
            let received = Query.queryStreaming inputs options |> TaskSeq.toListAsync |> fun task -> task.GetAwaiter().GetResult()
            match received with
            | [Ok (ResultMsg result)] when result.SessionId = "offline-demo" -> ()
            | other -> failwithf "Unexpected stream result: %A" other
            let lines = File.ReadAllLines logPath
            if lines.Length <> 2 || not(lines[0].Contains("first")) || not(lines[1].Contains("second")) then
                failwith "Input ordering failed"
            printfn "Streaming fixture: 2 ordered inputs, 1 local result"
        finally
            Directory.Delete(scratch,true)
    printfn "No Claude account or model request was used."
    0
