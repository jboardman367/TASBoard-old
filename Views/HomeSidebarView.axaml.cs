using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TASBoard.ViewModels;
using TASBoard.Models;

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

        private void UpdateDropdowns(object sender, PointerPressedEventArgs e)
        {
            var context = (HomeSidebarViewModel)DataContext;
            context.UpdateDropdowns();
        }

        private void DisplayKeysDownClicked(object sender, RoutedEventArgs e)
        {
            var check = (CheckBox)sender;
            var context = (HomeSidebarViewModel)DataContext;
            context.Workspace.SetDisplayKeyDown(check.IsChecked ?? false);
        }
    }
}
