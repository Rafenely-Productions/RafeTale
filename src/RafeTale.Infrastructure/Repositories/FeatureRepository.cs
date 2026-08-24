using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces.IRepositories;
using RafeTale.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace RafeTale.Infrastructure.Repositories;


public class FeatureRepository(RafeTaleDbContext context) : Repository<Feature>(context), IFeatureRepository
{
}