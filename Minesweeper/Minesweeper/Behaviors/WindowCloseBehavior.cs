using System.Windows.Interactivity;
using System.Windows;

namespace Minesweeper.Behaviors
{
    public sealed class WindowCloseBehavior : Behavior<Window>
    {
        public bool Close
        {
            get { return (bool)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        public static readonly DependencyProperty CloseProperty = DependencyProperty.Register("Close", typeof(bool), typeof(WindowCloseBehavior), new PropertyMetadata(false, OnCloseChanged));

        private static void OnCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = ((WindowCloseBehavior)d).AssociatedObject;
            var newValue = (bool)e.NewValue;
            if (newValue)
            {
                window.Close();
            }
        }
    }
}
