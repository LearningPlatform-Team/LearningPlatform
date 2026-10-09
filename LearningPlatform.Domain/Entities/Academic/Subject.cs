using LearningPlatform.Domain.Entities.Content;
using LearningPlatform.Domain.Enums;
namespace LearningPlatform.Domain.Entities.Academic
{
    public class Subject
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public YearLevel YearLevel { get; set; }

        public ICollection<SubjectOffering> Offerings { get; set; } = new List<SubjectOffering>();
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
