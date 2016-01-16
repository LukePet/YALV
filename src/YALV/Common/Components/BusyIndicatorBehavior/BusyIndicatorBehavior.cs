namespace YALV.Common
{
    #region #using Directives

    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    //using Progress;

    #endregion

    /// <summary>
    /// Refs: http://gallery.expression.microsoft.com/en-us/BusyIndicator
    /// </summary>
    public static class BusyIndicatorBehavior
    {
        /// <summary>
        /// Default animation framerate
        /// </summary>
        public static int? FRAMERATE = 30;

        #region Attached Properties

        public static readonly DependencyProperty AddMarginsProperty = DependencyProperty.RegisterAttached("AddMargins", typeof(bool),
                                                                                                           typeof(BusyIndicatorBehavior),
                                                                                                           new UIPropertyMetadata(false));


        public static readonly DependencyProperty BusyStateProperty = DependencyProperty.RegisterAttached("BusyState", typeof(bool),
                                                                                                          typeof(BusyIndicatorBehavior),
                                                                                                          new UIPropertyMetadata(false, OnBusyStateChanged));

        public static readonly DependencyProperty DimBackgroundProperty = DependencyProperty.RegisterAttached("DimBackground", typeof(bool),
                                                                                                              typeof(BusyIndicatorBehavior),
                                                                                                              new UIPropertyMetadata(true,
                                                                                                                                     OnDimBackgroundChanged));

        public static readonly DependencyProperty DimmerBrushProperty = DependencyProperty.RegisterAttached("DimmerBrush", typeof(Brush),
                                                                                                            typeof(BusyIndicatorBehavior),
                                                                                                            new UIPropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty DimmerOpacityProperty = DependencyProperty.RegisterAttached("DimmerOpacity", typeof(double),
                                                                                                              typeof(BusyIndicatorBehavior),
                                                                                                              new UIPropertyMetadata(0.35));

        public static readonly DependencyProperty DimTransitionDurationProperty = DependencyProperty.RegisterAttached("DimTransitionDuration",
                                                                                                                      typeof(Duration),
                                                                                                                      typeof(BusyIndicatorBehavior),
                                                                                                                      new UIPropertyMetadata(
                                                                                                                        new Duration(TimeSpan.FromSeconds(0.0))));


        public static readonly DependencyProperty TargetVisualProperty = DependencyProperty.RegisterAttached("TargetVisual", typeof(UIElement),
                                                                                                             typeof(BusyIndicatorBehavior),
                                                                                                             new UIPropertyMetadata(null));

        public static double GetDimmerOpacity(DependencyObject obj)
        {
            return (double)obj.GetValue(DimmerOpacityProperty);
        }

        public static void SetDimmerOpacity(DependencyObject obj, double value)
        {
            obj.SetValue(DimmerOpacityProperty, value);
        }

        public static bool GetAddMargins(DependencyObject obj)
        {
            return (bool)obj.GetValue(AddMarginsProperty);
        }

        public static void SetAddMargins(DependencyObject obj, bool value)
        {
            obj.SetValue(AddMarginsProperty, value);
        }

        public static Duration GetDimTransitionDuration(DependencyObject obj)
        {
            return (Duration)obj.GetValue(DimTransitionDurationProperty);
        }

        public static void SetDimTransitionDuration(DependencyObject obj, Duration value)
        {
            obj.SetValue(DimTransitionDurationProperty, value);
        }

        public static Brush GetDimmerBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(DimmerBrushProperty);
        }

        public static void SetDimmerBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(DimmerBrushProperty, value);
        }

        public static bool GetDimBackground(DependencyObject obj)
        {
            return (bool)obj.GetValue(DimBackgroundProperty);
        }

        public static void SetDimBackground(DependencyObject obj, bool value)
        {
            obj.SetValue(DimBackgroundProperty, value);
        }

        public static UIElement GetTargetVisual(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(TargetVisualProperty);
        }

        public static void SetTargetVisual(DependencyObject obj, UIElement value)
        {
            obj.SetValue(TargetVisualProperty, value);
        }

        public static bool GetBusyState(DependencyObject obj)
        {
            return (bool)obj.GetValue(BusyStateProperty);
        }

        public static void SetBusyState(DependencyObject obj, bool value)
        {
            obj.SetValue(BusyStateProperty, value);
        }

        #endregion

        #region Implementation

        private static void OnDimBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool shouldDimBackground = (bool)e.NewValue;
            bool wasDimmingBackground = (bool)e.OldValue;

            if (shouldDimBackground == wasDimmingBackground)
            {
                return;
            }

            if (!GetBusyState(d))
            {
                return;
            }

            var hostGridObject = (GetTargetVisual(d) ?? d);
            Debug.Assert(hostGridObject != null);

            var hostGrid = hostGridObject as Grid;
            if (hostGrid != null)
            {
                var grid = (Grid)LogicalTreeHelper.FindLogicalNode(hostGrid, "BusyIndicator");

                if (grid != null)
                {
                    var dimmer = (Rectangle)LogicalTreeHelper.FindLogicalNode(grid, "Dimmer");

                    if (dimmer != null)
                    {
                        dimmer.Visibility = (shouldDimBackground ? Visibility.Visible : Visibility.Collapsed);
                    }

                    if (shouldDimBackground)
                    {
                        //grid.Cursor = Cursors.Wait;
                        //grid.ForceCursor = true;

                        //InputManager.Current.PreProcessInput += OnPreProcessInput;
                    }
                    else
                    {
                        //grid.Cursor = Cursors.Arrow;
                        //grid.ForceCursor = false;

                        //InputManager.Current.PreProcessInput -= OnPreProcessInput;
                    }
                }
            }
        }

        private static void OnBusyStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isBusy = (bool)e.NewValue;
            bool wasBusy = (bool)e.OldValue;

            if (isBusy == wasBusy)
            {
                return;
            }

            var hostGridObject = (GetTargetVisual(d) ?? d);
            Debug.Assert(hostGridObject != null);

            var hostGrid = hostGridObject as Grid;
            if (hostGrid == null)
            {
                throw new InvalidCastException(
                    string.Format(
                        "The object being attached to must be of type {0}. Try embedding your visual inside a {0} control, and attaching the behavior to the {0} instead.",
                        typeof(Grid).Name));
            }

            if (isBusy)
            {
                Debug.Assert(LogicalTreeHelper.FindLogicalNode(hostGrid, "BusyIndicator") == null);

                bool dimBackground = GetDimBackground(d);
                var grid = new Grid
                            {
                                Name = "BusyIndicator",
                                Opacity = 0.0
                            };
                if (dimBackground)
                {
                    //grid.Cursor = Cursors.Wait;
                    //grid.ForceCursor = true;

                    //InputManager.Current.PreProcessInput += OnPreProcessInput;
                }
                grid.SetBinding(FrameworkElement.WidthProperty, new Binding("ActualWidth")
                                                                    {
                                                                        Source = hostGrid
                                                                    });
                grid.SetBinding(FrameworkElement.HeightProperty, new Binding("ActualHeight")
                                                                    {
                                                                        Source = hostGrid
                                                                    });
                for (int i = 1; i <= 3; ++i)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition
                                                {
                                                    Width = new GridLength(1, GridUnitType.Star)
                                                });
                    grid.RowDefinitions.Add(new RowDefinition
                                                {
                                                    Height = new GridLength(1, GridUnitType.Star)
                                                });
                }

                var viewbox = new Viewbox
                                {
                                    Width = 120.0,
                                    Height = 120.0,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    Stretch = Stretch.Uniform,
                                    StretchDirection = StretchDirection.Both,
                                    Child = new CircularProgressBar()
                                };
                grid.SetValue(Panel.ZIndexProperty, 1000);
                grid.SetValue(Grid.RowSpanProperty, Math.Max(1, hostGrid.RowDefinitions.Count));
                grid.SetValue(Grid.ColumnSpanProperty, Math.Max(1, hostGrid.ColumnDefinitions.Count));
                if (GetAddMargins(d))
                {
                    viewbox.SetValue(Grid.RowProperty, 1);
                    viewbox.SetValue(Grid.ColumnProperty, 1);
                }
                else
                {
                    viewbox.SetValue(Grid.RowSpanProperty, 3);
                    viewbox.SetValue(Grid.ColumnSpanProperty, 3);
                }
                viewbox.SetValue(Panel.ZIndexProperty, 1);

                var dimmer = new Rectangle
                                {
                                    Name = "Dimmer",
                                    Opacity = GetDimmerOpacity(d),
                                    Fill = GetDimmerBrush(d),
                                    Visibility = (dimBackground ? Visibility.Visible : Visibility.Collapsed)
                                };
                dimmer.SetValue(Grid.RowSpanProperty, 3);
                dimmer.SetValue(Grid.ColumnSpanProperty, 3);
                dimmer.SetValue(Panel.ZIndexProperty, 0);
                grid.Children.Add(dimmer);

                grid.Children.Add(viewbox);

                DoubleAnimation animation = new DoubleAnimation(1.0, GetDimTransitionDuration(d));
                Timeline.SetDesiredFrameRate(animation, FRAMERATE);
                grid.BeginAnimation(UIElement.OpacityProperty, animation);

                hostGrid.Children.Add(grid);
            }
            else
            {
                var grid = (Grid)LogicalTreeHelper.FindLogicalNode(hostGrid, "BusyIndicator");

                Debug.Assert(grid != null);

                if (grid != null)
                {
                    grid.Name = string.Empty;

                    var fadeOutAnimation = new DoubleAnimation(0.0, GetDimTransitionDuration(d));
                    Timeline.SetDesiredFrameRate(fadeOutAnimation, FRAMERATE);
                    fadeOutAnimation.Completed += (sender, args) => OnFadeOutAnimationCompleted(d, hostGrid, grid);
                    grid.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                }
            }
        }

        private static void OnPreProcessInput(object sender, PreProcessInputEventArgs e)
        {
            if (e.StagingItem.Input.Device != null)
            {
                e.Cancel();
            }
        }

        private static void OnFadeOutAnimationCompleted(DependencyObject d, Panel hostGrid, UIElement busyIndicator)
        {
            bool dimBackground = GetDimBackground(d);

            hostGrid.Children.Remove(busyIndicator);

            if (dimBackground)
            {
                InputManager.Current.PreProcessInput -= OnPreProcessInput;
            }
        }

        #endregion
    }
}