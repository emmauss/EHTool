using System.Text.RegularExpressions;
using EHTool.EHTool.Entities;
using Windows.UI.Xaml;

namespace EHTool.EHTool.Model
{
    public class GalleryListModel : BaseModel
    {
        public string FileCount { get; internal set; }
        public string Link { get; internal set; }
        internal Match LinkMatch { private get; set; }
        public string Title { get; internal set; }
        public string Token => LinkMatch.Groups[2].Value;
        public string ID => LinkMatch.Groups[1].Value;

        public string Type { get; internal set; }
    }
}
