using Common.Extension;
using EHTool.EHTool.Common.Helpers;
using EHTool.EHTool.Entities;
using EHTool.EHTool.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml.Media.Imaging;

namespace EHTool.EHTool.Model
{
    public class DownloadedImageModel : ImageModel
    {
        private bool _isInsideApp;
        private string _folderToken;

        internal DownloadedImageModel(DownloadItemModel item,string imagePage, int pageIndex)
        {
            ImagePage = imagePage;
            _serverType = item.ServerType;
            _isInsideApp = item.IsInsideApp;
            _id = item.ID;
            _folderToken = item.FolderToken;
            _pageIndex = pageIndex;
            _cancelTokenSource = new CancellationTokenSource();
        }

        protected override async Task GetImageOverrideAsync()
        {
            await GetDownloadedImage();
        }

        protected override async Task RefreshOverrideAsync()
        {
            StorageFolder folder = _isInsideApp ? await ApplicationData.Current.LocalFolder.CreateFolderAsync(_id, CreationCollisionOption.OpenIfExists) : await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(_folderToken);
            var downloadedfile = await folder.GetFileWithoutExtensionAsync($"{_pageIndex}");
            if (downloadedfile != null)
            {
                await downloadedfile.DeleteAsync();
            }
        }

        private async Task GetDownloadedImage()
        {
            StorageFolder folder;
            if (_isInsideApp)
            {
                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(_id, CreationCollisionOption.OpenIfExists);
            }
            else
            {
                folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(_folderToken);
            }
            var downloadedfile = await folder.GetFileWithoutExtensionAsync($"{_pageIndex}");
            var downloadedfileprops = downloadedfile == null ? null : await downloadedfile.GetBasicPropertiesAsync();
            var downloadedfilesize = downloadedfileprops?.Size;
            if (downloadedfilesize != null && downloadedfilesize != 0)
            {
                using (var fstream = await downloadedfile.OpenStreamForReadAsync())
                {
                    using (var ranstream = fstream.AsRandomAccessStream())
                    {
                        _image = new BitmapImage();
                        await _image.SetSourceAsync(ranstream);
                    }
                }
                OnPropertyChanged(nameof(Image));
            }
            else
            {
                await DownloadToFolder(folder);
                if (!_cancelTokenSource.Token.IsCancellationRequested)
                {
                    await DownloadHelper.AlterDownload(_id, _pageIndex, DownloadState.Complete);
                    var index = MainViewModel.Current.DownloadList.ToList().FindIndex((item) => { return item.ID == _id; });
                    MainViewModel.Current.DownloadList[index].Items[_pageIndex].State = DownloadState.Complete;
                    MainViewModel.Current.DownloadList[index].DownloadedCount = MainViewModel.Current.DownloadList[index].Items.Count((a) => { return a.State == DownloadState.Complete; });
                }
                else
                {
                    var file = await folder.GetFileWithoutExtensionAsync($"{_pageIndex}");
                    await file.DeleteAsync();
                }
            }
        }
    }
}
