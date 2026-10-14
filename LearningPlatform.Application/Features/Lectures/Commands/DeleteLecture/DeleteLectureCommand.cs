using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Lectures.Commands.DeleteLecture;

public sealed record DeleteLectureCommand(Guid Id) : IRequest<Result>;
