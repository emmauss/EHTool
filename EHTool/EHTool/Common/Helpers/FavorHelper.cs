using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHTool.EHTool.Model;
using Windows.Storage;
using static EHTool.EHTool.Common.Helpers.DatabaseHelper;

namespace EHTool.EHTool.Common.Helpers
{
    public static class FavorHelper
    {
        private const string SAVE_DATABASE_NAME = "Favor.json";

        public static async Task<IEnumerable<GalleryListModel>> GetFavorList()
        {
            return await GetList(SAVE_DATABASE_NAME);
        }
        public static async Task AddFavor(GalleryListModel item)
        {
            await Add(item, SAVE_DATABASE_NAME);
        }
        public static async Task RemoveFavor(GalleryListModel item)
        {
            await Remove(item, SAVE_DATABASE_NAME);
        }
        public static async Task RemoveFavor(string id)
        {
            await Remove(id, SAVE_DATABASE_NAME);
        }
        public static async Task<bool> IsFavor(GalleryListModel item)
        {
            return await IsExists(item, SAVE_DATABASE_NAME);
        }
        public static async Task<bool> IsFavor(string id)
        {
            return await IsExists(id, SAVE_DATABASE_NAME);
        }
    }
}
