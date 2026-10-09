
using LearningPlatform.Domain.Entities.Academic;
using System.Diagnostics;

namespace LearningPlatform.Domain.Entities.Grading
{
    public class Assessment
    {
        public Guid Id { get; set; }
        public Guid SubjectOfferingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal MaxScore { get; set; }
        public DateTime? ExamDate { get; set; }

        public SubjectOffering SubjectOffering { get; set; } = null!;
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

    }
}
