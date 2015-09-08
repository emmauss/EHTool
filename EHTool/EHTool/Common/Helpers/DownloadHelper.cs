using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHTool.EHTool.Model;
using static EHTool.EHTool.Common.Helpers.DatabaseHelper;

namespace EHTool.EHTool.Common.Helpers
{
    public static class DownloadHelper
    {
        private const string SAVE_DATABASE_NAME = "Download.json";

        public static async Task<IEnumerable<GalleryListModel>> GetDownloadList()
        {
            return await GetList(SAVE_DATABASE_NAME);
        }
        public static async Task AddDownload(GalleryListModel item)
        {
            await Add(item, SAVE_DATABASE_NAME);
        }
        public static async Task RemoveDownload(GalleryListModel item)
        {
            await Remove(item, SAVE_DATABASE_NAME);
        }
        public static async Task RemoveDownload(string id)
        {
            await Remove(id, SAVE_DATABASE_NAME);
        }
        public static async Task<bool> IsDownload(GalleryListModel item)
        {
            return await IsExists(item, SAVE_DATABASE_NAME);
        }
        public static async Task<bool> IsDownload(string id)
        {
            return await IsExists(id, SAVE_DATABASE_NAME);
        }
    }
}
