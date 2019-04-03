using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MooDL.Models.Resources;

namespace MooDL.Models.Web
{
    internal abstract class DownloaderBase
    {
        protected CookieWebClient cwc;

        protected virtual async Task<string> DownloadPageSource(string url)
            => Encoding.UTF8.GetString(await Download(url));

        protected async Task Write(string path, byte[] bytes, bool overwrite)
        {
            if (!overwrite && File.Exists(path))
                return;

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                await fileStream.WriteAsync(bytes, 0, bytes.Length);
        }

        protected virtual Resource[] GetResources(string html)
        {
            Regex resourceRegex = new Regex(html.Contains("download_folder.php")
                ? Resource.FolderResourceRegex : Resource.ResourceRegex);
            Resource[] resources = resourceRegex.Matches(html).Cast<Match>().Select(
                m => CreateResource(m.Groups[2].Value, m.Groups[1].Value, m.Groups[3].Value)).ToArray();

            return resources;
        }

        protected Resource CreateResource(string type, string url, string name)
        {
            Resource r = null;
            switch (type)
            {
                case "document":
                    r = new Docx(url, name);
                    break;
                case "powerpoint":
                    r = new Pptx(url, name);
                    break;
                case "spreadsheet":
                    r = new Xlsx(url, name);
                    break;
                case "archive":
                    r = new Zip(url, name);
                    break;
                case "pdf":
                    r = new Pdf(url, name);
                    break;
                case "icon":
                    r = new Folder(url, name);
                    break;
                case "unknown":
                    r = new Unknown(url, name);
                    break;
            }

            return r;
        }

        protected async Task<byte[]> Download(string url)
        {
            try
            {
                return await cwc.DownloadDataTaskAsync(url);
            }
            catch (WebException)
            {
                return Encoding.ASCII.GetBytes("download failed");
            }
        }
    }
}