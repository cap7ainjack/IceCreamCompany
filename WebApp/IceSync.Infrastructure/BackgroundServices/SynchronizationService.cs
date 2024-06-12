using IceSync.Application.Services.Background;
using IceSync.Application.Services.External;
using IceSync.Domain.Entities;
using IceSync.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IceSync.Infrastructure.BackgroundServices
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IExternalApiService _externalApiService;

        public SynchronizationService(IWorkflowRepository workflowRepository, IExternalApiService externalApiService)
        {
            _workflowRepository = workflowRepository;
            _externalApiService = externalApiService;
        }

        public async Task SynchronizeWorkflows()
        {
            var externalWorkflows = await _externalApiService.GetWorkflowsAsync();
            var internalWorkflows = await _workflowRepository.GetAll().ToListAsync();

            var externalWorkflowDict = externalWorkflows.ToDictionary(e => e.WorkflowId);
            var internalWorkflowDict = internalWorkflows.ToDictionary(i => i.WorkflowId);

            // New
            var newWorkflows = externalWorkflowDict.Values
                .Where(e => !internalWorkflowDict.ContainsKey(e.WorkflowId))
                .Select(e => new Workflow
                {
                    WorkflowId = e.WorkflowId,
                    WorkflowName = e.WorkflowName,
                    IsActive = e.IsActive,
                    MultiExecBehavior = e.MultiExecBehavior
                });

            await _workflowRepository.AddRangeAsync(newWorkflows);

            // Delete
            var deletedWorkflows = internalWorkflows
                .Where(i => !externalWorkflowDict.ContainsKey(i.WorkflowId))
                .ToList();

            _workflowRepository.DeleteRange(deletedWorkflows);

            // Update
            var updatedWorkflows = internalWorkflows
                .Where(i => externalWorkflowDict.ContainsKey(i.WorkflowId))
                .Select(i =>
                {
                    var externalWorkflow = externalWorkflowDict[i.WorkflowId];
                    i.WorkflowName = externalWorkflow.WorkflowName;
                    i.IsActive = externalWorkflow.IsActive;
                    i.MultiExecBehavior = externalWorkflow.MultiExecBehavior;
                    return i;
                })
                .ToList();

            _workflowRepository.UpdateRange(updatedWorkflows);

            await _workflowRepository.SaveAsync();
        }
    }
}
