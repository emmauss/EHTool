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

namespace EHTool.EHTool.ViewModel
{
    public class ReadingViewModel : INotifyPropertyChanged
    {
        public bool IsLoading { get; set; }
        public bool IsFailed { get; set; }
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

        public ObservableCollection<ImageModel> ImageList { get; private set; } = new ObservableCollection<ImageModel>();
        private int _selectedIndex = -1;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (!IsLoading && value != _selectedIndex)
                {
                    _selectedIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
                }
            }
        }

        public int MaxPageCount => ImageList.Count - 1;


        public event PropertyChangedEventHandler PropertyChanged;
        private string _id;
        private Task<IEnumerable<ImageListModel>> _task;

        public ReadingViewModel(Task<IEnumerable<ImageListModel>> task, string id)
        {
            _id = id;
            _task = task;
            _timer = new DispatcherTimer();
            _timer.Tick += TimerTick;
        }

        private void TimerTick(object sender, object e)
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

        public async Task CancelTask()
        {
            foreach (var item in ImageList)
            {
                await item.Cancel();
            }
        }

        public async Task LoadList()
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {

                var list = (await _task).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    ImageList.Add(new ImageModel(list[i], i, _id));
                }
                SelectedIndex = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxPageCount)));
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog("Can not connect to server", "Web Error");
                await dialog.ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }


    }
}
