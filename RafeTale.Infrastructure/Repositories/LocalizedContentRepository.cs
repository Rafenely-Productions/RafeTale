using RafeTale.Domain.Entities;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RafeTale.Infrastructure.Repositories
{
    public class LocalizedContentRepository : Repository<LocalizedContent>, ILocalizedContentRepository
    {
        public LocalizedContentRepository(RafeTaleDbContext context) : base(context) { }
        public async Task<LocalizedContent?> GetTranslationAsync(Guid entityId, LocProperty property, LocLanguage languageCode)
        {
            return await _context.Set<LocalizedContent>()
                        .FirstOrDefaultAsync(x => x.EntityId == entityId
                                       && x.Property == property
                                       && x.LanguageCode == languageCode);
        }
    }
}
