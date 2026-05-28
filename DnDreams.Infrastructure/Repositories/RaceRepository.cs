using DnDreams.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DnDreams.Infrastructure.Persistence;
using DocumentFormat.OpenXml.Wordprocessing;
using DnDreams.Domain.DTOs;
using System.Linq.Expressions;
using DnDreams.Domain.Interfaces.IRepositories;

namespace DnDreams.Infrastructure.Repositories;

public class RaceRepository : Repository<Race>, IRaceRepository
{
    public RaceRepository(DnDreamsDbContext context) : base(context) { }

    public async Task<Race?> GetByNameAsync(string name)
    {
        return await _context.Races.FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<Race?> GetByIdAsync(Guid id)
    {
        return await _context.Races.Include(r => r.Traits).FirstOrDefaultAsync(x => x.Id == id);
    }
}