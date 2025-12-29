using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Configurations
{
    public class HeroMissionConfiguration : IEntityTypeConfiguration<HeroMission>
    {
        public void Configure(EntityTypeBuilder<HeroMission> builder)
        {
            builder.ToTable(nameof(HeroMission));

            builder.HasKey(hm => new { hm.HeroId, hm.MissionId });

            builder.HasOne(hm => hm.Hero)
                .WithMany(h => h.HeroMissions)
                .HasForeignKey(hm => hm.HeroId);

            builder.HasOne(hm => hm.Mission)
                .WithMany(m => m.HeroMissions)
                .HasForeignKey(hm => hm.MissionId);
        }
    }
}
