using Rafedream.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rafedream.Application.Interfaces;

public interface ICharacterQueryService
{
    Task<IEnumerable<Character>> GetDashboardCharactersAsync();
}