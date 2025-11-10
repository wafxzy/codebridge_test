using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.Common.Models
{
    public class Dog
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Color { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Tail length must be a positive number")]
        public int TailLength { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Weight must be a positive number")]
        public int Weight { get; set; }
    }
}
