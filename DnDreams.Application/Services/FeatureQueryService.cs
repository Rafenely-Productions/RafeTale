// DnDreams.Application/Services/FeatureQueryService.cs
using DnDreams.Application.Interfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Application.Services;

public class FeatureQueryService : IFeatureQueryService
{
    private readonly IUnitOfWork _unitOfWork;

    public FeatureQueryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Feature>> GetDashboardFeaturesAsync()
    {
        return await _unitOfWork.Features.GetAllAsync();
    }
}