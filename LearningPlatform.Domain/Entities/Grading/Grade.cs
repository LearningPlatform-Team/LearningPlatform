
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Identity;

namespace LearningPlatform.Domain.Entities.Grading
{
    public class Grade
    {
        public Guid Id { get; set; }
        public int AssessmentId { get; set; }
        public int EnrollmentId { get; set; }
        public decimal Score { get; set; }
        public Guid GradedByUserId { get; set; }
        public DateTime GradedAt { get; set; }

        public Assessment Assessment { get; set; } = null!;
        public Enrollment Enrollment { get; set; } = null!;
        public User GradedByUser { get; set; } = null!;
    }
}
