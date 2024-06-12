
using IceSync.Domain.Entities;
using System.Linq;

namespace IceSync.Domain.Repositories
{
    public interface IWorkflowRepository
    {
        IQueryable<Workflow> GetAll();
        Task<Workflow> AddAsync(Workflow workflow);
        void Update(Workflow workflow);
        void Delete(Workflow workflow);
        Task SaveAsync(CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<Workflow> workflows);
        void UpdateRange(IEnumerable<Workflow> workflows); 
        void DeleteRange(IEnumerable<Workflow> workflows);
    }
}
