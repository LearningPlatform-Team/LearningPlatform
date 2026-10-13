using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using LearningPlatform.Application.Features.Note.Queries;
using LearningPlatform.Application.Common.Models.Pagination;
namespace LearningPlatform.Application.Features.Note.Queries.GetAllNotesQuery
{
    public record GetAllNotesQuery(string? Search = null, int PageNumber = 1, int PageSize = 20)
        : IRequest<ResultT<PaginatedList<NoteResponse>>>;

}
