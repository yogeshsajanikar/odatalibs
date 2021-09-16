module Institis.Giraffe.OData.Message

open System
open System.Collections
open System.Collections.Generic
open System.IO
open Microsoft.OData

type Response (stream: Stream) =
    let mutable headers: Map<string,string> = Map.empty
    
    interface IODataResponseMessage with
    
        member val StatusCode  = 200 with get, set
        
        member this.Headers with get () = headers |> Map.toSeq |> Seq.map (fun (k,v) -> KeyValuePair(k, v))
        
        member this.GetHeader header = raise (NotImplementedException())
        
        member this.SetHeader (header, value) = headers <- headers |> Map.add header value 
        
        member this.GetStream () = stream


