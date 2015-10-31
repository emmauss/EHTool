using Common.Extension;
using EHTool.EHTool.Common;
using EHTool.EHTool.Common.Helpers;
using EHTool.EHTool.Entities;
using EHTool.Shared;
using EHTool.Shared.Entities;
using EHTool.Shared.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using static EHTool.Shared.Helpers.CookieHelper; 

namespace EHTool.EHTool.Model
{
    [KnownType(typeof(DownloadItemModel))]
    [DataContract]
    public class DownloadItemModel : GalleryListModel
    {
        [DataMember]
        public string FolderToken { get; internal set; }
        [DataMember]
        public bool IsInsideApp { get; set; }
        [DataMember]
        public int MaxImageCount { get; internal set; }
        [DataMember]
        public int DownloadedCount { get; internal set; }
        [DataMember]
        public List<DownloadItemPagesModel> Items { get; internal set; }
        [IgnoreDataMember]
        private CancellationTokenSource _cancelTokenSource;
        [IgnoreDataMember]
        public bool IsDownloading { get; set; }

        internal DownloadItemModel()
        {

        }

        internal DownloadItemModel(GalleryListModel item)
        {
            FileCount = item.FileCount;
            ID = item.ID;
            ImageLink = item.ImageLink;
            Link = item.Link;
            ServerType = item.ServerType;
            Title = item.Title;
            Token = item.Token;
            Type = item.Type;
            IsInsideApp = true;
            //FolderToken = folderToken;
        }

        internal DownloadItemModel(GalleryListModel item,string folderToken)
        {
            FileCount = item.FileCount;
            ID = item.ID;
            ImageLink = item.ImageLink;
            Link = item.Link;
            ServerType = item.ServerType;
            Title = item.Title;
            Token = item.Token;
            Type = item.Type;
            FolderToken = folderToken;
            IsInsideApp = false;
        }

        ~DownloadItemModel()
        {
            _cancelTokenSource?.Dispose();
        }

        internal void Pause()
        {
            if (IsDownloading)
            {
                _cancelTokenSource.Cancel(true);
                IsDownloading = false;
                OnPropertyChanged(nameof(IsDownloading));
            }
        }

        internal async Task Start()
        {
            if (IsDownloading)
            {
                return;
            }
            IsDownloading = true;
            OnPropertyChanged(nameof(IsDownloading));
            Debug.WriteLine($"{Title} downloading");
            _cancelTokenSource = new CancellationTokenSource();
            if (Items == null)
                Items = new List<DownloadItemPagesModel>();

            if (Items.Count < MaxImageCount || MaxImageCount == 0)
            {
                Debug.WriteLine("Getting image list...");
                try
                {
                    GalleryDetail detail = new GalleryDetail(ID, Token, ServerType);
                    var list = await detail.GetImagePageList();
                    foreach (var item in list)
                    {
                        Items.Add(new DownloadItemPagesModel(item));
                    }
                    MaxImageCount = Items.Count;
                    OnPropertyChanged(nameof(MaxImageCount));
                    await DownloadHelper.AlterDownload(this);
                }
                catch
                {
                    IsDownloading = false;
                    OnPropertyChanged(nameof(IsDownloading));
                    return;
                }
            }
            StorageFolder folder;
            if (IsInsideApp)
            {
                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(ID, CreationCollisionOption.OpenIfExists);
            }
            else
            {
                folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(FolderToken);
            }
            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Add("Cookie", (ServerType == ServerTypes.ExHentai ? Cookie : null) + Unconfig);
                for (int i = 0; i < Items.Count; i++)
                {
                    //if (_cancelTokenSource.IsCancellationRequested)
                    //{
                    //    var file = await folder.GetFileWithoutExtensionAsync($"{i}");
                    //    await file.DeleteAsync();
                    //    return;
                    //}
                    if (Items[i].State != DownloadState.Complete)
                    {
                        Items[i].State = DownloadState.Downloading;

                        if (!await GetFromCache(folder,i))
                        {
                            var imagePage = Items[i].Link;
                            for (int j = 0; j < 5; j++)
                            {
                                //if (_cancelTokenSource.IsCancellationRequested)
                                //{
                                //    var file = await folder.GetFileWithoutExtensionAsync($"{i}");
                                //    await file.DeleteAsync();
                                //    return;
                                //}
                                try
                                {
                                    imagePage = await GetImage(folder, client, i, imagePage);
                                    break;
                                }
                                catch (System.Net.Http.HttpRequestException)
                                {
                                    Debug.WriteLine($"Page {i} error,retry {j}");
                                    continue;
                                }
                                catch (System.Net.WebException)
                                {
                                    Debug.WriteLine($"Page {i} error,retry {j}");
                                    continue;
                                }
                                catch (TaskCanceledException)
                                {
                                    var file = await folder.GetFileWithoutExtensionAsync($"{i}");
                                    await file.DeleteAsync();
                                    return;
                                }
                            }

                        }
                        if (Items[i].State != DownloadState.Complete)
                        {
                            Items[i].State = DownloadState.Error;
                        }
                    }
                }
            }
            IsDownloading = false;
            OnPropertyChanged(nameof(IsDownloading));
        }

        private async Task<string> GetImage(StorageFolder folder, System.Net.Http.HttpClient client, int i, string imagePage)
        {
            var htmlStr = await client.GetStringAsync(imagePage);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlStr);
            var m = Regex.Match(htmlStr, @"return nl\('([^\s]*)'\)");
            imagePage += $"&nl={m.Groups[1].Value}";
            var imageLink = doc.GetElementbyId("img").Attributes["src"].Value;
            var file = await folder.CreateFileAsync($"{i}{Path.GetExtension(imageLink)}", CreationCollisionOption.ReplaceExisting);
            using (var res = await client.GetAsync(imageLink, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, _cancelTokenSource.Token))
            {
                if (res.Content.Headers.ContentDisposition != null && res.Content.Headers.ContentDisposition.FileName.Contains("403.gif"))
                {
                    throw new System.Net.WebException();
                }
                else
                {
                    using (var fstream = await file.OpenStreamForWriteAsync())
                    {
                        using (var stream = await res.Content.ReadAsStreamAsync())
                        {
                            await stream.CopyToAsync(fstream, 81920, _cancelTokenSource.Token);
                        }
                    }
                }
                Items[i].State = DownloadState.Complete;
                DownloadedCount = Items.Count((a) => a.State == DownloadState.Complete);
                OnPropertyChanged(nameof(DownloadedCount));
                await DownloadHelper.AlterDownload(this);
                Debug.WriteLine($"Page {i} complete");
            }

            return imagePage;
        }

        private async Task<bool> GetFromCache(StorageFolder folder,int i)
        {
            var cachefolder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(ServerType.ToString(), CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync(ID, CreationCollisionOption.OpenIfExists);
            var cachefile = await cachefolder.GetFileWithoutExtensionAsync($"{i}");
            var fileprops = cachefile == null ? null : await cachefile.GetBasicPropertiesAsync();
            var filesize = fileprops?.Size;
            if (filesize != null && filesize != 0)
            {
                await cachefile.CopyAsync(folder);
                DownloadedCount++;
                OnPropertyChanged(nameof(DownloadedCount));
                Items[i].State = DownloadState.Complete;
                await DownloadHelper.AlterDownload(this);
                Debug.WriteLine($"Page {i} complete");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}