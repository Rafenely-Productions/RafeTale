using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DnDreams.Infrastructure.Repositories;


public class FeatureRepository : IFeatureRepository
{
    private readonly DnDreamsDbContext _context;
    public FeatureRepository(DnDreamsDbContext context) => _context = context;

    public async Task<IEnumerable<Feature>> GetAllAsync()
    {
        return await _context.Features.ToListAsync();
    }
}