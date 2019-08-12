using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PipelineSpace.Infra.Data.SqlServer.Extensions
{
    public static class MigrationBuilderExtensions
    {
        public static string GetSqlFromFile(this MigrationBuilder builder, string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            return File.ReadAllText($"Migrations/Scripts/{filePath}");
        }

        public static void SqlFile(this MigrationBuilder builder, string filePath)
        {
            string[] sql = builder.GetSqlFromFile(filePath).Split(new[] { "\r\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in sql)
            {
                builder.Sql(item);
            }
        }
    }
}
