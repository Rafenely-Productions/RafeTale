// Rafedream.Application/Services/FeatureQueryService.cs
using Rafedream.Application.Interfaces;
using Rafedream.Domain.Entities;
using Rafedream.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rafedream.Application.Services;

public class FeatureQueryService(IUnitOfWork unitOfWork) : IFeatureQueryService
{
    public async Task<IEnumerable<Feature?>> GetDashboardFeaturesAsync()
    {
        return await unitOfWork.Features.GetAllAsync();
    }
}