using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.Common.Helpers
{
    public class DogQueryParams
    {
        public string? Attribute { get; set; }
        public string? Order { get; set; } = "asc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
