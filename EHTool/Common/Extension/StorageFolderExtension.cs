using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Common.Extension
{
    public static class StorageFolderExtension
    {
        public static async Task<StorageFile> GetFileWithoutExtensionAsync(this StorageFolder folder, string name)
        {
            try
            {
                var list = await folder.GetFilesAsync();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].DisplayName == name)
                    {
                        return list[i];
                    }
                }
                return null;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
        }
    }
}
