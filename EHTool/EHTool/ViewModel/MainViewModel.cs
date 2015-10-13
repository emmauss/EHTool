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

using static Common.Helpers.SettingHelper;
using Windows.UI.Xaml;
using Windows.Storage;
using Windows.Storage.AccessCache;
using System.IO;
using Common.Helpers;
using EHTool.Shared;
using EHTool.Shared.Model;
using EHTool.Shared.Entities;

namespace EHTool.EHTool.ViewModel
{
    public class MainViewModel : Gallery, INotifyPropertyChanged
    {
        public bool IsLoading { get; private set; }
        public bool IsFailed { get; private set; }

        public static MainViewModel Current;

        public ObservableCollection<GalleryListModel> MainList { get; private set; } = new ObservableCollection<GalleryListModel>();
        public ObservableCollection<GalleryListModel> FavorList { get; private set; } = new ObservableCollection<GalleryListModel>();
        public ObservableCollection<LocalFolderModel> LocalFolderList { get; private set; } = new ObservableCollection<LocalFolderModel>();
        public GallerySearchOption SearchOption { get; set; } = new GallerySearchOption();
        public ObservableCollection<DownloadItemModel> DownloadList { get; set; } = new ObservableCollection<DownloadItemModel>();

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


        internal MainViewModel() : this(GetSetting<bool>(SettingNames.IsExhentaiMode) ? ServerTypes.ExHentai : ServerTypes.EHentai) { }

        internal MainViewModel(ServerTypes type)
        {
            ServerType = type;
            Current = this;
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
                await LoadFavorList();
                await LoadDownloadList();
                await CheckForDownloadList();
                await LoadLocalFolderList();
                await CheckForLocalFolder();
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
            catch (System.Net.Http.HttpRequestException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        private async Task CheckForLocalFolder()
        {
            if (LocalFolderList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < LocalFolderList.Count; i++)
            {
                StorageFolder folder;
                if (StorageApplicationPermissions.FutureAccessList.ContainsItem(LocalFolderList[i].FolderToken))
                {
                    try
                    {
                        folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(LocalFolderList[i].FolderToken);
                    }
                    catch (FileNotFoundException)
                    {
                        folder = null;
                    }
                }
                else
                {
                    folder = null;
                }
                if (folder == null)
                {
                    await LocalFolderHelper.Remove(LocalFolderList[i]);
                    LocalFolderList.Remove(LocalFolderList[i]);
                }

            }
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
                LoginDialog dialog = new LoginDialog();
                await dialog.ShowAsync();
                if (dialog.IsSuccess)
                {
                    Initialize();
                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }
        internal async Task LoadLocalFolderList()
        {
            LocalFolderList = new ObservableCollection<LocalFolderModel>(await LocalFolderHelper.GetList());
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalFolderList)));
        }
        internal async Task LoadDownloadList()
        {
            DownloadList = new ObservableCollection<DownloadItemModel>(await DownloadHelper.GetDownloadList());
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadList)));
        }

        internal async Task LoadFavorList()
        {
            FavorList = new ObservableCollection<GalleryListModel>(await FavorHelper.GetFavorList());
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FavorList)));
        }

        private async Task CheckForDownloadList()
        {
            if (DownloadList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < DownloadList.Count; i++)
            {
                StorageFolder folder;
                if (DownloadList[i].IsInsideApp)
                {
                    folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(DownloadList[i].ID, CreationCollisionOption.OpenIfExists);
                }
                else if (StorageApplicationPermissions.FutureAccessList.ContainsItem(DownloadList[i].FolderToken))
                {
                    try
                    {
                        folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(DownloadList[i].FolderToken);
                    }
                    catch (FileNotFoundException)
                    {
                        folder = null;
                    }
                }
                else
                {
                    folder = null;
                }
                if (folder == null)
                {
                    await DownloadHelper.RemoveDownload(DownloadList[i], false);
                    DownloadList.Remove(DownloadList[i]);
                }
                else
                {
                    if (GetSetting<bool>(SettingNames.IsAutoDownload) &&
                        (DownloadList[i].DownloadedCount < DownloadList[i].MaxImageCount || DownloadList[i].Items == null))
                    {
                        await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {
                            await DownloadList[i].Start();
                        });
                    }
                }
            }
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
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            catch (System.Net.WebException)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            catch (NullReferenceException)
            {
                IsFailed = true;
                MainList = new ObservableCollection<GalleryListModel>();
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.NoHitsFoundDialogContent);
                await dialog.ShowAsync();
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
                if (MaxPageCount > _currentPage +1)
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
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            catch (System.Net.WebException)
            {
                _currentPage--;
                MessageDialog dialog = new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle);
                await dialog.ShowAsync();
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));

        }
    }
}
