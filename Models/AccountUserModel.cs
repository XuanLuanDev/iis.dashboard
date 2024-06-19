namespace IIS.Dashboard.Models
{
    public class AccountUserModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Role> Roles { get; set; } =new List<Role>();
    }
}
