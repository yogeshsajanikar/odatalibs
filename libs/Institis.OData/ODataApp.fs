namespace Institis.OData.Application

open System
open Institis.OData.Message
open Microsoft.AspNetCore.Http
open Microsoft.OData
open Microsoft.OData.Edm

type ODataContext (context: HttpContext) =
    
    class end

// An ODATA application essentially converts an ODATA URI to a correct response. For the simplicity, the response
// is measured as a stream

/// An OData application is the one that takes an odata request, and converts it into a response.
type IApp =
    interface
        /// Handle the URI Request, and write the output to the given stream.
        abstract handle : HttpContext -> (HttpContext -> IODataRequestMessageAsync) -> (HttpContext -> IODataResponseMessageAsync)
    end

/// An ODATA app with an ability to handle each part of the URI separately.
[<AbstractClass>]
type App() =

    abstract handle : Request * MetaData * Response -> Async<Response>
    abstract handle : Request * Batch * Response -> Async<Response>
    abstract handle : Request * CollectionNavigation * Response -> Async<Response>

    interface IApp with

        member this.handle request response =
            let oDataUri = 
            match ODataRequest.parserType request.ODataUri with
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
