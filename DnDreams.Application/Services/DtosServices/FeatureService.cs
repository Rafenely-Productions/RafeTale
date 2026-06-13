using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using DnDreams.Domain.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
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

        public async Task<FeatureDto> ArmDto(Feature entity)
        {
            return new FeatureDto
            {
                Id = entity.Id,
                Name = await _loc.GetStringAsync(entity.Id, LocProperty.Name),
                Description = await _loc.GetStringAsync(entity.Id, LocProperty.Description),
            };
        }

        public FeatureDto ArmDto(Feature entity, Dictionary<LocProperty, Dictionary<Guid, string>> localizedWords)
        {
            var Name = localizedWords?.TryGetValue(LocProperty.Name, out var nameL) == true && nameL?.TryGetValue(entity.Id, out var entityName) == true ? entityName : "Uknown entity...";
            var Description = localizedWords?.TryGetValue(LocProperty.Description, out var descriptionL) == true && descriptionL?.TryGetValue(entity.Id, out var description) == true ? description : "Uknown entity...";
            var Material = localizedWords?.TryGetValue(LocProperty.MaterialComponentDescription, out var mateL) == true && mateL?.TryGetValue(entity.Id, out var matel) == true ? matel : "Uknown entity...";

            return new FeatureDto
            {
                Id = entity.Id,
                Name = Name,
                Description = Description,
            };
        }

        public async Task<List<FeatureDto>> GetAllAsync(Expression<Func<Feature, bool>>? filter, params Expression<Func<Feature, object>>[] includes)
        {
            var features = await _uow.Features.GetAllAsync(filter, includes);
            var descriptions = await _loc.GetAllAsync(LocEntity.Feature, [LocProperty.Name, LocProperty.Description, LocProperty.MaterialComponentDescription]);

            var featureDtos = new List<FeatureDto>(features.Count());
            foreach (var feature in features)
            {
                featureDtos.Add(ArmDto(feature!, descriptions));
            }
            return featureDtos;
        }

        public async Task<FeatureDto> GetByIdAsync(Guid id, params Expression<Func<Feature, object>>[] includes)
        {
            var feature = await _uow.Features.GetByIdAsync(id);
            if (feature == null)
                return null!;

            return await ArmDto(feature);
        }
    }
}
