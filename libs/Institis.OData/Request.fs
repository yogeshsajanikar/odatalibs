module Institis.OData.Message

open System
open System.Collections.Generic
open FSharpPlus.Operators
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Microsoft.OData
open Microsoft.OData.Edm
open Microsoft.AspNetCore.Http.Extensions
open Microsoft.OData.UriParser

type Request (model: EdmModel, context: HttpContext) =
    
    interface IODataRequestMessageAsync with
        member this.GetHeader header =
            let sValue = context.Request.Headers.Item header
            sValue.ToString()
            
        member this.SetHeader(header, value) = context.Request.Headers.Item header <- StringValues(value)
        member this.Headers with get () = context.Request.Headers |> Seq.map (fun kv -> KeyValuePair(kv.Key, kv.Value.ToString()))

        member this.Method
            with get () = context.Request.Method
            and set value = raise (NotImplementedException())

        member this.Url
            with get () = Uri(context.Request.GetEncodedUrl())
            and set value = raise (NotImplementedException())

        member this.GetStream() = context.Request.Body
        
        member this.GetStreamAsync() = async { return context.Request.Body } |> Async.StartAsTask
        
        
type ODataRequestComponent =
    | ODataRequestPath of ODataPathSegment * ODataUri
    | ODataQueryParam of ODataUri
    
let requestSegments (uri: ODataUri) = 
    let paths = uri.Path |> Seq.map (fun p -> ODataRequestPath (p, uri))
    seq {
        yield! paths
    }
    
let requestQueryParams (uri: ODataUri) =
    seq {
        yield ODataQueryParam uri
    }

let requestComponents (uri: ODataUri) =
    seq {
        yield! requestSegments uri
        yield! requestQueryParams uri 
    }
    
let navigationSource () = raise (NotImplementedException())


let canonicalUri (comps: seq<ODataRequestComponent>) : string = raise (NotImplementedException()) 

type CollectionNavigation = NA
type SingleNavigation = NA
type ActionImportCall = NA

type MetaDataFormat = MetaXml | MetaJson 
type MetaData = MetaData of format: MetaDataFormat
type Batch = NA


// Resource in OData can be of following type
type ResourceUriType =
    | EntitySetName of CollectionNavigation
    | Singleton of SingleNavigation
    | ActionImportCall of ActionImportCall
    | EntityColFunctionalImportCall of CollectionNavigation
    | EntityFunctionImportCall of SingleNavigation


// According to http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part2-url-conventions.html the URL can be of
// following types

type ODataUriType =
    | Invalid 
    | MetaUri of MetaData
    | BatchUri of Batch
    | ResourceUri of ResourceUriType



module ODataRequest =
    type private FirstSegmentHandler(parser: ODataUriParser) =
        inherit PathSegmentHandler()
        
        let _type = Invalid
        

//        member this.Handle (segment: MetadataSegment) =
//            let format =
//                match parser.CustomQueryOptions |> Seq.filter (fun kv -> kv.Key = "format") with
//                | s when s |> Seq.isEmpty -> MetaXml
//                | s when s |> Seq.head |> (fun x -> x.Key = "xml") -> MetaXml
//                | s when s |> Seq.head |> (fun x -> x.Key = "json") -> MetaXml
//                | 
//            
//            _type <- MetaUri 
    
    
    let parserType (parser: ODataUri) : ODataUriType = raise (NotImplementedException())


let oDataRequest (model: EdmModel) (ctx: HttpContext) : IODataRequestMessage = raise (NotImplementedException()) 
