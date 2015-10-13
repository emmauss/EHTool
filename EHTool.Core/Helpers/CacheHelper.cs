using EHTool.Shared.Helpers;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class CacheHelper
    {
        public static async Task SaveTextCache(string fileFolder, string fileName, string fileText)
        {
            var cachefolder = await FileSystem.Current.LocalStorage.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync("textcache", CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync(fileFolder, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(fileText);
        }

        public static async Task<string> GetTextCache(string fileFolder, string fileName)
        {
            var cachefolder = await FileSystem.Current.LocalStorage.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync("textcache", CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync(fileFolder, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            return await file.ReadAllTextAsync();
        }

        public static async Task<string> SaveByteArrayCache(string fileFolder, string fileName, byte[] fileText)
        {
            var cachefolder = await FileSystem.Current.LocalStorage.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync(fileFolder, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (var fstream = await file.OpenAsync(FileAccess.ReadAndWrite))
            {
                using (var stream = new MemoryStream(fileText))
                {
                    await stream.CopyToAsync(fstream);
                }
            }
            return file.Path;
        }

        public static async Task<byte[]> GetByteArrayCache(string fileFolder, string fileName)
        {

            var cachefolder = await FileSystem.Current.LocalStorage.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists);
            cachefolder = await cachefolder.CreateFolderAsync(fileFolder, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenAsync(FileAccess.Read))
            {
                return await ConvertHelper.StreamToBytes(stream);
            }
        }

        public static async Task ClearCache()
        {
            var folder = await FileSystem.Current.LocalStorage.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
            await folder.DeleteAsync();
        }
    }
}
