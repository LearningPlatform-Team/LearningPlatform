
namespace LearningPlatform.Domain.Entities.Academic
{
    public class AcademicTerm
    {
        public Guid Id { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int TermNumber { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsCurrent { get; set; }

        public ICollection<SubjectOffering> Offerings { get; set; } = new List<SubjectOffering>();

    }
}
