using Rafedream.Domain.Entities;
using Rafedream.Domain.Interfaces.IRepositories;
using Rafedream.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Rafedream.Infrastructure.Repositories;


public class FeatureRepository : Repository<Feature>, IFeatureRepository
{
    public FeatureRepository(RafedreamDbContext context) : base(context) { }
}