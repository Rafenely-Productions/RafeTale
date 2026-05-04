using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Models
{
    public class ExcelImportResult<T>
    {
        public string GameSystem { get; set; } = "DnD_5E";
        public string Version { get; set; } = "1.0";
        public List<T> Data { get; set; } = new();
    }
}
