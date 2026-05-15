using DnDreams.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Interfaces
{
    public interface IFeatureQueryService
    {
        Task<IEnumerable<Feature>> GetDashboardFeaturesAsync();

    }
}
