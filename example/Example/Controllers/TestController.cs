using Example.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Example.Controllers;
[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{
    private readonly IMediator _mediator;

    public TestController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> TestCmdErrorValidation(CancellationToken cancellationToken)
    {
        var commandRs = await _mediator.Send(new TestCommandRq(), cancellationToken);

        return Ok(commandRs);
    }

    [HttpGet]
    public async Task<IActionResult> TestCmdCache(CancellationToken cancellationToken)
    {
        var commandRs = await _mediator.Send(new TestCommandRq { Amount = 10000 }, cancellationToken);

        return Ok(commandRs);
    }
}
