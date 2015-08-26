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
        public BitmapImage ThumbImage { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
    }
}
