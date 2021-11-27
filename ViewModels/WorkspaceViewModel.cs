using Avalonia.Media;
using System.Collections.ObjectModel;
using TASBoard.Models;

namespace TASBoard.ViewModels
{
    public class WorkspaceViewModel : ViewModelBase
    {
        public WorkspaceViewModel()
        {
            _items = new();
            bg = new SolidColorBrush(new Color(255, 112, 112, 112));
        }

        public Brush bg;

        public void AddNewKey(string keyStyle, string keyName)
        {
            _items.Add(new KeyObject(keyStyle, keyName));
        }

        public void UpdateWorkspaceBackground(byte r, byte g, byte b)
        {
            bg = new SolidColorBrush(new Color(255, r, g, b));
        }

        private ObservableCollection<KeyObject> _items;
        public ObservableCollection<KeyObject> Items => _items;
    }
}
