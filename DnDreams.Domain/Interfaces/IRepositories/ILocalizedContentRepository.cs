using DnDreams.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces.IRepositories
{
    public interface ILocalizedContentRepository : IRepository<LocalizedContent>
    {
        Task<LocalizedContent?> GetTranslationAsync(Guid entityID, string property, string _currentCulture);
    }
}
