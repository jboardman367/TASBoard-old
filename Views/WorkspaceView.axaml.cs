using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TASBoard.Views
{
    public partial class WorkspaceView : UserControl
    {
        public WorkspaceView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
