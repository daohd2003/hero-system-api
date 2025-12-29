using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class FactionDtos
    {
        public class CreateFactionDto
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class FactionDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int MemberCount { get; set; } // Số lượng thành viên (Tính toán)
        }
    }
}
