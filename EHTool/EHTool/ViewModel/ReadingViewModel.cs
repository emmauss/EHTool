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

        internal ReadingViewModel(LocalFolderModel item)
        {
            LoadList(item);
        }

        private async void LoadList(LocalFolderModel item)
        {
            var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(item.FolderToken);
            var list = await folder.GetFilesAsync();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ContentType.ToLower().Contains("image"))
                {
                    ImageList.Add(new LocalImageModel(list[i]));
                }
            }
            if (ImageList.Count == 0)
            {
                MessageDialog dialog = new MessageDialog("Can not find any image in this folder", "Error");
                await dialog.ShowAsync();
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxPageCount)));
        }

        internal ReadingViewModel(DownloadItemModel item)
        {
            _timer = new DispatcherTimer();
            _timer.Tick += TimerTick;
            _id = item.ID;
            if (item.Items == null)
            {
                LoadList(item);
            }
            else
            {
                for (int i = 0; i < item.Items.Count; i++)
                {
                    ImageList.Add(new DownloadedImageModel(item, item.Items[i].Link, i));
                }
            }
        }

        private async void LoadList(DownloadItemModel item)
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                GalleryDetail detail = new GalleryDetail(item.ID, item.Token, item.ServerType);
                var list = (await detail.GetImagePageList()).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    ImageList.Add(new DownloadedImageModel(item, list[i].ImagePage, i));
                }
                SelectedIndex = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxPageCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
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

        internal ReadingViewModel(Task<IEnumerable<ImageListModel>> task, string id)
        {
            _id = id;
            _task = task;
            _timer = new DispatcherTimer();
            _timer.Tick += TimerTick;
            LoadList();
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

        private async Task LoadList()
        {
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                var isdownloaded = await DownloadHelper.IsDownload(_id);
                var list = (await _task).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    ImageList.Add(new CommonImageModel(list[i],_id,i));
                }
                SelectedIndex = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxPageCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
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
