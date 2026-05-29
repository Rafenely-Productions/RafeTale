using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Services.DtosServices
{
    public class LanguageService : IService<LanguageDto, Language>
    {
        public Task<List<LanguageDto>> GetAllAsync(Expression<Func<Language, bool>>? filter, params Expression<Func<Language, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<LanguageDto> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<LanguageDto> GetByIdAsync(Guid id, params Expression<Func<Language, object>>[] includes)
        {
            throw new NotImplementedException();
        }
    }
}
