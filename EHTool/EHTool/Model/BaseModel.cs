using System.IO;
using Common;
using System.Net.NetworkInformation;
using Windows.UI.Xaml.Media.Imaging;
using static EHTool.Common.Helpers.CookieHelper;
using static Common.Helpers.CacheHelper;
using static Common.Helpers.HttpHelper;
using static Common.Converters.Converter;

namespace EHTool.EHTool.Model
{
    public abstract class BaseModel : NotifyPropertyChanged
    {
        private bool _isbusy;
        private BitmapImage _image;


        public BitmapImage Image
        {
            get
            {
                if (_image == null)
                {
                    GetImage(ImageLink);
                }
                return _image;
            }
            private set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        internal string SaveFolder { private get; set; }


        private async void GetImage(string link)
        {
            if (_isbusy)
            {
                return;
            }
            _isbusy = true;
            byte[] imageBytes;
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                imageBytes = await GetByteArrayWith("GET", link, Cookie);
                await SaveByteArrayCache(SaveFolder, Path.GetFileName(ImageLink), imageBytes);
            }
            else
            {
                imageBytes = await GetByteArrayCache(SaveFolder, Path.GetFileName(ImageLink));
            }
            if (imageBytes != default(byte[]) && imageBytes.Length != 0)
            {
                Image = await ByteArrayToBitmapImage(imageBytes);
            }
            _isbusy = false;
        }

        public string ImageLink { get; internal set; }

    }
}
