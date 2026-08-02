using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Domain.Interfaces
{
    public interface IEntity
    {
        Guid Id { get; set; }
        //string TechnicalName { get; set; }
    }
}
