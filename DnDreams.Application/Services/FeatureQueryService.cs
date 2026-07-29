// DnDreams.Application/Services/FeatureQueryService.cs
using DnDreams.Application.Interfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Application.Services;

public class FeatureQueryService(IUnitOfWork unitOfWork) : IFeatureQueryService
{
    public async Task<IEnumerable<Feature?>> GetDashboardFeaturesAsync()
    {
        return await unitOfWork.Features.GetAllAsync();
    }
}