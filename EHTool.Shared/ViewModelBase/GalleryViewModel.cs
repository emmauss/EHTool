using EHTool.Shared.Entities;
using EHTool.Shared.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using static Common.Helpers.SettingHelper;

namespace EHTool.Shared.ViewModelBase
{
    public abstract class GalleryViewModel : Gallery, INotifyPropertyChanged
    {
        protected abstract void OnWebErrorOverride();
        protected abstract void OnExHentaiAccessOverride();
        protected abstract void OnNoHitOverride();
        protected abstract Task LoadOtherAsyncOverride();
        protected void OnPropertyChanged([CallerMemberName]string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public bool IsLoading { get; protected set; }
        public bool IsFailed { get; protected set; }

        public ObservableCollection<GalleryListModel> MainList { get; protected set; } = new ObservableCollection<GalleryListModel>();
        public GallerySearchOption SearchOption { get; set; } = new GallerySearchOption();

        public event PropertyChangedEventHandler PropertyChanged;
        private CurrentState _currentState = CurrentState.MainList;
        private int _currentPage = 0;
        public bool IsExhentaiMode
        {
            get
            {
                return GetSetting<bool>(SettingNames.IsExhentaiMode);
            }
            set
            {
                SetSetting(SettingNames.IsExhentaiMode, value);
                ServerType = value ? ServerTypes.ExHentai : ServerTypes.EHentai;
                Initialize();
            }
        }


        internal GalleryViewModel() : this(GetSetting<bool>(SettingNames.IsExhentaiMode) ? ServerTypes.ExHentai : ServerTypes.EHentai) { }

        internal GalleryViewModel(ServerTypes type)
        {
            ServerType = type;
            Initialize();
        }

        internal async void Initialize()
        {
            _currentState = CurrentState.MainList;
            _currentPage = 0;
            IsLoading = true;
            IsFailed = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                await LoadOtherAsyncOverride();
                MainList = new ObservableCollection<GalleryListModel>(await GetGalleryList());
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MainList)));
            }
            catch (ExHentaiAccessException)
            {
                IsLoading = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
                OnExHentaiAccessOverride();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                OnWebErrorOverride();
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                OnWebErrorOverride();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        internal async Task Refresh()
        {
            _currentState = CurrentState.MainList;
            _currentPage = 0;
            IsLoading = true;
            IsFailed = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                MainList = new ObservableCollection<GalleryListModel>(await GetGalleryList());
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MainList)));
            }
            catch (ExHentaiAccessException)
            {
                IsLoading = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
                OnExHentaiAccessOverride();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                OnWebErrorOverride();
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                OnWebErrorOverride();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }
        internal async Task Search(string keyword)
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
            catch (System.Net.Http.HttpRequestException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                OnWebErrorOverride();
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                OnWebErrorOverride();
            }
            catch (NullReferenceException)
            {
                IsFailed = true;
                MainList = new ObservableCollection<GalleryListModel>();
                OnNoHitOverride();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        internal async Task LoadMore()
        {
            if (IsLoading || IsFailed)
            {
                return;
            }
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            IEnumerable<GalleryListModel> list = null;
            try
            {
                if (MaxPageCount > _currentPage + 1)
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
                    foreach (var item in list)
                    {
                        MainList.Add(item);
                    }
                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                _currentPage--;
                OnWebErrorOverride();
            }
            catch (System.Net.WebException)
            {
                _currentPage--;
                OnWebErrorOverride();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));

        }
    }
}
