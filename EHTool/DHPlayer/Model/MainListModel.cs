using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace EHTool.DHPlayer.Model
{
    public class MainListModel
    {
        public BitmapImage ThumbImage { get; internal set; }
        public string Name { get; internal set; }
        public string FilePath { get; internal set; }
    }
}
