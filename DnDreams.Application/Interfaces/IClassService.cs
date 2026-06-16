using DnDreams.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Interfaces
{
    public interface IClassService
    {
        Task<ClassDefinitionDto> GetRaceByIdAsync(Guid id);
        Task<List<ClassDefinitionDto>> GetAllRacesAsync();
    }
}
