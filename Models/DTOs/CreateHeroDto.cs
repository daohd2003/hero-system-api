using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class CreateHeroDto
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        [MaxLength(50, ErrorMessage = "Tên quá dài, tối đa 50 ký tự")]
        public string Name { get; set; }

        [Required]
        public string SuperPower { get; set; }

        [Range(0, 100, ErrorMessage = "Level phải từ 1 đến 100")]
        public int Level { get; set; }
    }

    public class HeroDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Rank { get; set; } // Ví dụ: Level > 50 là Rank S
        public string FactionName { get; set; }
    }
}
