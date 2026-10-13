using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Features.MyLibrary.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.MyLibrary.Quiers.GetBookById
{
    public record GetBookByIdQuery(Guid Id) : IRequest<ResultT<BookResponse>>;
}
