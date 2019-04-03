using System.Threading.Tasks;
using MooDL.Models.Resources;

namespace MooDL.Models.Web
{
    internal class FolderDownloader : DownloaderBase
    {
        private readonly Folder folder;

        public FolderDownloader(CookieWebClient cwc, Folder folder)
        {
            this.cwc = cwc;
            this.folder = folder;
        }

        public async Task DownloadFiles(string path, bool overwrite)
        {
            foreach (Resource r in GetResources(await DownloadPageSource(folder.Url)))
                await Write($"{path}\\{r.Name}{r.Extension}", await Download(r.Url), overwrite);
        }
    }
}