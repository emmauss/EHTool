using System;
using System.ComponentModel;
using System.Threading.Tasks;
using EHTool.EHTool.Entities;
using Windows.UI.Xaml.Media.Imaging;
using HtmlAgilityPack;
using System.IO;
using System.Diagnostics;
using Windows.Storage;
using System.Threading;
using System.Text.RegularExpressions;
using Common.Extension;
using Windows.Storage.AccessCache;
using EHTool.EHTool.Common.Helpers;

using static EHTool.Common.Helpers.CookieHelper;
using System.Runtime.CompilerServices;

namespace EHTool.EHTool.Model
{
    public abstract class ImageModel : INotifyPropertyChanged
    {
        #region PropertyChangedMember
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion

        //internal ImageModel(string imagePage,ServerTypes type,string id,int pageindex,bool isinsidapp,string foldertoken,bool isdownloaded)
        //{
        //    ImagePage = imagePage;
        //    _serverType = type;
        //    _id = id;
        //    _pageIndex = pageindex;
        //    _isInsideApp = isinsidapp;
        //    _folderToken = foldertoken;
        //    _isDownload = isdownloaded;
        //    _cancelTokenSource = new CancellationTokenSource();
        //}

        public bool IsLoading { get; protected set; }
        public bool IsFailed { get; protected set; }
        public double TotalSize { get; protected set; } = 1;
        public double DownloadedSize { get; protected set; }
        public string ImagePage { get; protected set; }

        protected CancellationTokenSource _cancelTokenSource;
        protected BitmapImage _image;
        protected int _pageIndex;
        protected string _id;
        protected ServerTypes _serverType;


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
        ~ImageModel()
        {
            _cancelTokenSource?.Dispose();
        }

        public async Task Refresh()
        {
            if (IsLoading)
            {
                _cancelTokenSource.Cancel(true);
            }
            await RefreshOverrideAsync();
            _image = null;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
        }

        protected abstract Task RefreshOverrideAsync();

        public async Task Cancel()
        {
            if (IsLoading)
            {
                _cancelTokenSource.Cancel(true);
                var cachefolder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(_serverType.ToString(), CreationCollisionOption.OpenIfExists);
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
            await Refresh();
        }

        private async Task GetImage()
        {
            if (IsLoading || IsFailed)
            {
                return;
            }
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            await GetImageOverrideAsync();
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }

        protected abstract Task GetImageOverrideAsync();



        protected async Task DownloadToFolder(StorageFolder folder)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Cookie", (_serverType == ServerTypes.ExHentai ? Cookie : null) + Unconfig);
                    for (int j = 0; j < 5; j++)
                    {
                        if (_cancelTokenSource.IsCancellationRequested)
                        {
                            return;
                        }
                        try
                        {
                            await DownloadToFolder(folder, client);
                            break;
                        }
                        catch (System.Net.Http.HttpRequestException)
                        {
                            Debug.WriteLine($"Page {_pageIndex} error,retry {j}");
                            continue;
                        }
                        catch (System.Net.WebException)
                        {
                            Debug.WriteLine($"Page {_pageIndex} error,retry {j}");
                            continue;
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                return;
            }
            if (_image == null)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
            }
        }

        private async Task DownloadToFolder(StorageFolder folder, System.Net.Http.HttpClient client)
        {
            var htmlStr = await client.GetStringAsync(ImagePage);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlStr);
            var m = Regex.Match(htmlStr, @"return nl\('([^\s]*)'\)");
            ImagePage += $"&nl={m.Groups[1].Value}";
            var imageLink = doc.GetElementbyId("img").Attributes["src"].Value;
            var file = await folder.CreateFileAsync($"{_pageIndex}{Path.GetExtension(imageLink)}", CreationCollisionOption.ReplaceExisting);
            using (var res = await client.GetAsync(imageLink, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, _cancelTokenSource.Token))
            {
                if (res.Content.Headers.ContentDisposition != null && res.Content.Headers.ContentDisposition.FileName.Contains("403.gif"))
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
                            while ((bytesRead = await stream.ReadAsync(myBuffer, 0, myBuffer.Length, _cancelTokenSource.Token)) > 0)
                            {
                                await fstream.WriteAsync(myBuffer, 0, bytesRead);
                                DownloadedSize += bytesRead;
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadedSize)));
                            }
                        }
                    }
                    using (var fstream = await file.OpenStreamForReadAsync())
                    {
                        using (var ranstream = fstream.AsRandomAccessStream())
                        {
                            _image = new BitmapImage();
                            await _image.SetSourceAsync(ranstream);
                        }
                    }
                    OnPropertyChanged(nameof(Image));
                    Debug.WriteLine($"Page {_pageIndex} complete");
                }
            }
        }
    }
}
