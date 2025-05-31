namespace JOB_FINDER_API.Models.Requests
{
    public class UpdateUserFullRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? Image { get; set; }
        public int? RoleId { get; set; }
    }

}
