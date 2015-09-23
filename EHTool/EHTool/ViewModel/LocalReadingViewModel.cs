using EHTool.EHTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Windows.UI.Popups;

namespace EHTool.EHTool.ViewModel
{
    public class LocalReadingViewModel : ReadingViewModel
    {
        private LocalFolderModel _item;
        public LocalReadingViewModel(LocalFolderModel item):base()
        {
            _item = item;
            LoadList();
        }
        protected override async Task LoadListOverride()
        {
            var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(_item.FolderToken);
            var list = await folder.GetFilesAsync();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ContentType.ToLower().Contains("image"))
                {
                    ImageList.Add(new LocalImageModel(list[i]));
                }
            }
            if (ImageList.Count == 0)
            {
                MessageDialog dialog = new MessageDialog("Can not find any image in this folder", "Error");
                await dialog.ShowAsync();
            }
        }
    }
}
