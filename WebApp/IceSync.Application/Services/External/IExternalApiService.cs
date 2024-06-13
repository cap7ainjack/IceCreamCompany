using IceSync.Application.Models.ExternalApi;
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
