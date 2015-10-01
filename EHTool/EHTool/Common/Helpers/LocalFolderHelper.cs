using Common.Extension;
using EHTool.EHTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using static Common.Helpers.JsonHelper;
using static EHTool.EHTool.Common.Helpers.DatabaseHelper;

namespace EHTool.EHTool.Common.Helpers
{
    public static class LocalFolderHelper
    {
        private const string LOCALFOLDER_DATABASE_NAME = "LocalFolder.json";
        public static async Task<IEnumerable<LocalFolderModel>> GetList()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(LOCALFOLDER_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            return jsStr != "" ? FromJson<List<LocalFolderModel>>(jsStr) : new List<LocalFolderModel>();
        }

        public static async Task Add(string token,string name)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(LOCALFOLDER_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<LocalFolderModel>>(jsStr) : new List<LocalFolderModel>();
            var index = items.FindIndex((a) => a.FolderToken == token);
            if (index == -1)
            {
                items.Insert(0, new LocalFolderModel() { FolderToken = token, FolderName = name, });
                jsStr = ToJson(items);
                await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            }
        }

        public static async Task Remove(LocalFolderModel item)
        {
            await Remove(item.FolderToken);
        }

        public static async Task Remove(string folderToken)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(LOCALFOLDER_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<LocalFolderModel>>(jsStr) : new List<LocalFolderModel>();
            var index = items.FindIndex((a) => a.FolderToken == folderToken);
            if (index != -1)
            {
                items.RemoveAt(index);
                jsStr = ToJson(items);
                await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            }
        }


    }
}
