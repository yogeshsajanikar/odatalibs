module Institis.Giraffe.OData.Tests.SegmentTest

open System
open Institis.Giraffe.OData
open FsUnit
open Microsoft.OData.Edm
open Microsoft.OData.UriParser
open Xunit
open TestModel

[<Fact>]
let ``Metadata URL should translate to metadata ancestry`` () =
    let uri = Uri("$metadata", UriKind.Relative)
    let model = edmModel
    let parser = ODataUriParser(model, uri)
    let seg = parser.ParsePath()
    let target = Segment.segmentTarget seg.LastSegment

    target.targetKind
    |> should be (equal EdmTypeKind.None)

[<Fact>]
let ``Persons URL should translate to entity set ancestry`` () =
    let uri = Uri("Persons", UriKind.Relative)
    let model = edmModel
    let parser = ODataUriParser(model, uri)
    let seg = parser.ParsePath()
    let target = Segment.segmentTarget seg.LastSegment
    target.isEntitySet |> should be True

[<Fact>]
let ``Persons with type URL should translate to entity set ancestry with type`` () =
    let uri =
        Uri("Persons/Institis.OData.Types.Person", UriKind.Relative)

    let model = edmModel
    let parser = ODataUriParser(model, uri)
    let seg = parser.ParsePath()
    let target = Segment.segments seg
    target.isEntitySet |> should be True
