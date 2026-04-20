using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DirectoryScanner.Core;
using System.Windows;

namespace DirectoryScanner.WPF
{
    public class NodeViewModel : ViewModelBase
    {
        private readonly FileSystemNode _model;
        public NodeViewModel Parent { get; }

        public NodeViewModel(FileSystemNode model, NodeViewModel parent = null)
        {
            _model = model;
            Parent = parent;

            foreach (var childModel in _model.Children)
            {
                Children.Add(new NodeViewModel(childModel, this));
            }

            _model.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(e.PropertyName);

                if (e.PropertyName == nameof(Size))
                {
                    OnPropertyChanged(nameof(Percentage));
                    UpdateChildrenPercentages();
                }
            };
        }

        public string Name => _model.Name;
        public long Size => _model.Size;
        public string Type => _model.Type.ToString();

        public double Percentage
        {
            get
            {
                if (Parent == null || Parent.Size == 0) return 1.0;
                return (double)Size / Parent.Size;
            }
        }

        public ObservableCollection<NodeViewModel> Children { get; } = new ObservableCollection<NodeViewModel>();

        public void AddChild(FileSystemNode model)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Children.Add(new NodeViewModel(model, this));
            });
        }

        private void UpdateChildrenPercentages()
        {
            foreach (var child in Children)
            {
                child.OnPropertyChanged(nameof(Percentage));
            }
        }
    }
}
