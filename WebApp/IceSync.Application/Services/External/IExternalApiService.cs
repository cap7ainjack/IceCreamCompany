using IceSync.Application.Models;
using System;

namespace IceSync.Application.Services.External
{
    public interface IExternalApiService
    {
        Task<IEnumerable<ExternalApiWorkflow>> GetWorkflowsAsync();
        Task<bool> RunWorkflowAsync(int workflowId);
        Task<string> GetTokenAsync();
    }
}
