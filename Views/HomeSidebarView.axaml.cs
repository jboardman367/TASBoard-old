using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TASBoard.Views
{
    public partial class HomeSidebarView : UserControl
    {
        public HomeSidebarView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
