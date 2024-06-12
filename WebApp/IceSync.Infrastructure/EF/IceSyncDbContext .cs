using IceSync.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IceSync.Infrastructure.EF
{
    public class IceSyncDbContext : DbContext
    {
        public IceSyncDbContext(DbContextOptions options)
        : base(options)
        {
        }
        public DbSet<Workflow> Workflows { get; set; }
    }
}
