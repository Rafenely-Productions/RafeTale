// RafeTale.Application/Services/CharacterQueryService.cs
using RafeTale.Application.Interfaces;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RafeTale.Application.Services;

public class CharacterQueryService(IUnitOfWork unitOfWork) : ICharacterQueryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<Character>> GetDashboardCharactersAsync()
    {
        return await _unitOfWork.Characters.GetAllWithDetailsAsync();
    }
}