using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TASBoard.ViewModels;

namespace TASBoard.Views
{
    public partial class CanvasSidebarView : UserControl
    {
        public CanvasSidebarView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void UpdateDropdowns(object sender, PointerPressedEventArgs e)
        {
            var context = (CanvasSidebarViewModel)DataContext;
            context.UpdateDropdowns();
        }

        private void DisplayKeysDownClicked(object sender, RoutedEventArgs e)
        {
            var check = (CheckBox)sender;
            var context = (CanvasSidebarViewModel)DataContext;
            context.Workspace.SetDisplayKeyDown(check.IsChecked ?? false);
        }
    }
}
