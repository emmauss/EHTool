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
        }

        public bool IsLoading { get; private set; }
        public bool IsFailed { get; private set; }
        public ulong TotalSize { get; private set; }
        public ulong DownloadedSize { get; private set; }


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
                    GetImageLink();
                    return null;
                }
                return _image;
            }
        }

        public async void Reload()
        {
            var cachefolder = await(await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(ServerType.ToString(), CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync(_id, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync($"{_pageIndex}", CreationCollisionOption.OpenIfExists);
            await file.DeleteAsync();
            GetImageLink();
        }

        private async void GetImageLink()
        {
            if (IsLoading)
            {
                return;
            }
            IsLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            try
            {
                var cachefolder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(ServerType.ToString(), CreationCollisionOption.OpenIfExists);
                cachefolder = await cachefolder.CreateFolderAsync(_id, CreationCollisionOption.OpenIfExists);
                var file = await cachefolder.CreateFileAsync($"{_pageIndex}", CreationCollisionOption.OpenIfExists);
                var fbuf = await FileIO.ReadBufferAsync(file);
                if (fbuf.Length != 0)
                {
                    _image = new BitmapImage(new Uri(file.Path));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
                }
                else
                {
                    string htmlStr;
                    var webRequest = System.Net.WebRequest.CreateHttp(ImagePage);
                    webRequest.Headers["Cookie"] = (ServerType == ServerTypes.ExHentai ? Cookie : null) + Unconfig;
                    using (var webResponse = await webRequest.GetResponseAsync() as System.Net.HttpWebResponse)
                    {
                        using (var getContent = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                        {
                            htmlStr = await getContent.ReadToEndAsync();
                        }
                    }
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlStr);
                    var imageLink = doc.GetElementbyId("img").Attributes["src"].Value;
                    using (var client = new Windows.Web.Http.HttpClient())
                    {
                        using (var res = await client.GetAsync(new Uri(imageLink), Windows.Web.Http.HttpCompletionOption.ResponseHeadersRead))
                        {
                            
                            if (res.Headers.Contains(new KeyValuePair<string, string>("filename", "403.gif")))
                            {
                                IsFailed = true;
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
                                IsLoading = false;
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
                                return;
                            }
                            else
                            {
                                TotalSize = res.Content.Headers.ContentLength.GetValueOrDefault();
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalSize)));
                                using (var fstream = await file.OpenAsync(FileAccessMode.ReadWrite))
                                {
                                    using (var inputstream = await res.Content.ReadAsInputStreamAsync())
                                    {
                                        while (true)
                                        {
                                            var buf = new Windows.Storage.Streams.Buffer(1024);
                                            buf = (Windows.Storage.Streams.Buffer)(await inputstream.ReadAsync(buf, buf.Capacity, Windows.Storage.Streams.InputStreamOptions.None));
                                            if (buf.Length == 0)
                                            {
                                                break;
                                            }
                                            await fstream.WriteAsync(buf);
                                            DownloadedSize = fstream.Size;
                                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadedSize)));
                                        }
                                    }
                                }
                                _image = new BitmapImage(new Uri(file.Path));
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {
                IsFailed = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFailed)));
            }
            IsLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
        }
    }
}
