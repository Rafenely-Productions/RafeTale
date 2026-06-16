using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Entities
{
    public class Background : IEntity
    {
        public Guid Id { get; set; }
        public string TechnicalName { get; set; } = string.Empty;
        public ICollection<ASI> ASIs { get; set; } = [];
        public ICollection<SkillType> SkillProficiencies { get; set; } = [];

        public Feat Feat { get; set; } = null!;
        public Guid FeatId { get; set; }
    }
}
