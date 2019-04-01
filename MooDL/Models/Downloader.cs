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
        public bool Started { get;private set; } = false;
        public bool Finished { get; private set; } = false;
        public bool LoginSuccess { get; private set; } = true;
        public bool ConnectionSuccess { get; private set; } = true;
        public int ToDownload { get; private set; } = 1;
        public int Progress { get; private set; } = 0;

        public Downloader() => cwc = new CookieWebClient();

        public async Task DownloadFiles(string courseId, string username, SecureString password, string basePath, string folder, bool sort, bool overwrite)
        {
            await Login(username, password);
            var html = await DownloadPageSource("https://moodle.bbbaden.ch/course/view.php?id=" + courseId);
            AnalyseSource(html);
            var resources = GetResources(html);
            ToDownload = resources.Length;
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
            Started = true;
            try
            {
                return await base.DownloadPageSource(url);
            }
            catch (WebException)
            {
                ConnectionSuccess = false;
                return "";
            }
        }

        private void AnalyseSource(string html)
            => LoginSuccess = !html.Contains("page-login-index");

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
                    Progress++;
                    continue;
                }
                await Write($"{path}{subFolder}{r.Name}{r.Extension}", await cwc.DownloadDataTaskAsync(r.Url), overwrite);
                Progress++;
            }
            Finished = true;
        }
    }
}
