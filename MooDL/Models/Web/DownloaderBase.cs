using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MooDL.Models.Resources;

namespace MooDL.Models.Web
{
    internal abstract class DownloaderBase
    {
        internal class FileConfirmationEventArgs
        {
            public FileConfirmationEventArgs(string filename, long size)
            {
                Filename = filename;
                Size = size;
            }

            public string Filename { get; }
            public long Size { get; }
            public bool ShouldDownload { get; set; } = false;
        }

        // The maximum filesize before the user is asked
        public long ConfirmationFilesize { get; set; } = 1024 * 1024 * 100; // 100 MB
        // Event to confirm file download
        public event Func<object, FileConfirmationEventArgs, Task> OnFileConfirmation;

        protected async Task<bool> RaiseFileConfirmation(string filename, long size)
        {
            FileConfirmationEventArgs args = new FileConfirmationEventArgs(filename, size);
            await OnFileConfirmation?.Invoke(this, args);
            return args.ShouldDownload;
        }

        protected HttpClientHandler clientHandler = new HttpClientHandler() {CookieContainer = new CookieContainer()};

        protected virtual Task<string> DownloadPageSource(string url)
        {
            HttpClient client = new HttpClient(clientHandler);
            return client.GetStringAsync(url);
        }

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

        protected async Task<byte[]> Download(string url, string filename)
        {
            try
            {
                using (HttpClient client = new HttpClient(clientHandler, false))
                {
                    HttpResponseMessage responseMessage = await client.GetAsync(url);
                    long size = long.Parse(responseMessage.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());
                    if (size < ConfirmationFilesize || await RaiseFileConfirmation(filename, size))
                    {
                        return await responseMessage.Content.ReadAsByteArrayAsync();
                    }

                    return null;
                }
            }
            catch (WebException)
            {
                return Encoding.ASCII.GetBytes("download failed");
            }
        }

        public string BytesToHumanReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            return readable.ToString("0.### ") + suffix;
        }
    }
}