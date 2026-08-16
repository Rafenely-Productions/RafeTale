using RafeTale.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RafeTale.Application.Interfaces;

public interface ICharacterQueryService
{
    Task<IEnumerable<Character>> GetDashboardCharactersAsync();
}