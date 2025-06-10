using JOB_FINDER_API.Models;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<Message> Messages { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ExperienceLevel> ExperienceLevel { get; set; }
        public DbSet<CandidateSkill> CandidateSkill { get; set; }
        public DbSet<CandidateToCompanyRequest> CandidateToCompanyRequests { get; set; }
        public DbSet<UserFavoriteCompany> UserFavoriteCompanies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Candidate" },
                new Role { RoleId = 2, RoleName = "Company" },
                new Role { RoleId = 3, RoleName = "Admin" }
            );



            modelBuilder.Entity<Experience>().HasData(
new Experience { Id = 1, ExperienceName = "Less than 1 year", UserId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
new Experience { Id = 2, ExperienceName = "1-3 years", UserId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
new Experience { Id = 3, ExperienceName = "More than 3 years", UserId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
);
            modelBuilder.Entity<Industry>().HasData(
    new Industry { IndustryId = 1, IndustryName = "Information Technology", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
    new Industry { IndustryId = 2, IndustryName = "Finance", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
);
            modelBuilder.Entity<JobType>().HasData(
    new JobType { Id = 1, JobTypeName = "Full-time", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
    new JobType { Id = 2, JobTypeName = "Part-time", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
    new JobType { Id = 3, JobTypeName = "Remote", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
);
            modelBuilder.Entity<Level>().HasData(
    new Level { Id = 1, LevelName = "Intern", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
    new Level { Id = 2, LevelName = "Junior", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
    new Level { Id = 3, LevelName = "Senior", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
);
            modelBuilder.Entity<Skill>().HasData(
    new Skill { SkillId = 1, SkillName = "C#", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
    new Skill { SkillId = 2, SkillName = "JavaScript", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
    new Skill { SkillId = 3, SkillName = "SQL", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
);

            modelBuilder.Entity<ExperienceLevel>().HasData(
                new ExperienceLevel { id = 1, name = "Fresher" },
                new ExperienceLevel { id = 2, name = "Junior" },
                new ExperienceLevel { id = 3, name = "Middle" },
                new ExperienceLevel { id = 4, name = "Senior" }
            );

            modelBuilder.Entity<Job>().HasData(
                new Job
                {
                    JobId = 1,
                    Title = "Junior Software Engineer",
                    Description = "Develop and maintain web applications using C# and JavaScript.",
                    CompanyId = 2, 
                    Salary = 50000,
                    IndustryId = 1, 
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    LevelId = 2,
                    JobTypeId = 1, 
                    ExperienceLevelId = 2, 
                    TimeStart = DateTime.UtcNow,
                    TimeEnd = DateTime.UtcNow.AddDays(30),
                    Status = Job.JobStatus.active,
                    ProvinceName = "Ho Chi Minh City",
                    AddressDetail = "123 Tech Street, District 1",
                    DeactivatedByAdmin = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Job
                {
                    JobId = 2,
                    Title = "Senior Data Analyst",
                    Description = "Analyze financial data and generate reports.",
                    CompanyId = 2,
                    Salary = 80000,
                    IndustryId = 2, 
                    ExpiryDate = DateTime.UtcNow.AddDays(45),
                    LevelId = 3,
                    JobTypeId = 3, 
                    ExperienceLevelId = 4, 
                    TimeStart = DateTime.UtcNow,
                    TimeEnd = DateTime.UtcNow.AddDays(45),
                    Status = Job.JobStatus.pending,
                    ProvinceName = "Hanoi",
                    AddressDetail = "456 Finance Avenue, Ba Dinh",
                    DeactivatedByAdmin = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );


            modelBuilder.Entity<User>().HasData(
    new User
    {
        Id = 1,
        FullName = "tinh",
        Email = "tinhadmin@gmail.com",
        Phone = "0123456789",
        Password = "123",
        IsActive = true,
        RoleId = 1, 
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    },
    new User
    {
        Id = 2,
        FullName = "Tech Corp",
        Email = "contact@techcorp.com",
        Phone = "0987654321",
        Password = "123",
        IsActive = true,
        RoleId = 2, 
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    },
    new User
    {
        Id = 3,
        FullName = "Admin User",
        Email = "admin@jobfinder.com",
        Phone = "0912345678",
        Password = "123",
        IsActive = true,
        RoleId = 3, 
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    }

);
            modelBuilder.Entity<CandidateProfile>().HasData(
        new CandidateProfile
        {
            UserId = 1, 
            Gender = "Male",
            Dob = new DateTime(1995, 5, 15),
            JobTitle = "Software Developer",
            Description = "A passionate developer with expertise in C# and JavaScript.",
            Address = "123 Main Street",
            Province = "Ho Chi Minh",
            City = "Ho Chi Minh City",
            Language = "English, Vietnamese"
          
        }
    );

            modelBuilder.Entity<CompanyProfile>().HasData(
                new CompanyProfile
                {
                    UserId = 2, 
                    CompanyName = "Tech Corp",
                    CompanyProfileDescription = "A leading tech company specializing in software solutions.",
                    Location = "123 Tech Street, District 1, Ho Chi Minh City",
                    UrlCompanyLogo = "https://res.cloudinary.com/dzf0ccons/image/upload/v1748267504/image_user/bfltymsad63wfkyp3bvl.jpg",
                    ImageLogoLgr = "https://res.cloudinary.com/dzf0ccons/image/upload/v1748267504/image_user/bfltymsad63wfkyp3bvl.jpg", // Thêm trường ImageLogoLgr
                    TeamSize = "50-100 employees",
                    IsVerified = true,
                    Website = "https://techcorp.com",
                    Contact = "contact@techcorp.com",
                    IndustryId = 1,
                    IsActive = true
                }
            );

         
            modelBuilder.Entity<CandidateProfile>()
                .HasKey(cp => cp.UserId);
            modelBuilder.Entity<CandidateProfile>()
                .HasOne(cp => cp.User)
                .WithOne(u => u.CandidateProfile)
                .HasForeignKey<CandidateProfile>(cp => cp.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<CandidateProfile>()
                .HasMany(cp => cp.CandidateSkills)
                .WithOne()
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            
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

            // JobSkill
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

            // UserFavoriteJob
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

            // Message
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
            modelBuilder.Entity<Message>()
                .HasOne(m => m.RelatedJob)
                .WithMany()
                .HasForeignKey(m => m.RelatedJobId)
                .OnDelete(DeleteBehavior.SetNull);

            // CandidateSkill - User
            modelBuilder.Entity<CandidateSkill>()
                .HasOne(cs => cs.User)
                .WithMany() 
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // CandidateSkill - Skill
            modelBuilder.Entity<CandidateSkill>()
                .HasOne(cs => cs.Skill)
                .WithMany(s => s.CandidateSkills) 
                .HasForeignKey(cs => cs.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            // CandidateProfile - CandidateSkill
            modelBuilder.Entity<CandidateProfile>()
                .HasMany(cp => cp.CandidateSkills)
                .WithOne()
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Application
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
        }
    }
}