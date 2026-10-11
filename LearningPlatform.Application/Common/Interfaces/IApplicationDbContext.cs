using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Attendance;
using LearningPlatform.Domain.Entities.Audit;
using LearningPlatform.Domain.Entities.Content;
using LearningPlatform.Domain.Entities.Engagement;
using LearningPlatform.Domain.Entities.Grading;
using LearningPlatform.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        // Identity - Users, Roles, and UserRoles are available through IdentityDbContext
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<Otp> Otps { get; }

        // Academic structure & people
        DbSet<AcademicTerm> AcademicTerms { get; }
        DbSet<Subject> Subjects { get; }
        DbSet<SubjectOffering> SubjectOfferings { get; }
        DbSet<OfferingTa> OfferingTas { get; }
        DbSet<Enrollment> Enrollments { get; }
        DbSet<DoctorProfile> DoctorProfiles { get; }
        DbSet<StudentProfile> StudentProfiles { get; }
        DbSet<TaProfile> TaProfiles { get; }

        // Attendance, grades & audit
        DbSet<ClassSession> ClassSessions { get; }
        DbSet<AttendanceRecord> AttendanceRecords { get; }
        DbSet<Assessment> Assessments { get; }
        DbSet<Grade> Grades { get; }
        DbSet<AuditLog> AuditLogs { get; }

        // Learning content & personal space
        DbSet<Lecture> Lectures { get; }
        DbSet<Book> Books { get; }
        DbSet<PlaybackLog> PlaybackLogs { get; }
        DbSet<MyListItem> MyListItems { get; }
        DbSet<Note> Notes { get; }

        // Meetings, events, notices & notifications
        DbSet<Meeting> Meetings { get; }
        DbSet<Event> Events { get; }
        DbSet<EventReminder> EventReminders { get; }
        DbSet<Notice> Notices { get; }
        DbSet<Notification> Notifications { get; }

        // Core EF Core Operations
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
