﻿using JOB_FINDER_API.Models;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Data
{
    public class JobFinderDbContext : DbContext
    {
        public JobFinderDbContext(DbContextOptions<JobFinderDbContext> options)
            : base(options)
        {
        }

        // DbSet cho các bảng
        public DbSet<User> Users { get; set; }
        public DbSet<CandidateProfile> CandidateProfiles { get; set; }
        public DbSet<EmployerProfile> EmployerProfiles { get; set; }
        public DbSet<HRProfile> HRProfiles { get; set; }
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Candidate" },
                new Role { RoleId = 2, RoleName = "Employer" },
                new Role { RoleId = 3, RoleName = "HR" }
            );

            // CandidateProfile
            modelBuilder.Entity<CandidateProfile>()
                .HasKey(cp => cp.UserId);
            modelBuilder.Entity<CandidateProfile>()
                .HasOne(cp => cp.User)
                .WithOne(u => u.CandidateProfile)
                .HasForeignKey<CandidateProfile>(cp => cp.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // EmployerProfile
            modelBuilder.Entity<EmployerProfile>()
                .HasKey(ep => ep.UserId);
            modelBuilder.Entity<EmployerProfile>()
                .HasOne(ep => ep.User)
                .WithOne(u => u.EmployerProfile)
                .HasForeignKey<EmployerProfile>(ep => ep.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // HRProfile
            modelBuilder.Entity<HRProfile>()
                .HasKey(hp => hp.UserId);
            modelBuilder.Entity<HRProfile>()
                .HasOne(hp => hp.User)
                .WithOne(u => u.HRProfile)
                .HasForeignKey<HRProfile>(hp => hp.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<HRProfile>()
                .HasOne(hp => hp.Employer)
                .WithMany(u => u.HRs)
                .HasForeignKey(hp => hp.EmployerId)
                .OnDelete(DeleteBehavior.NoAction);

            // Job - Employer relationship (fix cascade issue)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Employer)
                .WithMany(u => u.PostedJobs)
                .HasForeignKey(j => j.EmployerId)
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

            // Indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<Job>()
                .HasIndex(j => j.CreatedAt);
            modelBuilder.Entity<Message>()
                .HasIndex(m => m.SentAt);
        }
    }
}
