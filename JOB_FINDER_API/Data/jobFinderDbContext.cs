using JOB_FINDER_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace JOB_FINDER_API.Data
{
    public class JobFinderDbContext : DbContext
    {
        public JobFinderDbContext(DbContextOptions<JobFinderDbContext> options)
            : base(options)
        {
        }


        public DbSet<User> Users { get; set; }
        public DbSet<CandidateProfile> CandidateProfiles { get; set; }
        public DbSet<CompanyProfile> CompanyProfile { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<JobType> JobTypes { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<UserFavoriteJob> UserFavoriteJobs { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<Embedding> Embeddings { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ExperienceLevel> ExperienceLevel { get; set; }
        public DbSet<CandidateToCompanyRequest> CandidateToCompanyRequests { get; set; }
        public DbSet<UserFavoriteCompany> UserFavoriteCompanies { get; set; }
        public DbSet<WorkExperience> WorkExperiences { get; set; }
        public DbSet<HighlightProject> HighlightProjects { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<ForeignLanguage> ForeignLanguages { get; set; }
        public DbSet<AboutMe> AboutMes { get; set; }
        public DbSet<JobView> JobViews { get; set; }
        public DbSet<TryMatchRecord> TryMatchRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        private static DateTime GetVietnamTime()
        {
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Candidate" },
                new Role { RoleId = 2, RoleName = "Company" },
                new Role { RoleId = 3, RoleName = "Admin" }
            );

            // Seed industries
            modelBuilder.Entity<Industry>().HasData(
                new Industry { IndustryId = 3, IndustryName = "Software Development", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() },
                new Industry { IndustryId = 4, IndustryName = "Cybersecurity", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() },
                new Industry { IndustryId = 5, IndustryName = "Data Science", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() },
                new Industry { IndustryId = 6, IndustryName = "Cloud Computing", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() },
                new Industry { IndustryId = 7, IndustryName = "UI/UX Design", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() },
                new Industry { IndustryId = 8, IndustryName = "Artificial Intelligence", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() },
                new Industry { IndustryId = 9, IndustryName = "DevOps", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() }
            );


            modelBuilder.Entity<JobType>().HasData(
                new JobType { JobTypeId = 1, JobTypeName = "Full-time", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() },
                new JobType { JobTypeId = 2, JobTypeName = "Part-time", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() },
                new JobType { JobTypeId = 3, JobTypeName = "Remote", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() }
            );

  
            modelBuilder.Entity<Level>().HasData(
                new Level { LevelId = 1, LevelName = "Intern", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() },
                new Level { LevelId = 2, LevelName = "Junior", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() },
                new Level { LevelId = 3, LevelName = "Senior", CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() }
            );


            modelBuilder.Entity<ExperienceLevel>().HasData(
                new ExperienceLevel { ExperienceLevelid = 1, name = "Fresher" },
                new ExperienceLevel { ExperienceLevelid = 2, name = "Junior" },
                new ExperienceLevel { ExperienceLevelid = 3, name = "Middle" },
                new ExperienceLevel { ExperienceLevelid = 4, name = "Senior" }
            );

            modelBuilder.Entity<CompanyProfile>()
                .HasKey(cp => cp.CompanyProfileId);

            modelBuilder.Entity<CompanyProfile>()
                .HasOne(cp => cp.User)
                .WithOne(u => u.CompanyProfile)
                .HasForeignKey<CompanyProfile>(cp => cp.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CompanyProfile>()
                .HasOne(cp => cp.Industry)
                .WithMany()
                .HasForeignKey(cp => cp.IndustryId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Job relationships
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Company)
                .WithMany(u => u.PostedJobs)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure JobSkill relationships
            modelBuilder.Entity<JobSkill>()
                .HasKey(js => new { js.JobId, js.SkillId });

            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.Job)
                .WithMany(j => j.JobSkills)
                .HasForeignKey(js => js.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.Skill)
                .WithMany(s => s.JobSkills)
                .HasForeignKey(js => js.SkillId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure UserFavoriteJob relationships
            modelBuilder.Entity<UserFavoriteJob>()
       .HasKey(ufj => new { ufj.UserId, ufj.JobId });

            modelBuilder.Entity<UserFavoriteJob>()
                .HasOne(ufj => ufj.User)
                .WithMany(u => u.FavoriteJobs)
                .HasForeignKey(ufj => ufj.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavoriteJob>()
                .HasOne(ufj => ufj.Job)
                .WithMany() // 
                .HasForeignKey(ufj => ufj.JobId)
                .OnDelete(DeleteBehavior.Cascade);
            // Configure Message relationships
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Application relationships
            modelBuilder.Entity<Application>()
                .HasOne(a => a.User)
                .WithMany(u => u.Applications)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.CV)
                .WithMany(cv => cv.Applications)
                .HasForeignKey(a => a.CvId)
                .HasConstraintName("FK_Applications_CVs_UniqueCvId")
                .OnDelete(DeleteBehavior.NoAction);

            // Configure unique index for User email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure index for Job CreatedAt
            modelBuilder.Entity<Job>()
                .HasIndex(j => j.CreatedAt);

            // Configure index for Message SentAt
            modelBuilder.Entity<Message>()
                .HasIndex(m => m.SentAt);

            // Configure Job to ExperienceLevel relationship
            modelBuilder.Entity<Job>()
                .HasOne(j => j.ExperienceLevel)
                .WithMany(el => el.Jobs)
                .HasForeignKey(j => j.ExperienceLevelId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure UserFavoriteCompany relationships
            modelBuilder.Entity<UserFavoriteCompany>()
       .HasKey(ufc => new { ufc.UserId, ufc.CompanyProfileId });

            modelBuilder.Entity<UserFavoriteCompany>()
                .HasOne(ufc => ufc.User)
                .WithMany(u => u.FavoriteCompanies)
                .HasForeignKey(ufc => ufc.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserFavoriteCompany>()
                .HasOne(ufc => ufc.CompanyProfile)
                .WithMany(cp => cp.UserFavoriteCompanies)
                .HasForeignKey(ufc => ufc.CompanyProfileId)
                .OnDelete(DeleteBehavior.NoAction);
            // Configure CandidateProfile relationships
            modelBuilder.Entity<CandidateProfile>()
                .HasOne(cp => cp.User)
                .WithOne(u => u.CandidateProfile)
                .HasForeignKey<CandidateProfile>(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Education relationships
            modelBuilder.Entity<Education>()
                .HasOne(e => e.CandidateProfile)
                .WithMany(cp => cp.Educations)
                .HasForeignKey(e => e.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure AboutMe relationships
            modelBuilder.Entity<AboutMe>()
                .HasOne(a => a.CandidateProfile)
                .WithMany(cp => cp.AboutMes)
                .HasForeignKey(a => a.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure WorkExperience relationships
            modelBuilder.Entity<WorkExperience>()
                .HasOne(w => w.CandidateProfile)
                .WithMany(cp => cp.WorkExperiences)
                .HasForeignKey(w => w.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure HighlightProject relationships
            modelBuilder.Entity<HighlightProject>()
                .HasOne(h => h.CandidateProfile)
                .WithMany(cp => cp.HighlightProjects)
                .HasForeignKey(h => h.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Certificate relationships
            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.CandidateProfile)
                .WithMany(cp => cp.Certificates)
                .HasForeignKey(c => c.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Award relationships
            modelBuilder.Entity<Award>()
                .HasOne(a => a.CandidateProfile)
                .WithMany(cp => cp.Awards)
                .HasForeignKey(a => a.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ForeignLanguage relationships
            modelBuilder.Entity<ForeignLanguage>()
                .HasOne(f => f.CandidateProfile)
                .WithMany(cp => cp.ForeginLanguages)
                .HasForeignKey(f => f.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Skill relationships
            modelBuilder.Entity<Skill>()
                .HasOne(s => s.CandidateProfile)
                .WithMany(cp => cp.Skills)
                .HasForeignKey(s => s.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Embedding
            modelBuilder.Entity<Embedding>().HasKey(e => e.Id);

            modelBuilder.Entity<Embedding>()
                .Property(e => e.Vector)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<float[]>(v, new JsonSerializerOptions()) ?? new float[0]
                );

            // Configure CandidateToCompanyRequest
            modelBuilder.Entity<User>()
                .HasOne(u => u.CandidateToCompanyRequest)
                .WithOne(r => r.User)
                .HasForeignKey<CandidateToCompanyRequest>(r => r.UserId);

            // Configure TryMatchRecord
            modelBuilder.Entity<TryMatchRecord>().ToTable("TryMatchRecords");
            modelBuilder.Entity<TryMatchRecord>().HasKey(t => t.TryMatchId);

            modelBuilder.Entity<TryMatchRecord>()
                .HasOne(t => t.User)
                .WithMany(u => u.TryMatchRecords)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TryMatchRecord>()
                .HasOne(t => t.Job)
                .WithMany(j => j.TryMatchRecords)
                .HasForeignKey(t => t.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TryMatchRecord>()
                .HasOne(t => t.CV)
                .WithMany(cv => cv.TryMatchRecords)
                .HasForeignKey(t => t.CvId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Notification
            modelBuilder.Entity<Notification>()
                .HasKey(n => n.NotificationId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
