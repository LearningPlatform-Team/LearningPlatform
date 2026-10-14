using LearningPlatform.Application.Features.Books.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Books.Queries.GetBookById;

public record GetBookByIdQuery(Guid Id)
    : IRequest<ResultT<BookResponse>>;