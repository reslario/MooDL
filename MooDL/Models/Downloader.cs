using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MooDL.Models
{
    class Downloader
    {
        private readonly CookieWebClient cwc;

        public bool Started { get; set; } = false;
        public bool Finished { get; set; } = false;
        public bool LoginSuccess { get; set; } = false;
        public bool ConnectionSuccess { get; set; } = true;
        public int Progress { get; set; } = 0;
        public int ToDownload { get; set; } = 1;

        public Downloader()
        {
            cwc = new CookieWebClient();
        }

        public async void DownloadFiles(string courseId, string username, string password, string basePath, string folder, bool sort, bool overwrite)
        {
            Login(username, password);
            await Task.Run(() => DownloadResources(GetResources(DownloadPageSource(courseId)), basePath, folder, sort, overwrite));
        }

        private void Login(string username, string password)
        {
            NameValueCollection details = new NameValueCollection
            {
                { "username", username },
                { "password", password },
            };

            cwc.UploadValues("https://moodle.bbbaden.ch/login/index.php", details);
        }

        private string DownloadPageSource(string courseId)
        {
            string html = "";
            try
            {
                html = cwc.DownloadString("https://moodle.bbbaden.ch/course/view.php?id=" + courseId);
            }
            catch (WebException)
            {
                ConnectionSuccess = false;
            }

            LoginSuccess = !html.Contains("Login");
            Started = true;
            return html;
        }

        private Resource[] GetResources(string html)
        {
            Regex resourceRegex = new Regex(@"<div class=""activityinstance""><a class="""" onclick="""" href=""([^""]+)""><img src=""https:\/\/moodle\.bbbaden\.ch\/theme\/image\.php\/_s\/lambda\/core\/1547110846\/f\/(unknown|document|spreadsheet|powerpoint|archive|pdf)[^<]*[^>]*>([^<]*)");
            MatchCollection resourceMatches = resourceRegex.Matches(html);
            Resource[] resources = new Resource[resourceMatches.Count];

            for (int i = 0; i < resourceMatches.Count; i++)
            {
                switch (resourceMatches[i].Groups[2].Value)
                {
                    case "document":
                        resources[i] = new Docx(resourceMatches[i].Groups[1].Value, resourceMatches[i].Groups[3].Value);
                        break;
                    case "powerpoint":
                        resources[i] = new Pptx(resourceMatches[i].Groups[1].Value, resourceMatches[i].Groups[3].Value);
                        break;
                    case "spreadsheet":
                        resources[i] = new Xlsx(resourceMatches[i].Groups[1].Value, resourceMatches[i].Groups[3].Value);
                        break;
                    case "archive":
                        resources[i] = new Zip(resourceMatches[i].Groups[1].Value, resourceMatches[i].Groups[3].Value);
                        break;
                    case "pdf":
                        resources[i] = new Pdf(resourceMatches[i].Groups[1].Value, resourceMatches[i].Groups[3].Value);
                        break;
                    case "unknown":
                        resources[i] = new Unknown(resourceMatches[i].Groups[1].Value, resourceMatches[i].Groups[3].Value);
                        break;
                }
            }

            ToDownload = resources.Length;
            return resources;
        }

        private async void DownloadResources(Resource[] resources, string basePath, string folder, bool sort, bool overwrite)
        {
            string path = $"{basePath}{folder}\\";
            string subFolder = "";

            Directory.CreateDirectory(path);

            if (sort)
                Array.ForEach(resources.Select(r => r.Type).Distinct().ToArray(), t => Directory.CreateDirectory(path + t));

            await Task.Run(() =>
            {
                foreach (Resource r in resources)
                {
                    subFolder = sort ? $"{r.Type}\\" : "";

                    if (!overwrite && File.Exists($"{path}{subFolder}{r.Name}{r.Extension}"))
                        continue;

                    File.WriteAllBytes($"{path}{subFolder}{r.Name}{r.Extension}", cwc.DownloadData(r.Url));
                    Progress++;
                }
                Finished = true;
            });
        }
    }
}
