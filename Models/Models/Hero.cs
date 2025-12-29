using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Hero
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SuperPower { get; set; }
        public int Level { get; set; }

        public Guid? FactionId { get; set; }
        public Faction? Faction { get; set; }

        public ICollection<HeroMission> HeroMissions { get; set; }
    }
}
