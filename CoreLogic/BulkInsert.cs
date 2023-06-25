using FastMember;
using MatrixApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using sun.util.logging;
using sun.util.logging.resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace MatrixApp.CoreLogic
{
    public static class BulkInsert
    {

        public static void BulkCopyToServer<T>(this DbContext db, IEnumerable<T> collection, List<string> exclude)
        {
            var messageEntityType = db.Model.FindEntityType(typeof(T));

            string? tableName = messageEntityType.GetSchema() + "." + messageEntityType.GetTableName();
            Dictionary<string, string>? tableColumnMappings = messageEntityType.GetProperties()
                .ToDictionary(p => p.PropertyInfo.Name, p => p.GetColumnName());
            IKey PrimaryKey = messageEntityType.FindPrimaryKey();
            string? conString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionString").Value;
            using (SqlConnection? connection = new SqlConnection(conString))
            {
                using (SqlBulkCopy? bulkCopy = new SqlBulkCopy(conString, SqlBulkCopyOptions.TableLock & SqlBulkCopyOptions.UseInternalTransaction))
                {
                    foreach ((string field, string column) in tableColumnMappings)
                    {       //need to exclude special or calculated columns that can't be inserted into
                        if (exclude.Contains(column))
                        {
                            continue;
                        }
                        bulkCopy.ColumnMappings.Add(field, column);
                    }
                    using (ObjectReader? reader = ObjectReader.Create(collection, tableColumnMappings.Keys.ToArray()))
                    {
                        //disable all Indexes except the Primary Key
                        db.Database.ExecuteSqlRaw("ALTER INDEX ALL ON "  + tableName + " DISABLE");
                        if (!string.IsNullOrWhiteSpace(PrimaryKey?.GetName() ?? null)  )
                        {
                            string table = "[" + tableName.Split(".")[0] + "]" + '.' + "[" + tableName.Split(".")[1] + "]";
                            string sql = "ALTER INDEX " + PrimaryKey?.GetName() + " ON " + table + " REBUILD";
                            db.Database.ExecuteSqlRaw(sql);
                        }
                        bulkCopy.DestinationTableName = tableName;
                        connection.Open();
                        try
                        {
                            bulkCopy.WriteToServer(reader);
                        }
                        catch(Exception ex)
                        {
                            ex.ToString();
                        }
                       db.Database.ExecuteSqlRawAsync("ALTER INDEX ALL ON "  + tableName + " REBUILD");
                       connection.Close();
                    }
                }
            }
        }
    }
}