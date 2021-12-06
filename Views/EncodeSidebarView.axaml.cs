using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TASBoard.ViewModels;
using TASBoard.Models;

namespace TASBoard.Views
{
    public partial class EncodeSidebarView : UserControl
    {
        public EncodeSidebarView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
