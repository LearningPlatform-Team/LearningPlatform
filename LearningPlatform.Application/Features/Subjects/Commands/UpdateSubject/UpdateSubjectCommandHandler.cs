using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Subjects.Commands.UpdateSubject;

public class UpdateSubjectCommandHandler
    : IRequestHandler<UpdateSubjectCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateSubjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        UpdateSubjectCommand request,
        CancellationToken cancellationToken)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(
                s => s.Id == request.Id,
                cancellationToken);

        if (subject is null)
        {
            return Result.Failure(
                "Subject was not found.",
                ErrorType.NotFound);
        }

        var duplicateExists = await _context.Subjects
            .AnyAsync(
                s => s.Id != request.Id &&
                     (s.Code == request.Code ||
                      s.Name == request.Name),
                cancellationToken);

        if (duplicateExists)
        {
            return Result.Failure(
                "Another subject with the same code or name already exists.",
                ErrorType.Conflict);
        }

        subject.Code = request.Code;
        subject.Name = request.Name;
        subject.Description = request.Description;
        subject.YearLevel = request.YearLevel;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
