using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Minesweeper.AttachedProperty
{
    public sealed class AttachedProperties : DependencyObject
    {
        static AttachedProperties()
        {

        }

        public static readonly DependencyProperty ImagePathProperty =
        DependencyProperty.RegisterAttached("ImagePath", typeof(string), typeof(AttachedProperties), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PopupPlacementTargetProperty =
            DependencyProperty.RegisterAttached("PopupPlacementTarget", typeof(DependencyObject), typeof(AttachedProperties), new PropertyMetadata(null, OnPopupPlacementTargetChanged));

        public static string GetImagePath(DependencyObject obj)
        {
            return (string)obj.GetValue(ImagePathProperty);
        }

        public static void SetImagePath(DependencyObject obj, string value)
        {
            obj.SetValue(ImagePathProperty, value);
        }

        public static DependencyObject GetPopupPlacementTarget(DependencyObject obj)
        {
            return (DependencyObject)obj.GetValue(PopupPlacementTargetProperty);
        }

        public static void SetPopupPlacementTarget(DependencyObject obj, DependencyObject value)
        {
            obj.SetValue(PopupPlacementTargetProperty, value);
        }

        private static void OnPopupPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Popup pop = d as Popup;
            if (e.OldValue is DependencyObject previousPlacementTarget)
            {
                Window window = Window.GetWindow(previousPlacementTarget);
                var element = previousPlacementTarget as FrameworkElement;
                if (window != null)
                {
                    CancelEventsListeningInWindow(window);
                }
                if (element != null)
                {
                    element.SizeChanged -= ElementSizeChanged;
                    element.LayoutUpdated -= ElementLayoutUpdated;
                }
            }

            if (e.NewValue is DependencyObject newPlacementTarget)
            {
                Window window = Window.GetWindow(newPlacementTarget);
                var element = newPlacementTarget as FrameworkElement;
                if (window != null)
                {
                    RegisterEventsInWindow(window);
                }
                else if (element != null)
                {
                    element.Loaded -= ElementLoaded;
                    element.Loaded += ElementLoaded;
                }
                if (element != null)
                {
                    element.SizeChanged -= ElementSizeChanged;
                    element.SizeChanged += ElementSizeChanged;
                    element.LayoutUpdated -= ElementLayoutUpdated;
                    element.LayoutUpdated += ElementLayoutUpdated;
                }

                void ElementLoaded(object sender, RoutedEventArgs e3)
                {
                    element.Loaded -= ElementLoaded;
                    window = Window.GetWindow(newPlacementTarget);
                    if (window != null)
                    {
                        RegisterEventsInWindow(window);
                    }
                }
            }

            void RegisterEventsInWindow(Window window)
            {
                window.LocationChanged -= WindowLocationChanged;
                window.LocationChanged += WindowLocationChanged;
                window.SizeChanged -= WindowSizeChanged;
                window.SizeChanged += WindowSizeChanged;
            }

            void CancelEventsListeningInWindow(Window window)
            {
                window.LocationChanged -= WindowLocationChanged;
                window.SizeChanged -= WindowSizeChanged;
            }

            void WindowLocationChanged(object s1, EventArgs e1)
            {
                UpdatePopupLocation();
            }

            void WindowSizeChanged(object sender, SizeChangedEventArgs e2)
            {
                UpdatePopupLocation();
            }

            void ElementSizeChanged(object sender, SizeChangedEventArgs e3)
            {
                UpdatePopupLocation();
            }

            void ElementLayoutUpdated(object sender, EventArgs e4)
            {
                UpdatePopupLocation();
            }

            void UpdatePopupLocation()
            {
                if (pop != null && pop.IsOpen)
                {
                    var method = typeof(Popup).GetMethod("UpdatePosition", BindingFlags.NonPublic | BindingFlags.Instance);
                    method?.Invoke(pop, null);
                }
            }
        }
    }
}
