using RafeTale.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Domain.Entities
{
    public class SubclassLevelProgression : IEntity
    {
        public Guid Id { get; set; }
        public int Level { get; set; }
        public List<Feature> Features { get; set; } = []; // Lo que ganas en este nivel
        public Subclass? Subclass { get; set; }
        public Guid SubclassId { get; set; }

    }
}
