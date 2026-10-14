using LearningPlatform.Api.Controllers;
using LearningPlatform.Application.Features.Books.Commands.CreateBook;
using LearningPlatform.Application.Features.Books.Commands.DeleteBook;
using LearningPlatform.Application.Features.Books.Commands.UpdateBook;
using LearningPlatform.Application.Features.Books.Queries.GetBookById;
using LearningPlatform.Application.Features.Books.Queries.GetBooks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[Route("api/[controller]")]
public class BooksController(IMediator mediator) : AppControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetBookByIdQuery(id),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            command,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateBookCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route ID and request ID do not match.");
        }

        var result = await mediator.Send(
            command,
            cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new DeleteBookCommand(id),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetBooksQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}