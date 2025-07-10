using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Models.filter;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public JobController(JobFinderDbContext context, EmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        // Get current time in Vietnam timezone
        private static DateTime GetVietnamTime()
        {
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
        }

        // GET: api/Job
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetJobs(
            [FromQuery] string role = "candidate",
            [FromQuery] int? companyId = null)
        {
            var now = GetVietnamTime();
            var query = _context.Jobs
                .Include(j => j.Industry)
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Company).ThenInclude(u => u.CompanyProfile)
                .Include(j => j.Level)
                .Include(j => j.JobType)
                .Include(j => j.ExperienceLevel)
                .AsQueryable();

            if (role == "candidate")
            {
                // Candidates only see active jobs that have started, are not expired, and are not locked by admin
                query = query.Where(j => j.Status == Job.JobStatus.active
                                        && !j.DeactivatedByAdmin
                                        && j.TimeStart.Date <= now.Date
                                        && j.TimeEnd.Date >= now.Date);
            }
            else if (role == "company" && companyId.HasValue)
            {
                // Companies only see their own jobs
                query = query.Where(j => j.CompanyId == companyId);
            }
            else if (role == "admin")
            {
                // Admins see all jobs without additional filters
            }
            else
            {
                return BadRequest("Invalid role parameter.");
            }

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
                job.ExperienceLevelId,
                ExperienceLevel = job.ExperienceLevel == null ? null : new
                {
                    job.ExperienceLevel.ExperienceLevelid,
                    job.ExperienceLevel.name
                },
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
                Skills = job.JobSkills.Select(js => new
                {
                    js.SkillId,
                    js.Skill.SkillName
                }).ToList(),
                job.DescriptionWeight,
                job.SkillsWeight,
                job.ExperienceWeight,
                job.EducationWeight
            });

            return Ok(result);
        }

        // GET: api/Job/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetJob(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Industry)
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Company).ThenInclude(u => u.CompanyProfile)
                .Include(j => j.Level)
                .Include(j => j.JobType)
                .Include(j => j.ExperienceLevel)
                .FirstOrDefaultAsync(j => j.JobId == id);

            if (job == null)
                return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            var userIdStr = User.Identity?.Name;
            int.TryParse(userIdStr, out var userId);

            // If job is locked, only allow admin or job owner to view
            if (job.DeactivatedByAdmin && role != "admin" && job.CompanyId != userId)
                return NotFound();

            // Anonymous users or candidates can only view active or expired jobs
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
                job.ExperienceLevelId,
                ExperienceLevel = job.ExperienceLevel == null ? null : new
                {
                    job.ExperienceLevel.ExperienceLevelid,
                    job.ExperienceLevel.name
                },
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
                Skills = job.JobSkills.Select(js => new
                {
                    js.SkillId,
                    js.Skill.SkillName
                }).ToList(),
                job.DescriptionWeight,
                job.SkillsWeight,
                job.ExperienceWeight,
                job.EducationWeight
            });
        }

        // GET: api/Job/{id}/view
        [AllowAnonymous]
        [HttpGet("{id}/view")]
        public async Task<IActionResult> ViewJob(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Industry)
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Company).ThenInclude(u => u.CompanyProfile)
                .Include(j => j.Level)
                .Include(j => j.JobType)
                .Include(j => j.ExperienceLevel)
                .FirstOrDefaultAsync(j => j.JobId == id);

            if (job == null)
                return NotFound();

            // Get current user ID if authenticated
            int? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
            }

            // Get IP address and User Agent
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string? userAgent = Request.Headers["User-Agent"].ToString();

            // Create new job view record
            var jobView = new JobView
            {
                JobId = id,
                UserId = userId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                ViewedAt = DateTime.UtcNow
            };

            _context.JobViews.Add(jobView);
            await _context.SaveChangesAsync();

            // Return job data similar to GetJob
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
                job.ExperienceLevelId,
                ExperienceLevel = job.ExperienceLevel == null ? null : new
                {
                    job.ExperienceLevel.ExperienceLevelid,
                    job.ExperienceLevel.name
                },
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
                Skills = job.JobSkills.Select(js => new
                {
                    js.SkillId,
                    js.Skill.SkillName
                }).ToList(),
                job.DescriptionWeight,
                job.SkillsWeight,
                job.ExperienceWeight,
                job.EducationWeight,
                ViewTracked = true
            });
        }

        // POST: api/Job/create
        [HttpPost("create")]
        public async Task<ActionResult<Job>> CreateJob([FromBody] JobCreateRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check for NaN in weights
            if (float.IsNaN(dto.DescriptionWeight) || float.IsNaN(dto.SkillsWeight) ||
                float.IsNaN(dto.ExperienceWeight) || float.IsNaN(dto.EducationWeight))
                return BadRequest("Weights cannot be NaN.");

            // Validate salary and dates
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
                ExperienceLevelId = dto.ExperienceLevelId,
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

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            // Add or create skills for the job
            if (dto.skillInputs != null && dto.skillInputs.Any())
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
                        }
                    }
                    else
                    {
                        continue;
                    }

                    _context.JobSkills.Add(new JobSkill { JobId = job.JobId, SkillId = skillId });
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetJob), new { id = job.JobId }, job);
        }

        // PUT: api/Job/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobUpdateRequest dto)
        {
            // Validate salary and dates
            if (!dto.IsSalaryNegotiable && (!dto.MinSalary.HasValue || !dto.MaxSalary.HasValue))
                return BadRequest("Minimum and maximum salary must be provided if salary is not negotiable.");
            if (dto.TimeEnd <= dto.TimeStart)
                return BadRequest("End time must be after start time.");
            if (dto.ExpiryDate <= GetVietnamTime())
                return BadRequest("Expiry date must be in the future.");

            var job = await _context.Jobs
                .Include(j => j.JobSkills)
                .FirstOrDefaultAsync(j => j.JobId == id);
            if (job == null)
                return NotFound();

            var userIdStr = User.Identity?.Name;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();

            if (role == "company")
            {
                if (job.CompanyId != userId)
                    return StatusCode(403, "You are not the owner of this job.");

                // Prevent editing if job is locked by admin or expired
                if (!job.CanCompanyEditContent())
                    return StatusCode(403, "Job is locked by admin or expired, you cannot edit it.");

                // If job is pending, do not update status
                if (job.Status == Job.JobStatus.pending)
                {
                    // Keep status as pending
                }
                // If job is inactive (not locked by admin, not expired), set status to pending
                else if (job.Status == Job.JobStatus.inactive && !job.IsExpired())
                {
                    job.Status = Job.JobStatus.pending;
                }
                // If job is active (approved by admin), set status to pending
                else if (job.Status == Job.JobStatus.active && !job.IsExpired())
                {
                    job.Status = Job.JobStatus.pending;
                }
            }
            else if (role != "admin")
            {
                return StatusCode(403, "You do not have permission to edit this job.");
            }

            // Update job details
            job.Title = dto.Title;
            job.Description = dto.Description;
            job.Education = dto.Education;
            job.YourSkill = dto.YourSkill;
            job.YourExperience = dto.YourExperience;
            job.IndustryId = dto.IndustryId;
            job.ExpiryDate = dto.ExpiryDate;
            job.LevelId = dto.LevelId;
            job.JobTypeId = dto.JobTypeId;
            job.ExperienceLevelId = dto.ExperienceLevelId;
            job.TimeStart = dto.TimeStart;
            job.TimeEnd = dto.TimeEnd;
            job.ProvinceName = dto.ProvinceName;
            job.AddressDetail = dto.AddressDetail;
            job.UpdatedAt = GetVietnamTime();
            job.IsSalaryNegotiable = dto.IsSalaryNegotiable;
            job.MinSalary = dto.IsSalaryNegotiable ? null : dto.MinSalary;
            job.MaxSalary = dto.IsSalaryNegotiable ? null : dto.MaxSalary;

            // Update JobSkills (commented out as in original code)
            /*
            if (dto.skillInputs != null)
            {
                var oldSkills = job.JobSkills.ToList();
                _context.JobSkills.RemoveRange(oldSkills);

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
                        }
                    }
                    else
                    {
                        continue;
                    }

                    _context.JobSkills.Add(new JobSkill { JobId = job.JobId, SkillId = skillId });
                }
                await _context.SaveChangesAsync();
            }
            */

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Job/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound();
            if (job.Status == Job.JobStatus.active)
                return BadRequest("Cannot delete an active job.");

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Job/filter
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Job>>> FilterJobs([FromQuery] JobFilterParams filter)
        {
            var query = _context.Jobs
                .Where(j => !j.DeactivatedByAdmin)
                .Include(j => j.Industry)
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(j => j.Title.Contains(filter.Title));
            if (filter.IndustryId.HasValue)
                query = query.Where(j => j.IndustryId == filter.IndustryId);
            if (filter.LevelId.HasValue)
                query = query.Where(j => j.LevelId == filter.LevelId);
            if (filter.JobTypeId.HasValue)
                query = query.Where(j => j.JobTypeId == filter.JobTypeId);
            if (filter.ExperienceLevelId.HasValue)
                query = query.Where(j => j.ExperienceLevelId == filter.ExperienceLevelId);
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
            if (filter.SkillIds != null && filter.SkillIds.Any())
                query = query.Where(j => j.JobSkills.Any(js => filter.SkillIds.Contains(js.SkillId)));
            if (!string.IsNullOrEmpty(filter.SkillName))
                query = query.Where(j => j.JobSkills.Any(js => js.Skill.SkillName.Contains(filter.SkillName)));

            var jobs = await query.ToListAsync();
            return Ok(jobs);
        }

        // PUT: api/Job/{id}/status
        [Authorize]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateJobStatus(int id, [FromQuery] Job.JobStatus newStatus)
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
                return BadRequest(new { message = "Cannot change status of an expired job." });

            if (role == "admin")
            {
                if (job.Status == newStatus)
                    return BadRequest("Job is already in the specified status.");

                // Send email only when transitioning from pending to active
                bool shouldSendEmail = job.Status == Job.JobStatus.pending && newStatus == Job.JobStatus.active;

                job.Status = newStatus;
                job.UpdatedAt = GetVietnamTime();
                job.DeactivatedByAdmin = newStatus == Job.JobStatus.inactive;

                await _context.SaveChangesAsync();

                if (shouldSendEmail)
                {
                    await SendJobApprovalEmail(job);
                }

                return Ok($"Admin updated job #{id} status to {newStatus}. Background service will manage timing automatically.");
            }

            if (role == "company")
            {
                if (job.CompanyId != userId)
                    return Forbid("You are not the owner of this job.");

                if (job.Status == Job.JobStatus.pending)
                    return Forbid("Job is pending approval. Only admin can update its status.");

                if (job.Status == Job.JobStatus.active && newStatus == Job.JobStatus.inactive)
                {
                    job.Status = Job.JobStatus.inactive;
                    job.DeactivatedByAdmin = false;
                    job.UpdatedAt = GetVietnamTime();
                    await _context.SaveChangesAsync();
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
                    return Ok("Company reactivated the job successfully.");
                }

                return BadRequest("Company can only deactivate an active job or activate an inactive job.");
            }

            return Forbid("You do not have permission to update job status.");
        }

        // PUT: api/Job/{id}/lock
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/lock")]
        public async Task<IActionResult> LockJob(int id, [FromQuery] bool isLock)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound("Job not found.");

            job.DeactivatedByAdmin = isLock;

            // If locking, set status to inactive if currently active
            if (isLock && job.Status == Job.JobStatus.active)
                job.Status = Job.JobStatus.inactive;

            // If unlocking and job is not expired, allow admin to activate later if needed
            job.UpdatedAt = GetVietnamTime();
            await _context.SaveChangesAsync();

            return Ok(isLock ? "Job has been locked by admin." : "Job has been unlocked by admin.");
        }

        // Send email to users who favorited the company when a job is approved
        private async Task SendJobApprovalEmail(Job job)
        {
            var favoriteUsers = await _context.UserFavoriteCompanies
                .Where(f => f.CompanyId == job.CompanyId)
                .Select(f => f.User)
                .ToListAsync();

            var companyProfile = await _context.CompanyProfile
                .FirstOrDefaultAsync(c => c.UserId == job.CompanyId);

            string companyName = companyProfile?.CompanyName ?? "Company";
            string jobUrl = _configuration["Frontend:JobUrl"]?.Replace("{JobId}", job.JobId.ToString())
                           ?? $"https://job-finder-fe.vercel.app/job-single-v3/{job.JobId}";

            string mailBody = $@"
                <div style='font-family: Arial, sans-serif;'>
                    <h2 style='color:#2d8cf0;'>{companyName} has posted a new job!</h2>
                    <p><b>Job Title:</b> {job.Title}</p>
                    <p><b>Location:</b> {job.ProvinceName}</p>
                    <p><b>Start Date:</b> {job.TimeStart:dd/MM/yyyy}</p>
                    <p><b>Application Deadline:</b> {job.ExpiryDate:dd/MM/yyyy}</p>
                    <p><b>Description:</b> {job.Description}</p>
                    <div style='margin:20px 0;'>
                        <a href='{jobUrl}' style='background:#2d8cf0;color:#fff;padding:10px 20px;border-radius:4px;text-decoration:none;font-weight:bold;'>View Details & Apply</a>
                    </div>
                </div>";

            foreach (var user in favoriteUsers)
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    _emailService.SendEmail(
                        user.Email,
                        $"[{companyName}] New Job Posted: {job.Title}",
                        mailBody,
                        true
                    );
                }
            }
        }

        // Deactivate expired jobs
        private async Task AutoDeactivateExpiredJobs()
        {
            var now = GetVietnamTime();
            var expiredJobs = await _context.Jobs
                .Where(j => j.Status == Job.JobStatus.active && j.TimeEnd < now)
                .ToListAsync();

            foreach (var job in expiredJobs)
            {
                job.Status = Job.JobStatus.inactive;
                job.UpdatedAt = now;
            }

            if (expiredJobs.Any())
                await _context.SaveChangesAsync();
        }
    }
}