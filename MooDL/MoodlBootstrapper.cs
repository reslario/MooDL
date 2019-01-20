using System.Windows;
using Caliburn.Micro;
using MooDL.ViewModels;

namespace MooDL
{
    public class MoodlBootstrapper : BootstrapperBase
    {
        public MoodlBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
