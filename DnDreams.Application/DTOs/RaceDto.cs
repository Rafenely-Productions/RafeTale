using DnDreams.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.DTOs
{
    public class RaceDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = "";
        public string Resistances { get; init; } = "";
        public Race Race { get; init; } = null!;
    }
}
