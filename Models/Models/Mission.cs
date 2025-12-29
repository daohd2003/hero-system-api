using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Mission
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int DifficultyLevel { get; set; }

        public ICollection<HeroMission> HeroMissions { get; set; }
    }
}
