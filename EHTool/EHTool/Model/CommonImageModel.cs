using Common.Extension;
using EHTool.EHTool.Entities;
using EHTool.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace EHTool.EHTool.Model
{
    public class CommonImageModel : ImageModel
    {
        internal CommonImageModel(ImageListModel item,string id,int pageIndex)
        {
            _id = id;
            _pageIndex = pageIndex;
            ImagePage = item.ImagePage;
            _serverType = item.ServerType;
            _cancelTokenSource = new CancellationTokenSource();
        }
        protected override async Task GetImageOverrideAsync()
        {
            await GetUnDownloadedImage();
        }

        protected override async Task RefreshOverrideAsync()
        {
            var cachefolder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(_serverType.ToString(), CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync(_id, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync($"{_pageIndex}", CreationCollisionOption.OpenIfExists);
            await file.DeleteAsync();
        }
        private async Task GetUnDownloadedImage()
        {
            var cachefolder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(_serverType.ToString(), CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync(_id, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.GetFileWithoutExtensionAsync($"{_pageIndex}");
            //if current file is now downloading,it will throw UnauthorizedAccessException
            var fileprops = file == null ? null : await file.GetBasicPropertiesAsync();
            var filesize = fileprops?.Size;
            if (filesize != null && filesize != 0)
            {
                _image = new BitmapImage(new Uri(file.Path));
                OnPropertyChanged(nameof(Image));
            }
            else
            {
                await DownloadToFolder(cachefolder);
            }
        }


    }
}
