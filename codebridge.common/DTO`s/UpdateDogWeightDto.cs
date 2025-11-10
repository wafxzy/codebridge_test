using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.Common.DTO_s
{
    public class UpdateDogWeightDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Weight must be a positive number")]
        public int Weight { get; set; }
    }
}
