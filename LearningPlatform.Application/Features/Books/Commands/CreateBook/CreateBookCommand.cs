using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Books.Commands.CreateBook;

public record CreateBookCommand(
    Guid? SubjectId,
    string Title,
    string? Description,
    string FileStorageKey
) : IRequest<Result>;