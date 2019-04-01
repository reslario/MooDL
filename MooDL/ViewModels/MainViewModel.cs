﻿using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using Caliburn.Micro;
using MooDL.Models;

namespace MooDL.ViewModels
{
    class MainViewModel : PropertyChangedBase
    {
        private string _basePath = "Folder containing your courses...";
        public string BasePath
        {
            get => _basePath;
            set
            {
                _basePath = value;
                NotifyOfPropertyChange(() => BasePath);
            }
        }

        public string CourseId { get; set; } = "Course ID";

        private string _error = "";
        public string Error
        {
            get => _error;
            set
            {
                _error = value;
                NotifyOfPropertyChange(() => Error);
            }
        }

        public string Username { get; set; } = "username";

        private SecureString _password;
        public SecureString Password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        private string _folder = "Course";
        public string Folder
        {
            get => _folder;
            set => _folder = Regex.Replace(value, @"(?:\/|\\|\?|%|\*|:|\||""|<|>|\.)", " ");
        }

        public bool Sort { get; set; } = true;

        public bool Overwrite { get; set; } = false;

        private bool pathSelected;

        private Brush _courseIdBorder;
        public Brush CourseIdBorder
        {
            get => _courseIdBorder;
            set
            {
                _courseIdBorder = value;
                NotifyOfPropertyChange(() => CourseIdBorder);
            }
        }

        private static readonly Brush eee = new BrushConverter().ConvertFromString("#EEEEEE") as SolidColorBrush;
        private Brush _basePathColor = eee;
        public Brush BasePathColor
        {
            get => _basePathColor;
            set
            {
                _basePathColor = value;
                NotifyOfPropertyChange(() => BasePathColor);
            }
        }

        private int _progress = 0;
        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                NotifyOfPropertyChange(() => Progress);
            }
        }

        private int _toDownload = 1;
        public int ToDownload
        {
            get => _toDownload;
            set
            {
                _toDownload = value;
                NotifyOfPropertyChange(() => ToDownload);
            }
        }

        public async Task Download()
        {
            if (CourseIdIsValid() && PathIsValid())
            {
                Downloader dl = new Downloader();
                Task downloadTask = dl.DownloadFiles(CourseId, Username, Password, BasePath, Folder, Sort, Overwrite);

                await Task.WhenAny(downloadTask, Task.Run(async () =>
                {
                    while (!downloadTask.IsCompleted)
                    {
                        ShowFeedback(dl);
                    }
                }));

                Progress = 0;
                ToDownload = 1;
            }
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

        private void ShowFeedback(Downloader dl)
        {
            Error = "";
            ToDownload = dl.ToDownload;
            Progress = dl.Progress;

            if (dl.LoginSuccess && dl.ConnectionSuccess)
            {
                Error = "";
            }
            else if (!dl.LoginSuccess)
            {
                Error = "Login Failed";
            }
            else if (!dl.ConnectionSuccess)
            {
                Error = "Connection failed";
            }
        }
    }
}
