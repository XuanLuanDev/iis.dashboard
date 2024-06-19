using IIS.Dashboard.Common;
using IIS.Dashboard.Logic;
using IIS.Dashboard.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Web.Administration;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace IIS.Dashboard.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext Context;
        public LoginModel(AppDbContext dbContext  ) {
            this.Context = dbContext;
        }
        public string SummaryError { get; set; } = "";
        public void OnGet()
        {
            ViewData["Title"] = "Login";
        }
        public IActionResult OnPostLogin(string email,string password)
        {
            this.SummaryError = "";
            try
            {
             var user =   AuthLogic.Login(email, password, Context);
              HttpContext.Session.SetString(StringConst.SessionKeyName, System.Text.Json.JsonSerializer.Serialize(user));
                var roles = (from u in Context.Users
                            join ur in Context.UserRoles on u.Guid equals ur.UserGuid
                            join r in Context.Roles on ur.RoleGuid equals r.Guid
                            where u.Guid == user.Guid
                            select r.Name
                           ).ToArray();
                string roleList = roles != null && roles.Length > 0 ? String.Join(",", roles) : "";
                HttpContext.Session.SetString(StringConst.SessionKeyRole, roleList);
                HttpContext.Session.SetString(StringConst.LoginUserNameKey, user.Name);
                return   new RedirectToPageResult("/home");
            }
            catch (Exception ex)
            {
                this.SummaryError = ex.Message;
                return Page();
            }
        }
    }
}
