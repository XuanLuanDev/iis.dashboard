using IIS.Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IIS.Dashboard.Pages
{
    public class AccountModel : PageModel
    {
        public List<User> Accounts { get; set; } = new List<User>();
        public void OnGet()
        {
        }
        public void OnPostRefresh()
        {
           
        }
        public void OnPostAdd()
        {

        }
    }
}
