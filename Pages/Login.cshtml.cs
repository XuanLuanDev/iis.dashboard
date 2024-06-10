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
        public const string SessionKeyName = "IISUser";
        public LoginModel(AppDbContext dbContext  ) {
            this.Context = dbContext;
        }
        public string SummaryError { get; set; } = "";
        public void OnGet()
        {
        }
        public IActionResult OnPostLogin(string email,string password)
        {
            this.SummaryError = "";
            try
            {
             var user =   AuthLogic.Login(email, password, Context);
              HttpContext.Session.SetString(SessionKeyName, System.Text.Json.JsonSerializer.Serialize(user));
              return   new RedirectToPageResult("/index");
            }
            catch (Exception ex)
            {
                this.SummaryError = ex.Message;
                return Page();
            }
        }
    }
}
