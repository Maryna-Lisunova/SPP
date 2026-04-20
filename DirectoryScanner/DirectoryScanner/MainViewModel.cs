using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using DirectoryScanner.Core;
using System.IO;

namespace DirectoryScanner.WPF
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Scanner _scanner = new Scanner();
        private CancellationTokenSource _cts;
        private NodeViewModel _rootNode;
        private string _selectedPath;

        public NodeViewModel RootNode { get => _rootNode; set { _rootNode = value; OnPropertyChanged(); } }
        public string SelectedPath { get => _selectedPath; set { _selectedPath = value; OnPropertyChanged(); } }


        public ICommand ScanCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BrowseCommand { get; }

        public MainViewModel()
        {
            ScanCommand = new RelayCommand(_ => StartScan());
            CancelCommand = new RelayCommand(_ => CancelScan());
            BrowseCommand = new RelayCommand(_ => BrowseFolder());
        }

        private void BrowseFolder()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedPath = dialog.SelectedPath;
                }
            }
        }

        private async void StartScan()
        {
            if (string.IsNullOrWhiteSpace(SelectedPath) || !Directory.Exists(SelectedPath))
            {
                System.Windows.MessageBox.Show("Пожалуйста, выберите корректную папку!");
                return;
            }

            _cts = new CancellationTokenSource();

            try
            {
                var rootModel = await _scanner.ScanDirectory(SelectedPath, _cts.Token);
                RootNode = new NodeViewModel(rootModel);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при сканировании: {ex.Message}");
            }
        }

        private void CancelScan() => _cts?.Cancel();
    }
}
