using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Chargoon.ContainerManagement.Data.Migrations
{
    public class Migration_20201109_Description : IMigration
    {
        public void Up(SqlConnection conn)
        {
            conn.Execute($@"ALTER TABLE [Template] ADD [Description] VARCHAR(512) NULL;");
        }

        public void Down(SqlConnection conn)
        {

        }
    }
}
