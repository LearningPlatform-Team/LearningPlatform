using LearningPlatform.Application.Features.Books.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Subjects.Queries.GetSubjectBooks;

public record GetSubjectBooksQuery(Guid SubjectId)
    : IRequest<ResultT<List<BookResponse>>>;