using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Services.DtosServices
{
    public class FeatureService : IService<FeatureDto, Feature>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILocalizationService _loc;

        public FeatureService(IUnitOfWork uow, ILocalizationService loc)
        {
            _uow = uow;
            _loc = loc;
        }

        public Task<FeatureDto> ArmDto(Feature entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<FeatureDto>> GetAllAsync(Expression<Func<Feature, bool>>? filter, params Expression<Func<Feature, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<FeatureDto> GetByIdAsync(Guid id, params Expression<Func<Feature, object>>[] includes)
        {
            throw new NotImplementedException();
        }
    }
}
