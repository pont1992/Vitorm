﻿using System;

using Vitorm.Sql.DataReader;
using Vitorm.Sql.SqlTranslate;
using Vitorm.StreamQuery;

namespace Vitorm.SqlServer.SqlTranslate
{
    public class QueryTranslateService : BaseQueryTranslateService
    {

        /* // sql
SELECT [t].[id], [t].[birth], [t].[fatherId], [t].[motherId], [t].[name]
FROM (
    SELECT [m].[id], [m].[birth], [m].[fatherId], [m].[motherId], [m].[name], ROW_NUMBER() OVER(ORDER BY [m].[fatherId], [m].[motherId] DESC) AS [__RowNumber__]
    FROM [User] AS [m]
    WHERE [m].[id] <> 2
) AS [t]
WHERE ([t].[__RowNumber__] > 1) AND ([t].[__RowNumber__] <= 13);

// if no orders:
ROW_NUMBER() OVER(ORDER BY @@RowCount) AS [__RowNumber__]

         */

        public QueryTranslateService(SqlTranslateService sqlTranslator) : base(sqlTranslator)
        {
        }

  
        public override string BuildCountQuery(QueryTranslateArgument arg, CombinedStream stream)
        {
            // select count(*) from (select distinct fatherid,motherId from "User" u) u;
            return $"select count(*) from ({BuildQuery(arg, stream)}) u;";
        }

        protected override string ReadSelect(QueryTranslateArgument arg, CombinedStream stream, string prefix = "select")
        {
            switch (stream.method)
            {
                case "Count":
                    //return prefix + " " + "count(*)";
                case "" or null or "ToList" or nameof(Orm_Extensions.ToExecuteString):
                    {
                        var reader = new DataReader();
                        return prefix + " " + BuildReader(arg, stream, reader);
                    }
                case "FirstOrDefault" or "First" or "LastOrDefault" or "Last":
                    {
                        stream.take = 1;
                        stream.skip = null;

                        if (stream.method.Contains("Last"))
                            ReverseOrder(arg, stream);

                        var nullable = stream.method.Contains("OrDefault");
                        var reader = new DataReader_FirstRow { nullable = nullable };
                        return prefix + " " + BuildReader(arg, stream, reader);
                    }
            }
            throw new NotSupportedException("not supported method: " + stream.method);
        }


    }
}
