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
using Windows.UI.Xaml;
using Windows.Storage;
using Windows.Storage.AccessCache;
using System.IO;
using EHTool.Shared;
using EHTool.Shared.Model;
using EHTool.Shared.Entities;
using EHTool.Shared.ViewModelBase;

using static Common.Helpers.SettingHelper;

namespace EHTool.EHTool.ViewModel
{
    public class MainViewModel : GalleryViewModel, INotifyPropertyChanged
    {

        public static MainViewModel Current;
        
        public ObservableCollection<GalleryListModel> FavorList { get; private set; } = new ObservableCollection<GalleryListModel>();
        public ObservableCollection<LocalFolderModel> LocalFolderList { get; private set; } = new ObservableCollection<LocalFolderModel>();
        public ObservableCollection<DownloadItemModel> DownloadList { get; set; } = new ObservableCollection<DownloadItemModel>();


        internal MainViewModel() : this(GetSetting<bool>(SettingNames.IsExhentaiMode) ? ServerTypes.ExHentai : ServerTypes.EHentai)
        {
            
        }

        internal MainViewModel(ServerTypes type) :base(type)
        {
            Current = this;
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

        internal async Task LoadLocalFolderList()
        {
            LocalFolderList = new ObservableCollection<LocalFolderModel>(await LocalFolderHelper.GetList());
            OnPropertyChanged(nameof(LocalFolderList));
        }
        internal async Task LoadDownloadList()
        {
            DownloadList = new ObservableCollection<DownloadItemModel>(await DownloadHelper.GetDownloadList());
            OnPropertyChanged(nameof(DownloadList));
        }

        internal async Task LoadFavorList()
        {
            FavorList = new ObservableCollection<GalleryListModel>(await FavorHelper.GetFavorList());
            OnPropertyChanged(nameof(FavorList));
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
                        await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => await DownloadList[i].Start());
                    }
                }
            }
        }



        protected override async void OnWebErrorOverride()
            => await new MessageDialog(StaticResourceLoader.WebErrorDialogContent, StaticResourceLoader.WebErrorDialogTitle).ShowAsync();

        protected override async void OnExHentaiAccessOverride()
        {
            LoginDialog dialog = new LoginDialog();
            await dialog.ShowAsync();
            if (dialog.IsSuccess)
            {
                Initialize();
            }
        }

        protected override async void OnNoHitOverride() 
            => await new MessageDialog(StaticResourceLoader.NoHitsFoundDialogContent).ShowAsync();

        protected override async Task LoadOtherAsyncOverride()
        {
            await LoadFavorList();
            await LoadDownloadList();
            await CheckForDownloadList();
            await LoadLocalFolderList();
            await CheckForLocalFolder();
        }
    }
}
