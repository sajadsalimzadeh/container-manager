using Dapper;
using Dapper.FastCrud;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Data.Migrations
{
    public class Migrator : BaseRepository
    {
        private List<IMigration> migrations = new List<IMigration>()
        {
            new Migration_20201014_Initial(),
            new Migration_20201106_Command(),
            new Migration_20201109_Description(),
        };

        public Migrator(IConfiguration configuration) : base(configuration)
        {
        }

        private bool IsUpdateHistoryExists(string id)
        {
            return conn.ExecuteScalar("SELECT 1 FROM __UpdateHistory WHERE Id = @Id", new { Id = id }) != null;
        }
        private bool AddUpdateHistory(string id)
        {
            return conn.ExecuteScalar("INSERT INTO __UpdateHistory VALUES (@Id)", new { Id = id }) != null;
        }
        private bool DeleteUpdateHistory(string id)
        {
            return conn.ExecuteScalar("DELETE FROM __UpdateHistory WHERE Id = @Id", new { Id = id }) != null;
        }

        private bool IsTableExists(string name, string schema = "dbo")
        {
            return conn.ExecuteScalar("(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @Schema AND  TABLE_NAME = @Name)",
                new
                {
                    Schema = schema,
                    Name = name
                }) != null;
        }

        private void CreateUpdateHistoryTable()
        {
            conn.Execute($@"CREATE TABLE __UpdateHistory (
                Id VARCHAR(64) NOT NULL UNIQUE
            )");
        }

        public void Up()
        {
            if (!IsTableExists("__UpdateHistory")) CreateUpdateHistoryTable();

            foreach (var migration in migrations.OrderBy(x => x.GetType().Name))
            {
                var id = migration.GetType().Name;
                if (!IsUpdateHistoryExists(id))
                {
                    migration.Up(conn);
                    AddUpdateHistory(id);
                }
            }
        }

        public void Down()
        {
            foreach (var migration in migrations.OrderByDescending(x => x.GetType().Name))
            {
                var id = migration.GetType().Name;
                if (IsUpdateHistoryExists(id))
                {
                    migration.Down(conn);
                    DeleteUpdateHistory(id);
                }
            }
        }
    }
}
