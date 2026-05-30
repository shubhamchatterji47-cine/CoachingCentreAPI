using CoachingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CoachingAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassSubject> ClassSubjects { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
       public DbSet<AdmissionApplication> AdmissionApplications { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Mark>()
                .HasOne(m => m.EnteredByTeacher)
                .WithMany()
                .HasForeignKey(m => m.EnteredByTeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ClassSubject>()
                .HasOne(cs => cs.Teacher)
                .WithMany(t => t.ClassSubjects)
                .HasForeignKey(cs => cs.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed admin user
            var adminHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName = "System Administrator",
                Email = "admin@coaching.com",
                PasswordHash = adminHash,
                Role = "Admin",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Academic", Description = "School and college subject coaching", Icon = "📚", IsActive = true },
                new Category { Id = 2, Name = "Competitive Exams", Description = "JEE, NEET, UPSC preparation", Icon = "🏆", IsActive = true },
                new Category { Id = 3, Name = "Skills & Vocational", Description = "Professional skills and vocational training", Icon = "🛠️", IsActive = true },
                new Category { Id = 4, Name = "Language", Description = "English, Hindi and other languages", Icon = "🌐", IsActive = true }
            );
        }

        // FIX: Convert all DateTime to UTC before saving — required by Npgsql/PostgreSQL
        public override int SaveChanges()
        {
            ConvertDateTimesToUtc();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConvertDateTimesToUtc();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ConvertDateTimesToUtc()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                foreach (var prop in entry.Properties)
                {
                    if (prop.CurrentValue is DateTime dt && dt.Kind == DateTimeKind.Unspecified)
                        prop.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);

                    if (prop.CurrentValue is DateTime dtLocal && dtLocal.Kind == DateTimeKind.Local)
                        prop.CurrentValue = dtLocal.ToUniversalTime();
                }
            }
        }
    }
}