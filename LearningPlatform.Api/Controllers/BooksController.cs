using LearningPlatform.Api.Controllers;
using LearningPlatform.Application.Features.Books.Commands.CreateBook;
using LearningPlatform.Application.Features.Books.Commands.DeleteBook;
using LearningPlatform.Application.Features.Books.Commands.UpdateBook;
using LearningPlatform.Application.Features.Books.Queries.GetBookById;
using LearningPlatform.Application.Features.Books.Queries.GetBooks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

/// <summary>
/// Digital Textbooks &amp; Library | الكتب والمراجع الرقمية
/// </summary>
/// <remarks>
/// Management of PDF textbooks, book covers, author info, and course reading materials.
/// إدارة الكتب الإلكترونية، المراجع، وروابط التحميل والقراءة.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class BooksController(IMediator mediator) : AppControllerBase
{
    /// <summary>
    /// Get book by ID | جلب تفاصيل كتاب بالمعرف
    /// </summary>
    /// <param name="id">Book GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns book details and PDF file URL.</response>
    /// <response code="404">Book not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetBookByIdQuery(id),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Create new book entry | إضافة كتاب جديد
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="command">Book details (Title, Author, SubjectId, FileUrl, CoverImageUrl, Description).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Book created successfully.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update existing book | تعديل بيانات كتاب
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="id">Book GUID.</param>
    /// <param name="command">Updated book details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Book updated successfully.</response>
    /// <response code="400">Route ID does not match command ID.</response>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateBookCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID and request ID do not match.");

        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete book | حذف كتاب
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="id">Book GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Book deleted successfully.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new DeleteBookCommand(id),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Query all books | استعلام وتصفح جميع الكتب
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="query">Search and pagination options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns list of books.</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetBooksQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}