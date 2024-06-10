using IIS.Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Web.Administration;
using System;

namespace IIS.Dashboard.Pages
{
    public class IndexModel : PageModel
    {
        public const string SessionKeyName = "IISUser";
        private readonly ILogger<IndexModel> _logger;
        public List<Website> Websites { get; set; } =new List<Website>();
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            this.GetAllSites();
           
        }
        public void GetAllSites()
        {
            using (ServerManager server = new ServerManager())
            {
                this.Websites = new List<Website>();
                foreach (var item in server.Sites)
                {
                    var web = new Website()
                    {
                        Name = item.Name,
                        State = item.State.ToString(),
                        Id = item.Id
                    };
                    
                    foreach (var b in item.Bindings)
                    {
                        web.Port =b.EndPoint.Port;
                        break;
                    }
                    this.Websites.Add(web);

                }
            }
        }
      
        public void OnPostStart(long site)
        {
            using (ServerManager server = new ServerManager())
            {
                foreach (var item in server.Sites)
                {

                   if(item.Id == site && (item.State == ObjectState.Stopped || item.State == ObjectState.Stopping))
                    {
                        item.Start();
                        break;
                    }

                }
            }
            this.GetAllSites();
        }
        public void OnPostStop(long site)
        {
            using (ServerManager server = new ServerManager())
            {
                foreach (var item in server.Sites)
                {

                    if (item.Id == site && (item.State == ObjectState.Started || item.State == ObjectState.Starting))
                    {
                        item.Stop();
                        break;
                    }

                }
            }
            this.GetAllSites();
        }
        public void OnPostRefresh()
        {
            this.GetAllSites();
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
        public IActionResult OnGet()
        {
            if (!Auth())
            {
                return new RedirectToPageResult("/login");
            }
            return Page();

        }
        public bool Auth()
        {
            string ss = HttpContext.Session.GetString(SessionKeyName);
            return   !string.IsNullOrEmpty(ss);
        }
    }
}