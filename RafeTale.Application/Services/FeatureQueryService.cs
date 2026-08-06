// RafeTale.Application/Services/FeatureQueryService.cs
using RafeTale.Application.Interfaces;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RafeTale.Application.Services;

public class FeatureQueryService(IUnitOfWork unitOfWork) : IFeatureQueryService
{
    public async Task<IEnumerable<Feature?>> GetDashboardFeaturesAsync()
    {
        return await unitOfWork.Features.GetAllAsync();
    }
}