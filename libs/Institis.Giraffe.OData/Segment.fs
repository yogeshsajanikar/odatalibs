namespace Institis.Giraffe.OData

open System
open Microsoft.OData.Edm
open Microsoft.OData.Edm
open Microsoft.OData.UriParser


[<RequireQualifiedAccess>]
module Segment =
    
    let private typeToKind: IEdmType option -> EdmTypeKind = function
        | None -> EdmTypeKind.None
        | Some x -> x.TypeKind
        
    let private typeToElementType (edmType : IEdmType option) =
        edmType
        |> Option.filter (fun x -> x.TypeKind = EdmTypeKind.Collection)
        |> Option.map (fun x -> (x :?> IEdmCollectionType).ElementType.Definition )
    
    type TargetAncestry =
        {
            parents: ODataPathSegment list
            children: ODataPathSegment list
            canonical: ODataPathSegment list
            targetType: IEdmType option
            elementType: IEdmType option
            navigationSource: IEdmNavigationSource option
            property: IEdmStructuralProperty option
            isRaw: bool
            isRef: bool
            isOperation: bool
            dynamic: string option 
             }
            member this.targetKind with get () = typeToKind this.targetType
            member this.elementKind with get () = typeToKind this.elementType
            member this.isSingleton with get () =
                match this.navigationSource with
                | Some x when x.NavigationSourceKind() = EdmNavigationSourceKind.Singleton -> true
                | _                                                                        -> false 
            member this.isEntitySet with get () =
                match this.navigationSource with
                | Some x when this.targetKind = EdmTypeKind.Collection -> true
                | _                                                    -> false
            
    let updateAncestry (segment: ODataPathSegment) (ancestry: TargetAncestry) =
        {
         ancestry with
            parents = ancestry.children @ ancestry.parents
            children = [segment]
        }
    
    let emptyAncestry = {
        parents = List.empty
        children = List.empty
        canonical = List.empty
        targetType = None
        elementType = None
        navigationSource = None
        property = None
        isRaw = false
        isOperation = false
        isRef = false
        dynamic = None
    }
    
    /// Handle the path and convert it to a target
    type SegmentHandler () =
        inherit PathSegmentHandler ()
        
        let mutable ancestry = emptyAncestry
            
        member this.targetAncestry with get () = ancestry
            
        member this.HandleDefault (segment: ODataPathSegment) = 
            let targetType = Some segment.EdmType
            ancestry <- {
                    updateAncestry segment ancestry with 
                        targetType = targetType
                        elementType = targetType |> typeToElementType  
                        canonical  =  segment :: ancestry.canonical
            }
    
        override this.Handle (segment: TypeSegment) = this.HandleDefault segment
            
            
        override this.Handle (segment: NavigationPropertySegment) =
            this.HandleDefault segment
            ancestry <- { ancestry with navigationSource = Some segment.NavigationSource }
            
        override this.Handle (segment: EntitySetSegment) = 
            this.HandleDefault segment
            ancestry <- {
                ancestry with
                    navigationSource = Some (upcast segment.EntitySet)
                    canonical = [ segment :> ODataPathSegment ]
            }

        override this.Handle (segment: SingletonSegment) =
            this.HandleDefault segment
            ancestry <- {
                ancestry with 
                    navigationSource = Some (upcast segment.Singleton)
                    canonical  =  [ segment :> ODataPathSegment ]
            }
        
        
        override this.Handle (segment: KeySegment) =
            this.HandleDefault segment
        
        override this.Handle (segment: PropertySegment) =
            this.HandleDefault segment
            ancestry <- { ancestry with property = Some segment.Property }

        override this.Handle (segment: OperationImportSegment) =
            this.HandleDefault segment
            ancestry <- {
                ancestry with
                    navigationSource = Some (upcast segment.EntitySet)
                    isOperation = true
            }

        override this.Handle (segment: OperationSegment) =
            this.HandleDefault segment
            ancestry <- {
                ancestry with
                    navigationSource = Some (upcast segment.EntitySet)
                    isOperation = true
            }
                                        
        override this.Handle (segment: DynamicPathSegment) =
            this.HandleDefault segment
            ancestry <- {
                ancestry with
                    dynamic = Some segment.Identifier
            }
            
        override this.Handle (segment: CountSegment) =
            this.HandleDefault segment
            ancestry <- {
                ancestry with
                    isRaw = true
            }
        
        override this.Handle (segment: NavigationPropertyLinkSegment) =
            this.HandleDefault segment
            ancestry <- {
                ancestry with
                    navigationSource = Some segment.NavigationSource
                    isRef = true
            }                
        override this.Handle (segment: ValueSegment) =
            this.HandleDefault segment
            ancestry <- {
                ancestry with
                    isRaw = true
            }
            
        override this.Handle (segment: BatchSegment) =
            this.HandleDefault segment
            ancestry <- {
                ancestry with
                    targetType = None
                    elementType = None
            }
            
        override this.Handle (segment: MetadataSegment) =
            ancestry <- {
                ancestry with
                    targetType = None
                    elementType = None
                    children = segment :> ODataPathSegment :: ancestry.children
                    canonical = segment :> ODataPathSegment :: ancestry.canonical
            }
        
        override this.Handle (segment: BatchReferenceSegment) =
            raise (NotImplementedException()) |> ignore 
        
        override this.Handle (segment: AnnotationSegment) =
            raise (NotImplementedException()) |> ignore 

        /// Return a list of canonicals 
        member this.buildCanonical (root: Uri) (key: KeySegment option) =
            let canonical = match key with
                            | Some kSegment when this.targetAncestry.isEntitySet ->
                                if not (List.isEmpty this.targetAncestry.canonical) && List.head this.targetAncestry.canonical :? TypeSegment then
                                    kSegment :> ODataPathSegment :: this.targetAncestry.canonical
                                else
                                    this.targetAncestry.canonical @ [kSegment :> ODataPathSegment]
                            | _                                                  ->
                                this.targetAncestry.canonical
                                
            canonical |> List.rev 
            
                 
    
    let segmentTarget (segment: ODataPathSegment) =
        let handler = SegmentHandler ()
        segment.HandleWith handler
        handler.targetAncestry            