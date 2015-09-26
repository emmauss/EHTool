using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Helpers;
using EHTool.EHTool.Model;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using EHTool.EHTool.Common;
using EHTool.EHTool.Common.Helpers;
using Windows.Storage.AccessCache;
using EHTool.EHTool.Entities;

namespace EHTool.EHTool.ViewModel
{
    public abstract class ReadingViewModel : INotifyPropertyChanged
    {
        public bool IsLoading { get; protected set; }
        public bool IsFailed { get; protected set; }
        private DispatcherTimer _timer;
        public int TimerInterval { get; set; } = 10;
        private bool _isAutoPlay;
        public bool IsAutoPlay
        {
            get { return _isAutoPlay; }
            set
            {
                _isAutoPlay = value;
                OnAutoPlayPropertyChanged();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAutoPlay)));
            }
        }
        public FlowDirection ReadingDirection 
            => SettingHelpers.GetSetting<bool>("IsReadingRTL") ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public ObservableCollection<ImageModel> ImageList { get; protected set; } = new ObservableCollection<ImageModel>();
        protected int _selectedIndex = -1;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value != _selectedIndex)
                {
                    _selectedIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
                }
            }
        }

        public int MaxPageCount => ImageList.Count - 1;


        public event PropertyChangedEventHandler PropertyChanged;

        internal ReadingViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += TimerTick;
            //LoadList();
        }

        internal async void LoadList()
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            await LoadListOverride();
            if (ImageList != null && ImageList.Count != 0)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageList)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxPageCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
            }
            else
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }



        protected abstract Task LoadListOverride();

        internal void ToNext()
        {
            if (SettingHelpers.GetSetting<bool>("IsReadingDoublePage"))
            {
                if (MaxPageCount > SelectedIndex + 2)
                {
                    SelectedIndex += 2;
                }
            }
            else
            {
                if (MaxPageCount > SelectedIndex + 1)
                {
                    SelectedIndex++;
                }
            }
        }

        internal void ToPrev()
        {
            if (SettingHelpers.GetSetting<bool>("IsReadingDoublePage"))
            {
                if (SelectedIndex - 2 > -1)
                {
                    SelectedIndex -= 2;
                }
            }
            else
            {
                if (SelectedIndex - 1 > -1)
                {
                    SelectedIndex--;
                }
            }
        }

        private void TimerTick(object sender, object e)
        {
            if (SettingHelpers.GetSetting<bool>("IsReadingDoublePage"))
            {
                if (MaxPageCount > SelectedIndex + 2)
                {
                    SelectedIndex += 2;
                }
                else
                {
                    _timer.Stop();
                }
            }
            else
            {
                if (MaxPageCount > SelectedIndex + 1)
                {
                    SelectedIndex++;
                }
                else
                {
                    _timer.Stop();
                }
            }
        }

        private void OnAutoPlayPropertyChanged()
        {
            if (IsAutoPlay)
            {
                _timer.Interval = TimeSpan.FromSeconds(TimerInterval);
                _timer.Start();
            }
            else
            {
                _timer.Stop();
            }
        }

        internal async Task Refresh()
        {
            if (SettingHelpers.GetSetting<bool>("IsReadingDoublePage"))
            {
                await ImageList[SelectedIndex].Refresh();
                if (SelectedIndex + 1 < MaxPageCount)
                {
                    await ImageList[SelectedIndex + 1].Refresh();
                }
            }
            else
            {
                await ImageList[SelectedIndex].Refresh();
            }
        }

        internal async Task CancelTask()
        {
            foreach (var item in ImageList)
            {
                await item.Cancel();
            }
        }

    }
}
