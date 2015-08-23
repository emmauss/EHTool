using System.Text.RegularExpressions;

namespace EHTool.EHTool.Model
{
    public class GalleryListModel : BaseModel
    {
        public string FliesCount { get; internal set; }
        public string Link { get; internal set; }
        internal Match LinkMatch { private get; set; }
        public string Title { get; internal set; }
        public string Token
        {
            get
            {
                return LinkMatch.Groups[2].Value;
            }
        }
        public string ID
        {
            get
            {
                return LinkMatch.Groups[1].Value;
            }
        }
    }
}
