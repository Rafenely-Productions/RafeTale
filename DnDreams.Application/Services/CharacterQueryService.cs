// DnDreams.Application/Services/CharacterQueryService.cs
using DnDreams.Application.Interfaces;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDreams.Application.Services;

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