﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LootEditor.Model;
using LootEditor.Model.Enums;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LootEditor.View.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private static readonly string RECENT_FILE_NAME = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loot Editor", "RecentFiles.json");
        private const int RECENT_FILE_COUNT = 10;
        private string saveFileName;
        private LootFile lootFile;
        private LootRuleListViewModel lootRuleListViewModel;
        private SalvageCombineListViewModel salvageCombineViewModel;
        private bool isBusy = false;
        private string busyStatus;

        public string SaveFileName
        {
            get => string.IsNullOrEmpty(saveFileName) ? "New File" : Path.GetFileName(saveFileName);
            set
            {
                if (saveFileName != value)
                {
                    saveFileName = value;
                    RaisePropertyChanged(nameof(SaveFileName));

                    if (!string.IsNullOrEmpty(saveFileName))
                        AddRecentFile(saveFileName);
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

                    LootRuleListViewModel = new LootRuleListViewModel(lootFile);
                    SalvageCombineListViewModel = new SalvageCombineListViewModel(LootFile);
                }
            }
        }

        public LootRuleListViewModel LootRuleListViewModel
        {
            get => lootRuleListViewModel;
            set
            {
                if (lootRuleListViewModel != value)
                {
                    if (lootRuleListViewModel != null)
                        lootRuleListViewModel.PropertyChanged -= VM_PropertyChanged;

                    lootRuleListViewModel = value;
                    lootRuleListViewModel.PropertyChanged += VM_PropertyChanged;
                    RaisePropertyChanged(nameof(LootRuleListViewModel));
                    RaisePropertyChanged(nameof(IsDirty));
                }
            }
        }

        public SalvageCombineListViewModel SalvageCombineListViewModel
        {
            get => salvageCombineViewModel;
            set
            {
                if (salvageCombineViewModel != value)
                {
                    if (salvageCombineViewModel != null)
                        salvageCombineViewModel.PropertyChanged -= VM_PropertyChanged;

                    salvageCombineViewModel = value;
                    salvageCombineViewModel.PropertyChanged += VM_PropertyChanged;
                    RaisePropertyChanged(nameof(SalvageCombineListViewModel));
                    RaisePropertyChanged(nameof(IsDirty));
                }
            }
        }

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    RaisePropertyChanged(nameof(IsBusy));
                }
            }
        }

        public string BusyStatus
        {
            get => busyStatus;
            set
            {
                if (busyStatus != value)
                {
                    busyStatus = value;
                    RaisePropertyChanged(nameof(BusyStatus));
                }
            }
        }

        public List<string> RecentFiles { get; } = new List<string>();

        public bool IsDirty => LootRuleListViewModel.IsDirty || SalvageCombineListViewModel.IsDirty;

        public RelayCommand NewFileCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand SaveFileCommand { get; }
        public RelayCommand SaveAsCommand { get; }
        public RelayCommand<CancelEventArgs> ClosingCommand { get; }
        public RelayCommand<Window> ExitCommand { get; }
        public RelayCommand<string> OpenRecentFileCommand { get; }
        public RelayCommand<Model.Enums.SalvageGroup> AddUpdateSalvageRulesCommand { get; }

        public MainViewModel()
        {
            LootFile = new LootFile();

            if (File.Exists(RECENT_FILE_NAME))
            {
                var json = File.ReadAllText(RECENT_FILE_NAME);
                var files = JsonConvert.DeserializeObject<IEnumerable<string>>(json);
                RecentFiles.AddRange(files);
                while (RecentFiles.Count > RECENT_FILE_COUNT)
                    RecentFiles.RemoveAt(RecentFiles.Count - 1);
                RaisePropertyChanged(nameof(RecentFiles));
            }
            else

            NewFileCommand = new RelayCommand(async () =>
            {
                if (IsDirty)
                {
                    var mbResult = MessageBox.Show("File has changed. Would you like to save changes?", "File Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (mbResult == MessageBoxResult.Yes)
                    {
                        await SaveFileAsync(SaveFileName).ConfigureAwait(false);
                    }
                    else if (mbResult == MessageBoxResult.Cancel)
                        return;
                }

                LootFile = new LootFile();
                SaveFileName = null;
            });

            OpenFileCommand = new RelayCommand(async () =>
            {
                if (IsDirty)
                {
                    var mbResult = MessageBox.Show("File has changed. Would you like to save changes?", "File Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (mbResult == MessageBoxResult.Yes)
                    {
                        SaveAsCommand.Execute(null);
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

                var result = ofd.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    await OpenFileAsync(ofd.FileName).ConfigureAwait(false);
                }
            });

            SaveFileCommand = new RelayCommand(async () =>
            {
                if (string.IsNullOrEmpty(saveFileName))
                {
                    SaveAsCommand.Execute(null);
                }
                else
                    await SaveFileAsync(saveFileName).ConfigureAwait(false);
            });

            SaveAsCommand = new RelayCommand(async () =>
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
            });

            ClosingCommand = new RelayCommand<CancelEventArgs>(e =>
            {
                if (IsDirty)
                {
                    var mbResult = MessageBox.Show("File has changed. Would you like to save changes? Press cancel to keep running the application.", "File Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (mbResult == MessageBoxResult.Cancel)
                        e.Cancel = true;
                    else if (mbResult == MessageBoxResult.Yes)
                    {
                        SaveAsCommand.Execute(null);
                    }
                }
            });

            ExitCommand = new RelayCommand<Window>(w => w.Close());

            OpenRecentFileCommand = new RelayCommand<string>(async fileName =>
            {
                if (File.Exists(fileName))
                    await OpenFileAsync(fileName).ConfigureAwait(false);
                else
                {
                    RecentFiles.RemoveAll(f => fileName.Equals(f, StringComparison.OrdinalIgnoreCase));
                    MessageBox.Show($"The file {fileName} is no longer on disk. Removing from recent files list.",
                        "File Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });

            AddUpdateSalvageRulesCommand = new RelayCommand<Model.Enums.SalvageGroup>(group =>
            {
                var dlg = new Dialogs.DialogService();
                var wk = new Dialogs.SalvageGroupWorkmanshipViewModel(group);
                var result = dlg.ShowDialog("Select Workmanship", wk);
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
            });
        }

        public async Task OpenFileAsync(string fileName)
        {
            BusyStatus = "Opening file...";
            IsBusy = true;
            try
            {
                using (var fs = File.OpenRead(fileName))
                using (var reader = new StreamReader(fs))
                {
                    var lf = new LootFile();
                    await lf.ReadFileAsync(reader).ConfigureAwait(false);
                    LootFile = lf;
                }

                SaveFileName = fileName;
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

        private async Task SaveFileAsync(string fileName)
        {
            BusyStatus = "Saving file...";
            IsBusy = true;
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(fs, System.Text.Encoding.UTF8, 65535))
                {
                    await LootFile.WriteFileAsync(writer).ConfigureAwait(false);
                    await fs.FlushAsync();
                    fs.Close();
                }

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

        private void AddRecentFile(string fileName)
        {
            RecentFiles.RemoveAll(f => fileName.Equals(f, StringComparison.OrdinalIgnoreCase));
            RecentFiles.Insert(0, fileName);
            while (RecentFiles.Count > RECENT_FILE_COUNT)
                RecentFiles.RemoveAt(RecentFiles.Count - 1);

            RaisePropertyChanged(nameof(RecentFiles));

            var json = JsonConvert.SerializeObject(RecentFiles);
            if (!Directory.Exists(Path.GetDirectoryName(RECENT_FILE_NAME)))
                Directory.CreateDirectory(Path.GetDirectoryName(RECENT_FILE_NAME));
            File.WriteAllText(RECENT_FILE_NAME, json);
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
                RaisePropertyChanged(nameof(IsDirty));
        }
    }
}
