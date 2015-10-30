using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
#if WINDOWS_UWP
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
#else
using Xamarin.Forms;
#endif

using static EHTool.Shared.Helpers.CookieHelper;
using static Common.Helpers.CacheHelper;
using static EHTool.Shared.Helpers.ConvertHelper;
using static EHTool.Shared.Helpers.HttpHelper;

namespace EHTool.Shared.Model
{

    [DataContract]
    public abstract class BaseModel : HostLinkModel, INotifyPropertyChanged
    {
        [IgnoreDataMember]
        private bool _isbusy;
        [IgnoreDataMember]
#if WINDOWS_UWP
        private BitmapImage _image;
#else
        private ImageSource _image;
#endif
        #region PropertyChangedMember
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion
#if WINDOWS_UWP
        public BaseModel()
        {
            if (IsPhone)
            {
                Width = Window.Current.Bounds.Width / 3 - 10d;
                Height = Width * 4d / 3d;
            }
            else
            {
                int b = (int)Window.Current.Bounds.Width / 200;
                Width = Window.Current.Bounds.Width / b - 10d;
                Height = Width * 4d / 3d;
                Window.Current.SizeChanged += Current_SizeChanged;
            }
        }
        [IgnoreDataMember]
        public double Width { get; private set; }
        [IgnoreDataMember]
        public double Height { get; private set; }
        [IgnoreDataMember]
        public bool IsPhone => Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");
        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            int b = (int)Window.Current.Bounds.Width / 200;
            Width = e.Size.Width / b - 10d;
            Height = Width * 4d / 3d;
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Height));
        }
#endif
        [IgnoreDataMember]
#if WINDOWS_UWP
        public BitmapImage Image
#else
        public ImageSource Image
#endif
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
            System.Diagnostics.Debug.WriteLine($"{ImageLink} req!");
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
                catch (System.Net.Http.HttpRequestException)
                {
                    imageBytes = default(byte[]);
                }
                catch (System.Net.WebException)
                {
                    imageBytes = default(byte[]);
                }
                catch(System.ArgumentException e)
                {

                }
            }
            if (imageBytes != default(byte[]) && imageBytes.Length != 0)
            {
#if WINDOWS_UWP
                Image = await ByteArrayToBitmapImage(imageBytes);
#else
                Image = ImageSource.FromStream(() => new MemoryStream(imageBytes));
#endif
            }
            _isbusy = false;
        }
        [DataMember]
        public string ImageLink { get; internal set; }

    }
}
