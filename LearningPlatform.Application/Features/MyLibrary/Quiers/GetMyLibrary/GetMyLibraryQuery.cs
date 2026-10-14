using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Application.Features.MyLibrary.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.MyLibrary.Quiers.GetMyLibrary
{
    public record GetMyLibraryQuery(Guid? SubjectId = null, string? Search = null, int PageNumber = 1, int PageSize = 20)
           : IRequest<ResultT<PaginatedList<BookResponse>>>;
}
