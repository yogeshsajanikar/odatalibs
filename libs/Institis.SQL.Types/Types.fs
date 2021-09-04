namespace Institis.SQL.Types

    open System.Collections.Generic
    
    module Data =
    
        type Literal =
            | String of string
            | Integer32 of int32
            | Integer64 of int64
            | Boolean of bool
            
        type FuncArgs =
            | PositionalArgs of Expression array
            | NamedArgs of IDictionary<string, Expression>
            
        and FuncCall = {
            name: string
            args: FuncArgs
        }
        
        and OpExpr =
            | Binary of string * Expression * Expression 
            | Unary of string * Expression
            
        and Parameter =
            | QParam
            | NParam of int
            | NameParam of string
            
        and Reference =
            | RName of string 
            
        and Expression =
            | Val of Literal
            | Ref of Reference
            | FuncCall of FuncCall
            | OpExpr of OpExpr
            | Parameter of Parameter
            | Select of Selection
            
        and Source =
            | Reference of Reference
            | Select of Selection * string
            | Join of Join
            
        and ColumnExpression = {
            expression: Expression
            asName: string
        }
           
        and Projection =
            | SelectAll
            | CExpr of ColumnExpression array
            
        and SortOrder = Ascending | Descending
        
        and NullOrder = FirstNulls | LastNulls
            
        and OrderTerm = {
            orderExpr: Expression
            sort: SortOrder option
            nulls: NullOrder option
        }
            
        and Selection = {
            from: Source
            distinct: bool option
            columns: Projection
            where: OpExpr
            groupBy: Expression array
            orderBy: OrderTerm
        }
        
        and JoinType = | LeftJoin | RightJoin | FullJoin | InnerJoin | CrossJoin 
        
        and Join = {
            JoinType: JoinType
            first: Source
            second: Source
            Condition: OpExpr
        }
            
        and SQL =
            | Select of Selection