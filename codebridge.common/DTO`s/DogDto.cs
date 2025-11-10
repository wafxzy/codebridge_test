using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.Common.DTO_s
{
    public class DogDto
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int TailLength { get; set; }
        public int Weight { get; set; }
    }
}
