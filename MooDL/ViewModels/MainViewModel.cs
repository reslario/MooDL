using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using MooDL.Models.Input;
using MooDL.Models.Web;

namespace MooDL.ViewModels
{
    internal class MainViewModel : PropertyChangeBase
    {
        private static readonly Brush eee = new BrushConverter().ConvertFromString("#EEEEEE") as SolidColorBrush;
        private Brush _basePathColor = eee;

        private bool pathSelected;

        private string _basePath = "Folder containing your courses...";
        public string BasePath
        {
            get => _basePath;
            set => SetProperty(ref _basePath, value);
        }

        private string _courseId = "Course ID";
        public string CourseId
        {
            get => _courseId;
            set => SetProperty(ref _courseId, value);
        }

        private string _error = "";
        public string Error
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }

        public string Username { get; set; } = "username";

        private SecureString _password;
        public SecureString Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _folder = "Course";
        public string Folder
        {
            get => _folder;
            set => _folder = Regex.Replace(value, @"(?:\/|\\|\?|%|\*|:|\||""|<|>|\.)", " ");
        }

        public bool Sort { get; set; } = true;

        public bool Overwrite { get; set; } = false;

        private Brush _courseIdBorder;
        public Brush CourseIdBorder
        {
            get => _courseIdBorder;
            set => SetProperty(ref _courseIdBorder, value);
        }

        public Brush BasePathColor
        {
            get => _basePathColor;
            set => SetProperty(ref _basePathColor, value);
        }

        private int _progress;
        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private int _toDownload = 1;
        public int ToDownload
        {
            get => _toDownload;
            set => SetProperty(ref _toDownload, value);
        }

        private ICommand _downloadCommand;
        public ICommand DownloadCommand
            => _downloadCommand ?? (_downloadCommand = new CommandHandler(async () => await Download()));

        private ICommand _pathSelectCommand;
        public ICommand PathSelectCommand
            => _pathSelectCommand ?? (_pathSelectCommand = new CommandHandler(PathSelect));

        public async Task Download()
        {
            if (CourseIdIsValid() && PathIsValid())
            {
                Error = "";
                Downloader dl = new Downloader();
                AddHandlers(dl);
                await dl.DownloadFiles(CourseId, Username, Password, BasePath, Folder, Sort, Overwrite);
            }
        }

        private void AddHandlers(Downloader dl)
        {
            dl.OnConnectionFailed += (s, args) => Error = "Connection failed";
            dl.OnLoginFailed += (s, args) => Error = "Login failed";
            dl.OnProgress += (s, progress) => Progress = progress;
            dl.OnResourcesFound += (s, amount) =>
            {
                Error = amount > 0 ? "" : "No resources found";
                ToDownload = amount;
            };
            dl.OnFinished += (s, args) =>
            {
                ToDownload = 1;
                Progress = 0;
            };
        }

        public void PathSelect()
        {
            CommonOpenFileDialog cofd = new CommonOpenFileDialog
            {
                InitialDirectory = @"C:\Users",
                IsFolderPicker = true
            };
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                pathSelected = true;
                BasePathColor = eee;
                BasePath = $"{cofd.FileName}\\";
            }
        }

        private bool CourseIdIsValid()
        {
            int id;

            if (int.TryParse(CourseId, out id) && id >= 0)
            {
                CourseIdBorder = null;
                return true;
            }


            CourseIdBorder = Brushes.Red;
            return false;
        }

        private bool PathIsValid()
        {
            if (pathSelected)
            {
                BasePathColor = eee;
                return true;
            }

            BasePathColor = Brushes.Red;
            return false;
        }
    }
}