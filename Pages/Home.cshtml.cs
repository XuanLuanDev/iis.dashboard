using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IIS.Dashboard.Pages
{
    public class HomeModel : PageModel
    {
        public HomeModel()
        {
        }
        public void OnGet()
        {
            ViewData["Title"] = "Home";
        }
    }
}
