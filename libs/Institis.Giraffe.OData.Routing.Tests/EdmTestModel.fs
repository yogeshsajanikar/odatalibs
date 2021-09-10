namespace Institis.Giraffe.OData

open Microsoft.OData.Edm


module TestModel =
    
    let edmNamespace = "Institis.OData"
    let typeNamespace = $"{edmNamespace}.Types"
    
    let edmModel =
        let model = EdmModel()
        
        let idType = EdmTypeDefinition(typeNamespace, "IdType", EdmPrimitiveTypeKind.Double)
        let idTypeRef = EdmTypeDefinitionReference(idType, false)
        
        model.AddElement idType
        
        let stringType = EdmTypeDefinition(typeNamespace, "StringType", EdmPrimitiveTypeKind.String)
        let stringTypeRef = EdmTypeDefinitionReference(stringType, false)
        
        let personType = EdmEntityType(typeNamespace, "Person")
        let employeeType = EdmEntityType(typeNamespace, "Employee", personType)
        
        let personKey = personType.AddStructuralProperty ("ID", EdmCoreModel.Instance.GetInt32 false)
        personType.AddStructuralProperty("SSN", EdmCoreModel.Instance.GetString true) |> ignore
        personType.AddKeys personKey

        model.AddElement personType
        model.AddElement employeeType
        
        let container = EdmEntityContainer(typeNamespace, "Types")
        container.AddEntitySet ("Persons", personType) |> ignore
        
        model.AddElement container        
        model
