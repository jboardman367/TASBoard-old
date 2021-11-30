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
            workspace = w;
        }

        public Brush Background 
        { 
            get => workspace.backgroundColor; 
            set => this.RaiseAndSetIfChanged(ref workspace.backgroundColor, value); 
        }
        
        public void UpdateWorkspaceBackground(byte r, byte g, byte b)
        {
            Background = new SolidColorBrush(new Color(255, r, g, b));
        }

        public ObservableCollection<ICanvasElement> Items { get => workspace.AllCanvasElements; }
    }
}
