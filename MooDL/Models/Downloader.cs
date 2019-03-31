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
    class Downloader
    {
        private readonly CookieWebClient cwc;

        public bool Started { get; set; } = false;
        public bool Finished { get; set; } = false;
        public bool LoginSuccess { get; set; } = true;
        public bool ConnectionSuccess { get; set; } = true;
        public int Progress { get; set; } = 0;
        public int ToDownload { get; set; } = 1;

        public Downloader()
        {
            cwc = new CookieWebClient();
        }

        public async Task DownloadFiles(string courseId, string username, SecureString password, string basePath, string folder, bool sort, bool overwrite)
        {
            await Login(username, password);
            string html = await DownloadPageSource(courseId);
            AnalyseSource(html);
            await DownloadResources(GetResources(html), basePath, folder, sort, overwrite);
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

        private async Task<string> DownloadPageSource(string courseId)
        {
            Started = true;
            try
            {
                return Encoding.UTF8.GetString(await cwc.DownloadDataTaskAsync("https://moodle.bbbaden.ch/course/view.php?id=" + courseId));
            }
            catch (WebException)
            {
                ConnectionSuccess = false;
                return "";
            }
        }

        private void AnalyseSource(string html)
            => LoginSuccess = !html.Contains("page-login-index");

        private Resource[] GetResources(string html)
        {
            Regex resourceRegex = new Regex(Resource.ResourceRegex);
            MatchCollection resourceMatches = resourceRegex.Matches(html);
            Resource[] resources = new Resource[resourceMatches.Count];

            for (int i = 0; i < resourceMatches.Count; i++)
            {
                string url = resourceMatches[i].Groups[1].Value;
                string name = resourceMatches[i].Groups[3].Value;

                switch (resourceMatches[i].Groups[2].Value)
                {
                    case "document":
                        resources[i] = new Docx(url, name);
                        break;
                    case "powerpoint":
                        resources[i] = new Pptx(url, name);
                        break;
                    case "spreadsheet":
                        resources[i] = new Xlsx(url, name);
                        break;
                    case "archive":
                        resources[i] = new Zip(url, name);
                        break;
                    case "pdf":
                        resources[i] = new Pdf(url, name);
                        break;
                    case "unknown":
                        resources[i] = new Unknown(url, name);
                        break;
                }
            }

            ToDownload = resources.Length;
            return resources;
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

                await Write($"{path}{subFolder}{r.Name}{r.Extension}", await cwc.DownloadDataTaskAsync(r.Url), overwrite);
                Progress++;
            }
            Finished = true;
        }

        private async Task Write(string path, byte[] bytes, bool overwrite)
        {
            if (!overwrite && File.Exists(path))
                return;

            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                await fileStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
