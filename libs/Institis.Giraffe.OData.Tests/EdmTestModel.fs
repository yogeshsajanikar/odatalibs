namespace Institis.Giraffe.OData

open System.IO
open System.Xml
open Microsoft.OData.Edm
open FsUnit
open Microsoft.OData.Edm.Csdl
open Xunit
open FSharp.Control.Tasks

module TestModel =

    let edmNamespace = "Institis.OData"
    let typeNamespace = $"{edmNamespace}.Types"

    let edmModel =
        let model = EdmModel()

        let idType =
            EdmTypeDefinition(typeNamespace, "IdType", EdmPrimitiveTypeKind.Double)

        let idTypeRef =
            EdmTypeDefinitionReference(idType, false)

        model.AddElement idType

        let stringType =
            EdmTypeDefinition(typeNamespace, "StringType", EdmPrimitiveTypeKind.String)

        let stringTypeRef =
            EdmTypeDefinitionReference(stringType, false)

        let personType = EdmEntityType(typeNamespace, "Person")

        let employeeType =
            EdmEntityType(typeNamespace, "Employee", personType)

        let personKey =
            personType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32 false)

        personType.AddStructuralProperty("SSN", EdmCoreModel.Instance.GetString true)
        |> ignore

        personType.AddKeys personKey

        model.AddElement personType
        model.AddElement employeeType

        let container =
            EdmEntityContainer(typeNamespace, "Types")

        container.AddEntitySet("Persons", personType)
        |> ignore

        model.AddElement container
        model


    [<Fact>]
    let ``Model should be serialized to XML`` () =

        let writeMeta (outStream: Stream) =
            async {
                use xmlWriter = XmlWriter.Create(outStream)

                let success, _ =
                    CsdlWriter.TryWriteCsdl(edmModel, xmlWriter, CsdlTarget.OData)

                return success
            }

        task {
            use metaStream = new MemoryStream()
            let! result = writeMeta metaStream |> Async.StartAsTask
            result |> should be True
            metaStream.Position <- 0L
            use xmlReader = XmlReader.Create metaStream
            let readResult, model, _  = CsdlReader.TryParse xmlReader
            readResult |> should be True
        }
