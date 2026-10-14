using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Books.Commands.UpdateBook;

public record UpdateBookCommand(
    Guid Id,
    Guid? SubjectId,
    string Title,
    string? Description,
    string FileStorageKey
) : IRequest<Result>;