using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using DnDreams.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreamsInfrastructure.Persistance
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly DnDreamsDbContext _context;

        public CharacterRepository(DnDreamsDbContext context)
        {
            _context = context;
        }

        public async Task BulkInsertCharactersAsync(IEnumerable<Character> characters)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Characters.AddRangeAsync(characters);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}