using IIS.Dashboard.Common;
using IIS.Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Web.Administration;

namespace IIS.Dashboard.Pages
{
    public class ApplicationPoolModel : PageModel
    {
        public List<AppPool> AppPools { get; set; } =new List<AppPool>();
        public ApplicationPoolModel()
        {
            AppPools = new List<AppPool>();
        }
        public void GetAllPools()
        {
            using (ServerManager server = new ServerManager())
            {
               this.AppPools = new List<AppPool>();
                foreach (var item in server.ApplicationPools)
                {
                    this.AppPools.Add(new AppPool()
                    {
                        Name = item.Name,
                         State =item.State.ToString()
                    });
                
                }
            }
        }
        public IActionResult OnGet()
        {
            ViewData["Title"] = "Application Pool";
            if (!Auth())
            {
                return new RedirectToPageResult("/login");
            }
            this.GetAllPools();
            return Page();

        }
        public bool Auth()
        {
            string ss = HttpContext.Session.GetString(StringConst.SessionKeyName);
            return !string.IsNullOrEmpty(ss);
        }
        public void OnPostStop(string pool)
        {
            using (ServerManager server = new ServerManager())
            {
                foreach (var item in server.ApplicationPools)
                {
                    if(item.Name == pool)
                    {
                        item.Stop();
                    }

                }
            }
            this.GetAllPools();
        }
        public void OnPostStart(string pool)
        {
            using (ServerManager server = new ServerManager())
            {
                foreach (var item in server.ApplicationPools)
                {
                    if (item.Name == pool)
                    {
                        item.Start();
                    }

                }
            }
            this.GetAllPools();
        }
        public void OnPostRefresh()
        {
            this.GetAllPools();
        }
        public void OnPostAdd()
        {

        }
        public void OnPostEdit()
        {

        }
        public void OnPostDelete()
        {

        }
    }
}
