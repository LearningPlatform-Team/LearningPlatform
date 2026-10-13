using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.MyGrades.Queries.GetMyGrades
{
    public record GetMyGradesQuery(Guid? TermId = null) : IRequest<ResultT<List<MyGradeResponse>>>;

}
