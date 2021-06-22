using LootEditor.ViewModels;
using System.Windows.Controls;

namespace LootEditor
{
    /// <summary>
    /// Interaction logic for TemplateEditorView.xaml
    /// </summary>
    public partial class TemplateEditor : UserControl
    {
        public TemplateEditor()
        {
            InitializeComponent();
        }

        private void TextBox_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox t)
            {
                if (t.Visibility == System.Windows.Visibility.Visible)
                {
                    t.Focus();
                    t.SelectAll();
                }
            }
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (sender is TextBox t && DataContext is TemplateEditorViewModel vm)
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                    vm.SelectedTemplate.IsEditingTemplateName = false;
                else if (e.Key == System.Windows.Input.Key.Escape)
                {
                    t.Text = vm.SelectedTemplate.Name;
                    vm.SelectedTemplate.IsEditingTemplateName = false;
                }
            }
        }
    }
}
