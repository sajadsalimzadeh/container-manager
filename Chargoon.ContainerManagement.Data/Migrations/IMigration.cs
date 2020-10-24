using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Chargoon.ContainerManagement.Data.Migrations
{
    public interface IMigration
    {
        void Up(SqlConnection conn);
        void Down(SqlConnection conn);
    }
}
