using TASBoard.Models;

namespace TASBoard.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase currentSidebar;
        private ViewModelBase currentWorkspace;

        public MainWindowViewModel()
        {
            Workspace w = new();
            currentSidebar = new HomeSidebarViewModel(w);
            currentWorkspace = new WorkspaceViewModel(w);
        }

        public ViewModelBase CurrentSidebar
        {
            get => currentSidebar;
            private set => currentSidebar = value;
        }

        public ViewModelBase CurrentWorkspace
        {
            get => currentWorkspace;
            private set => currentWorkspace = value;
        } 
    }
}
