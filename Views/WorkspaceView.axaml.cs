using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;

namespace TASBoard.Views
{
    public partial class WorkspaceView : UserControl
    {
        private Point pointerStartLocation;
        private Point imageStartLocation;
        private Image? heldImage;
        public WorkspaceView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void ImageClick(object sender, PointerPressedEventArgs e)
        {
            var pointer = e.GetCurrentPoint(this);
            var properties = pointer.Properties;
            if (properties.IsLeftButtonPressed && !properties.IsRightButtonPressed)
            {
                // Image is being picked up
                heldImage = (Image)sender;
                imageStartLocation = new(heldImage.Bounds.Position.X, heldImage.Bounds.Y);
                pointerStartLocation = pointer.Position;
                e.Handled = true;
            }

            if (heldImage is not null && properties.IsRightButtonPressed && sender == heldImage)
            {
                // Image is being snapped back to starting location
                heldImage.Arrange(new Rect(imageStartLocation, heldImage.Bounds.Size));
                heldImage = null;
                e.Handled = true;
            }
        }

        public void ImageRelease(object sender, PointerReleasedEventArgs e)
        {
            var pointer = e.GetCurrentPoint(this);
            var properties = pointer.Properties;
            if (heldImage is not null && !properties.IsLeftButtonPressed)
            {
                // Image is released
                heldImage = null;
                e.Handled = true;
            }
        }

        public void PointerMovedOnCanvas(object sender, PointerEventArgs e)
        {
            var pointer = e.GetCurrentPoint(this);
            var properties = pointer.Properties;
            if (heldImage is not null && properties.IsLeftButtonPressed)
            {
                // Image is being dragged along
                var distanceMoved = pointer.Position - pointerStartLocation;
                heldImage.Arrange(new Rect(imageStartLocation + distanceMoved, heldImage.Bounds.Size));
                e.Handled = true;
            }
        }

        public void PointerLeftCanvas(object sender, PointerEventArgs e)
        {
            var pointer = e.GetCurrentPoint(this);
            var properties = pointer.Properties;
            if (heldImage is not null)
            {
                // Image has been dragged out of the canvas,
                // so snap back to the start
                heldImage.Arrange(new Rect(imageStartLocation, heldImage.Bounds.Size));
                heldImage = null;
                e.Handled = true;
            }
        }
    }
}
