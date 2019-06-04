using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using MooDL.Models.Resources;

namespace MooDL.Models.Web
{
    internal class Downloader : DownloaderBase
    {
        private int progress;

        public event EventHandler OnFinished;
        public event EventHandler OnLoginFailed;
        public event EventHandler OnConnectionFailed;
        public event EventHandler<int> OnProgress;
        public event EventHandler<int> OnResourcesFound;

        public async Task DownloadFiles(string courseId, string username, SecureString password, string basePath,
            string folder, bool sort, bool overwrite)
        {
            if (!await Login(username, password))
            {
                OnConnectionFailed.Invoke(this, null);
                OnFinished.Invoke(this, null);
                return;

            }
            string html = await DownloadPageSource("https://moodle.bbbaden.ch/course/view.php?id=" + courseId);
            if (GotLoginPage(html))
            {
                OnLoginFailed.Invoke(this, null);
                OnFinished.Invoke(this, null);
                return;
            }
            Resource[] resources = GetResources(html);
            OnResourcesFound.Invoke(this, resources.Length);
            if (resources.Length <= 0)
            {
                OnFinished.Invoke(this, null);
                return;
            }

            await DownloadResources(resources, basePath, folder, sort, overwrite);
        }

        private async Task<bool> Login(string username, SecureString password)
        {
            Dictionary<string, string> details = new Dictionary<string, string>()
            {
                {"username", username},
                {
                    "password",
                    Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(password ?? new SecureString()))
                }
            };

            try
            {
                await new HttpClient(clientHandler).PostAsync("https://moodle.bbbaden.ch/login/index.php", new FormUrlEncodedContent(details));
                return true;
            }
            catch (WebException)
            {
                OnConnectionFailed.Invoke(this, null);
                return false;
            }
        }


        protected override async Task<string> DownloadPageSource(string url)
        {
            try
            {
                return await base.DownloadPageSource(url);
            }
            catch (WebException)
            {
                OnConnectionFailed.Invoke(this, null);
                return "";
            }
        }

        private bool GotLoginPage(string html) => html.Contains("page-login-index");

        private async Task DownloadResources(Resource[] resources, string basePath, string folder, bool sort, bool overwrite)
        {
            string path = $"{basePath}{folder}\\";
            List<Task> tasks = new List<Task>();

            foreach (Resource r in resources)
            {
                string subFolder = sort ? $"{r.Type}\\" : "";

                if (r is Folder f)
                {
                    FolderDownloader fdl = new FolderDownloader(clientHandler, f);
                    tasks.Add(fdl.DownloadFiles($"{path}{subFolder}{r.Name}", overwrite));
                }
                else
                {
                    tasks.Add(Download(r.Url, r.Name + r.Extension).ContinueWith(async x =>
                    {
                        byte[] bytes = x.Result;
                        if (bytes != null)
                        {
                           await Write($"{path}{subFolder}{r.Name}{r.Extension}", bytes, overwrite);
                        }
                    }));
                }
            }

            while (tasks.Count > 0)
            {
                tasks.Remove(await Task.WhenAny(tasks));
                progress++;
                OnProgress.Invoke(this, progress);
            }
            

            OnFinished.Invoke(this, null);
        }
    }
}