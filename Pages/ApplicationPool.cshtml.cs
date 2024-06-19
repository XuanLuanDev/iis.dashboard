using IIS.Dashboard.Common;
using IIS.Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Web.Administration;
using System.Collections.Generic;

namespace IIS.Dashboard.Pages
{
    public class ApplicationPoolModel : PageModel
    {
        public List<AppPool> AppPools { get; set; } =new List<AppPool>();
        public bool IsEditMode { get; set; } = false;
        public AppPoolEdit CurrentAppPoolEdit { get; set; } =new AppPoolEdit();
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
            this.CurrentAppPoolEdit = new AppPoolEdit();
            this.CurrentAppPoolEdit.Enable32BitAppOnWin64 = true;
            this.CurrentAppPoolEdit.Mode = "Classic";
            this.CurrentAppPoolEdit.RuntimeVersion = "v4.0";
            IsEditMode = true;
        }
        public void OnPostSave(string name,bool enable32BitAppOnWin64,string mode,string runtimeVersion,bool isEdit)
        {
           if(isEdit == false)
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    ApplicationPool newPool = serverManager.ApplicationPools.Add(name);
                    newPool.ManagedRuntimeVersion = runtimeVersion;
                    newPool.Enable32BitAppOnWin64 = enable32BitAppOnWin64;
                    newPool.ManagedPipelineMode = mode == "Classic"? ManagedPipelineMode.Classic: ManagedPipelineMode.Integrated;
                    serverManager.CommitChanges();
                }
                IsEditMode = false;
                this.GetAllPools();
            }

        }
        public void OnPostEdit()
        {

        }
        public void OnPostDelete()
        {

        }
        public void OnPostCancel()
        {
            this.IsEditMode = false;
        }
    }
}
