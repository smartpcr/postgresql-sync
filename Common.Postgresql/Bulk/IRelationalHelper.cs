using Npgsql.Bulk.Model;
using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.Bulk
{
    internal interface IRelationalHelper
    {
        NpgsqlConnection GetNpgsqlConnection(DbContext context);


        IDbContextTransaction EnsureOrStartTransaction(DbContext context, IsolationLevel defaultIsolationLevel);

        List<ColumnInfo> GetColumnsInfo(DbContext context, string tableName);
    }
}
