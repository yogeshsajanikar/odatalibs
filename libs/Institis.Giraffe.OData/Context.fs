namespace Institis.Giraffe.OData

// Note: This is taken from https://github.com/OData/odata.net/blob/4e263ca4b94d01f2d105d8282c62ec272cfcd1bb/test/EndToEndTests/Services/ODataWCFLibrary/PathSegmentToExpressionTranslator.cs

[<AutoOpen>]
module Context =

    type Context = bool option
