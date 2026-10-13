using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Mylist.Command.CreateList
{
    public record AddToMyListCommand(Guid LectureId) : IRequest<Result>;
}
