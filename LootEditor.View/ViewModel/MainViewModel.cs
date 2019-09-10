using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LootEditor.Model;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace LootEditor.View.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string saveFileName;
        private LootFile lootFile;
        private LootRuleListViewModel lootRuleListViewModel;
        private SalvageCombineListViewModel salvageCombineViewModel;

        public string SaveFileName
        {
            get => string.IsNullOrEmpty(saveFileName) ? "New File" : Path.GetFileName(saveFileName);
            set
            {
                if (saveFileName != value)
                {
                    saveFileName = value;
                    RaisePropertyChanged(nameof(SaveFileName));
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

        public bool IsDirty => LootRuleListViewModel.IsDirty || SalvageCombineListViewModel.IsDirty;

        public RelayCommand NewFileCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand SaveFileCommand { get; }
        public RelayCommand SaveAsCommand { get; }
        public RelayCommand<CancelEventArgs> ClosingCommand { get; }
        public RelayCommand<Window> ExitCommand { get; }

        public MainViewModel()
        {
            LootFile = new LootFile();

            NewFileCommand = new RelayCommand(async () =>
            {
                if (IsDirty)
                {
                    var mbResult = MessageBox.Show("File has changed. Would you like to save changes?", "File Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (mbResult == MessageBoxResult.Yes)
                    {
                        await SaveFileAsync(saveFileName).ConfigureAwait(false);
                    }
                    else if (mbResult == MessageBoxResult.Cancel)
                        return;
                }

                LootFile = new LootFile();
            });

            OpenFileCommand = new RelayCommand(async () =>
            {
                if (IsDirty)
                {
                    var mbResult = MessageBox.Show("File has changed. Would you like to save changes?", "File Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (mbResult == MessageBoxResult.Yes)
                    {
                        await SaveFileAsync(saveFileName).ConfigureAwait(false);
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
                    try
                    {
                        using (var fs = File.OpenRead(ofd.FileName))
                        {
                            var lf = new LootFile();
                            await lf.ReadFileAsync(fs);
                            LootFile = lf;
                            SaveFileName = ofd.FileName;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"There was an error loading the file. The message was: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });

            SaveFileCommand = new RelayCommand(async () =>
            {
                if (string.IsNullOrEmpty(saveFileName))
                {
                    var sfd = new SaveFileDialog()
                    {
                        CheckPathExists = true,
                        FileName = "Loot Files|*.utl",
                        OverwritePrompt = true
                    };

                    var result = sfd.ShowDialog();
                    if (result.HasValue && result.Value)
                    {
                        SaveFileName = sfd.FileName;
                    }
                    else
                        return;
                }

                if (!string.IsNullOrEmpty(saveFileName))
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

            ClosingCommand = new RelayCommand<CancelEventArgs>(async e =>
            {
                if (IsDirty)
                {
                    var mbResult = MessageBox.Show("File has changed. Would you like to save changes? Press cancel to keep running the application.", "File Changed", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (mbResult == MessageBoxResult.Cancel)
                        e.Cancel = true;
                    else if (mbResult == MessageBoxResult.Yes)
                    {
                        if (string.IsNullOrEmpty(saveFileName))
                        {
                            var sfd = new SaveFileDialog()
                            {
                                CheckPathExists = true,
                                FileName = "Loot Files|*.utl",
                                OverwritePrompt = true
                            };

                            var result = sfd.ShowDialog();
                            if (result.HasValue && result.Value)
                            {
                                SaveFileName = sfd.FileName;
                            }
                            else if (!result.HasValue)
                                e.Cancel = true;
                        }
                        if (!string.IsNullOrEmpty(saveFileName))
                            await SaveFileAsync(saveFileName).ConfigureAwait(false);
                    }
                }
            });

            ExitCommand = new RelayCommand<Window>(w => w.Close());
        }

        private async Task SaveFileAsync(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await LootFile.WriteFileAsync(fs).ConfigureAwait(false);
                await fs.FlushAsync();
                fs.Close();
            }

            LootRuleListViewModel.Clean();
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
                RaisePropertyChanged(nameof(IsDirty));
        }
    }
}
