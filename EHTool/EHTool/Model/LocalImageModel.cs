using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EHTool.EHTool.Model
{
    public class LocalImageModel : ImageModel
    {
        private StorageFile storageFile;

        public LocalImageModel(StorageFile storageFile)
        {
            this.storageFile = storageFile;
        }

        protected override async Task GetImageOverrideAsync()
        {
            using (var fstream = await storageFile.OpenStreamForReadAsync())
            {
                using (var franstream = fstream.AsRandomAccessStream())
                {
                    _image = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                    await _image.SetSourceAsync(franstream);
                }
                OnPropertyChanged(nameof(Image));
            }
        }

        protected override async Task RefreshOverrideAsync()
        {

        }
    }
}
