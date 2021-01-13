using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Chargoon.ContainerManagement.Data.Migrations
{
    public class Migration_20201228_Host : IMigration
    {
        public void Up(SqlConnection conn)
        {
            conn.Execute($@"ALTER TABLE [User] ADD [Host] NVARCHAR(50) NULL;");
        }

        public void Down(SqlConnection conn)
        {

        }
    }
}
