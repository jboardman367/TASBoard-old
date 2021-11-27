namespace TASBoard.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase currentSidebar;
        private ViewModelBase currentWorkspace;

        public MainWindowViewModel()
        {
            currentSidebar = new HomeSidebarViewModel();
            currentWorkspace = new WorkspaceViewModel();
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
