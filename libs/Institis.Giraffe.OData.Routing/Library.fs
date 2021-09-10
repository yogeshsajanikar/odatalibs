namespace Institis.Giraffe.OData

open System
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.OData.Edm
open Microsoft.OData.UriParser

[<RequireQualifiedAccess>]
module Routing =

    /// <summary>
    /// Filters the odata routes for the input EDM model
    /// </summary>
    /// <param name="serviceRoot">Root URI of the service</param>
    /// <param name="model"> Reference EDM Model </param>
    /// <returns>A Giraffe <see cref="Giraffe.HttpHandler"/></returns>
    let odataRoute (serviceRoot: Uri) (model: EdmModel) : HttpHandler =
        let parser = ODataUriParser(model, serviceRoot)
        fun (next: HttpFunc) (ctx: HttpContext) ->
            parser.ParsePath
            raise (NotImplementedException())

