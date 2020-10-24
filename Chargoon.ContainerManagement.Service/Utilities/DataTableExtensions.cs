using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Utilities
{
    public static class DataTableExtensions
    {
        public static List<Dictionary<string, object>> ToDictionary(this DataTable dt)
        {
            var result = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Dictionary<string, object>();
                foreach (DataColumn column in dt.Columns)
                {
                    item[column.ColumnName] = row[column.ColumnName];
                }
                result.Add(item);
            }
            return result;
        }
    }
}
