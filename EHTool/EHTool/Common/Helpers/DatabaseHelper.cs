﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EHTool.EHTool.Model;
using Windows.Data.Json;
using Windows.Storage;
using Common.Extension;
using static Common.Helpers.JsonHelper;

namespace EHTool.EHTool.Common.Helpers
{
    public static class DatabaseHelper
    {
        public static async Task<IEnumerable<GalleryListModel>> GetList(string dbName)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            return jsStr != "" ? FromJson(jsStr) : new List<GalleryListModel>();
        }

        public static async Task Add(GalleryListModel item, string dbName)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<GalleryListModel>>(jsStr) : new List<GalleryListModel>();
            var index = items.FindIndex((a) => { return a.ID == item.ID; });
            if (index == -1)
            {
                items.Add(item);
                jsStr = ToJson(items);
                await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            }
        }
        public static async Task Remove(GalleryListModel item, string dbName)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<GalleryListModel>>(jsStr) : new List<GalleryListModel>();
            var index = items.FindIndex((a) => { return a.ID == item.ID; });
            if (index != -1)
            {
                items.RemoveAt(index);
                jsStr = ToJson(items);
                await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            }
        }
        public static async Task Remove(string id, string dbName)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<GalleryListModel>>(jsStr) : new List<GalleryListModel>();
            var index = items.FindIndex((a) => { return a.ID == id; });
            if (index != -1)
            {
                items.RemoveAt(index);
                jsStr = ToJson(items);
                await FileIO.WriteTextAsync(file, jsStr, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            }
        }
        public static async Task<bool> IsExists(GalleryListModel item, string dbName)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<GalleryListModel>>(jsStr) : new List<GalleryListModel>();
            var index = items.FindIndex((a) => { return a.ID == item.ID; });
            if (index == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static async Task<bool> IsExists(string id, string dbName)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);
            var jsStr = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf16LE);
            var items = jsStr != "" ? FromJson<List<GalleryListModel>>(jsStr) : new List<GalleryListModel>();
            var index = items.FindIndex((a) => { return a.ID == id; });
            if (index == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static IEnumerable<GalleryListModel> FromJson(string json)
            => from item in JsonArray.Parse(json)
               select new GalleryListModel
               {
                   ServerType = (Entities.ServerTypes)item.GetNamedNumber("ServerType"),
                   ImageLink = item.GetNamedString("ImageLink"),
                   FileCount = item.GetNamedString("FileCount"),
                   ID = item.GetNamedString("ID"),
                   Link = item.GetNamedString("Link"),
                   Title = item.GetNamedString("Title"),
                   Token = item.GetNamedString("Token"),
                   Type = item.GetNamedString("Type"),
               };

    }
}
