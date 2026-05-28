using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Interfaces
{
    public interface ILocalizationService
    {
        Task<string> GetStringAsync(Guid entityId, string property);
        Task<Dictionary<Guid, string>> GetAllAsync(string entityType, string propertyType = "Name");
    }
}
