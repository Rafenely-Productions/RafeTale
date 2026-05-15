using DnDreams.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Application.Interfaces;

public interface ICharacterQueryService
{
    Task<IEnumerable<Character>> GetDashboardCharactersAsync();
}