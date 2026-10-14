using LearningPlatform.Application.Common.Models;
using LearningPlatform.Application.Features.Books.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Books.Queries.GetBooks;

public record GetBooksQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? SubjectId = null,
    string? Search = null)
    : IRequest<ResultT<PagedResult<BookResponse>>>;