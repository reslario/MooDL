using System.Windows;
using System.Windows.Input;

namespace MooDL.Views   
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void grid_bar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GetWindow(sender as DependencyObject).DragMove();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            GetWindow(sender as DependencyObject).Close();
        }

        private void btn_minimise_Click(object sender, RoutedEventArgs e)
        {
            GetWindow(sender as DependencyObject).WindowState = WindowState.Minimized;
        }
    }
}
