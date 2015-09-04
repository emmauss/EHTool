using System;
using System.IO;
using System.Threading.Tasks;
using Common.Converters;
using Windows.Storage;

namespace Common.Helpers
{
    public static class CacheHelper
    {
        public static async Task SaveTextCache(string fileFolder, string fileName, string fileText)
        {
            var cachefolder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("textcache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(fileFolder, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, fileText);
        }

        public static async Task<string> GetTextCache(string fileFolder, string fileName)
        {
            var cachefolder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("textcache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(fileFolder, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            return await FileIO.ReadTextAsync(file);
        }

        public static async Task SaveByteArrayCache(string fileFolder, string fileName, byte[] fileText)
        {
            var cachefolder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(fileFolder, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(file, fileText);
        }

        public static async Task<byte[]> GetByteArrayCache(string fileFolder, string fileName)
        {
            var cachefolder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("bytescache", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(fileFolder, CreationCollisionOption.OpenIfExists);
            var file = await cachefolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            using (var stream = await file.OpenStreamForReadAsync())
            {
                return await Converter.StreamToBytes(stream);
            }
        }
        public static async Task<ulong> GetCacheSize()
        {
            var folder = ApplicationData.Current.TemporaryFolder;
            return await GetFolderSize(folder);
        }

        public async static Task<ulong> GetFolderSize(StorageFolder folder)
        {
            ulong size = 0;
            foreach (StorageFile thisFile in await folder.GetFilesAsync())
            {
                var props = await thisFile.GetBasicPropertiesAsync();
                size += props.Size;
            }
            foreach (StorageFolder thisFolder in await folder.GetFoldersAsync())
            {
                size += await GetFolderSize(thisFolder);
            }

            return size;
        }

        public static async Task ClearCache()
        {
            var folderlist = await ApplicationData.Current.TemporaryFolder.GetFoldersAsync();
            foreach (var folder in folderlist)
            {
                await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            folderlist = await ApplicationData.Current.LocalCacheFolder.GetFoldersAsync();
            foreach (var folder in folderlist)
            {
                await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }
    }
}
