using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DirectoryScanner.Core
{
    public enum NodeType
    {
        File,
        Directory
    }

    public class FileSystemNode : INotifyPropertyChanged
    {
        private long _size;
        private string _name;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Path { get; set; }

        public long Size
        {
            get => _size;
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        }

        public NodeType Type { get; set; }

        public bool IsSymbolicLink { get; set; }

        public ObservableCollection<FileSystemNode> Children { get; } = new ObservableCollection<FileSystemNode>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
