using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Note.Command.DeleteNote
{
    public record DeleteNoteCommand(Guid Id) : IRequest<Result>;

}
