using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.Common.DTO_s
{
    public class CreateDogDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Color is required")]
        [StringLength(100, ErrorMessage = "Color cannot exceed 100 characters")]
        public string Color { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Tail length must be a positive number")]
        public int TailLength { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Weight must be a positive number")]
        public int Weight { get; set; }
    }
}
