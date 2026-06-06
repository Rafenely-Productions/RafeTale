using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.DTOs
{
    public class BackgroundDto : IDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ToolProficiency { get; set; } = string.Empty;
        public IEnumerable<ASI> ASIs { get; set; } = [];
        public IEnumerable<SkillType> SkillTypes { get; set; } = [];

        public Feat Feat { get; set; } = null!;
        
    }
}
