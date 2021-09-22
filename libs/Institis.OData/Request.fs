namespace Institis.OData.Message

open System
open System.Collections.Generic
open FSharpPlus
open Microsoft.OData
open Microsoft.OData.Edm
open Microsoft.OData.UriParser

type Request (model: EdmModel, url: Uri, headers: IDictionary<string,string>, method: string) =
    
    let mutable _headers = headers
    
    do
        if (url.IsAbsoluteUri) then 
            invalidArg "url" "The URL must be relative"
    
    interface IODataRequestMessage with
        member this.GetHeader header = _headers.Item header
        member this.SetHeader(header, value) = _headers.Item header <- value
        member this.Headers with get () = upcast _headers 

        member this.Method
            with get () = method
            and set value = raise (NotImplementedException())

        member this.Url
            with get () = url
            and set value = raise (NotImplementedException())

        member this.GetStream() = raise (NotImplementedException())


    member val Parser  = ODataUriParser(model, url)


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
    
    
    let parserType (parser: ODataUriParser) : ODataUriType = raise (NotImplementedException())
