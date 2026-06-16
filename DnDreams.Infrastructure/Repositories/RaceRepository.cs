using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces.IRepositories;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DnDreams.Infrastructure.Repositories;

public class RaceRepositorys : Repository<Race>, IRaceRepository
{
    protected RaceRepositorys(DnDreamsDbContext context) : base(context) { }

}