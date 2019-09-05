using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;
using LootEditor.Model;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LootEditor.View.ViewModel
{
    public class MainViewModel : ViewModelBase, IDropTarget
    {
        private string saveFileName;
        private LootFile lootFile;
        private LootRuleViewModel selectedRule;
        private bool isDirty = false;

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

        public bool IsDirty
        {
            get => isDirty || LootRules.Any(r => r.IsDirty);
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    RaisePropertyChanged(nameof(IsDirty));
                }
            }
        }

        public RelayCommand NewFileCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand SaveFileCommand { get; }
        public RelayCommand SaveAsCommand { get; }
        public RelayCommand<CancelEventArgs> ClosingCommand { get; }
        public RelayCommand AddRuleCommand { get; }
        public RelayCommand CloneRuleCommand { get; }
        public RelayCommand DeleteRuleCommand { get; }
        public RelayCommand<int> MoveSelectedItemDownCommand { get; }
        public RelayCommand<int> MoveSelectedItemUpCommand { get; }
        public RelayCommand CutItemCommand { get; }
        public RelayCommand CopyItemCommand { get; }
        public RelayCommand PasteItemCommand { get; }

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
                    else if (mbResult != MessageBoxResult.Cancel)
                        LootFile = new LootFile();
                }
            });

            OpenFileCommand = new RelayCommand(async () =>
            {
                if (IsDirty)
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


            AddRuleCommand = new RelayCommand(() =>
            {
                var rule = new LootRule()
                {
                    Name = "New Rule",
                    Action = LootAction.Keep
                };

                lootFile.AddRule(rule);

                var vm = new LootRuleViewModel(rule);
                LootRules.Add(vm);

                SelectedRule = vm;
            });

            CloneRuleCommand = new RelayCommand(() =>
            {
                var sel = SelectedRule;
                if (sel != null)
                {
                    var newRule = sel.CloneRule();
                    lootFile.AddRule(newRule);

                    var vm = new LootRuleViewModel(newRule);
                    LootRules.Add(vm);

                    IsDirty = true;
                    SelectedRule = vm;
                }
            }, () => SelectedRule != null);

            DeleteRuleCommand = new RelayCommand(() =>
            {
                var sel = SelectedRule;
                if (sel != null)
                {
                    LootRules.Remove(sel);
                    lootFile.RemoveRule(sel.Rule);
                    IsDirty = true;
                }
            }, () => SelectedRule != null);

            MoveSelectedItemDownCommand = new RelayCommand<int>(index =>
            {
                LootRules.Move(index, index + 1);
                LootFile.MoveRule(index, index + 1);
                IsDirty = true;
            }, _ => SelectedRule != null);

            MoveSelectedItemUpCommand = new RelayCommand<int>(index =>
            {
                LootRules.Move(index, index - 1);
                LootFile.MoveRule(index, index - 1);
                IsDirty = true;
            }, _ => SelectedRule != null);

            CutItemCommand = new RelayCommand(() =>
            {
                Clipboard.SetData(typeof(LootRule).Name, SelectedRule.Rule);
                DeleteRuleCommand.Execute(null);
            }, () => SelectedRule != null);

            CopyItemCommand = new RelayCommand(() =>
            {
                Clipboard.SetData(typeof(LootRule).Name, SelectedRule.Rule);
            }, () => SelectedRule != null);

            PasteItemCommand = new RelayCommand(() =>
            {
                var data = Clipboard.GetData(typeof(LootRule).Name) as LootRule;

                var newRule = data.Clone() as LootRule;
                LootFile.AddRule(newRule);

                var vm = new LootRuleViewModel(newRule);
                LootRules.Add(vm);

                IsDirty = true;
                SelectedRule = vm;
            }, () => Clipboard.ContainsData(typeof(LootRule).Name));
        }

        private async Task SaveFileAsync(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await LootFile.WriteFileAsync(fs).ConfigureAwait(false);
                await fs.FlushAsync();
                fs.Close();
            }

            foreach (var rule in LootRules.Where(r => r.IsDirty))
                rule.Clean();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is LootRuleViewModel)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.InsertIndex > dropInfo.DragInfo.SourceIndex)
            {
                LootFile.MoveRule(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex - 1);
                LootRules.Move(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex - 1);
            }
            else
            {
                LootFile.MoveRule(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex);
                LootRules.Move(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex);
            }
            IsDirty = true;
        }
    }
}
