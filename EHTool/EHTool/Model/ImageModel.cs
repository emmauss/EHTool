using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHTool.EHTool.Entities;
using Windows.UI.Xaml.Media.Imaging;

using static EHTool.Common.Helpers.CookieHelper;
using static Common.Helpers.HttpHelper;
using HtmlAgilityPack;
using System.IO;
using System.Diagnostics;
using Windows.Storage;
using Windows.Foundation;
using Windows.UI.Xaml;
using System.Threading;

namespace EHTool.EHTool.Model
{
    public class ImageModel : INotifyPropertyChanged
    {
        public ImageModel(ImageListModel item, int pageIndex, string id)
        {
            ImagePage = item.ImagePage;
            ServerType = item.ServerType;
            _pageIndex = pageIndex;
            _id = id;
            _cancelTokenSource = new CancellationTokenSource();
        }

        public bool IsLoading { get; private set; }
        public bool IsFailed { get; private set; }
        public double TotalSize { get; private set; }
        public double DownloadedSize { get; private set; }
        private CancellationTokenSource _cancelTokenSource;


        public string ImagePage { get; private set; }
        public ServerTypes ServerType { get; private set; }
        private BitmapImage _image;
        private int _pageIndex;
        private string _id;


        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapImage Image
        {
            get
            {
                if (_image == null)
                {
                    GetImage();
                }
                return _image;
            }
        }

        public async Task Cancel()
        {
            if (IsLoading)
            {
                _cancelTokenSource.Cancel(false);
                var cachefolder = await(await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(ServerType.ToString(), CreationCollisionOption.OpenIfExists);
                cachefolder = await cachefolder.CreateFolderAsync(_id, CreationCollisionOption.OpenIfExists);
                var file = await cachefolder.CreateFileAsync($"{_pageIndex}", CreationCollisionOption.OpenIfExists);
                await file.DeleteAsync();
            }
        }

        public async void ReloadClick()
        {
            Debug.WriteLine($"page {_pageIndex} clicked");
            await Reload();
        }

        public async Task Reload()
        {
            IsFailed = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
            var cachefolder = await(await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(ServerType.ToString(), CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync(_id, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync($"{_pageIndex}", CreationCollisionOption.OpenIfExists);
            await file.DeleteAsync();
            await GetImage();
        }

        private async Task GetImage()
        {
            if (IsLoading)
            {
                return;
            }
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            var cachefolder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(ServerType.ToString(), CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync(_id, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync($"{_pageIndex}", CreationCollisionOption.OpenIfExists);
            //if current file is now downloading,it will throw UnauthorizedAccessException
            var fbuf = await FileIO.ReadBufferAsync(file);
            if (fbuf.Length != 0)
            {
                _image = new BitmapImage(new Uri(file.Path));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
            }
            else
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Cookie", (ServerType == ServerTypes.ExHentai ? Cookie : null) + Unconfig);
                    var htmlStr = await client.GetStringAsync(ImagePage);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlStr);
                    var imageLink = doc.GetElementbyId("img").Attributes["src"].Value;
                    using (var res = await client.GetAsync(imageLink, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, _cancelTokenSource.Token))
                    {
                        if (res.Content.Headers.ContentDisposition?.FileName == "403.gif")
                        {
                            throw new System.Net.WebException();
                        }
                        else
                        {
                            TotalSize = res.Content.Headers.ContentLength.GetValueOrDefault();
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalSize)));
                            using (var fstream = await file.OpenStreamForWriteAsync())
                            {
                                using (var stream = await res.Content.ReadAsStreamAsync())
                                {
                                    int bytesRead = 0;
                                    byte[] myBuffer = new byte[1024 * 1024];
                                    while ((bytesRead = await stream.ReadAsync(myBuffer, 0, myBuffer.Length)) > 0)
                                    {
                                        if (_cancelTokenSource.IsCancellationRequested)
                                        {
                                            return;
                                        }
                                        else
                                        {
                                            await fstream.WriteAsync(myBuffer, 0, bytesRead);
                                            DownloadedSize += bytesRead;
                                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadedSize)));
                                        }
                                    }
                                }
                            }
                            Debug.WriteLine($"page {_pageIndex} complete");
                            _image = new BitmapImage(new Uri(file.Path));
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
                        }
                    }
                }

            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }
    }
}
