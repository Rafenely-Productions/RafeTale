using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces.IRepositories;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DnDreams.Infrastructure.Repositories;


public class FeatureRepository : Repository<Feature>, IFeatureRepository
{
    public FeatureRepository(DnDreamsDbContext context) : base(context) { }
}