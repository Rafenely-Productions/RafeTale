using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces.IRepositories;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Infrastructure.Repositories
{
    public class LocalizedContentRepository : Repository<LocalizedContent>, ILocalizedContentRepository
    {
        public LocalizedContentRepository(DnDreamsDbContext context) : base(context) { }
        public async Task<LocalizedContent?> GetTranslationAsync(Guid entityId, string property, string languageCode)
        {
            return await _context.Set<LocalizedContent>()
                        .FirstOrDefaultAsync(x => x.EntityId == entityId
                                       && x.Property.ToLower() == property.ToLower()
                                       && x.LanguageCode.ToLower() == languageCode.ToLower());
        }
    }
}
