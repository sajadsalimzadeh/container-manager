using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Utilities
{
    public static class ConsoleParser
    {
        public static DataTable ParseTableStdOut(string input)
        {
            var lines = input.Split('\n');
            var dt = new DataTable();
            if(lines.Length > 0)
            {
                var header_line = lines[0];
                var headers = header_line.Split("   ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
                var headerIndices = headers.Select(x => header_line.IndexOf(x)).ToList();
                foreach (var item in headers)
                {
                    dt.Columns.Add(item);
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    for (int j = 0; j < headerIndices.Count; j++)
                    {
                        var items = new string[headerIndices.Count];
                        var headerIndex = headerIndices[j];
                        if (j + 1 < headerIndices.Count)
                        {
                            items[j] = line.Substring(headerIndex, headerIndices[j + 1] - headerIndex).Trim();
                        }
                        else
                        {
                            items[j] = line.Substring(headerIndex).Trim();
                        }
                        dt.Rows.Add(items);
                    }
                }
            }
            return dt;
        }
    }
}
