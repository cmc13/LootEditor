using System.Windows;

namespace LootEditor.View.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogBase.xaml
    /// </summary>
    public partial class DialogBase : Window
    {
        public DialogBase()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
