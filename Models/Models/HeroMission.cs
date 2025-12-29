using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class HeroMission
    {
        public Guid HeroId { get; set; }
        public Hero Hero { get; set; }

        public Guid MissionId { get; set; }
        public Mission Mission { get; set; }

        public DateTime CompletedDate { get; set; }
        public string ResultRank { get; set; }
    }
}
