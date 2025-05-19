using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        [JsonIgnore] 
        public ICollection<User> Users { get; set; } = new List<User>();
    }

}
