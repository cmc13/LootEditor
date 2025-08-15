using LootEditor.Services;
using LootEditor.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace LootEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void DockPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (DataContext is MainViewModel vm)
                {
                    await vm.OpenRecentFileCommand.ExecuteAsync(files.FirstOrDefault()).ConfigureAwait(false);
                }
            }
        }

        private void DockPanel_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Handled = true;
                e.Effects = DragDropEffects.Copy;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var settings = WindowSettingsManager.Load();
            if (settings != null)
            {
                Width = settings.Width;
                Height = settings.Height;
                Top = settings.Top;
                Left = settings.Left;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            WindowSettingsManager.Save(new(Width, Height, Top, Left));
        }
    }
}
