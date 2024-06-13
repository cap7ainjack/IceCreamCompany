using IceSync.Application.DTOs;
using IceSync.Application.Models.ExternalApi;
using IceSync.Application.Services.External;
using MediatR;

namespace IceSync.Application.Queries
{
    public class GetWorkflowsQuery : IRequest<IEnumerable<WorkflowDto>>
    {

    }

    public class GetWorkflowsHandler : IRequestHandler<GetWorkflowsQuery, IEnumerable<WorkflowDto>>
    {
        private readonly IExternalApiService _externalApiService;

        public GetWorkflowsHandler(IExternalApiService externalApiService)
        {
            _externalApiService = externalApiService;
        }

        public async Task<IEnumerable<WorkflowDto>> Handle(GetWorkflowsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<ExternalApiWorkflow> workflows = await _externalApiService.GetWorkflowsAsync();
            return workflows.Select(w => new WorkflowDto
            {
                WorkflowId = w.WorkflowId,
                WorkflowName = w.WorkflowName,
                IsActive = w.IsActive,
                MultiExecBehavior = w.MultiExecBehavior
            });
        }
    }
}
