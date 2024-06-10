namespace IIS.Dashboard.Models
{
    public class AccountUserModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid RoleGuid { get; set; }
        public string RoleName { get; set; }
    }
}
