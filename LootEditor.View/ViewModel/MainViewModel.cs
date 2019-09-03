using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LootEditor.Model;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LootEditor.View.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string saveFileName;
        private LootFile lootFile;
        private LootRuleViewModel selectedRule;

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

                    foreach (var vm in LootRules)
                        vm.PropertyChanged -= Vm_PropertyChanged;
                    LootRules.Clear();
                    foreach (var rule in lootFile.Rules)
                    {
                        var vm = new LootRuleViewModel(rule);
                        vm.PropertyChanged += Vm_PropertyChanged;
                        LootRules.Add(vm);
                    }
                    RaisePropertyChanged(nameof(IsDirty));
                }
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                RaisePropertyChanged(nameof(IsDirty));
            }
        }

        public ObservableCollection<LootRuleViewModel> LootRules { get; } = new ObservableCollection<LootRuleViewModel>();

        public LootRuleViewModel SelectedRule
        {
            get => selectedRule;
            set
            {
                if (selectedRule != value)
                {
                    selectedRule = value;
                    RaisePropertyChanged(nameof(SelectedRule));
                }
            }
        }

        public bool IsDirty => LootRules.Any(r => r.IsDirty);

        public RelayCommand NewFileCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand SaveFileCommand { get; }

        public MainViewModel()
        {
            LootFile = new LootFile();

            NewFileCommand = new RelayCommand(() =>
            {
                if (LootRules.Any(r => r.IsDirty))
                {
                    MessageBox.Show("asdfsa");
                }
            });

            OpenFileCommand = new RelayCommand(async () =>
            {
                if (LootRules.Any(r => r.IsDirty))
                {
                    var mbResult = MessageBox.Show("File has changed. Would you like to save changes?", "File Changed", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (mbResult == MessageBoxResult.Yes)
                    {
                        await SaveFileAsync(saveFileName).ConfigureAwait(false);
                    }
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
                    using (var fs = File.OpenRead(ofd.FileName))
                    {
                        var lf = new LootFile();
                        await lf.ReadFileAsync(fs);
                        LootFile = lf;
                        SaveFileName = ofd.FileName;
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
                }

                if (!string.IsNullOrEmpty(saveFileName))
                    await SaveFileAsync(saveFileName).ConfigureAwait(false);
            });
        }

        private async Task SaveFileAsync(string fileName)
        {
            using (var fs = File.OpenWrite(SaveFileName))
            {
                await LootFile.WriteFileAsync(fs).ConfigureAwait(false);

                foreach (var rule in LootRules.Where(r => r.IsDirty))
                    rule.Clean();
            }
        }
    }
}
