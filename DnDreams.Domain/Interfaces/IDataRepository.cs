using DnDreams.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Domain.Interfaces
{
    public interface IDataRepository
    {
        Task BulkInsertCharactersAsync(IEnumerable<Character> characters);
        Task BulkInsertDataAsync(List<Race> races, List<ClassDefinition> classes, List<Character> characters, List<ClassLevelProgression> progressions);
        Task<IEnumerable<Character>> GetAllCharactersAsync();
        Task<IEnumerable<Feature>> GetAllFeaturesAsync();

    }
}
