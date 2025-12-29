using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class MissionDtos
    {
        public class CreateMissionDto
        {
            [Required]
            public string Title { get; set; }
            public int DifficultyLevel { get; set; }
        }

        public class MissionDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int DifficultyLevel { get; set; }
        }

        public class AssignMissionDto
        {
            public Guid HeroId { get; set; }
            public Guid MissionId { get; set; }
            public string ResultRank { get; set; } // Ví dụ: "S", "A", "B"
        }
    }
}
