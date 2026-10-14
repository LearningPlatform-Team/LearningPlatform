using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Books.Commands.DeleteBook;

public record DeleteBookCommand(Guid Id) : IRequest<Result>;