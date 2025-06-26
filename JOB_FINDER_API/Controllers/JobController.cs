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
        public JobController(JobFinderDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        // GET: api/Job
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetJobs()
        {
            var jobs = await _context.Jobs
                .Include(j => j.Industry)
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Company).ThenInclude(u => u.CompanyProfile)
                .Include(j => j.Level)
                .Include(j => j.JobType)
                .Include(j => j.ExperienceLevel)
                .ToListAsync();

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
                    job.Company.Id,
                    job.Company.FullName,
                    job.Company.Email,
                    job.Company.CompanyProfile?.CompanyName,
                    job.Company.CompanyProfile?.Location,
                    job.Company.CompanyProfile?.UrlCompanyLogo
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
                    job.Level.Id,
                    job.Level.LevelName
                },
                job.JobTypeId,
                JobType = job.JobType == null ? null : new
                {
                    job.JobType.Id,
                    job.JobType.JobTypeName
                },
                job.ExperienceLevelId,
                ExperienceLevel = job.ExperienceLevel == null ? null : new
                {
                    job.ExperienceLevel.id,
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
            });

            return Ok(result);
        }


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

            var role = User.FindFirst(ClaimTypes.Role)?.Value.ToLower();
            var userIdStr = User.Identity?.Name;
            int.TryParse(userIdStr, out var userId);

            // Nếu job bị lock, chỉ cho phép admin hoặc company chủ job xem
            if (job.DeactivatedByAdmin && role != "admin" && job.CompanyId != userId)
                return NotFound();

            // Nếu là anonymous hoặc candidate, chỉ cho xem job Active hoặc đã hết hạn
            bool isAnonymous = !User.Identity.IsAuthenticated;
            if (isAnonymous || role == "candidate")
            {
                bool isActive = job.Status == Job.JobStatus.active;
                bool isExpired = job.TimeEnd < DateTime.UtcNow;

                // Chỉ cho xem nếu job active hoặc expired
                if (!isActive && !isExpired)
                {
                    return StatusCode(403, "You do not have permission to view this job.");
                }
            }

            // ... trả về thông tin job như cũ
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
                    job.Company.Id,
                    job.Company.FullName,
                    job.Company.Email,
                    job.Company.CompanyProfile?.CompanyName,
                    job.Company.CompanyProfile?.Location,
                    job.Company.CompanyProfile?.UrlCompanyLogo
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
                    job.Level.Id,
                    job.Level.LevelName
                },
                job.JobTypeId,
                JobType = job.JobType == null ? null : new
                {
                    job.JobType.Id,
                    job.JobType.JobTypeName
                },
                job.ExperienceLevelId,
                ExperienceLevel = job.ExperienceLevel == null ? null : new
                {
                    job.ExperienceLevel.id,
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
            });
        }


        [HttpPost("create")]
        public async Task<ActionResult<Job>> CreateJob([FromBody] JobCreateRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra NaN
            if (float.IsNaN(dto.DescriptionWeight) || float.IsNaN(dto.SkillsWeight) ||
                float.IsNaN(dto.ExperienceWeight) || float.IsNaN(dto.EducationWeight))
                return BadRequest("Trọng số không được là NaN.");

            // Kiểm tra các ràng buộc khác
            if (!dto.IsSalaryNegotiable && (!dto.MinSalary.HasValue || !dto.MaxSalary.HasValue))
                return BadRequest("Minimum and maximum salary must be entered if 'negotiable salary' is not selected.");
            if (dto.TimeEnd <= dto.TimeStart)
                return BadRequest("TimeEnd must be after TimeStart.");
            if (dto.ExpiryDate <= DateTime.UtcNow)
                return BadRequest("ExpiryDate must be in the future.");

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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
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

            // Thêm hoặc tạo mới Skill cho Job
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



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobUpdateRequest dto)
        {
            // Kiểm tra các ràng buộc khác
            if (!dto.IsSalaryNegotiable && (!dto.MinSalary.HasValue || !dto.MaxSalary.HasValue))
                return BadRequest("Minimum and maximum salary must be entered if 'negotiable salary' is not selected.");
            if (dto.TimeEnd <= dto.TimeStart)
                return BadRequest("TimeEnd must be after TimeStart.");
            if (dto.ExpiryDate <= DateTime.UtcNow)
                return BadRequest("ExpiryDate must be in the future.");

            var job = await _context.Jobs
                .Include(j => j.JobSkills)
                .FirstOrDefaultAsync(j => j.JobId == id);
            if (job == null) return NotFound();

            var userIdStr = User.Identity?.Name;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            var role = User.FindFirst(ClaimTypes.Role)?.Value.ToLower();

            if (role == "company")
            {
                if (job.CompanyId != userId)
                    return StatusCode(403, "Bạn không phải chủ sở hữu job này.");

                // Không cho phép edit nếu bị admin lock hoặc hết hạn
                if (!job.CanCompanyEditContent())
                    return StatusCode(403, "Job đã bị admin khóa hoặc đã hết hạn, bạn không có quyền chỉnh sửa.");

                // Nếu job đang pending, chỉ cho phép chỉnh nội dung, KHÔNG cập nhật status
                if (job.Status == Job.JobStatus.pending)
                {
                    // Không cập nhật status, giữ nguyên trạng thái pending
                }
                // Nếu job inactive (không bị admin lock, chưa hết hạn), cho phép chỉnh sửa và set lại status về pending
                else if (job.Status == Job.JobStatus.inactive && !job.IsExpired())
                {
                    job.Status = Job.JobStatus.pending;
                }
                // Nếu job active (đã được admin duyệt), cho phép chỉnh sửa và set lại status về pending
                else if (job.Status == Job.JobStatus.active && !job.IsExpired())
                {
                    job.Status = Job.JobStatus.pending;
                }
            }
            else if (role == "admin")
            {
                // Admin có thể chỉnh sửa mọi thứ
            }
            else
            {
                return StatusCode(403, "Bạn không có quyền chỉnh sửa job này.");
            }

            // Cập nhật nội dung
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
            job.UpdatedAt = DateTime.UtcNow;
            job.IsSalaryNegotiable = dto.IsSalaryNegotiable;
            job.MinSalary = dto.IsSalaryNegotiable ? null : dto.MinSalary;
            job.MaxSalary = dto.IsSalaryNegotiable ? null : dto.MaxSalary;
     
            //// Cập nhật lại JobSkill
            //if (dto.skillInputs != null)
            //{
            //    var oldSkills = job.JobSkills.ToList();
            //    _context.JobSkills.RemoveRange(oldSkills);

            //    foreach (var input in dto.skillInputs)
            //    {
            //        int skillId;
            //        if (input.SkillId.HasValue)
            //        {
            //            skillId = input.SkillId.Value;
            //        }
            //        else if (!string.IsNullOrWhiteSpace(input.SkillName))
            //        {
            //            var existingSkill = await _context.Skills
            //                .FirstOrDefaultAsync(s => s.SkillName.ToLower() == input.SkillName.ToLower());
            //            if (existingSkill != null)
            //            {
            //                skillId = existingSkill.SkillId;
            //            }
            //            else
            //            {
            //                var newSkill = new Skill { SkillName = input.SkillName };
            //                _context.Skills.Add(newSkill);
            //                await _context.SaveChangesAsync();
            //                skillId = newSkill.SkillId;
            //            }
            //        }
            //        else
            //        {
            //            continue;
            //        }

            //        _context.JobSkills.Add(new JobSkill { JobId = job.JobId, SkillId = skillId });
            //    }
            //    await _context.SaveChangesAsync();
            //}

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            if (job.Status == Job.JobStatus.active)
                return BadRequest("Cannot delete a job that is already active.");

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return NoContent();
        }

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
            return jobs;
        }
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateJobStatus(int id, [FromQuery] Job.JobStatus newStatus)
        {
            await AutoDeactivateExpiredJobs();

            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound("Job not found.");

           
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");


            var role = User.FindFirst(ClaimTypes.Role)?.Value.ToLower();

            if (job.TimeEnd < DateTime.UtcNow)
                return BadRequest("Cannot change status of expired job.");


            if (role == "admin")
            {
                if (job.Status == newStatus)
                    return BadRequest("Job already in specified status.");
                // Chỉ gửi mail khi duyệt từ pending sang active
                bool shouldSendMail = job.Status == Job.JobStatus.pending && newStatus == Job.JobStatus.active;

                job.Status = newStatus;
                job.UpdatedAt = DateTime.UtcNow;

                // Nếu admin inactive job thì set flag DeactivatedByAdmin = true, ngược lại false
                if (newStatus == Job.JobStatus.inactive)
                    job.DeactivatedByAdmin = true;
                else
                    job.DeactivatedByAdmin = false;

                await _context.SaveChangesAsync();

                if (shouldSendMail)
                {
                    // Lấy danh sách user đã yêu thích công ty này
                    var favoriteUsers = await _context.UserFavoriteCompanies
                        .Where(f => f.CompanyId == job.CompanyId)
                        .Select(f => f.User)
                        .ToListAsync();

                    // Lấy thông tin company profile
                    var companyProfile = await _context.CompanyProfile
                        .FirstOrDefaultAsync(c => c.UserId == job.CompanyId);

                    string companyName = companyProfile?.CompanyName ?? "Công ty";
                    string jobUrl = $"http://localhost:3000/job-single-v3/{job.JobId}"; // Thay bằng domain thật

                    string mailBody = $@"
                        <div style='font-family: Arial, sans-serif;'>
                            <h2 style='color:#2d8cf0;'>Công ty {companyName} vừa đăng việc mới!</h2>
                            <p><b>Title Job:</b> {job.Title}</p>
                            <p><b>Địa điểm:</b> {job.ProvinceName}</p>
                            <p><b>Hạn nộp:</b> {job.ExpiryDate:dd/MM/yyyy}</p>
                            <p><b>Mô tả:</b> {job.Description}</p>
                            <div style='margin:20px 0;'>
                                <a href='{jobUrl}' style='background:#2d8cf0;color:#fff;padding:10px 20px;border-radius:4px;text-decoration:none;font-weight:bold;'>Xem chi tiết & Ứng tuyển</a>
                            </div>
                        </div>
                    ";

                    foreach (var user in favoriteUsers)
                    {
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            _emailService.SendEmail(
                                user.Email,
                                $"[{companyName}] vừa đăng việc mới: {job.Title}",
                                mailBody,
                                true
                            );
                        }
                    }
                }

                return Ok($"Admin updated job #{id} status to {newStatus}.");
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
                    job.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return Ok("Company deactivated the job successfully.");
                }

                if (job.Status == Job.JobStatus.inactive && newStatus == Job.JobStatus.active)
                {
                    if (job.DeactivatedByAdmin)
                        return Forbid("Job was deactivated by admin. Company cannot reactivate it.");

                    job.Status = Job.JobStatus.active;
                    job.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return Ok("Company reactivated the job successfully.");
                }

                return BadRequest("Company can only deactivate an active job or activate an inactive job.");
            }

            return Forbid("You do not have permission to update job status.");
        }



        private async Task AutoDeactivateExpiredJobs()
        {
            var now = DateTime.UtcNow;
            var expiredJobs = await _context.Jobs
                .Where(j => j.Status == Job.JobStatus.active && j.TimeEnd < now)
                .ToListAsync();

            foreach (var job in expiredJobs)
            {
                job.Status = Job.JobStatus.inactive;
                job.UpdatedAt = now;
            }

            if (expiredJobs.Count > 0)
                await _context.SaveChangesAsync();
        }

        [HttpPut("{id}/lock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LockJob(int id, [FromQuery] bool isLock)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound("Job not found.");

            job.DeactivatedByAdmin = isLock;

            // Nếu lock thì chuyển trạng thái về inactive nếu đang active
            if (isLock && job.Status == Job.JobStatus.active)
                job.Status = Job.JobStatus.inactive;

            // Nếu unlock và job chưa hết hạn, cho phép admin active lại nếu muốn
            if (!isLock && !job.IsExpired())
            {
                // Không tự động chuyển trạng thái, chỉ unlock
                // Admin có thể dùng API đổi status nếu muốn
            }

            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(isLock ? "Job đã bị admin khóa." : "Job đã được admin mở khóa.");
        }

    }
}