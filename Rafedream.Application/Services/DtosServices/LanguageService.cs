using Rafedream.Application.DTOs;
using Rafedream.Domain.Helpers;
using Rafedream.Application.Interfaces.DtosInterfaces;
using Rafedream.Domain.Entities;
using Rafedream.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Application.Services.DtosServices
{
    public class LanguageService : IService<LanguageDto, Language>
    {
        public Task<LanguageDto> ArmDto(Language entity)
        {
            throw new NotImplementedException();
        }

        public LanguageDto ArmDto(Language entity, Dictionary<LocProperty, Dictionary<Guid, string>>? localizedWords = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<LanguageDto>> GetAllAsync(Expression<Func<Language, bool>>? filter, params Expression<Func<IQueryable<Language>, IQueryable<Language>>>?[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<List<LanguageDto>> GetAllAsync(Expression<Func<Language, bool>>? filter, Action<IncludeAggregator<Language>>? includes = null)
        {
            throw new NotImplementedException();
        }

        public Task<LanguageDto> GetByIdAsync(Guid id, params Expression<Func<Language, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<LanguageDto> GetByIdAsync(Guid id, Action<IncludeAggregator<Language>>? includes = null)
        {
            throw new NotImplementedException();
        }
    }
}
