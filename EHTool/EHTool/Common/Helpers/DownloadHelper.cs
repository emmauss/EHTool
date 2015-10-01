using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHTool.EHTool.Model;
using Windows.Data.Json;
using Common.Extension;
using EHTool.EHTool.Entities;
using Windows.Storage;
using Windows.Storage.AccessCache;

using static Common.Helpers.JsonHelper;
using static EHTool.EHTool.Common.Helpers.DatabaseHelper;

namespace EHTool.EHTool.Common.Helpers
{
    public static class DownloadHelper
    {
        private const string SAVE_DATABASE_NAME = "Download.json";

        public static async Task<IEnumerable<DownloadItemModel>> GetDownloadList()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SAVE_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            return jsStr != "" ? FromJson(jsStr) : new List<DownloadItemModel>();
        }
        public static async Task AddDownload(DownloadItemModel item)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SAVE_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<DownloadItemModel>>(jsStr) : new List<DownloadItemModel>();
            var index = items.FindIndex((a) => a.ID == item.ID);
            if (index == -1)
            {
                items.Insert(0,item);
                jsStr = ToJson(items);
                await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            }
        }
        public static async Task RemoveDownload(DownloadItemModel item,bool isDeleteFolder = true)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SAVE_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<DownloadItemModel>>(jsStr) : new List<DownloadItemModel>();
            var index = items.FindIndex((a) => a.ID == item.ID);
            if (index != -1)
            {
                if (isDeleteFolder)
                {
                    StorageFolder folder;
                    if (items[index].IsInsideApp)
                    {
                        folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(items[index].ID, CreationCollisionOption.OpenIfExists);
                    }
                    else
                    {
                        folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(items[index].FolderToken);
                    }
                    await folder.DeleteAsync();
                }
                items.RemoveAt(index);
                jsStr = ToJson(items);
                await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            }
        }

        public static async Task<DownloadItemModel> GetItem(string id)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SAVE_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<DownloadItemModel>>(jsStr) : new List<DownloadItemModel>();
            var index = items.FindIndex((a) => a.ID == id);
            return index != -1 ? items[index] : null;
        }

        public static async Task AlterDownload(string id,int pageIndex,DownloadState state)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SAVE_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<DownloadItemModel>>(jsStr) : new List<DownloadItemModel>();
            var index = items.FindIndex((a) => a.ID == id);
            if (index != -1)
            {
                items[index].Items[pageIndex].State = state;
                items[index].DownloadedCount = items[index].Items.Count((a) => a.State == DownloadState.Complete);
                jsStr = ToJson(items);
                await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            }
        }

        public static async Task AlterDownload(DownloadItemModel item)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SAVE_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<DownloadItemModel>>(jsStr) : new List<DownloadItemModel>();
            var index = items.FindIndex((a) => a.ID == item.ID);
            if (index != -1)
            {
                items[index] = item;
                jsStr = ToJson(items);
                await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            }
        }
        public static async Task<bool> IsDownload(DownloadItemModel item)
        {
            return await IsExists(item, SAVE_DATABASE_NAME);
        }
        public static async Task<bool> IsDownload(string id)
        {
            return await IsExists(id, SAVE_DATABASE_NAME);
        }

        private static IEnumerable<DownloadItemModel> FromJson(string json)
            => from item in JsonArray.Parse(json)
               select new DownloadItemModel
               {
                   ServerType = (ServerTypes)item.GetNamedNumber("ServerType"),
                   ImageLink = item.GetNamedString("ImageLink"),
                   FileCount = item.GetNamedString("FileCount"),
                   IsInsideApp = item.GetNamedBoolean("IsInsideApp"),
                   ID = item.GetNamedString("ID"),
                   Link = item.GetNamedString("Link"),
                   Title = item.GetNamedString("Title"),
                   Token = item.GetNamedString("Token"),
                   Type = item.GetNamedString("Type"),
                   FolderToken = item.GetNamedString("FolderToken"),
                   DownloadedCount = (int)item.GetNamedNumber("DownloadedCount"),
                   MaxImageCount = (int)item.GetNamedNumber("MaxImageCount"),
                   Items = item.GetNamedArray("Items") != null ?
                   (from a in item.GetNamedArray("Items")
                    select new DownloadItemPagesModel
                    {
                        Link = a.GetNamedString("Link"),
                        State = (DownloadState)a.GetNamedNumber("State"),
                    })?.ToList() : null,
               };
    }
}
