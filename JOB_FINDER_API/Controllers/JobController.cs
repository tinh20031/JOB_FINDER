using JOB_FINDER_API.Data;
using JOB_FINDER_API.Hubs;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Models.filter;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly EmailService _emailService;
        private readonly NotificationService _notificationService;
        private readonly ILogger<JobController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public JobController(
            JobFinderDbContext context,
            EmailService emailService,
            NotificationService notificationService,
            ILogger<JobController> logger,
            IConfiguration configuration,
           IHubContext<NotificationHub> notificationHubContext)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
            _logger = logger;
            _configuration = configuration;
            _notificationHubContext = notificationHubContext;
        }


        private static DateTime GetVietnamTime()
        {
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetJobs(
            [FromQuery] string role = "candidate",
            [FromQuery] int? companyId = null,
            [FromQuery] bool? onlyTrending = null)
        {
            var now = GetVietnamTime();
            var query = _context.Jobs
                .Include(j => j.Industry)
                //.Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Company).ThenInclude(u => u.CompanyProfile)
                .Include(j => j.Level)
                .Include(j => j.JobType)
                .AsQueryable();
            query = query.Where(j => j.Status != Job.JobStatus.draft);
           
            if (onlyTrending.HasValue && onlyTrending.Value)
            {
                query = query.Where(j => j.IsTrending);
            }

            if (role == "candidate")
            {
                query = query.Where(j => j.Status == Job.JobStatus.active
                                        && !j.DeactivatedByAdmin
                                        && j.TimeStart.Date <= now.Date
                                        && j.TimeEnd.Date >= now.Date);
            }
            else if (role == "company" && companyId.HasValue)
            {
                query = query.Where(j => j.CompanyId == companyId);
            }
            else if (role == "admin")
            {
                // Admins see all jobs
            }
            else
            {
                return BadRequest("Invalid role parameter.");
            }

            // If only trending jobs, order by view count
            if (onlyTrending.HasValue && onlyTrending.Value)
            {
                // Join with job views to get the count
                var jobsWithViews = await query
                    .Select(j => new
                    {
                        Job = j,
                        ViewCount = j.JobViews.Count()
                    })
                    .OrderByDescending(x => x.ViewCount)
                    .ToListAsync();

                var result = jobsWithViews.Select(x => new
                {
                    x.Job.JobId,
                    x.Job.Title,
                    x.Job.Description,
                    x.Job.Education,
                    x.Job.YourSkill,
                    x.Job.YourExperience,
                    x.Job.CompanyId,
                    Company = x.Job.Company == null ? null : new
                    {
                        x.Job.Company.UserId,
                        x.Job.Company.FullName,
                        x.Job.Company.Email,
                        CompanyName = x.Job.Company.CompanyProfile?.CompanyName,
                        Location = x.Job.Company.CompanyProfile?.Location,
                        UrlCompanyLogo = x.Job.Company.CompanyProfile?.UrlCompanyLogo
                    },
                    x.Job.IndustryId,
                    Industry = x.Job.Industry == null ? null : new
                    {
                        x.Job.Industry.IndustryId,
                        x.Job.Industry.IndustryName
                    },
                    x.Job.ExpiryDate,
                    x.Job.LevelId,
                    Level = x.Job.Level == null ? null : new
                    {
                        x.Job.Level.LevelId,
                        x.Job.Level.LevelName
                    },
                    x.Job.JobTypeId,
                    JobType = x.Job.JobType == null ? null : new
                    {
                        x.Job.JobType.JobTypeId,
                        x.Job.JobType.JobTypeName
                    },
                    x.Job.Quantity,
                    x.Job.TimeStart,
                    x.Job.TimeEnd,
                    x.Job.Status,
                    x.Job.ProvinceName,
                    x.Job.AddressDetail,
                    x.Job.IsSalaryNegotiable,
                    x.Job.MinSalary,
                    x.Job.MaxSalary,
                    x.Job.CreatedAt,
                    x.Job.UpdatedAt,
                    /*Skills = x.Job.JobSkills.Select(js => new
                    {
                        js.SkillId,
                        js.Skill.SkillName
                    }).ToList(),*/
                    x.Job.DescriptionWeight,
                    x.Job.SkillsWeight,
                    x.Job.ExperienceWeight,
                    x.Job.EducationWeight,
                    IsTrending = x.Job.IsTrending,
                    ViewCount = x.ViewCount,
                    DeactivatedByAdmin = x.Job.DeactivatedByAdmin
                });

                return Ok(result);
            }
            else
            {
                var jobs = await query.ToListAsync();

            var result = jobs.Select(job => new
            {
                job.JobId,
                job.Title,
                job.Description,
                IsTrending = job.IsTrending,
                job.Education,
                job.YourSkill,
                job.YourExperience,
                job.CompanyId,
                DeactivatedByAdmin = job.DeactivatedByAdmin,
                Company = job.Company == null ? null : new
                {
                    job.Company.UserId,
                    job.Company.FullName,
                    job.Company.Email,
                    CompanyName = job.Company.CompanyProfile?.CompanyName,
                    Location = job.Company.CompanyProfile?.Location,
                    UrlCompanyLogo = job.Company.CompanyProfile?.UrlCompanyLogo
                },
                job.IndustryId,
                Industry = job.Industry == null ? null : new
                {
                    job.Industry.IndustryId,
                    job.Industry.IndustryName
                },
                job.ExpiryDate,
                job.LevelId,
                Level = job.Level == null ? null : new
                {
                    job.Level.LevelId,
                    job.Level.LevelName
                },
                job.JobTypeId,
                JobType = job.JobType == null ? null : new
                {
                    job.JobType.JobTypeId,
                    job.JobType.JobTypeName
                },
                job.Quantity, // Thêm trường này
                job.TimeStart,
                job.TimeEnd,
                job.Status,
                job.ProvinceName,
                job.AddressDetail,
                job.IsSalaryNegotiable,
                job.MinSalary,
                job.MaxSalary,
                job.CreatedAt,
                job.UpdatedAt,
                /*Skills = job.JobSkills.Select(js => new
                {
                    js.SkillId,
                    js.Skill.SkillName
                }).ToList(),*/
                job.DescriptionWeight,
                job.SkillsWeight,
                job.ExperienceWeight,
                job.EducationWeight
            });

            return Ok(result);
            }
        }


        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetJob(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Industry)
                //.Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Company).ThenInclude(u => u.CompanyProfile)
                .Include(j => j.Level)
                .Include(j => j.JobType)
                // XÓA: .Include(j => j.ExperienceLevel)
                .FirstOrDefaultAsync(j => j.JobId == id);

            if (job == null)
                return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            var userIdStr = User.Identity?.Name;
            int.TryParse(userIdStr, out var userId);

            if (job.DeactivatedByAdmin && role != "admin" && job.CompanyId != userId)
                return NotFound();

            bool isAnonymous = !User.Identity.IsAuthenticated;
            if (isAnonymous || role == "candidate")
            {
                bool isActive = job.Status == Job.JobStatus.active;
                bool isExpired = job.TimeEnd < GetVietnamTime();

                if (!isActive && !isExpired)
                    return StatusCode(403, "You do not have permission to view this job.");
            }

            return Ok(new
            {
                job.JobId,
                job.Title,
                job.Description,
                job.YourSkill,
                job.YourExperience,
                job.Education,
                job.CompanyId,
                Company = job.Company == null ? null : new
                {
                    job.Company.UserId,
                    job.Company.FullName,
                    job.Company.Email,
                    CompanyName = job.Company.CompanyProfile?.CompanyName,
                    Location = job.Company.CompanyProfile?.Location,
                    UrlCompanyLogo = job.Company.CompanyProfile?.UrlCompanyLogo
                },
                job.IndustryId,
                Industry = job.Industry == null ? null : new
                {
                    job.Industry.IndustryId,
                    job.Industry.IndustryName
                },
                job.ExpiryDate,
                job.LevelId,
                Level = job.Level == null ? null : new
                {
                    job.Level.LevelId,
                    job.Level.LevelName
                },
                job.JobTypeId,
                JobType = job.JobType == null ? null : new
                {
                    job.JobType.JobTypeId,
                    job.JobType.JobTypeName
                },
                job.Quantity, // Thêm trường này
                job.TimeStart,
                job.TimeEnd,
                job.Status,
                job.ProvinceName,
                job.AddressDetail,
                job.IsSalaryNegotiable,
                job.MinSalary,
                job.MaxSalary,
                job.CreatedAt,
                job.UpdatedAt,
               /* Skills = job.JobSkills.Select(js => new
                {
                    js.SkillId,
                    js.Skill.SkillName
                }).ToList(),*/
                job.DescriptionWeight,
                job.SkillsWeight,
                job.ExperienceWeight,
                job.EducationWeight
            });
        }



        private async Task<(bool CanPost, string Message, CompanySubscriptionType Tier)> CheckJobPostingLimit(int companyId)
        {
            // Get active job count for this company
            var activeJobCount = await _context.Jobs
                .CountAsync(j => j.CompanyId == companyId &&
                        (j.Status == Job.JobStatus.active || j.Status == Job.JobStatus.pending));

            // Check if company has an active subscription
            var subscription = await _context.CompanySubscriptions
                .Where(s => s.UserId == companyId && s.IsActive && s.EndDate > DateTime.UtcNow)
                .Include(s => s.SubscriptionType)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

            // If no subscription, use Free tier limits
            if (subscription == null)
            {
                var freeTier = await _context.CompanySubscriptionTypes
                    .FirstOrDefaultAsync(t => t.PackageType == CompanySubscriptionPackageType.Free);

                int limit = freeTier?.JobPostLimit ?? 2; // Default Free tier limit is 2 jobs

                if (activeJobCount >= limit)
                {
                    return (false,
                        $"You've reached the job posting limit for the Free tier ({limit} jobs). " +
                        "Please upgrade your subscription to post more jobs.",
                        freeTier);
                }

                return (true,
                    $"You can post this job. You have posted {activeJobCount} out of {limit} jobs allowed in your Free tier.",
                    freeTier);
            }

            // Company has a subscription
            var tierLimit = subscription.SubscriptionType.JobPostLimit;

            // Premium tier has unlimited job posts
            if (subscription.SubscriptionType.PackageType == CompanySubscriptionPackageType.Premium)
            {
                return (true,
                    "You have a Premium subscription with unlimited job posts.",
                    subscription.SubscriptionType);
            }

            // Check if within limits for Basic tier
            if (activeJobCount >= tierLimit)
            {
                return (false,
                    $"You've reached the job posting limit for your {subscription.SubscriptionType.Name} " +
                    $"tier ({tierLimit} jobs). Please upgrade your subscription to post more jobs.",
                    subscription.SubscriptionType);
            }

            return (true,
                $"You can post this job. You have posted {activeJobCount} out of {tierLimit} jobs " +
                $"allowed in your {subscription.SubscriptionType.Name} tier.",
                subscription.SubscriptionType);
        }


        [HttpPost("create")]
        public async Task<ActionResult<Job>> CreateJob([FromBody] JobCreateRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if company can post more jobs based on their subscription
            var (canPost, message, tier) = await CheckJobPostingLimit(dto.CompanyId);
            if (!canPost)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = message,
                    SubscriptionTier = tier.Name,
                    JobPostLimit = tier.JobPostLimit,
                    UpgradeRequired = true
                });
            }

            // Continue with regular job creation...
            if (dto.Quantity < 1)
                return BadRequest("Quantity must be at least 1.");

            if (float.IsNaN(dto.DescriptionWeight) || float.IsNaN(dto.SkillsWeight) ||
                float.IsNaN(dto.ExperienceWeight) || float.IsNaN(dto.EducationWeight))
                return BadRequest("Weights cannot be NaN.");

            if (!dto.IsSalaryNegotiable && (!dto.MinSalary.HasValue || !dto.MaxSalary.HasValue))
                return BadRequest("Minimum and maximum salary must be provided if salary is not negotiable.");
            if (dto.TimeEnd <= dto.TimeStart)
                return BadRequest("End time must be after start time.");
            if (dto.ExpiryDate <= GetVietnamTime())
                return BadRequest("Expiry date must be in the future.");

            var job = new Job
            {
                Title = dto.Title,
                Description = dto.Description,
                Education = dto.Education,
                YourSkill = dto.YourSkill,
                YourExperience = dto.YourExperience,
                CompanyId = dto.CompanyId,
                IndustryId = dto.IndustryId,
                ExpiryDate = dto.ExpiryDate,
                LevelId = dto.LevelId,
                JobTypeId = dto.JobTypeId,
                Quantity = dto.Quantity,
                TimeStart = dto.TimeStart,
                TimeEnd = dto.TimeEnd,
                ProvinceName = dto.ProvinceName,
                AddressDetail = dto.AddressDetail,
                CreatedAt = GetVietnamTime(),
                UpdatedAt = GetVietnamTime(),
                Status = Job.JobStatus.pending,
                IsSalaryNegotiable = dto.IsSalaryNegotiable,
                MinSalary = dto.IsSalaryNegotiable ? null : dto.MinSalary,
                MaxSalary = dto.IsSalaryNegotiable ? null : dto.MaxSalary,
                DescriptionWeight = dto.DescriptionWeight / 100f,
                SkillsWeight = dto.SkillsWeight / 100f,
                ExperienceWeight = dto.ExperienceWeight / 100f,
                EducationWeight = dto.EducationWeight / 100f
            };
            // Cập nhật RemainingJobPosts trong CompanySubscription
            var subscription = await _context.CompanySubscriptions
                .Where(s => s.UserId == dto.CompanyId && s.IsActive && s.EndDate > DateTime.UtcNow)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (subscription != null)
            {
                // Trừ 1 từ RemainingJobPosts
                subscription.RemainingJobPosts = Math.Max(0, subscription.RemainingJobPosts - 1);
                subscription.UpdatedAt = DateTime.UtcNow;
                _logger.LogInformation($"Decreased remaining job posts for company {dto.CompanyId} to {subscription.RemainingJobPosts}");
            }
            else
            {
                // Với gói Free, vẫn tạo job nhưng không cần giảm số dư
                _logger.LogInformation($"Company {dto.CompanyId} using Free tier or no active subscription. Job created without deducting from subscription.");
            }
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created new job #{job.JobId} with title: {job.Title}");

            /*if (dto.skillInputs != null && dto.skillInputs.Any())
            {
                foreach (var input in dto.skillInputs)
                {
                    int skillId;
                    if (input.SkillId.HasValue)
                    {
                        skillId = input.SkillId.Value;
                    }
                    else if (!string.IsNullOrWhiteSpace(input.SkillName))
                    {
                        var existingSkill = await _context.Skills
                            .FirstOrDefaultAsync(s => s.SkillName.ToLower() == input.SkillName.ToLower());
                        if (existingSkill != null)
                        {
                            skillId = existingSkill.SkillId;
                        }
                        else
                        {
                            var newSkill = new Skill { SkillName = input.SkillName };
                            _context.Skills.Add(newSkill);
                            await _context.SaveChangesAsync();
                            skillId = newSkill.SkillId;
                            _logger.LogInformation($"Created new skill: {input.SkillName} with ID: {skillId}");
                        }
                    }
                    else
                    {
                        continue;
                    }

                    _context.JobSkills.Add(new JobSkill { JobId = job.JobId, SkillId = skillId });
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Added skills to job #{job.JobId}");
            }*/

            return CreatedAtAction(nameof(GetJob), new { id = job.JobId }, new
            {
                Job = job,
                SubscriptionInfo = new
                {
                    SubscriptionTier = tier.Name,
                    JobPostLimit = tier.JobPostLimit,
                    Message = message
                }
            });
        }


        [HttpPost("trending")]
        [Authorize(Roles = "Company")]
        public async Task<ActionResult<Job>> CreateTrendingJob([FromBody] JobCreateRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var companyId))
                return Unauthorized("Invalid user ID");

            // Verify the company ID matches the requesting user
            if (dto.CompanyId != companyId)
                return BadRequest("Company ID mismatch");

            // Check if company has an active subscription
            var subscription = await _context.CompanySubscriptions
                .Where(s => s.UserId == companyId && s.IsActive && s.EndDate > DateTime.UtcNow)
                .Include(s => s.SubscriptionType)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

            // If no subscription or free tier, return error
            if (subscription == null)
            {
                var freeTier = await _context.CompanySubscriptionTypes
                    .FirstOrDefaultAsync(t => t.PackageType == CompanySubscriptionPackageType.Free);

                return BadRequest(new
                {
                    Success = false,
                    Message = "Trending jobs require a Basic or Premium subscription. Free tier doesn't include trending jobs.",
                    SubscriptionTier = "Free",
                    TrendingJobLimit = 0,
                    UpgradeRequired = true
                });
            }

            // Check if subscription allows trending jobs
            if (subscription.SubscriptionType.TrendingJobLimit <= 0)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = $"Your {subscription.SubscriptionType.Name} subscription doesn't include trending job features.",
                    SubscriptionTier = subscription.SubscriptionType.Name,
                    TrendingJobLimit = 0,
                    UpgradeRequired = true
                });
            }

            // Check if company has remaining trending job posts
            if (subscription.RemainingTrendingJobPosts <= 0)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "You have used all your trending job posts. Please purchase more or upgrade your subscription.",
                    SubscriptionTier = subscription.SubscriptionType.Name,
                    RemainingTrendingJobs = 0,
                    UpgradeRequired = true
                });
            }

            // Count current active trending jobs
            var activeTrendingJobs = await _context.Jobs
                .CountAsync(j => j.CompanyId == companyId &&
                              j.IsTrending &&
                              (j.Status == Job.JobStatus.active || j.Status == Job.JobStatus.pending));

            if (activeTrendingJobs >= subscription.SubscriptionType.TrendingJobLimit)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = $"You have reached the maximum limit of {subscription.SubscriptionType.TrendingJobLimit} active trending jobs for your subscription tier.",
                    SubscriptionTier = subscription.SubscriptionType.Name,
                    CurrentTrendingJobs = activeTrendingJobs,
                    MaxTrendingJobs = subscription.SubscriptionType.TrendingJobLimit
                });
            }

            // Basic validation
            if (dto.Quantity < 1)
                return BadRequest("Quantity must be at least 1.");

            if (float.IsNaN(dto.DescriptionWeight) || float.IsNaN(dto.SkillsWeight) ||
                float.IsNaN(dto.ExperienceWeight) || float.IsNaN(dto.EducationWeight))
                return BadRequest("Weights cannot be NaN.");

            if (!dto.IsSalaryNegotiable && (!dto.MinSalary.HasValue || !dto.MaxSalary.HasValue))
                return BadRequest("Minimum and maximum salary must be provided if salary is not negotiable.");

            if (dto.TimeEnd <= dto.TimeStart)
                return BadRequest("End time must be after start time.");

            if (dto.ExpiryDate <= GetVietnamTime())
                return BadRequest("Expiry date must be in the future.");

            // Create the job as trending
            var job = new Job
            {
                Title = dto.Title,
                Description = dto.Description,
                Education = dto.Education,
                YourSkill = dto.YourSkill,
                YourExperience = dto.YourExperience,
                CompanyId = dto.CompanyId,
                IndustryId = dto.IndustryId,
                ExpiryDate = dto.ExpiryDate,
                LevelId = dto.LevelId,
                JobTypeId = dto.JobTypeId,
                Quantity = dto.Quantity,
                TimeStart = dto.TimeStart,
                TimeEnd = dto.TimeEnd,
                ProvinceName = dto.ProvinceName,
                AddressDetail = dto.AddressDetail,
                CreatedAt = GetVietnamTime(),
                UpdatedAt = GetVietnamTime(),
                Status = Job.JobStatus.pending,
                IsSalaryNegotiable = dto.IsSalaryNegotiable,
                MinSalary = dto.IsSalaryNegotiable ? null : dto.MinSalary,
                MaxSalary = dto.IsSalaryNegotiable ? null : dto.MaxSalary,
                DescriptionWeight = dto.DescriptionWeight / 100f,
                SkillsWeight = dto.SkillsWeight / 100f,
                ExperienceWeight = dto.ExperienceWeight / 100f,
                EducationWeight = dto.EducationWeight / 100f,
                IsTrending = true // Mark as trending job
            };

            _context.Jobs.Add(job);

            // Deduct trending job post count
            subscription.RemainingTrendingJobPosts--;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created new trending job #{job.JobId} with title: {job.Title}");

            // Handle skills same as regular job creation
            /*if (dto.skillInputs != null && dto.skillInputs.Any())
            {
                foreach (var input in dto.skillInputs)
                {
                    int skillId;
                    if (input.SkillId.HasValue)
                    {
                        skillId = input.SkillId.Value;
                    }
                    else if (!string.IsNullOrWhiteSpace(input.SkillName))
                    {
                        var existingSkill = await _context.Skills
                            .FirstOrDefaultAsync(s => s.SkillName.ToLower() == input.SkillName.ToLower());
                        if (existingSkill != null)
                        {
                            skillId = existingSkill.SkillId;
                        }
                        else
                        {
                            var newSkill = new Skill { SkillName = input.SkillName };
                            _context.Skills.Add(newSkill);
                            await _context.SaveChangesAsync();
                            skillId = newSkill.SkillId;
                            _logger.LogInformation($"Created new skill: {input.SkillName} with ID: {skillId}");
                        }
                    }
                    else
                    {
                        continue;
                    }

                    _context.JobSkills.Add(new JobSkill { JobId = job.JobId, SkillId = skillId });
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Added skills to trending job #{job.JobId}");
            }*/

            return CreatedAtAction(nameof(GetJob), new { id = job.JobId }, new
            {
                Job = job,
                IsTrending = true,
                SubscriptionInfo = new
                {
                    SubscriptionTier = subscription.SubscriptionType.Name,
                    RemainingTrendingJobs = subscription.RemainingTrendingJobPosts,
                    MaxTrendingJobs = subscription.SubscriptionType.TrendingJobLimit
                }
            });
        }

        /* [HttpGet("trending")]
         public async Task<ActionResult<IEnumerable<object>>> GetTrendingJobs(
     [FromQuery] string role = "candidate",
     [FromQuery] int? companyId = null,
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10)
         {
             var now = GetVietnamTime();
             var query = _context.Jobs
                 .Where(j => j.IsTrending == true) // Filter to only trending jobs
                 .Include(j => j.Industry)
                 .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                 .Include(j => j.Company).ThenInclude(u => u.CompanyProfile)
                 .Include(j => j.Level)
                 .Include(j => j.JobType)
                 .Include(j => j.JobViews) // Include job views for ranking
                 .AsQueryable();

             // Apply the same filtering as GetJobs
             if (role == "candidate")
             {
                 query = query.Where(j => j.Status == Job.JobStatus.active
                                         && !j.DeactivatedByAdmin
                                         && j.TimeStart.Date <= now.Date
                                         && j.TimeEnd.Date >= now.Date);
             }
             else if (role == "company" && companyId.HasValue)
             {
                 query = query.Where(j => j.CompanyId == companyId);
             }
             else if (role == "admin")
             {
                 // Admins see all trending jobs
             }
             else
             {
                 return BadRequest("Invalid role parameter.");
             }

             // Count total trending jobs after filtering
             var totalCount = await query.CountAsync();

             // Calculate the number of pages
             var pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

             // Order trending jobs by view count (descending) and then by creation date
             var orderedJobs = await query
                 .OrderByDescending(j => j.JobViews.Count())
                 .ThenByDescending(j => j.CreatedAt)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

             // For each job, get its view count
             var jobsWithViewCounts = orderedJobs.Select(job => new
             {
                 Job = job,
                 ViewCount = job.JobViews.Count
             }).ToList();

             // Format the response the same way as GetJobs
             var result = jobsWithViewCounts.Select(item => new
             {
                 item.Job.JobId,
                 item.Job.Title,
                 item.Job.Description,
                 item.Job.Education,
                 item.Job.YourSkill,
                 item.Job.YourExperience,
                 item.Job.CompanyId,
                 DeactivatedByAdmin = item.Job.DeactivatedByAdmin,
                 IsTrending = true, // Always true in this endpoint
                 ViewCount = item.ViewCount, // Include the view count
                 Company = item.Job.Company == null ? null : new
                 {
                     item.Job.Company.UserId,
                     item.Job.Company.FullName,
                     item.Job.Company.Email,
                     CompanyName = item.Job.Company.CompanyProfile?.CompanyName,
                     Location = item.Job.Company.CompanyProfile?.Location,
                     UrlCompanyLogo = item.Job.Company.CompanyProfile?.UrlCompanyLogo
                 },
                 item.Job.IndustryId,
                 Industry = item.Job.Industry == null ? null : new
                 {
                     item.Job.Industry.IndustryId,
                     item.Job.Industry.IndustryName
                 },
                 item.Job.ExpiryDate,
                 item.Job.LevelId,
                 Level = item.Job.Level == null ? null : new
                 {
                     item.Job.Level.LevelId,
                     item.Job.Level.LevelName
                 },
                 item.Job.JobTypeId,
                 JobType = item.Job.JobType == null ? null : new
                 {
                     item.Job.JobType.JobTypeId,
                     item.Job.JobType.JobTypeName
                 },
                 item.Job.Quantity,
                 item.Job.TimeStart,
                 item.Job.TimeEnd,
                 item.Job.Status,
                 item.Job.ProvinceName,
                 item.Job.AddressDetail,
                 item.Job.IsSalaryNegotiable,
                 item.Job.MinSalary,
                 item.Job.MaxSalary,
                 item.Job.CreatedAt,
                 item.Job.UpdatedAt,
                 Skills = item.Job.JobSkills.Select(js => new
                 {
                     js.SkillId,
                     js.Skill.SkillName
                 }).ToList(),
                 item.Job.DescriptionWeight,
                 item.Job.SkillsWeight,
                 item.Job.ExperienceWeight,
                 item.Job.EducationWeight
             });

             return Ok(new
             {
                 TotalCount = totalCount,
                 PageCount = pageCount,
                 CurrentPage = page,
                 PageSize = pageSize,
                 Jobs = result
             });
         }*/
        [HttpGet("trending")]
        public async Task<ActionResult<IEnumerable<object>>> GetTrendingJobs(
     [FromQuery] string role = "candidate",
     [FromQuery] int? companyId = null,
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10)
        {
            var now = GetVietnamTime();
            var query = _context.Jobs
                .Where(j => j.IsTrending == true) // Filter to only trending jobs
                .Include(j => j.Industry)
                //.Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Company).ThenInclude(u => u.CompanyProfile)
                .Include(j => j.Level)
                .Include(j => j.JobType)
                .AsQueryable();

            // Apply the same filtering as GetJobs
            if (role == "candidate")
            {
                query = query.Where(j => j.Status == Job.JobStatus.active
                                        && !j.DeactivatedByAdmin
                                        && j.TimeStart.Date <= now.Date
                                        && j.TimeEnd.Date >= now.Date);
            }
            else if (role == "company" && companyId.HasValue)
            {
                query = query.Where(j => j.CompanyId == companyId);
            }
            else if (role == "admin")
            {
                // Admins see all trending jobs
            }
            else
            {
                return BadRequest("Invalid role parameter.");
            }

            // First get all job IDs and their view counts
            var jobViewCounts = await _context.JobViews
                .GroupBy(v => v.JobId)
                .Select(g => new { JobId = g.Key, ViewCount = g.Count() })
                .ToListAsync();

            // Create a dictionary for quick lookup
            var viewCountDict = jobViewCounts.ToDictionary(v => v.JobId, v => v.ViewCount);

            // Get the filtered jobs
            var jobs = await query.ToListAsync();

            // Count total trending jobs after filtering
            var totalCount = jobs.Count;

            // Manually order the jobs by view count and creation date
            var orderedJobs = jobs
                .Select(j => new {
                    Job = j,
                    ViewCount = viewCountDict.ContainsKey(j.JobId) ? viewCountDict[j.JobId] : 0
                })
                .OrderByDescending(x => x.ViewCount)
                .ThenByDescending(x => x.Job.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Format the response the same way as GetJobs
            var result = orderedJobs.Select(item => new
            {
                item.Job.JobId,
                item.Job.Title,
                item.Job.Description,
                item.Job.Education,
                item.Job.YourSkill,
                item.Job.YourExperience,
                item.Job.CompanyId,
                DeactivatedByAdmin = item.Job.DeactivatedByAdmin,
                IsTrending = true,
                ViewCount = item.ViewCount, // Include the view count in the response
                Company = item.Job.Company == null ? null : new
                {
                    item.Job.Company.UserId,
                    item.Job.Company.FullName,
                    item.Job.Company.Email,
                    CompanyName = item.Job.Company.CompanyProfile?.CompanyName,
                    Location = item.Job.Company.CompanyProfile?.Location,
                    UrlCompanyLogo = item.Job.Company.CompanyProfile?.UrlCompanyLogo
                },
                item.Job.IndustryId,
                Industry = item.Job.Industry == null ? null : new
                {
                    item.Job.Industry.IndustryId,
                    item.Job.Industry.IndustryName
                },
                item.Job.ExpiryDate,
                item.Job.LevelId,
                Level = item.Job.Level == null ? null : new
                {
                    item.Job.Level.LevelId,
                    item.Job.Level.LevelName
                },
                item.Job.JobTypeId,
                JobType = item.Job.JobType == null ? null : new
                {
                    item.Job.JobType.JobTypeId,
                    item.Job.JobType.JobTypeName
                },
                item.Job.Quantity,
                item.Job.TimeStart,
                item.Job.TimeEnd,
                item.Job.Status,
                item.Job.ProvinceName,
                item.Job.AddressDetail,
                item.Job.IsSalaryNegotiable,
                item.Job.MinSalary,
                item.Job.MaxSalary,
                item.Job.CreatedAt,
                item.Job.UpdatedAt,
                /*Skills = item.Job.JobSkills.Select(js => new
                {
                    js.SkillId,
                    js.Skill.SkillName
                }).ToList(),*/
                item.Job.DescriptionWeight,
                item.Job.SkillsWeight,
                item.Job.ExperienceWeight,
                item.Job.EducationWeight
            });

            return Ok(new
            {
                TotalCount = totalCount,
                PageCount = (int)Math.Ceiling(totalCount / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                Jobs = result
            });
        }

        [HttpPut("{id}/trending")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> ToggleTrendingStatus(int id, [FromBody] bool setTrending)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound("Job not found");

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var companyId))
                return Unauthorized("Invalid user ID");

            // Check if company owns the job
            if (job.CompanyId != companyId)
                return Forbid("You don't have permission to modify this job");

            // If already in desired state, just return
            if (job.IsTrending == setTrending)
                return Ok(new
                {
                    Success = true,
                    Message = $"Job is already {(setTrending ? "trending" : "not trending")}"
                });

            // Get company subscription
            var subscription = await _context.CompanySubscriptions
                .Where(s => s.UserId == companyId && s.IsActive && s.EndDate > DateTime.UtcNow)
                .Include(s => s.SubscriptionType)
                .FirstOrDefaultAsync();

            if (setTrending) // Setting job as trending
            {
                // Can't set trending without a subscription
                if (subscription == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "You need a Basic or Premium subscription to create trending jobs"
                    });
                }

                // Check if subscription allows trending jobs
                if (subscription.SubscriptionType.TrendingJobLimit <= 0)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Your current subscription doesn't include trending jobs"
                    });
                }

                // Check if there are remaining trending job slots
                if (subscription.RemainingTrendingJobPosts <= 0)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "You have used all your trending job posts. Please upgrade your subscription."
                    });
                }

                // Check for max active trending jobs
                var activeTrendingCount = await _context.Jobs
                    .CountAsync(j => j.CompanyId == companyId &&
                                 j.IsTrending &&
                                 (j.Status == Job.JobStatus.active || j.Status == Job.JobStatus.pending));

                if (activeTrendingCount >= subscription.SubscriptionType.TrendingJobLimit)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = $"You have reached the maximum limit of {subscription.SubscriptionType.TrendingJobLimit} active trending jobs"
                    });
                }

                // Set as trending and reduce count
                job.IsTrending = true;
                subscription.RemainingTrendingJobPosts--;
                subscription.UpdatedAt = DateTime.UtcNow;
            }
            else // Removing trending status
            {
                job.IsTrending = false;
                // Don't refund the trending post count
            }

            job.UpdatedAt = GetVietnamTime();
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = $"Job is now {(setTrending ? "trending" : "not trending")}",
                IsTrending = job.IsTrending,
                RemainingTrendingPosts = subscription?.RemainingTrendingJobPosts ?? 0
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobUpdateRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Quantity < 1)
                return BadRequest("Quantity must be at least 1.");

            if (!dto.IsSalaryNegotiable && (!dto.MinSalary.HasValue || !dto.MaxSalary.HasValue))
                return BadRequest("Minimum and maximum salary must be provided if salary is not negotiable.");
            if (dto.TimeEnd <= dto.TimeStart)
                return BadRequest("End time must be after start time.");
            if (dto.ExpiryDate <= GetVietnamTime())
                return BadRequest("Expiry date must be in the future.");

            var job = await _context.Jobs
                //.Include(j => j.JobSkills)
                .FirstOrDefaultAsync(j => j.JobId == id);
            if (job == null)
                return NotFound("Job not found.");

            var userIdStr = User.Identity?.Name;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();

            if (role == "company")
            {
                if (job.CompanyId != userId)
                    return StatusCode(403, "You are not the owner of this job.");

                if (!job.CanCompanyEditContent())
                    return StatusCode(403, "Job is locked by admin or expired, you cannot edit it.");

                if (job.Status == Job.JobStatus.pending)
                {
                    // Keep status as pending
                }
                else if (job.Status == Job.JobStatus.inactive && !job.IsExpired())
                {
                    job.Status = Job.JobStatus.pending;
                }
                else if (job.Status == Job.JobStatus.active && !job.IsExpired())
                {
                    job.Status = Job.JobStatus.pending;
                }
                else if (job.Status == Job.JobStatus.inactivebyadmin && !job.IsExpired())
                {
                  job.Status = Job.JobStatus.pending;
                }
            }
            else if (role != "admin")
            {
                return StatusCode(403, "You do not have permission to edit this job.");
            }

            job.Title = dto.Title;
            job.Description = dto.Description;
            job.Education = dto.Education;
            job.YourSkill = dto.YourSkill;
            job.YourExperience = dto.YourExperience;
            job.IndustryId = dto.IndustryId;
            job.ExpiryDate = dto.ExpiryDate;
            job.LevelId = dto.LevelId;
            job.JobTypeId = dto.JobTypeId;
            job.Quantity = dto.Quantity; // Thay thế ExperienceLevelId
            job.TimeStart = dto.TimeStart;
            job.TimeEnd = dto.TimeEnd;
            job.ProvinceName = dto.ProvinceName;
            job.AddressDetail = dto.AddressDetail;
            job.UpdatedAt = GetVietnamTime();
            job.IsSalaryNegotiable = dto.IsSalaryNegotiable;
            job.MinSalary = dto.IsSalaryNegotiable ? null : dto.MinSalary;
            job.MaxSalary = dto.IsSalaryNegotiable ? null : dto.MaxSalary;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Updated job #{id} by user with role: {role}");

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound("Job not found.");
            if (job.Status == Job.JobStatus.active)
                return BadRequest("Cannot delete an active job.");

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Deleted job #{id}");

            return NoContent();
        }


        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Job>>> FilterJobs([FromQuery] JobFilterParams filter)
        {
            var query = _context.Jobs
                .Where(j => !j.DeactivatedByAdmin)
                .Include(j => j.Industry)
                //.Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(j => j.Title.Contains(filter.Title));
            if (filter.IndustryId.HasValue)
                query = query.Where(j => j.IndustryId == filter.IndustryId);
            if (filter.LevelId.HasValue)
                query = query.Where(j => j.LevelId == filter.LevelId);
            if (filter.JobTypeId.HasValue)
                query = query.Where(j => j.JobTypeId == filter.JobTypeId);

            if (filter.Quantity.HasValue)
                query = query.Where(j => j.Quantity == filter.Quantity);

            if (filter.MinSalary.HasValue)
                query = query.Where(j => j.MinSalary >= filter.MinSalary);
            if (filter.MaxSalary.HasValue)
                query = query.Where(j => j.MaxSalary <= filter.MaxSalary);
            if (!string.IsNullOrEmpty(filter.ProvinceName))
                query = query.Where(j => j.ProvinceName.Contains(filter.ProvinceName));
            if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<Job.JobStatus>(filter.Status, out var status))
                query = query.Where(j => j.Status == status);
            if (filter.CompanyId.HasValue)
                query = query.Where(j => j.CompanyId == filter.CompanyId);
            if (filter.TimeStart.HasValue)
                query = query.Where(j => j.TimeStart >= filter.TimeStart);
            if (filter.TimeEnd.HasValue)
                query = query.Where(j => j.TimeEnd <= filter.TimeEnd);
           /* if (filter.SkillIds != null && filter.SkillIds.Any())
                query = query.Where(j => j.JobSkills.Any(js => filter.SkillIds.Contains(js.SkillId)));
            if (!string.IsNullOrEmpty(filter.SkillName))
                query = query.Where(j => j.JobSkills.Any(js => js.Skill.SkillName.Contains(filter.SkillName)));*/

            var jobs = await query.ToListAsync();
            _logger.LogInformation($"Filtered {jobs.Count} jobs with provided parameters");

            return Ok(jobs);
        }

        [Authorize]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateJobStatus(int id, [FromQuery] Job.JobStatus newStatus)
        {
            try
            {
                await AutoDeactivateExpiredJobs();

                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                    return NotFound("Job not found.");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized("Invalid user ID.");

                var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();

                if (job.TimeEnd < GetVietnamTime())
                    return BadRequest("Cannot change status of an expired job.");

                if (role == "admin")
                {
                    var previousStatus = job.Status;
                    if (job.Status == newStatus)
                        return BadRequest("Job is already in the specified status.");


                   
                    if (previousStatus == Job.JobStatus.pending &&
                        newStatus != Job.JobStatus.active &&
                        newStatus != Job.JobStatus.inactive &&
                        newStatus != Job.JobStatus.inactivebyadmin)
                        return BadRequest("Pending jobs can only be set to active, inactive, or inactivebyadmin.");

                    if (previousStatus == Job.JobStatus.active &&
                        newStatus != Job.JobStatus.inactive &&
                        newStatus != Job.JobStatus.inactivebyadmin)
                        return BadRequest("Active jobs can only be set to inactive or inactivebyadmin.");

                    if ((previousStatus == Job.JobStatus.inactive || previousStatus == Job.JobStatus.inactivebyadmin) &&
                        newStatus != Job.JobStatus.active)
                        return BadRequest("Inactive jobs can only be set to active.");

                    bool isChangingFromPending = previousStatus == Job.JobStatus.pending;
                    bool isApproving = newStatus == Job.JobStatus.active;
                    bool isRejecting = newStatus == Job.JobStatus.inactive || newStatus == Job.JobStatus.inactivebyadmin;

                    job.Status = newStatus;
                    job.UpdatedAt = GetVietnamTime();

                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Job #{id} status updated to {newStatus} by admin");

                    if (isChangingFromPending && (isApproving || isRejecting))
                    {
                        _logger.LogInformation($"Sending job status notification to company for job #{id}");
                        await _notificationService.CreateJobStatusNotification(job, isApproving);
                    }

                    if (isChangingFromPending && isApproving)
                    {
                        _logger.LogInformation($"Preparing to send notifications to candidates for job #{id}");
                        try
                        {
                            var favoriteUsers = await _context.UserFavoriteCompanies
                                .Where(f => f.CompanyProfileId == job.CompanyId)
                                .Include(f => f.User)
                                .Select(f => f.User)
                                .ToListAsync();
                            _logger.LogInformation($"Found {favoriteUsers.Count} users who favorited company {job.CompanyId}");

                            var allCandidates = await _context.Users
                                .Where(u => u.RoleId == 1) // Assuming 1 is Candidate role
                                .ToListAsync();
                            _logger.LogInformation($"Found {allCandidates.Count} total candidates");

                            var companyUser = await _context.Users.FindAsync(job.CompanyId);
                            if (companyUser != null)
                            {
                                await _notificationService.CreateNewJobNotification(job, companyUser, allCandidates, favoriteUsers);
                                _logger.LogInformation($"Notifications sent for job #{id}");
                            }
                            else
                            {
                                _logger.LogWarning($"Company user not found for company ID {job.CompanyId}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error sending notifications: {ex.Message}");
                            _logger.LogError($"Stack trace: {ex.StackTrace}");
                        }
                    }

                    return Ok($"Admin updated job #{id} status to {newStatus}.");
                }
                else if (role == "company")
                {
                    if (job.CompanyId != userId)
                        return Forbid("You are not the owner of this job.");

                    if (job.Status == Job.JobStatus.pending)
                        return Forbid("Job is pending approval. Only admin can update its status.");

                    if (job.Status == Job.JobStatus.inactivebyadmin)
                        return Forbid("Job was inactivated by admin. You cannot change its status.");


                    if (job.Status == Job.JobStatus.active && newStatus == Job.JobStatus.inactive)
                    {
                        job.Status = Job.JobStatus.inactive;
                        job.DeactivatedByAdmin = false;

                        job.UpdatedAt = GetVietnamTime();
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Job #{id} deactivated by company");
                        return Ok("Company deactivated the job successfully.");
                    }

                    if (job.Status == Job.JobStatus.inactive && newStatus == Job.JobStatus.active)
                    {
                        if (job.DeactivatedByAdmin)
                            return Forbid("Job was deactivated by admin. Company cannot reactivate it.");
                        if (job.TimeStart > GetVietnamTime())
                            return BadRequest("Cannot activate job before its start date.");
                        job.Status = Job.JobStatus.active;
                        job.UpdatedAt = GetVietnamTime();
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Job #{id} reactivated by company");
                        return Ok("Company reactivated the job successfully.");
                    }

                    return BadRequest("Company can only deactivate an active job or activate an inactive job.");
                }
                else
                {
                    return Forbid("You do not have permission to update job status.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating job status: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the job status.");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/lock")]
        public async Task<IActionResult> LockJob(int id, [FromQuery] bool isLock)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound("Job not found.");

            job.DeactivatedByAdmin = isLock;

            if (isLock && job.Status == Job.JobStatus.active)
                job.Status = Job.JobStatus.inactive;

            job.UpdatedAt = GetVietnamTime();
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Job #{id} {(isLock ? "locked" : "unlocked")} by admin");

            return Ok(isLock ? "Job has been locked by admin." : "Job has been unlocked by admin.");
        }


        [AllowAnonymous]
        [HttpGet("{id}/view")]
        public async Task<IActionResult> ViewJob(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Industry)
                //.Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Company).ThenInclude(u => u.CompanyProfile)
                .Include(j => j.Level)
                .Include(j => j.JobType)
                // XÓA: .Include(j => j.ExperienceLevel)
                .FirstOrDefaultAsync(j => j.JobId == id);

            if (job == null)
                return NotFound("Job not found.");

            int? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
            }

            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string? userAgent = Request.Headers["User-Agent"].ToString();

            var jobView = new JobView
            {
                JobId = id,
                UserId = userId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                ViewedAt = GetVietnamTime()
            };

            _context.JobViews.Add(jobView);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Recorded view for job #{id} from IP: {ipAddress}");

            return Ok(new
            {
                job.JobId,
                job.Title,
                job.Description,
                job.YourSkill,
                job.YourExperience,
                job.Education,
                job.CompanyId,
                Company = job.Company == null ? null : new
                {
                    job.Company.UserId,
                    job.Company.FullName,
                    job.Company.Email,
                    CompanyName = job.Company.CompanyProfile?.CompanyName,
                    Location = job.Company.CompanyProfile?.Location,
                    UrlCompanyLogo = job.Company.CompanyProfile?.UrlCompanyLogo
                },
                job.IndustryId,
                Industry = job.Industry == null ? null : new
                {
                    job.Industry.IndustryId,
                    job.Industry.IndustryName
                },
                job.ExpiryDate,
                job.LevelId,
                Level = job.Level == null ? null : new
                {
                    job.Level.LevelId,
                    job.Level.LevelName
                },
                job.JobTypeId,
                JobType = job.JobType == null ? null : new
                {
                    job.JobType.JobTypeId,
                    job.JobType.JobTypeName
                },
                job.Quantity, // Thêm trường này
                job.TimeStart,
                job.TimeEnd,
                job.Status,
                job.ProvinceName,
                job.AddressDetail,
                job.IsSalaryNegotiable,
                job.MinSalary,
                job.MaxSalary,
                job.CreatedAt,
                job.UpdatedAt,
               /* Skills = job.JobSkills.Select(js => new
                {
                    js.SkillId,
                    js.Skill.SkillName
                }).ToList(),*/
                job.DescriptionWeight,
                job.SkillsWeight,
                job.ExperienceWeight,
                job.EducationWeight,
                ViewTracked = true
            });
        }


        private async Task AutoDeactivateExpiredJobs()
        {
            var now = GetVietnamTime();
            var expiredJobs = await _context.Jobs
                .Where(j => j.Status == Job.JobStatus.active && j.TimeEnd <= now)
                .ToListAsync();

            foreach (var job in expiredJobs)
            {
                job.Status = Job.JobStatus.inactive;
                job.UpdatedAt = now;
            }

            if (expiredJobs.Any())
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Deactivated {expiredJobs.Count} expired jobs");
            }
        }

        /*[HttpGet("notify-upcoming-start-new")]
        public async Task<IActionResult> NotifyUpcomingStartNew(int daysBefore = 2)
        {
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var cutoffDate = now.AddDays(daysBefore);

            var upcomingJobs = await _context.Jobs
                .Where(j => j.TimeStart >= now && j.TimeStart <= cutoffDate && !j.DeactivatedByAdmin)
                .Include(j => j.Company)
                .ToListAsync();

            if (!upcomingJobs.Any())
            {
                _logger.LogInformation("No jobs found with upcoming start dates within {0} days.", daysBefore);
                return Ok(new { Success = true, Message = "No upcoming start dates to notify.", Count = 0, Jobs = new object[0] });
            }

            var jobsData = upcomingJobs
                .Where(job => job.Company != null)
                .Select(job => new
                {
                    JobId = job.JobId,
                    Title = job.Title,
                    DaysRemaining = (job.TimeStart.Date - now.Date).Days,
                    Link = $"{_configuration["AppSettings:BaseUrl"]}/job-single-v3/{job.JobId}"
                })
                .ToList();

            return Ok(new
            {
                Success = true,
                Message = $"Notified {jobsData.Count} jobs with upcoming start dates.",
                Count = jobsData.Count,
                Jobs = jobsData
            });
        }*/
        // Update only this method in JobController.cs
        [HttpGet("notify-upcoming-start-new")]
        public async Task<IActionResult> NotifyUpcomingStartNew(int daysBefore = 2)
        {
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            // Today's jobs that have just started
            var todaysStartingJobs = await _context.Jobs
                .Where(j => j.TimeStart.Date == now.Date &&
                          j.Status == Job.JobStatus.active &&
                          !j.DeactivatedByAdmin)
                .Include(j => j.Company)
                .ToListAsync();

            // Upcoming jobs for notification preview only
            var upcomingJobs = await _context.Jobs
                .Where(j => j.TimeStart > now &&
                          j.TimeStart <= now.AddDays(daysBefore) &&
                          j.Status == Job.JobStatus.active &&
                          !j.DeactivatedByAdmin)
                .Include(j => j.Company)
                .ToListAsync();

            int notifiedCount = 0;

            // Process jobs that are starting today - send notifications
            foreach (var job in todaysStartingJobs.Where(j => j.Company != null))
            {
                try
                {
                    await _notificationService.SendStartDateReachedNotifications(job);
                    notifiedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending notifications for job {job.JobId}: {ex.Message}");
                }
            }

            var allJobs = todaysStartingJobs.Concat(upcomingJobs).ToList();

            if (!allJobs.Any())
            {
                _logger.LogInformation("No jobs found with start dates today or within {0} days.", daysBefore);
                return Ok(new { Success = true, Message = "No jobs to notify.", Count = 0, Jobs = new object[0] });
            }

            var jobsData = allJobs
                .Where(job => job.Company != null)
                .Select(job => new
                {
                    JobId = job.JobId,
                    Title = job.Title,
                    StartDate = job.TimeStart.Date,
                    IsToday = job.TimeStart.Date == now.Date,
                    DaysRemaining = (job.TimeStart.Date - now.Date).Days,
                    NotificationsSent = job.TimeStart.Date == now.Date,
                    Link = $"{_configuration["AppSettings:BaseUrl"]}/job-single-v3/{job.JobId}"
                })
                .ToList();

            return Ok(new
            {
                Success = true,
                Message = $"Processed {allJobs.Count} jobs. Sent notifications for {notifiedCount} jobs starting today.",
                TodaysJobsCount = todaysStartingJobs.Count,
                UpcomingJobsCount = upcomingJobs.Count,
                NotificationsSent = notifiedCount,
                Jobs = jobsData
            });
        }



        [AllowAnonymous]
        [HttpGet("company/{companyId}/highlight")]
        public async Task<ActionResult<IEnumerable<object>>> GetCompanyHighlightJobs(
            int companyId,
            [FromQuery] int limit = 5, 
            [FromQuery] string timeRange = "7d")
        {
            var now = GetVietnamTime();
            var query = _context.Jobs
                .Where(j => j.CompanyId == companyId
                            && j.Status == Job.JobStatus.active
                            && !j.DeactivatedByAdmin
                            && j.TimeStart.Date <= now.Date
                            && j.TimeEnd.Date >= now.Date)
                .Include(j => j.Industry)
                //.Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Company).ThenInclude(u => u.CompanyProfile)
                .Include(j => j.Level)
                .Include(j => j.JobType)
                .AsQueryable();

            // Apply time range for view counts
            DateTime? startTime = null;
            switch (timeRange.ToLower())
            {
                case "24h":
                    startTime = now.AddHours(-24);
                    break;
                case "7d":
                    startTime = now.AddDays(-7);
                    break;
                case "30d":
                    startTime = now.AddDays(-30);
                    break;
                case "all":
                default:
                    break;
            }

            // Get job view counts
            var jobViewCounts = await _context.JobViews
                .Where(v => startTime == null || v.ViewedAt >= startTime)
                .GroupBy(v => v.JobId)
                .Select(g => new { JobId = g.Key, TotalViews = g.Count() })
                .ToListAsync();

            // Get unique view counts
            var uniqueViewCounts = await _context.JobViews
                .Where(v => startTime == null || v.ViewedAt >= startTime)
                .Select(v => new { v.JobId, v.UserId, v.IpAddress })
                .Distinct()
                .GroupBy(v => v.JobId)
                .Select(g => new { JobId = g.Key, UniqueViews = g.Count() })
                .ToListAsync();

            var viewCountDict = jobViewCounts.ToDictionary(v => v.JobId, v => v.TotalViews);
            var uniqueViewCountDict = uniqueViewCounts.ToDictionary(v => v.JobId, v => v.UniqueViews);

            // Get filtered jobs
            var jobs = await query.ToListAsync();

            // Order jobs by view count and creation date, limit to top N
            var orderedJobs = jobs
                .Select(j => new
                {
                    Job = j,
                    TotalViews = viewCountDict.ContainsKey(j.JobId) ? viewCountDict[j.JobId] : 0,
                    UniqueViews = uniqueViewCountDict.ContainsKey(j.JobId) ? uniqueViewCountDict[j.JobId] : 0
                })
                .OrderByDescending(x => x.TotalViews)
                .ThenByDescending(x => x.Job.CreatedAt)
                .Take(limit)
                .ToList();

            // Get BaseUrl from configuration
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                _logger.LogWarning("BaseUrl is not configured in AppSettings.");
                baseUrl = "http://localhost:3000/"; // Fallback for safety
            }

            // Ensure BaseUrl ends with a slash
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            // Format response with filter URL for each job
            var result = orderedJobs.Select(item => new
            {
                item.Job.JobId,
                item.Job.Title,
                item.Job.Description,
                item.Job.Education,
                item.Job.YourSkill,
                item.Job.YourExperience,
                item.Job.CompanyId,
                TotalViews = item.TotalViews,
                UniqueViews = item.UniqueViews,
                IsTrending = item.Job.IsTrending,
                Company = item.Job.Company == null ? null : new
                {
                    item.Job.Company.UserId,
                    item.Job.Company.FullName,
                    item.Job.Company.Email,
                    CompanyName = item.Job.Company.CompanyProfile?.CompanyName,
                    Location = item.Job.Company.CompanyProfile?.Location,
                    UrlCompanyLogo = item.Job.Company.CompanyProfile?.UrlCompanyLogo
                },
                item.Job.IndustryId,
                Industry = item.Job.Industry == null ? null : new
                {
                    item.Job.Industry.IndustryId,
                    item.Job.Industry.IndustryName
                },
                item.Job.ExpiryDate,
                item.Job.LevelId,
                Level = item.Job.Level == null ? null : new
                {
                    item.Job.Level.LevelId,
                    item.Job.Level.LevelName
                },
                item.Job.JobTypeId,
                JobType = item.Job.JobType == null ? null : new
                {
                    item.Job.JobType.JobTypeId,
                    item.Job.JobType.JobTypeName
                },
                item.Job.Quantity,
                item.Job.TimeStart,
                item.Job.TimeEnd,
                item.Job.Status,
                item.Job.ProvinceName,
                item.Job.AddressDetail,
                item.Job.IsSalaryNegotiable,
                item.Job.MinSalary,
                item.Job.MaxSalary,
                item.Job.CreatedAt,
                item.Job.UpdatedAt,
               /* Skills = item.Job.JobSkills.Select(js => new
                {
                    js.SkillId,
                    js.Skill.SkillName
                }).ToList(),*/
                item.Job.DescriptionWeight,
                item.Job.SkillsWeight,
                item.Job.ExperienceWeight,
                item.Job.EducationWeight,
                //FilterUrl = $"{baseUrl}api/Job/filter?IndustryId={item.Job.IndustryId}&LevelId={item.Job.LevelId}&JobTypeId={item.Job.JobTypeId}&ProvinceName={Uri.EscapeDataString(item.Job.ProvinceName ?? "")}&SkillIds={string.Join(",", item.Job.JobSkills.Select(js => js.SkillId))}"
                FilterUrl = $"{baseUrl}api/Job/filter?IndustryId={item.Job.IndustryId}&LevelId={item.Job.LevelId}&JobTypeId={item.Job.JobTypeId}&ProvinceName={Uri.EscapeDataString(item.Job.ProvinceName ?? "")}"
            });

            return Ok(new
            {
                TotalCount = orderedJobs.Count,
                TimeRange = timeRange,
                Jobs = result 
            });
        }


        [HttpPost("save-draft")]
        [Consumes("application/json")]
        public async Task<ActionResult<Job>> SaveDraft()
        {
            var request = await Request.ReadFromJsonAsync<JobDraftRequest>();
            _logger.LogInformation($"Received payload: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");

            if (request == null)
                return BadRequest("Invalid request body.");

            if (request.CompanyId <= 0)
                return BadRequest("CompanyId is required.");
            if (request.Quantity < 1)
                return BadRequest("Quantity must be at least 1.");

            if (request.ExpiryDate.HasValue && request.ExpiryDate.Value <= GetVietnamTime())
                return BadRequest("ExpiryDate must be in the future.");

            var job = new Job
            {
                Title = request.Title ?? string.Empty,
                Description = request.Description ?? string.Empty,
                Education = request.Education ?? string.Empty,
                YourSkill = request.YourSkill ?? string.Empty,
                YourExperience = request.YourExperience ?? string.Empty,
                CompanyId = request.CompanyId,
                IndustryId = request.IndustryId ?? 1,
                ExpiryDate = request.ExpiryDate ?? DateTime.MaxValue,
                LevelId = request.LevelId ?? 1,
                JobTypeId = request.JobTypeId ?? 1,
                Quantity = request.Quantity,
                TimeStart = request.TimeStart ?? DateTime.MinValue,
                TimeEnd = request.TimeEnd ?? DateTime.MinValue,
                ProvinceName = request.ProvinceName ?? string.Empty,
                AddressDetail = request.AddressDetail ?? string.Empty,
                CreatedAt = GetVietnamTime(),
                UpdatedAt = GetVietnamTime(),
                Status = Job.JobStatus.draft,
                IsSalaryNegotiable = request.IsSalaryNegotiable,
                MinSalary = request.IsSalaryNegotiable ? null : request.MinSalary,
                MaxSalary = request.IsSalaryNegotiable ? null : request.MaxSalary,
                DescriptionWeight = (request.DescriptionWeight ?? 0) / 100f,
                SkillsWeight = (request.SkillsWeight ?? 0) / 100f,
                ExperienceWeight = (request.ExperienceWeight ?? 0) / 100f,
                EducationWeight = (request.EducationWeight ?? 0) / 100f
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Saved new draft job #{job.JobId} with title: {job.Title}");

            return CreatedAtAction(nameof(GetJob), new { id = job.JobId }, new { Job = job });
        }


        [Authorize]
        [HttpDelete("draft/{id}")]
     
        public async Task<IActionResult> DeleteDraft(int id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var companyId))
                return Unauthorized("Invalid user ID");

            var draft = await _context.Jobs
                .FirstOrDefaultAsync(j => j.JobId == id && j.CompanyId == companyId && j.Status == Job.JobStatus.draft);
            if (draft == null)
                return NotFound("Draft not found.");

            _context.Jobs.Remove(draft);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Deleted draft job #{id} for company {companyId}");

            return NoContent();
        }



        [Authorize]
        [HttpGet("drafts")]
      
        public async Task<ActionResult<IEnumerable<object>>> GetDraftJobs()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var companyId))
                return Unauthorized("Invalid user ID");

            var query = _context.Jobs
                .Where(j => j.CompanyId == companyId && j.Status == Job.JobStatus.draft)
                .Include(j => j.Industry)
                //.Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Company).ThenInclude(u => u.CompanyProfile)
                .Include(j => j.Level)
                .Include(j => j.JobType)
                .AsQueryable();

            var jobs = await query.ToListAsync();

            var result = jobs.Select(job => new
            {
                job.JobId,
                job.Title,
                job.Description,
                job.Education,
                job.YourSkill,
                job.YourExperience,
                job.CompanyId,
                Company = job.Company == null ? null : new
                {
                    job.Company.UserId,
                    job.Company.FullName,
                    job.Company.Email,
                    CompanyName = job.Company.CompanyProfile?.CompanyName,
                    Location = job.Company.CompanyProfile?.Location,
                    UrlCompanyLogo = job.Company.CompanyProfile?.UrlCompanyLogo
                },
                job.IndustryId,
                Industry = job.Industry == null ? null : new
                {
                    job.Industry.IndustryId,
                    job.Industry.IndustryName
                },
                job.ExpiryDate,
                job.LevelId,
                Level = job.Level == null ? null : new
                {
                    job.Level.LevelId,
                    job.Level.LevelName
                },
                job.JobTypeId,
                JobType = job.JobType == null ? null : new
                {
                    job.JobType.JobTypeId,
                    job.JobType.JobTypeName
                },
                job.Quantity,
                job.TimeStart,
                job.TimeEnd,
                job.Status,
                job.ProvinceName,
                job.AddressDetail,
                job.IsSalaryNegotiable,
                job.MinSalary,
                job.MaxSalary,
                job.CreatedAt,
                job.UpdatedAt,
               /* Skills = job.JobSkills.Select(js => new
                {
                    js.SkillId,
                    js.Skill.SkillName
                }).ToList(),*/
                job.DescriptionWeight,
                job.SkillsWeight,
                job.ExperienceWeight,
                job.EducationWeight,
                job.IsTrending
            });

            return Ok(result);
        }




    }
}