using MooDL.Models.Resources;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MooDL.Models
{
    class Downloader : DownloaderBase
    {
        public event EventHandler OnFinished;
        public event EventHandler OnLoginFailed;
        public event EventHandler OnConnectionFailed;
        public event EventHandler<int> OnProgress;
        public event EventHandler<int> OnResourcesFound;

        private int progress = 0;

        public Downloader() => cwc = new CookieWebClient();

        public async Task DownloadFiles(string courseId, string username, SecureString password, string basePath, string folder, bool sort, bool overwrite)
        {
            await Login(username, password);
            var html = await DownloadPageSource("https://moodle.bbbaden.ch/course/view.php?id=" + courseId);
            AnalyseSource(html);
            var resources = GetResources(html);
            OnResourcesFound.Invoke(this, resources.Length);
            await DownloadResources(resources, basePath, folder, sort, overwrite);
        }

        private async Task Login(string username, SecureString password)
        {
            NameValueCollection details = new NameValueCollection
            {
                { "username", username },
                { "password", Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(password ?? new SecureString())) },
            };

            await cwc.UploadValuesTaskAsync(new Uri("https://moodle.bbbaden.ch/login/index.php"), details);
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

        private void AnalyseSource(string html)
        {
            if (html.Contains("page-login-index"))
                OnLoginFailed.Invoke(this, null);
        }

        private async Task DownloadResources(Resource[] resources, string basePath, string folder, bool sort, bool overwrite)
        {
            string path = $"{basePath}{folder}\\";

            Directory.CreateDirectory(path);

            if (sort)
                foreach (string s in resources.Select(r => r.Type).Distinct())
                    Directory.CreateDirectory(path + s);


            foreach (Resource r in resources)
            {
                string subFolder = sort ? $"{r.Type}\\" : "";

                if (r is Folder f)
                {
                    FolderDownloader fdl = new FolderDownloader(cwc, f);
                    await fdl.DownloadFiles($"{path}{subFolder}{r.Name}", overwrite);
                }
                else
                {
                    await Write($"{path}{subFolder}{r.Name}{r.Extension}", await Download(r.Url), overwrite);
                }

                progress++;
                OnProgress.Invoke(this, progress);
            }

            OnFinished.Invoke(this, null);
        }
    }
}
