using Windows.UI.Xaml;

namespace Common.Triggers
{
    public class ControlSizeTrigger : StateTriggerBase
    {
        private double _minHeight, _minWidth = -1;
        private FrameworkElement _targetElement;
        private double _currentHeight, _currentWidth;

        public double MinHeight
        {
            get
            {
                return _minHeight;
            }
            set
            {
                _minHeight = value;
            }
        }
        public double MinWidth
        {
            get
            {
                return _minWidth;
            }
            set
            {
                _minWidth = value;
            }
        }
        public FrameworkElement TargetElement
        {
            get
            {
                return _targetElement;
            }
            set
            {
                _targetElement = value;
                _targetElement.SizeChanged += _targetElement_SizeChanged;
            }
        }

        private void _targetElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _currentHeight = e.NewSize.Height;
            _currentWidth = e.NewSize.Width;
            UpdateTrigger();
        }

        private void UpdateTrigger()
        {

            if (_targetElement != null && (_minWidth > 0 || _minHeight > 0))
            {
                if (_minHeight > 0 && _minWidth > 0)
                {
                    SetActive((_currentHeight >= _minHeight) && (_currentWidth >= _minWidth));
                }
                else if (_minHeight > 0)
                {
                    SetActive(_currentHeight >= _minHeight);
                }
                else
                {
                    SetActive(_currentWidth >= _minWidth);
                }
            }
            else
            {
                SetActive(false);
            }
        }
    }
}
