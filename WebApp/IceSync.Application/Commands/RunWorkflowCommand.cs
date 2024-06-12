using IceSync.Application.Services.External;
using MediatR;

namespace IceSync.Application.Commands
{
    public class RunWorkflowCommand : IRequest<bool>
    {
        public int WorkflowId { get; set; }

        public RunWorkflowCommand(int workflowId)
        {
            WorkflowId = workflowId;
        }
    }

    public class RunWorkflowHandler : IRequestHandler<RunWorkflowCommand, bool>
    {
        private readonly IExternalApiService _externalApiService;

        public RunWorkflowHandler(IExternalApiService externalApiService)
        {
            _externalApiService = externalApiService;
        }

        public async Task<bool> Handle(RunWorkflowCommand request, CancellationToken cancellationToken)
        {
            return await _externalApiService.RunWorkflowAsync(request.WorkflowId);
        }
    }
}