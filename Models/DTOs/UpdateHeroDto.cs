using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class UpdateHeroDto
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        [MaxLength(50, ErrorMessage = "Tên quá dài")]
        public string Name { get; set; }

        [Required]
        public string SuperPower { get; set; }

        [Range(0, 100, ErrorMessage = "Level phải từ 0 đến 100")]
        public int Level { get; set; }

        public Guid? FactionId { get; set; }
    }
}
