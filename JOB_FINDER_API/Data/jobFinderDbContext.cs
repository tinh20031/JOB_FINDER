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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Candidate" },
                new Role { RoleId = 2, RoleName = "Company" },
                new Role { RoleId = 3, RoleName = "Admin" }
            );



            modelBuilder.Entity<Industry>().HasData(
         new Industry { IndustryId = 3, IndustryName = "Software Development", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
         new Industry { IndustryId = 4, IndustryName = "Cybersecurity", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
         new Industry { IndustryId = 5, IndustryName = "Data Science", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
         new Industry { IndustryId = 6, IndustryName = "Cloud Computing", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
         new Industry { IndustryId = 7, IndustryName = "UI/UX Design", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
         new Industry { IndustryId = 8, IndustryName = "Artificial Intelligence", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
         new Industry { IndustryId = 9, IndustryName = "DevOps", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
     );


            modelBuilder.Entity<JobType>().HasData(
                new JobType { JobTypeId = 1, JobTypeName = "Full-time", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new JobType { JobTypeId = 2, JobTypeName = "Part-time", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new JobType { JobTypeId = 3, JobTypeName = "Remote", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<Level>().HasData(
                new Level { LevelId = 1, LevelName = "Intern", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Level { LevelId = 2, LevelName = "Junior", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Level { LevelId = 3, LevelName = "Senior", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<ExperienceLevel>().HasData(
                new ExperienceLevel { ExperienceLevelid = 1, name = "Fresher" },
                new ExperienceLevel { ExperienceLevelid = 2, name = "Junior" },
                new ExperienceLevel { ExperienceLevelid = 3, name = "Middle" },
                new ExperienceLevel { ExperienceLevelid = 4, name = "Senior" }

            );


        
            modelBuilder.Entity<CompanyProfile>()
                .HasKey(cp => cp.UserId);

            modelBuilder.Entity<CompanyProfile>()
                .HasOne(cp => cp.User)
                .WithOne(u => u.CompanyProfile)
                .HasForeignKey<CompanyProfile>(cp => cp.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Job>()
                .HasOne(j => j.Company)
                .WithMany(u => u.PostedJobs)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);

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

            modelBuilder.Entity<UserFavoriteJob>()
                .HasKey(ufj => new { ufj.UserId, ufj.JobId });

            modelBuilder.Entity<UserFavoriteJob>()
                .HasOne(ufj => ufj.User)
                .WithMany(u => u.FavoriteJobs)
                .HasForeignKey(ufj => ufj.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserFavoriteJob>()
                .HasOne(ufj => ufj.Job)
                .WithMany(j => j.FavoritedByUsers)
                .HasForeignKey(ufj => ufj.JobId)
                .OnDelete(DeleteBehavior.NoAction);

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

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Job>()
                .HasIndex(j => j.CreatedAt);

            modelBuilder.Entity<Message>()
                .HasIndex(m => m.SentAt);

            modelBuilder.Entity<Job>()
                .HasOne(j => j.ExperienceLevel)
                .WithMany(el => el.Jobs)
                .HasForeignKey(j => j.ExperienceLevelId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserFavoriteCompany>()
         .HasKey(ufc => new { ufc.UserId, ufc.CompanyId }); 

            modelBuilder.Entity<UserFavoriteCompany>()
                .HasOne(ufc => ufc.User)
                .WithMany(u => u.FavoriteCompanies) 
                .HasForeignKey(ufc => ufc.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<UserFavoriteCompany>()
                .HasOne(ufc => ufc.Company)
                .WithMany()
                .HasForeignKey(ufc => ufc.CompanyId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();


            modelBuilder.Entity<CandidateProfile>()
                .HasOne(cp => cp.User)
                .WithOne(u => u.CandidateProfile)
                .HasForeignKey<CandidateProfile>(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Education>()
                .HasOne(e => e.CandidateProfile)
                .WithMany(cp => cp.Educations)
                .HasForeignKey(e => e.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AboutMe>()
                .HasOne(a => a.CandidateProfile)
                .WithMany(cp => cp.AboutMes)
                .HasForeignKey(a => a.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkExperience>()
                .HasOne(w => w.CandidateProfile)
                .WithMany(cp => cp.WorkExperiences)
                .HasForeignKey(w => w.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HighlightProject>()
                .HasOne(h => h.CandidateProfile)
                .WithMany(cp => cp.HighlightProjects)
                .HasForeignKey(h => h.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.CandidateProfile)
                .WithMany(cp => cp.Certificates)
                .HasForeignKey(c => c.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Award>()
                .HasOne(a => a.CandidateProfile)
                .WithMany(cp => cp.Awards)
                .HasForeignKey(a => a.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ForeignLanguage>()
                .HasOne(f => f.CandidateProfile)
                .WithMany(cp => cp.ForeginLanguages)
                .HasForeignKey(f => f.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Skill>()
                .HasOne(s => s.CandidateProfile)
                .WithMany(cp => cp.Skills)
                .HasForeignKey(s => s.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Embedding>().HasKey(e => e.Id);

            modelBuilder.Entity<Embedding>()
                .Property(e => e.Vector)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<float[]>(v, new JsonSerializerOptions()) ?? new float[0]
                );

        }
    }
}