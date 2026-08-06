using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace RafeTale.Infrastructure.Repositories;


public class FeatureRepository : Repository<Feature>, IFeatureRepository
{
    public FeatureRepository(RafeTaleDbContext context) : base(context) { }
}