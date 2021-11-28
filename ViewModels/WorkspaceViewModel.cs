using Avalonia.Media;
using ReactiveUI;
using System.Collections.ObjectModel;
using TASBoard.Models;

namespace TASBoard.ViewModels
{
    public class WorkspaceViewModel : ViewModelBase
    {
        private Workspace workspace;
        public WorkspaceViewModel(Workspace w)
        {
            bg = new SolidColorBrush(new Color(255, 112, 112, 112));
            workspace = w;
        }

        private Brush bg;
        public Brush Background 
        { 
            get => bg; 
            set => this.RaiseAndSetIfChanged(ref bg, value); 
        }
        
        public void UpdateWorkspaceBackground(byte r, byte g, byte b)
        {
            Background = new SolidColorBrush(new Color(255, r, g, b));
        }

        public ObservableCollection<KeyObject> Items { get => workspace.AllKeys; }
    }
}
