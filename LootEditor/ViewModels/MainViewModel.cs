using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LootEditor.Dialogs;
using LootEditor.Models;
using LootEditor.Models.Enums;
using LootEditor.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace LootEditor.ViewModels;

public partial class MainViewModel
    : ObservableRecipient
{
    private static readonly string RECENT_FILE_NAME = Path.Combine(FileSystemService.AppDataDirectory, "RecentFiles.json");
    private readonly BackupService backupService = new();
    private const int RECENT_FILE_COUNT = 10;
    private string saveFileName = null;
    private LootFile lootFile = null;
    private readonly SettingsViewModel settingsViewModel = new();
    private readonly DialogService dialogService = new();
    private readonly FileSystemService fileSystemService = new();

    public string SaveFileName
    {
        get => Path.GetFileName(saveFileName);
        set
        {
            if (saveFileName != value)
            {
                saveFileName = value;
                OnPropertyChanged(nameof(SaveFileName));

                if (!string.IsNullOrEmpty(saveFileName))
                    Application.Current.Dispatcher.Invoke(() => AddRecentFile(saveFileName));
            }
        }
    }

    private LootFile LootFile
    {
        get => lootFile;
        set
        {
            if (lootFile != value)
            {
                lootFile = value;

                LootRuleListViewModel = new LootRuleListViewModel(LootFile);
                SalvageCombineListViewModel = new SalvageCombineListViewModel(LootFile);
            }
        }
    }

    [ObservableProperty]
    private LootRuleListViewModel lootRuleListViewModel = null;

    partial void OnLootRuleListViewModelChanging(LootRuleListViewModel oldValue, LootRuleListViewModel newValue)
    {
        if (oldValue != null)
            oldValue.PropertyChanged -= VM_PropertyChanged;
    }

    partial void OnLootRuleListViewModelChanged(LootRuleListViewModel oldValue, LootRuleListViewModel newValue)
    {
        newValue.PropertyChanged += VM_PropertyChanged;
        OnPropertyChanged(nameof(IsDirty));
    }

    [ObservableProperty]
    private SalvageCombineListViewModel salvageCombineListViewModel = null;

    partial void OnSalvageCombineListViewModelChanging(SalvageCombineListViewModel oldValue, SalvageCombineListViewModel newValue)
    {
        if (oldValue != null)
            oldValue.PropertyChanged -= VM_PropertyChanged;
    }

    partial void OnSalvageCombineListViewModelChanged(SalvageCombineListViewModel oldValue, SalvageCombineListViewModel newValue)
    {
        newValue.PropertyChanged += VM_PropertyChanged;
        OnPropertyChanged(nameof(IsDirty));
    }

    public SettingsViewModel SettingsViewModel => settingsViewModel;

    [ObservableProperty]
    private bool isBusy = false;

    [ObservableProperty]
    private string busyStatus;

    public ObservableCollection<string> RecentFiles { get; } = [];

    public bool IsDirty => LootRuleListViewModel.IsDirty || SalvageCombineListViewModel.IsDirty;

    public MainViewModel()
    {
        LootFile = new LootFile();

        if (backupService.BackupExists)
        {
            try
            {
                var mbResult = MessageBox.Show($"File {backupService.BackupFileName ?? "[New File]"} was not saved properly. Would you like to restore it?", "Restore Backup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mbResult == MessageBoxResult.Yes)
                {
                    Task.Run(async () =>
                    {
                        using var fs = backupService.OpenBackupFileForReadAccess();
                        using var reader = new StreamReader(fs);
                        await ReadLootFileAsync(reader).ConfigureAwait(false);
                        SaveFileName = backupService.BackupFileName;
                    });
                }
            }
            catch { }
            finally
            {
                backupService.DeleteBackupFile();
            }
        }
    }

    [RelayCommand]
    private void Load()
    {
        if (fileSystemService.FileExists(RECENT_FILE_NAME))
        {
            using var fs = fileSystemService.OpenFileForReadAccess(RECENT_FILE_NAME);
            foreach (var file in JsonSerializer.Deserialize<string[]>(fs))
            {
                RecentFiles.Add(file);
                if (RecentFiles.Count >= RECENT_FILE_COUNT)
                    break;
            }
        }
    }

    [RelayCommand]
    private async Task NewFileAsync()
    {
        if (IsDirty)
        {
            var mbResult = MessageBox.Show($"File {SaveFileName ?? "[New File]"} has changed. Would you like to save changes?", "File Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
            {
                await SaveAsync().ConfigureAwait(false);
            }
            else if (mbResult == MessageBoxResult.Cancel)
                return;
        }

        LootFile = new LootFile();
        SaveFileName = null;
    }

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        if (IsDirty)
        {
            var mbResult = MessageBox.Show($"File {SaveFileName ?? "[New File]"} has changed. Would you like to save changes?", "File Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
            {
                await SaveAsync().ConfigureAwait(false);
            }
            else if (mbResult == MessageBoxResult.Cancel)
                return;
        }

        var ofd = new OpenFileDialog()
        {
            Multiselect = false,
            Filter = "Loot Files|*.utl",
            CheckFileExists = true,
            CheckPathExists = true
        };

        await Application.Current.Dispatcher.Invoke(async () =>
        {
            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                await OpenFileAsync(ofd.FileName).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(saveFileName))
        {
            await SaveAsAsync().ConfigureAwait(false);
        }
        else
            await SaveFileAsync(saveFileName).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task SaveAsAsync()
    {
        var sfd = new SaveFileDialog()
        {
            CheckPathExists = true,
            Filter = "Loot Files|*.utl",
            OverwritePrompt = true
        };

        var result = sfd.ShowDialog();
        if (result.HasValue && result.Value)
        {
            SaveFileName = sfd.FileName;

            if (!string.IsNullOrEmpty(saveFileName))
                await SaveFileAsync(saveFileName).ConfigureAwait(false);
        }
    }

    [RelayCommand]
    private async Task OpenRecentFileAsync(string fileName)
    {
        if (IsDirty)
        {
            var mbResult = MessageBox.Show($"File {SaveFileName ?? "[New File]"} has changed. Would you like to save changes?", "File Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
            {
                await SaveAsync().ConfigureAwait(false);
            }
            else if (mbResult == MessageBoxResult.Cancel)
                return;
        }

        if (fileSystemService.FileExists(fileName))
            await OpenFileAsync(fileName).ConfigureAwait(false);
        else
        {
            for (var i = RecentFiles.Count - 1; i >= 0; i--)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (RecentFiles[i].Equals(fileName, StringComparison.OrdinalIgnoreCase))
                        RecentFiles.RemoveAt(i);
                });
            }
            MessageBox.Show($"The file {fileName} is no longer on disk. Removing from recent files list.",
                "File Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private static void Exit(Window window) => window.Close();

    [RelayCommand]
    private async Task ClosingAsync(CancelEventArgs e)
    {
        if (IsDirty)
        {
            var mbResult = MessageBox.Show("File has changed. Would you like to save changes? Press cancel to keep running the application.", "File Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Cancel)
                e.Cancel = true;
            else if (mbResult == MessageBoxResult.Yes)
            {
                await SaveAsync().ConfigureAwait(false);
            }
            else
            {
                backupService.StopBackups();
            }
        }
    }

    [RelayCommand]
    private void AddUpdateSalvageRules(SalvageGroup group)
    {
        var wk = new SalvageGroupWorkmanshipViewModel(group);
        var result = dialogService.ShowDialog("Select Workmanship", wk);
        if (!result.HasValue || result.Value == false)
        {
            return;
        }

        var materials = group.GetMaterials();
        foreach (var m in materials)
        {
            var td = TypeDescriptor.GetConverter(typeof(Material));
            var ruleName = $"S: {td.ConvertToInvariantString(m)}";

            var vm = LootRuleListViewModel.LootRules.FirstOrDefault(r => r.Name.Equals(ruleName));
            if (vm != null)
            {
                dynamic criteria = vm.Criteria.FirstOrDefault(c => c.Type == LootCriteriaType.DoubleValKeyGE &&
                    ((ValueKeyLootCriteria<DoubleValueKey, double>)c.Criteria).Key == DoubleValueKey.SalvageWorkmanship);
                if (criteria != null)
                {
                    criteria.Value = wk.Workmanship;
                }
                else
                {
                    var newCriteria = LootCriteria.CreateLootCriteria(LootCriteriaType.DoubleValKeyGE);
                    ((ValueKeyLootCriteria<DoubleValueKey, double>)newCriteria).Key = DoubleValueKey.SalvageWorkmanship;
                    ((ValueKeyLootCriteria<DoubleValueKey, double>)newCriteria).Value = 0;
                    vm.AddCriteria(newCriteria);
                }
            }
            else
            {
                var newRule = new LootRule()
                {
                    Name = ruleName,
                    Action = LootAction.Salvage
                };

                // Add criteria for workmanship
                var newCriteria = LootCriteria.CreateLootCriteria(LootCriteriaType.DoubleValKeyGE);
                ((ValueKeyLootCriteria<DoubleValueKey, double>)newCriteria).Key = DoubleValueKey.SalvageWorkmanship;
                ((ValueKeyLootCriteria<DoubleValueKey, double>)newCriteria).Value = wk.Workmanship;
                newRule.AddCriteria(newCriteria);

                newCriteria = LootCriteria.CreateLootCriteria(LootCriteriaType.LongValKeyE);
                ((ValueKeyLootCriteria<LongValueKey, int>)newCriteria).Key = LongValueKey.Material;
                ((ValueKeyLootCriteria<LongValueKey, int>)newCriteria).Value = (int)m;
                newRule.AddCriteria(newCriteria);

                LootRuleListViewModel.AddRule(newRule);
            }
        }
    }

    [RelayCommand]
    private void BulkAdd()
    {
        var vm = new BulkUpdateViewModel();
        var result = dialogService.ShowDialog("Bulk Add/Update", vm);
        if (result.HasValue && result.Value)
        {
            var matchingRules = LootRuleListViewModel.LootRules.Where(r => (vm.Name == null || Regex.IsMatch(r.Name, vm.Name)) &&
                (!vm.Action.HasValue || vm.Action.Value == r.Action) &&
                (!vm.ApplyToDisabled.HasValue || vm.ApplyToDisabled.Value == r.IsDisabled)).ToArray();

            if (matchingRules.Length > 0)
            {
                var message = new StringBuilder()
                    .Append("This update will apply to the following ").Append(matchingRules.Length).Append(" rule").AppendLine(matchingRules.Length == 1 ? ":" : "s:");

                foreach (var rule in matchingRules.Take(10))
                    message.Append("    (").Append(rule.Action).Append(") ").AppendLine(rule.Name);

                if (matchingRules.Length > 10)
                    message.Append("    (").Append(matchingRules.Length - 10).AppendLine(" more...)");

                message.Append("Do you wish to continue?");

                var mbResult = MessageBox.Show(message.ToString(), "Continue?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mbResult == MessageBoxResult.Yes)
                {
                    foreach (var rule in matchingRules)
                    {
                        rule.AddCriteria(vm.LootCriteriaViewModel.Criteria);
                    }
                }
            }
            else
            {
                MessageBox.Show("No matching rules found.", "No Match", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    [RelayCommand]
    public async Task ImportAsync()
    {
        var ofd = new OpenFileDialog()
        {
            Multiselect = false,
            Filter = "Loot Files|*.utl",
            CheckFileExists = true,
            CheckPathExists = true
        };

        var result = ofd.ShowDialog();
        if (result.HasValue && result.Value)
        {
            using var fs = fileSystemService.OpenFileForReadAccess(ofd.FileName);
            using var reader = new StreamReader(fs);
            var lf = new LootFile();
            await lf.ReadFileAsync(reader).ConfigureAwait(false);

            var vm = new ImportRulesViewModel(lf);

            Application.Current.Dispatcher.Invoke(() =>
            {
                var rulesToAdd = new List<LootRule>();
                var rulesToReplace = new List<LootRule>();
                var mbResult = dialogService.ShowDialog("Select Rules to Import", vm);
                if (mbResult.HasValue && mbResult.Value == true)
                {
                    SkipOverwriteAddDialogResult? doForAllResult = null;
                    foreach (var rule in vm.CheckedRules)
                    {
                        if (LootRuleListViewModel.LootRules.Any(r => r.Name.Equals(rule.Name)))
                        {
                            SkipOverwriteAddDialogResult? eResult;
                            if (!doForAllResult.HasValue)
                            {
                                eResult = dialogService.ShowEnumDialog<SkipOverwriteAddDialogResult>($"Both files contain a rule named {(string.IsNullOrEmpty(rule.Name) ? "<blank>" : rule.Name)}. What would you like to do?", "Rule Exists", out var doForAll);
                                if (doForAll && eResult.HasValue)
                                    doForAllResult = eResult;
                            }
                            else
                                eResult = doForAllResult;

                            switch (eResult)
                            {
                                case null:
                                    return;

                                case SkipOverwriteAddDialogResult.Skip:
                                    continue;

                                case SkipOverwriteAddDialogResult.Overwrite:
                                    rulesToReplace.Add(rule);
                                    continue;
                            }
                        }

                        rulesToAdd.Add(rule);
                    }

                    foreach (var r in rulesToAdd)
                        LootRuleListViewModel.AddRule(r);
                    foreach (var r in rulesToReplace)
                        LootRuleListViewModel.ReplaceRule(r);
                }
            });
        }
    }

    public async Task OpenFileAsync(string fileName, string saveFileName = null, bool ignoreFirstLine = false)
    {
        BusyStatus = "Opening file...";
        IsBusy = true;
        try
        {
            using var fs = fileSystemService.OpenFileForReadAccess(fileName);
            using var reader = new StreamReader(fs);
            if (ignoreFirstLine)
                await reader.ReadLineAsync().ConfigureAwait(false);
            await ReadLootFileAsync(reader).ConfigureAwait(false);

            SaveFileName = saveFileName ?? fileName;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"There was an error loading the file. The message was: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ReadLootFileAsync(StreamReader reader)
    {
        var lf = new LootFile();
        await lf.ReadFileAsync(reader).ConfigureAwait(false);
        LootFile = lf;
    }

    private async Task SaveFileAsync(string fileName)
    {
        BusyStatus = "Saving file...";
        IsBusy = true;
        try
        {
            await WriteFileAsync(fileName).ConfigureAwait(false);
            SaveFileName = fileName;

            LootRuleListViewModel.Clean();
            SalvageCombineListViewModel.Clean();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"There was an error saving the file. The message was: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task WriteFileAsync(string fileName, string saveFileName = null, bool writeFileName = false)
    {
        using var fs = fileSystemService.OpenFileForWriteAccess(fileName);
        using var writer = new StreamWriter(fs, Encoding.UTF8, 65535);
        if (writeFileName)
            await writer.WriteLineAsync(saveFileName ?? "").ConfigureAwait(false);
        await LootFile.WriteFileAsync(writer).ConfigureAwait(false);
    }

    private void AddRecentFile(string fileName)
    {
        for (var i = RecentFiles.Count - 1; i >= 0; i--)
        {
            if (RecentFiles[i].Equals(fileName, StringComparison.OrdinalIgnoreCase))
                RecentFiles.RemoveAt(i);
        }

        RecentFiles.Insert(0, fileName);
        while (RecentFiles.Count > RECENT_FILE_COUNT)
            RecentFiles.RemoveAt(RecentFiles.Count - 1);

        fileSystemService.TryCreateDirectory(Path.GetDirectoryName(RECENT_FILE_NAME));
        using var fs = fileSystemService.OpenFileForWriteAccess(RECENT_FILE_NAME);
        using var writer = new Utf8JsonWriter(fs);
        JsonSerializer.Serialize(writer, RecentFiles);
    }

    private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsDirty")
        {
            OnPropertyChanged(nameof(IsDirty));

            if (IsDirty)
            {
                backupService.StartBackups(LootFile, SaveFileName);
            }
            else
            {
                backupService.StopBackups();
            }
        }
    }
}
