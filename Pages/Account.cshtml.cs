using IIS.Dashboard.Common;
using IIS.Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Nodes;

namespace IIS.Dashboard.Pages
{
    public class AccountModel : PageModel
    {
        public List<AccountUserModel> Accounts { get; set; } = new List<AccountUserModel>();
        private User CurrentUser { get; set; }
        private List<Role> CurrentRoles { get; set; } = new List<Role>();
        private readonly AppDbContext Context;
        public AccountModel(AppDbContext dbContext)
        {
            this.Context = dbContext;
            Accounts = new List<AccountUserModel>();
           
        }
        public bool Auth()
        {
            string ss = HttpContext.Session.GetString(StringConst.SessionKeyName);
            return !string.IsNullOrEmpty(ss);
        }
        public IActionResult OnGet()
        {
            if (!Auth())
            {
                return new RedirectToPageResult("/login");
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(StringConst.SessionKeyName)))
            {
                CurrentUser = System.Text.Json.JsonSerializer.Deserialize<User>(HttpContext.Session.GetString(StringConst.SessionKeyName));

            }
            if (CurrentUser != null)
            {
                CurrentRoles = (from u in Context.Users
                            join ur in Context.UserRoles on u.Guid equals ur.UserGuid
                            join r in Context.Roles on ur.RoleGuid equals r.Guid
                            where u.Guid == CurrentUser.Guid
                            select r
                           ).ToList();
                if (CurrentRoles != null && CurrentRoles.Count > 0 && CurrentRoles.Any(r=>r.Name == "AccountManager" || r.Name == "SupperUser"))
                {
                    this.GetAllAccount();
                    return Page();
                }
            }
            return new RedirectToPageResult("/home");
        }
        public void GetAllAccount()
        {

            Accounts = (from u in Context.Users
                        join ur in Context.UserRoles on u.Guid equals ur.UserGuid
                        join r in Context.Roles on ur.RoleGuid equals r.Guid

                        select new AccountUserModel()
                        {
                            Email = u.Email,
                            Name = u.Name,
                            Guid = u.Guid,
                            RoleGuid = r.Guid,
                            RoleName = r.Name
                        }
                       ).ToList();
        }
        public void OnPostRefresh()
        {
            this.GetAllAccount();
        }
        public void OnPostAdd()
        {

        }
    }
}
