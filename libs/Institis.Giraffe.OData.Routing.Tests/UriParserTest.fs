namespace Institis.Giraffe.OData

open System
open Microsoft.OData.UriParser
open Xunit


module UriParserTests =
    
    //open Institis.Giraffe.OData
    
    [<Fact>]
    let ``URI Parser parses relative URI`` () =
       let parser = ODataUriParser (TestModel.edmModel, Uri("Persons", UriKind.Relative))
       let opath = parser.ParsePath ()
       Assert.Equal(1, opath.Count)

