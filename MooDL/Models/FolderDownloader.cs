using MooDL.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooDL.Models
{
    class FolderDownloader : DownloaderBase
    {
        private readonly Folder folder;

        public FolderDownloader(CookieWebClient cwc, Folder folder)
        {
            this.cwc = cwc;
            this.folder = folder;
        }

        public async Task DownloadFiles(string path, bool overwrite)
        {
            var resources = GetResources(await DownloadPageSource(folder.Url));
            foreach (var r in resources)
            {
                byte[] data = await Download(r.Url);
                await Write($"{path}\\{r.Name}{r.Extension}", data, overwrite);
            }
        }
    }
}
