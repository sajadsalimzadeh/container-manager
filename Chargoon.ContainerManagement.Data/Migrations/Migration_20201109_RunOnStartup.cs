using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Chargoon.ContainerManagement.Data.Migrations
{
    public class Migration_20201109_RunOnStartup : IMigration
    {
        public void Up(SqlConnection conn)
        {
            conn.Execute($@"ALTER TABLE [TemplateCommand] ADD [RunOnStartup] BIT NOT NULL DEFAULT 0;");
        }

        public void Down(SqlConnection conn)
        {

        }
    }
}
