using System;
using System.Windows;

namespace LootEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for YesNoAllDialog.xaml
    /// </summary>
    public partial class EnumDialog : Window
    {
        public EnumDialog()
        {
            InitializeComponent();

            this.Loaded += EnumDialog_Loaded;
        }

        private void EnumDialog_Loaded(object sender, RoutedEventArgs e)
        {
            dynamic vm = DataContext;
            vm.CloseDialog = new Action(Close);
        }
    }
}
