using GalaSoft.MvvmLight.Threading;
using LootEditor.View.ViewModel;
using System.IO;
using System.Windows;

namespace LootEditor.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherHelper.Initialize();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 1)
            {
                if (File.Exists(e.Args[0]))
                {
                    var vm = Resources["MainViewModel"] as MainViewModel;
                    await vm.OpenFileAsync(e.Args[0]).ConfigureAwait(false);
                }
            }
        }
    }
}
