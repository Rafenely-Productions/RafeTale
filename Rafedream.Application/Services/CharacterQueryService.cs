// Rafedream.Application/Services/CharacterQueryService.cs
using Rafedream.Application.Interfaces;
using Rafedream.Domain.Entities;
using Rafedream.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rafedream.Application.Services;

public class CharacterQueryService : ICharacterQueryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CharacterQueryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Character>> GetDashboardCharactersAsync()
    {
        return await _unitOfWork.Characters.GetAllWithDetailsAsync();
    }
}