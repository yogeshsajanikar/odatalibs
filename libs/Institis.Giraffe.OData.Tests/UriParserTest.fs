namespace Institis.Giraffe.OData

open System
open Microsoft.OData
open Microsoft.OData.Edm
open Microsoft.OData.UriParser
open Xunit
open FsUnit.Xunit


module UriParserTests =
    
    //open Institis.Giraffe.OData
    
    [<Fact>]
    let ``URI Parser parses relative URI`` () =
       let parser = ODataUriParser (TestModel.edmModel, Uri("Persons", UriKind.Relative))
       let oDataPath = parser.ParsePath ()
       Assert.Equal(1, oDataPath.Count)
       Assert.Equal("Persons", oDataPath.FirstSegment.Identifier)


    [<Fact>]
    let ``URI Parser raises exception for invalid path`` () =
        (fun () -> 
            let parser = ODataUriParser (EdmModel(), Uri("Persons", UriKind.Relative))
            parser.ParsePath () |> ignore
            ) |> should throw typeof<ODataUnrecognizedPathException>
            
        