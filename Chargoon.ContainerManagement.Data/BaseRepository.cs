using Dapper.FastCrud;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chargoon.ContainerManagement.Data
{
    public abstract class BaseRepository
    {
        protected readonly SqlConnection conn;
        protected readonly IConfiguration configuration;

        public BaseRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            conn = new SqlConnection(configuration.GetConnectionString("Default"));
        }
    }
}
