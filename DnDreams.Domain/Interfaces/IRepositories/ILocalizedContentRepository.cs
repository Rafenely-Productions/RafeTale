using DnDreams.Domain.Entities;
using DnDreams.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces.IRepositories
{
    public interface ILocalizedContentRepository : IRepository<LocalizedContent>
    {
        Task<LocalizedContent?> GetTranslationAsync(Guid entityID, LocProperty property, LocLanguage _currentCulture);
    }
}
