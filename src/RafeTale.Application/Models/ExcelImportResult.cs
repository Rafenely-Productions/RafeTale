using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Application.Models
{
    public class ExcelImportResult<T>
    {
        public string GameSystem { get; set; } = "5";
        public string Version { get; set; } = "1.0";
        public List<T> Data { get; set; } = [];
    }
}
