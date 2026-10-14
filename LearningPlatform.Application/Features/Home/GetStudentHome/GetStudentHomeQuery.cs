using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Home.GetStudentHome
{
    public sealed record GetStudentHomeQuery(
        Guid StudentId,
        int ItemsCount = 5
        ) : IRequest<ResultT<StudentHomeDto>>;
}
