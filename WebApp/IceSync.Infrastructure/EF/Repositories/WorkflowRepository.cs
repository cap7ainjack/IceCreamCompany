using IceSync.Domain.Entities;
using IceSync.Domain.Repositories;

namespace IceSync.Infrastructure.EF.Repositories
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly IceSyncDbContext _context;

        public WorkflowRepository(IceSyncDbContext context)
        {
            _context = context;
        }

        public IQueryable<Workflow> GetAll()
        {
            return this._context.Set<Workflow>();
        }
        public async Task<Workflow> AddAsync(Workflow entity)
        {
            await _context.Set<Workflow>().AddAsync(entity);

            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<Workflow> workflows) 
        {
            await _context.Workflows.AddRangeAsync(workflows);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        public void Update(Workflow workflow)
        {
            _context.Workflows.Update(workflow);
        }

        public void UpdateRange(IEnumerable<Workflow> workflows)
        {
            _context.Workflows.UpdateRange(workflows);
        }

        public void Delete(Workflow workflow)
        {
            this._context.Workflows.Remove(workflow);
        }
        public void DeleteRange(IEnumerable<Workflow> workflows) 
        {
            _context.Workflows.RemoveRange(workflows);
        }
    }
}
