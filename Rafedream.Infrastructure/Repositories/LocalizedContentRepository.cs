using Rafedream.Domain.Entities;
using Rafedream.Domain.Enums;
using Rafedream.Domain.Interfaces.IRepositories;
using Rafedream.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rafedream.Infrastructure.Repositories
{
    public class LocalizedContentRepository : Repository<LocalizedContent>, ILocalizedContentRepository
    {
        public LocalizedContentRepository(RafedreamDbContext context) : base(context) { }
        public async Task<LocalizedContent?> GetTranslationAsync(Guid entityId, LocProperty property, LocLanguage languageCode)
        {
            return await _context.Set<LocalizedContent>()
                        .FirstOrDefaultAsync(x => x.EntityId == entityId
                                       && x.Property == property
                                       && x.LanguageCode == languageCode);
        }
    }
}
