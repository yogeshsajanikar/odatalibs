namespace Institis.OData.Application

open System
open Institis.OData.Message
open Microsoft.OData.Edm

// An ODATA application essentially converts an ODATA URI to a correct response. For the simplicity, the response
// is measured as a stream

/// A generic odata app, that can process the URI request and convert it into a result
type IApp =
    interface
        /// Handle the URI Request, and write the output to the given stream.
        abstract handle : Request -> Response -> Async<Response>
    end

/// An ODATA app with an ability to handle each part of the URI separately.
[<AbstractClass>]
type App() =

    abstract handle : Request * MetaData * Response -> Async<Response>
    abstract handle : Request * Batch * Response -> Async<Response>
    abstract handle : Request * CollectionNavigation * Response -> Async<Response>

    interface IApp with

        member this.handle request response =
            match ODataRequest.parserType request.Parser with
            | MetaUri x -> this.handle (request, x, response)
            | BatchUri x -> raise (NotImplementedException())
            | ResourceUri x ->
                match x with
                | EntitySetName x -> this.handle (request, x, response)
                | _ -> raise (NotImplementedException())


type EdmApp(model: EdmModel) =
    inherit App()

    override this.handle(request: Request, x: MetaData, response: Response) : Async<Response> = raise (NotImplementedException())
    override this.handle(request: Request, x: Batch, response: Response) : Async<Response> = raise (NotImplementedException())
    override this.handle(request: Request, x: CollectionNavigation, response: Response) : Async<Response> = raise (NotImplementedException())
