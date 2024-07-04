namespace TaskManagement.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MediatR;
    using TaskManagement.Application.Commands;
    using TaskManagement.Application.Queries;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var result = await _mediator.Send(new GetTaskByIdQuery { Id = id });
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int? priority = null, [FromQuery] int? status = null)
        {
            var result = await _mediator.Send(new GetTasksQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Priority = priority,
                Status = status
            });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTaskById), new { id = result }, command);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskCommand command)
        {
            if (id != command.TaskId) return BadRequest();
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id, DeleteTaskCommand command)
        {
            if (id != command.TaskId) return BadRequest();
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
