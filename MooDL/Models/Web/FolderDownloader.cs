using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MooDL.Models.Resources;

namespace MooDL.Models.Web
{
    internal class FolderDownloader : DownloaderBase
    {
        private readonly Folder folder;

        public FolderDownloader(HttpClientHandler handler, Folder folder)
        {
            clientHandler = handler;
            this.folder = folder;
        }

        public async Task DownloadFiles(string path, bool overwrite)
        {
            foreach (Resource r in GetResources(await DownloadPageSource(folder.Url)))
            {
                byte[] bytes = await Download(r.Url, r.Name + r.Extension);
                if (bytes != null)
                {
                    await Write($"{path}\\{r.Name}{r.Extension}", bytes, overwrite);
                }
            }
        }
    }
}