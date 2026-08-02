using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Application.Models
{
    public class ImportSummary
    {
        public int Count { get; set; }
        public string Version { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
    }
}
