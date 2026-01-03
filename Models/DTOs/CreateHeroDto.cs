using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "Password không được để trống")]
        public string Password { get; set; }
    }

    public class HeroDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Rank { get; set; }
        public string FactionName { get; set; }
    }
}
