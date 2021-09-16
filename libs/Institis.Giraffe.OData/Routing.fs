namespace Institis.Giraffe.OData

open System
open System.IO
open System.Xml
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.OData
open Microsoft.OData.Edm
open Microsoft.OData.Edm.Csdl
open Microsoft.OData.UriParser
open FSharp.Control.Tasks


module Routing =

    type ODataRequest(model: EdmModel, uriParser: ODataUriParser, path: ODataPath) =

        member _.model = model
        member _.parser = uriParser
        member _.path = path

    [<Literal>]
    let odataKey =
        "Institis-OData-05be63fe-d45f-4e5f-9afb-3213974808dd"

    let getODataRequestContext (ctx: HttpContext) : ODataRequest option =
        match ctx.Items.ContainsKey odataKey with
        | true ->
            match ctx.Items.Item odataKey with
            | :? ODataRequest as oRequest -> Some oRequest
            | _ -> None
        | _ -> None


    exception ODataRequestContextAlreadySet of ODataRequest

    let setODataRequestContext (req: ODataRequest) (ctx: HttpContext) : unit =
        match ctx.Items.ContainsKey odataKey with
        | true ->
            ctx
            |> getODataRequestContext
            |> Option.map (fun x -> raise (ODataRequestContextAlreadySet x))
            |> ignore
        | _ -> ctx.Items.Item odataKey <- req

    /// <summary>
    /// Filters the odata routes for the input EDM model
    /// </summary>
    /// <param name="model"> Reference EDM Model </param>
    /// <param name="next"> Next handler in the pipeline </param>
    /// <param name="ctx"> Http context </param>
    /// <returns>A Giraffe <see cref="Giraffe.HttpHandler"/></returns>
    let odataRoute (model: EdmModel) (next: HttpFunc) (ctx: HttpContext) =
        let path =
            Uri(ctx |> SubRouting.getNextPartOfPath, UriKind.Relative)

        let parser = ODataUriParser(model, path)

        try
            let path = parser.ParsePath()

            ctx
            |> setODataRequestContext (ODataRequest(model, parser, path))

            next ctx
        with
        | :? ODataException -> skipPipeline
        | _ -> skipPipeline


    /// Match metadata request
    let metadata (next: HttpFunc) (ctx: HttpContext) =
        match ctx |> getODataRequestContext with
        | Some oDataReq when oDataReq.path.Count >= 1 ->
            match oDataReq.path.LastSegment with
            | :? MetadataSegment  -> next ctx
            | _ -> skipPipeline
        | _ -> skipPipeline


    // Dish out metadata document
    let metadoc (next: HttpFunc) (ctx: HttpContext) =
        ctx.SetContentType "application/xml; charset=utf-8"

        match ctx |> getODataRequestContext with
        | None -> skipPipeline
        | Some oDataReq ->

            let writeMeta (output: Stream) =
                async {
                    use xmlWriter = XmlWriter.Create(output)

                    let success, _ =
                        CsdlWriter.TryWriteCsdl(oDataReq.model, xmlWriter, CsdlTarget.OData)

                    xmlWriter.Flush()
                    return success
                }

            task {
                use metaStream = new MemoryStream()
                let! success = writeMeta metaStream |> Async.StartAsTask

                if success then
                    metaStream.Position <- 0L
                    return! ctx.WriteStreamAsync(true, metaStream, None, None)
                else
                    return! skipPipeline
            }
