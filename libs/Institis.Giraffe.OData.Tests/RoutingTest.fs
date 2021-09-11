namespace Institis.Giraffe.OData

open System.Collections.Generic
open System.IO
open System.Text
open System.Threading.Tasks
open FsUnit
open Microsoft.AspNetCore.Http
open Microsoft.OData.Edm
open NSubstitute
open Xunit
open Giraffe
open Institis.Giraffe.OData
open FSharp.Control.Tasks

module RoutingTest =
    
    let next : HttpFunc = Some >> Task.FromResult
    
    let getBody (ctx : HttpContext) =
        ctx.Response.Body.Position <- 0L
        use reader = new StreamReader(ctx.Response.Body, Encoding.UTF8)
        reader.ReadToEnd()


    [<Fact>]
    let ``odataRoute GET: "/Persons" should return "Person found"`` () =
        let context = Substitute.For<HttpContext>()
        let model = TestModel.edmModel
        let app = GET >=> choose [
            Routing.odataRoute model >=> text "Person found"
            setStatusCode 404 >=> text "Not found"
        ]
        
        context.Request.Method.ReturnsForAnyArgs "GET" |> ignore
        context.Request.Path.ReturnsForAnyArgs (PathString("/Persons")) |> ignore
        context.Response.Body <- new MemoryStream()
        context.Items <- Dictionary() 
        
        task {
            let! result = app next context
            match result with
            | None     -> result |> should not' (equal None)
            | Some ctx ->
                ctx |> getBody |> should be (equal "Person found")
                ctx.Items.ContainsKey Routing.odataKey |> should be (equal true)

        }

    [<Fact>]
    let ``odataRoute GET: "/Persons" should return 404`` () =
            let context = Substitute.For<HttpContext>()
            let model = EdmModel()
            let app = GET >=> choose [
                Routing.odataRoute model >=> text "Person found"
                setStatusCode 404 >=> text "Not found"
            ]
            
            context.Request.Method.ReturnsForAnyArgs "GET" |> ignore
            context.Request.Path.ReturnsForAnyArgs (PathString("/Persons")) |> ignore
            context.Response.StatusCode.ReturnsForAnyArgs (404) |> ignore
            context.Response.Body <- new MemoryStream()
            
            task {
                let! result = app next context
                match result with
                | None     -> result |> should not' (equal None)
                | Some ctx ->
                    ctx.Response.StatusCode |> should be (equal 404)
                    ctx.Items.ContainsKey Routing.odataKey |> should be (equal false)
            }
            
    [<Fact>]
    let ``odataRoute GET: "/$metadata" should handle metadata`` () =
        let context = Substitute.For<HttpContext>()
        let model = TestModel.edmModel
        let app = GET >=> choose [
            Routing.odataRoute model >=> choose [
                Routing.metadata >=> Routing.metadoc
                setStatusCode 500 >=> text "Error serializing metadata"
            ]
            setStatusCode 404 >=> text "Not found"
        ]
        
        context.Request.Method.ReturnsForAnyArgs "GET" |> ignore
        context.Request.Path.ReturnsForAnyArgs (PathString("/$metadata")) |> ignore
        context.Response.Body <- new MemoryStream()
        context.Items <- Dictionary() 
        
        task {
            let! result = app next context
            match result with
            | None     -> result |> should not' (equal None)
            | Some ctx ->
                ctx |> getBody |> should startWith "<?xml"
                ctx.Items.ContainsKey Routing.odataKey |> should be (equal true)
                let oContext = ctx |> Routing.getODataRequestContext |> Option.map (fun oContext -> oContext.path)
                oContext |> should not' (equal None)
                oContext |> Option.get |> (fun oCtx ->
                        oCtx.Count |> should be (equal 1)
                    )
        }
        
    [<Fact>]
    let ``odataRoute GET: "/Persons/Institis.OData.Types.Person" should handle correctly`` () =
        let context = Substitute.For<HttpContext>()
        let model = TestModel.edmModel
        let app = GET >=> choose [
            Routing.odataRoute model >=> text "Returning Institis.OData.Types.Person"
            setStatusCode 404 >=> text "Not found"
        ]
        
        context.Request.Method.ReturnsForAnyArgs "GET" |> ignore
        context.Request.Path.ReturnsForAnyArgs (PathString("/Persons/Institis.OData.Types.Person")) |> ignore
        context.Response.Body <- new MemoryStream()
        context.Items <- Dictionary() 
        
        task {
            let! result = app next context
            match result with
            | None     -> result |> should not' (equal None)
            | Some ctx ->
                ctx |> getBody |> should be (equal "Returning Institis.OData.Types.Person")
                ctx.Items.ContainsKey Routing.odataKey |> should be (equal true)
                let oContext = ctx |> Routing.getODataRequestContext |> Option.map (fun oContext -> oContext.path)
                oContext |> should not' (equal None)
                oContext |> Option.get |> (fun oCtx ->
                        oCtx.Count |> should be (equal 2)
                    )
        }