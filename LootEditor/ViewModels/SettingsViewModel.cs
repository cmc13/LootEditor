using CommunityToolkit.Mvvm.ComponentModel;

namespace LootEditor.ViewModels;

public class SettingsViewModel : ObservableRecipient
{
    private readonly TemplateEditorViewModel templateEditorViewModel = new();

    public TemplateEditorViewModel TemplateEditorViewModel => templateEditorViewModel;

    public string Name { get; } = "Settings";
}
