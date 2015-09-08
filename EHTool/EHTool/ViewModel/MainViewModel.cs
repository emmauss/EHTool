using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using EHTool.EHTool.Common;
using EHTool.EHTool.Common.Helpers;
using EHTool.EHTool.Entities;
using EHTool.EHTool.Model;
using EHTool.EHTool.View;
using Windows.UI.Popups;

using static Common.Helpers.SettingHelpers;

namespace EHTool.EHTool.ViewModel
{
    public class MainViewModel : Gallery, INotifyPropertyChanged
    {
        public bool IsLoading { get; private set; }
        public bool IsFailed { get; private set; }

        public ObservableCollection<GalleryListModel> MainList { get; private set; } = new ObservableCollection<GalleryListModel>();
        public ObservableCollection<GalleryListModel> FavorList { get; private set; } = new ObservableCollection<GalleryListModel>();
        public GallerySearchOption SearchOption { get; set; } = new GallerySearchOption();

        public event PropertyChangedEventHandler PropertyChanged;

        private CurrentState _currentState = CurrentState.MainList;
        private int _currentPage = 0;
        public bool IsExhentaiMode
        {
            get
            {
                return GetSetting<bool>("IsExhentaiMode");
            }
            set
            {
                SetSetting("IsExhentaiMode", value);
                ServerType = value ? ServerTypes.ExHentai : ServerTypes.EHentai;
                Initialize();
            }
        }


        public MainViewModel() : this(GetSetting<bool>("IsExhentaiMode") ? ServerTypes.ExHentai : ServerTypes.EHentai) { }

        public MainViewModel(ServerTypes type)
        {
            ServerType = type;
            Initialize();
        }

        public async void Initialize()
        {
            _currentState = CurrentState.MainList;
            _currentPage = 0;
            IsLoading = true;
            IsFailed = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                await LoadFavorList();
                MainList = new ObservableCollection<GalleryListModel>(await GetGalleryList());
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MainList)));
            }
            catch (ExHentaiAccessException)
            {
                IsLoading = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
                LoginDialog dialog = new LoginDialog();
                await dialog.ShowAsync();
                if (dialog.IsSuccess)
                {
                    Initialize();
                }
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog($"Can not connect to {ServerType}", "Web Error");
                await dialog.ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        public async Task LoadFavorList()
        {
            FavorList = new ObservableCollection<GalleryListModel>(await FavorHelper.GetFavorList());
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FavorList)));
        }

        public async Task Search(string keyword)
        {
            _currentState = CurrentState.Search;
            _currentPage = 0;
            IsLoading = true;
            IsFailed = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                SearchOption.KeyWord = keyword;
                MainList = new ObservableCollection<GalleryListModel>(await GetGalleryList(SearchOption));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MainList)));
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog($"Can not connect to {ServerType}", "Web Error");
                await dialog.ShowAsync();
            }
            catch (NullReferenceException)
            {
                IsFailed = true;
                MainList = new ObservableCollection<GalleryListModel>();
                MessageDialog dialog = new MessageDialog($"No hits found");
                await dialog.ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }
        public async Task LoadMore()
        {
            if (IsFailed)
            {
                return;
            }
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            IEnumerable<GalleryListModel> list = null;
            try
            {
                switch (_currentState)
                {
                    case CurrentState.MainList:
                        list = await GetGalleryList(++_currentPage);
                        break;
                    case CurrentState.Search:
                        list = await GetGalleryList(SearchOption, ++_currentPage);
                        break;
                }
            }
            catch (System.Net.WebException)
            {
                _currentPage--;
                MessageDialog dialog = new MessageDialog($"Can not connect to {ServerType}", "Web Error");
                await dialog.ShowAsync();
            }
            if (list != null)
            {
                foreach (var item in list)
                {
                    MainList.Add(item);
                }
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));

        }
    }
}
