using JOB_FINDER_API.Models;

public class ProfileStrengthResult
{
    public int Percentage { get; set; }
    public List<string> MissingFields { get; set; } = new();
}

public class ProfileStrengthService
{
    public ProfileStrengthResult Calculate(CandidateProfile profile)
    {
        int totalSections = 11;
        int filled = 0;
        var missing = new List<string>();

        // Họ tên
        if (profile.User != null && !string.IsNullOrWhiteSpace(profile.User.FullName)) filled++;
        else missing.Add("Họ tên");

        // Email
        if (profile.User != null && !string.IsNullOrWhiteSpace(profile.User.Email)) filled++;
        else missing.Add("Email");

        // Số điện thoại
        if (profile.User != null && !string.IsNullOrWhiteSpace(profile.User.Phone)) filled++;
        else missing.Add("Phone");

        // Kỹ năng
        if (profile.Skills != null && profile.Skills.Any()) filled++;
        else missing.Add("Skills");

        // Giới thiệu bản thân
        if (!string.IsNullOrWhiteSpace(profile.AboutMeDescription)) filled++;
        else missing.Add("About Me");

        // Học vấn
        if (profile.Educations != null && profile.Educations.Any()) filled++;
        else missing.Add("Educations");

        // Kinh nghiệm làm việc
        if (profile.WorkExperiences != null && profile.WorkExperiences.Any()) filled++;
        else missing.Add("Work Experiences");

        // Dự án nổi bật
        if (profile.HighlightProjects != null && profile.HighlightProjects.Any()) filled++;
        else missing.Add("Highlight Projects");

        // Chứng chỉ
        if (profile.Certificates != null && profile.Certificates.Any()) filled++;
        else missing.Add("Certificates");

        // Giải thưởng
        if (profile.Awards != null && profile.Awards.Any()) filled++;
        else missing.Add("Awards");

        // Ngoại ngữ
        if (profile.ForeignLanguages != null && profile.ForeignLanguages.Any()) filled++;
        else missing.Add("Foreign Languages");

        // Tính phần trăm cơ bản
        int percent = (int)((double)filled / totalSections * 100);

        // Trừ % nếu thiếu các trường quan trọng (KHÔNG thêm vào gợi ý)
        int deduction = 0;
        if (string.IsNullOrWhiteSpace(profile.Address))
            deduction += 5;
        if (!profile.Dob.HasValue)
            deduction += 5;
        if (string.IsNullOrWhiteSpace(profile.PersonalLink))
            deduction += 5;
        if (string.IsNullOrWhiteSpace(profile.Gender))
            deduction += 5;

        percent -= deduction;
        if (percent < 0) percent = 0;
        if (percent > 100) percent = 100;

        return new ProfileStrengthResult
        {
            Percentage = percent,
            MissingFields = missing.Distinct().ToList()
        };
    }
}