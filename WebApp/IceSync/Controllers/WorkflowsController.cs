using IceSync.Application.Commands;
using IceSync.Application.DTOs;
using IceSync.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IceSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkflowsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkflowDto>>> GetWorkflows()
        {
            var workflows = await _mediator.Send(new GetWorkflowsQuery());
            return Ok(workflows);
        }

        [HttpPost]
        [Route("{workflowId}/run")]
        public async Task<ActionResult> RunWorkflow(int workflowId)
        {
            var result = await _mediator.Send(new RunWorkflowCommand(workflowId));
            if (result)
            {
                return Ok(new { Success = true });
            }

            return BadRequest(new { Success = false });
        }
    }
}
