using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Domain.Entities
{
    public class SchoolOfMagic : IEntity
    {
        public Guid Id { get; set; }
        public SchoolOfMagicEnum TechnicalName { get; set; }
    }
}
