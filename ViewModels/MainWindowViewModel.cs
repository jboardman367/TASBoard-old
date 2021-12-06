using ReactiveUI;
using TASBoard.Models;

namespace TASBoard.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase currentSidebar;
        private ViewModelBase currentWorkspace;
        private Workspace w;

        public MainWindowViewModel()
        {
            w = new();
            currentSidebar = new EncodeSidebarViewModel(w);
            currentWorkspace = new WorkspaceViewModel(w);
        }

        public ViewModelBase CurrentSidebar
        {
            get => currentSidebar;
            private set => this.RaiseAndSetIfChanged(ref currentSidebar, value);
        }

        public ViewModelBase CurrentWorkspace
        {
            get => currentWorkspace;
            private set => this.RaiseAndSetIfChanged(ref currentWorkspace, value);
        } 

        public void EncodeButton()
        {
            if (currentSidebar is EncodeSidebarViewModel)
                return;
            CurrentSidebar = new EncodeSidebarViewModel(w);
        }

        public void CanvasButton()
        {
            if (currentSidebar is CanvasSidebarViewModel)
                return;
            CurrentSidebar = new CanvasSidebarViewModel(w);
        }
    }
}
