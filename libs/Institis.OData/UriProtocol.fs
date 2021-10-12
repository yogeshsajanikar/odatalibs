module Institis.OData.UriProtocol

open Microsoft.OData
open Microsoft.OData.UriParser

type ODataRelativeUri =
    | Batch of BatchSegment
    | Metadata of MetadataSegment
    | Entity
    | Resource of ResourcePath
    
and ResourcePath =
    | EntitySet of CollectionNavigation option 
    | Singleton of SingletonNavigation option 
    | ActionImport
    | EntityColumnFunctionImport of CollectionNavigation option 
    | EntityFunctionImport of SingleNavigation option 
    | ComplexColumnFunctionImport of ComplexColumnPath option 
    | ComplexFunctionImport of ComplexPath option 
    | PrimitiveColumnFunctionImport of PrimitiveColumnPath option 
    | FunctionImport of QuerySegment option 
    | CrossJoin of QuerySegment option 
    | All of QualifiedTypeName option
    
and CollectionNavigation =
    CollectionNavigation of QualifiedTypeName option * CollectionNavigationPath
    
and CollectionNavigationPath =
    | KeyPredicate of KeyPredicate * SingleNavigation option 
    | FilterInPath of FilterInPath * CollectionNavigation option
    | Each of Each * BoundOperation option
    | Count of Count
    | Ref of Ref
    | QuerySegment of QuerySegment
    
and KeyPredicate =
    | SimpleKey of KeyPropertyValue
    | CompoundKey of KeyValuePair list 
    | KeyPathSegments
    
and SimpleKey = 
