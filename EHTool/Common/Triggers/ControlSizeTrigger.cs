using Windows.UI.Xaml;

namespace Common.Triggers
{
    public class ControlSizeTrigger : StateTriggerBase
    {
        public double MinWindowWidth
        {
            get { return (double)GetValue(MinWindowWidthProperty); }
            set { SetValue(MinWindowWidthProperty, value); OnValueChanged(); }
        }

        // Using a DependencyProperty as the backing store for MinWindowWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinWindowWidthProperty =
            DependencyProperty.Register("MinWindowWidth", typeof(double), typeof(ControlSizeTrigger), new PropertyMetadata(0d));



        public double MaxWindowWidth
        {
            get { return (double)GetValue(MaxWindowWidthProperty); }
            set { SetValue(MaxWindowWidthProperty, value); OnValueChanged(); }
        }

        // Using a DependencyProperty as the backing store for MaxWindowWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxWindowWidthProperty =
            DependencyProperty.Register("MaxWindowWidth", typeof(double), typeof(ControlSizeTrigger), new PropertyMetadata(0d));



        public double MinWindowHeight
        {
            get { return (double)GetValue(MinWindowHeightProperty); }
            set { SetValue(MinWindowHeightProperty, value); OnValueChanged(); }
        }

        // Using a DependencyProperty as the backing store for MinWindowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinWindowHeightProperty =
            DependencyProperty.Register("MinWindowHeight", typeof(double), typeof(ControlSizeTrigger), new PropertyMetadata(0d));



        public double MaxWindowHeight
        {
            get { return (double)GetValue(MaxWindowHeightProperty); }
            set { SetValue(MaxWindowHeightProperty, value); OnValueChanged(); }
        }

        // Using a DependencyProperty as the backing store for MaxWindowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxWindowHeightProperty =
            DependencyProperty.Register("MaxWindowHeight", typeof(double), typeof(ControlSizeTrigger), new PropertyMetadata(0d));

        private void OnValueChanged()
        {
            if (!IsPhone)
            {
                UpdateTrigger(Window.Current.Bounds.Height, Window.Current.Bounds.Width);
            }
        }

        private bool IsPhone => Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");

        public ControlSizeTrigger()
        {
            if (IsPhone)
            {
                SetActive(false);
            }
            else
            {
                UpdateTrigger(Window.Current.Bounds.Height, Window.Current.Bounds.Width);
                Window.Current.SizeChanged += Current_SizeChanged;
            }
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            var height = e.Size.Height;
            var width = e.Size.Width;
            UpdateTrigger(height, width);
        }

        private void UpdateTrigger(double height, double width)
        {
            if (MinWindowWidth > 0 || MinWindowHeight > 0)
            {
                if (MinWindowHeight > 0 && MinWindowWidth > 0 && MaxWindowHeight > 0 && MaxWindowWidth > 0)
                {
                    SetActive((height >= MinWindowHeight) && (width >= MinWindowWidth) && (height < MaxWindowHeight) && (width < MaxWindowWidth));
                }
                else if (MinWindowHeight > 0)
                {
                    if (MaxWindowHeight > 0)
                    {
                        SetActive(height >= MinWindowHeight && height < MaxWindowHeight);
                    }
                    else
                    {
                        SetActive(height >= MinWindowHeight);
                    }
                }
                else
                {
                    if (MaxWindowWidth > 0)
                    {
                        SetActive(width >= MinWindowWidth && width < MaxWindowWidth);
                    }
                    else
                    {
                        SetActive(width >= MinWindowWidth);
                    }
                }
            }
            else
            {
                SetActive(false);
            }
        }
    }
}
