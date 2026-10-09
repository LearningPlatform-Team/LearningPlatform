
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Attendance;
using LearningPlatform.Domain.Entities.Audit;
using LearningPlatform.Domain.Entities.Content;
using LearningPlatform.Domain.Entities.Engagement;
using LearningPlatform.Domain.Entities.Grading;
using LearningPlatform.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Infrastructure.Data
{
    public class AppDbContext : DbContext , IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // Identity
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        // Academic structure & people
        public DbSet<AcademicTerm> AcademicTerms => Set<AcademicTerm>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<SubjectOffering> SubjectOfferings => Set<SubjectOffering>();
        public DbSet<OfferingTa> OfferingTas => Set<OfferingTa>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<DoctorProfile> DoctorProfiles => Set<DoctorProfile>();
        public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
        public DbSet<TaProfile> TaProfiles => Set<TaProfile>();

        // Attendance, grades & audit
        public DbSet<ClassSession> ClassSessions => Set<ClassSession>();
        public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
        public DbSet<Assessment> Assessments => Set<Assessment>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        // Learning content & personal space
        public DbSet<Lecture> Lectures => Set<Lecture>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<PlaybackLog> PlaybackLogs => Set<PlaybackLog>();
        public DbSet<MyListItem> MyListItems => Set<MyListItem>();
        public DbSet<Note> Notes => Set<Note>();

        // Meetings, events, notices & notifications
        public DbSet<Meeting> Meetings => Set<Meeting>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<EventReminder> EventReminders => Set<EventReminder>();
        public DbSet<Notice> Notices => Set<Notice>();
        public DbSet<Notification> Notifications => Set<Notification>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure your entity mappings here
            modelBuilder.ApplyConfigurationsFromAssembly(
                        typeof(AppDbContext).Assembly);
        }
    }
}
