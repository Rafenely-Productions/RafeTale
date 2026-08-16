using RafeTale.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Domain.Entities
{
    public class XpRules : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Level { get; set; } = 0;
        public int RequiredXp { get; set; } = 0;
        public int Bonus { get; set; } = 0;
    }
}