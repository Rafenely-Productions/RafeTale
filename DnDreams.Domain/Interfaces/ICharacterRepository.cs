using DnDreams.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces
{
    public interface ICharacterRepository
    {
        Task BulkInsertCharactersAsync(IEnumerable<Character> characters);
        // Aquí podrías agregar: Task<Character> GetByIdAsync(int id);
    }
}
