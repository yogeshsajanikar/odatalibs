# OData and Giraffe

### OData 

[OData](https://www.odata.org/) defines a set of REST APIs based on Entity Data Model. [OData Library](https://github.com/OData/odata.net) 
is a C# based library for working with ODATA and ASP.NET.  

### Giraffe 
[Giraffe](https://giraffe.wiki/) is a functional, fsharp based micro web framework. 

## Institis.Giraffe.OData

This is a library to write functional first odata web service. It is possible to write
highly compositional OData service.

### Examples

#### Adding EDM model

`oDataRoute` is a web handler that inserts EDM model into Giraffe. It parses the OData
URL and make it available as a `ODataRequest` in the context available for subsequent
web handlers.

```f#
    open Institis.Giraffe.OData.Routing

    let model: EdmModel = ...
        GET >=> oDataRoute model >=> handlers 
    ]
```

#### Metadata 

Once `oDataRoute` has successfully parsed the path, it is possible to serve metadata
by using `metadata` which matches the OData metadata request, along with `metadoc` which
serializes the EDM to XML. 

```f#
    GET >=> oDataRoute model >=> choose [
        metadata >=> metadoc
        setStatusCode 500 "Error serializing metadata"
    ]

```

### [IN PROGRESS]

