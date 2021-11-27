using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using TASBoard.ViewModels;

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
            
            if (DataContext is null) { return; }
            HomeSidebarViewModel context = (HomeSidebarViewModel)DataContext;
            //context.SelectedStyle = (string)e.AddedItems[0];
            context.UpdateDropdowns();
        }
    }
}
