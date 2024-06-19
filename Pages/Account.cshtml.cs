using IIS.Dashboard.Common;
using IIS.Dashboard.Logic;
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
        public bool IsEditMode { get; set; } = false;
        public List<Role> Roles { get; set; } = new List<Role>();
        public AccountUserModel SelectedUser { get; set; } = new AccountUserModel();
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
            ViewData["Title"] = "Account Manager";
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
                      

                        select new AccountUserModel()
                        {
                            Email = u.Email,
                            Name = u.Name,
                            Guid = u.Guid,
                            Roles = (from ur in Context.UserRoles
                                     join r in Context.Roles on ur.RoleGuid equals r.Guid
                                     where ur.UserGuid == u.Guid
                                     select r
                        ).ToList()
                        }
                       ).ToList();
        }
        public void OnPostRefresh()
        {
            this.GetAllAccount();
        }
        public void OnPostAdd()
        {
            this.Roles =Context.Roles.ToList();
            this.SelectedUser = new AccountUserModel();
            this.IsEditMode = true;
        }
        public void OnPostSave(string name,string email,string password,string confirmPassword,List<Guid> role)
        {

            if(string.IsNullOrEmpty(name)|| string.IsNullOrEmpty(email)|| string.IsNullOrEmpty(password)||string.IsNullOrEmpty(confirmPassword)|| role.Count == 0 || password!= confirmPassword)
            {
                return;
            }
            byte[] saltBytes = AuthLogic.GenerateSalt();
            string hashedPassword = AuthLogic.HashPassword(password, saltBytes);
            string base64Salt = Convert.ToBase64String(saltBytes);
            var newAcc = new User()
            {
                Email = email,
                Guid = Guid.NewGuid(),
                Name = name,
                Password = hashedPassword,
                Salt = base64Salt
            };
            Context.Users.Add(newAcc);
            Context.SaveChanges();

            foreach (var item in role)
            {
                Context.UserRoles.Add(new UserRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleGuid = item,
                    UserGuid = newAcc.Guid
                });
                Context.SaveChanges();
            }

            this.IsEditMode = false;
            this.GetAllAccount();
        }
        public void OnPostCancel()
        {
            this.IsEditMode = false;
        }
    }
}
