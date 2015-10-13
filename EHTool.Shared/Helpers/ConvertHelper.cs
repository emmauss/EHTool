using System.IO;
using System.Threading.Tasks;

#if WINDOWS_UWP
using System;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
#else
using Xamarin.Forms;
#endif

namespace EHTool.Shared.Helpers
{
    static class ConvertHelper
    {
        internal static async Task<byte[]> StreamToBytes(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await input.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
#if WINDOWS_UWP
        internal static async Task<BitmapImage> ByteArrayToBitmapImage(byte[] byteArray)
        {
            var bitmapImage = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(byteArray.AsBuffer());
                stream.Seek(0);
                await bitmapImage.SetSourceAsync(stream);
            }
            return bitmapImage;
        }
#else
        internal static ImageSource ByteArrayToBitmapImage(byte[] byteArray)
        {
            ImageSource source;
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                source = StreamImageSource.FromStream(()=>ms);
                return source;
            }
        }
#endif


    }
}
