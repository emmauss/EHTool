using System.IO;
using System.Net.NetworkInformation;
using Windows.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using static EHTool.Common.Helpers.CookieHelper;
using static Common.Helpers.CacheHelper;
using static Common.Helpers.HttpHelper;
using static Common.Converters.Converter;
using EHTool.EHTool.Entities;
using Windows.UI.Xaml;

namespace EHTool.EHTool.Model
{
    public abstract class BaseModel : HostLinkModel, INotifyPropertyChanged
    {
        private bool _isbusy;
        private BitmapImage _image;
        #region PropertyChangedMember
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion
        public BaseModel()
        {
            int b = (int)Window.Current.Bounds.Width / 200;
            Width = Window.Current.Bounds.Width / b - 10d;
            Height = Width * 4d / 3d;
            Window.Current.SizeChanged += Current_SizeChanged;
        }
        public double Width { get; private set; }
        public double Height { get; private set; }


        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            int b = (int)Window.Current.Bounds.Width / 200;
            Width = e.Size.Width / b - 10d;
            Height = Width * 4d / 3d;
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Height));
        }
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


        private async void GetImage(string link)
        {
            if (_isbusy)
            {
                return;
            }
            _isbusy = true;
            byte[] imageBytes;
            //check cache if exists
            imageBytes = await GetByteArrayCache(ServerType.ToString(), Path.GetFileName(ImageLink));
            if ((imageBytes == default(byte[]) || imageBytes.Length == 0) && NetworkAvailable)//if not cached
            {
                try
                {
                    imageBytes = await GetByteArrayWith("GET", link, Cookie, HostLink);
                    await SaveByteArrayCache(ServerType.ToString(), Path.GetFileName(ImageLink), imageBytes);
                }
                catch (System.Net.WebException)
                {
                    imageBytes = default(byte[]);
                }
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
