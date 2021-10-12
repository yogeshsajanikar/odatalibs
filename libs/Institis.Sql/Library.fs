module Institis.Sql.Query

open Microsoft.FSharp.Quotations
open Microsoft.OData.Edm

type Query<'T> = Q

type QueryBuilder () =
    
    member this.For (tz: Query<'T>, f: 'T -> Query<'R>) : Query<'R> = Q
    
    member this.Yield (t: 'T) : Query<'T> = Q
    
    member this.Quote (expr: Expr<_>) = expr
    
    
    