using System.IO;
using Windows.Storage.FileProperties;

namespace EHTool.DHPlayer.Model
{
    public class VideoListModel : MainListModel
    {
        public VideoProperties Detail { get; set; }
        public bool CanSystemPlay { get; set; }
        public string Title
        {
            get
            {
                return Detail?.Title == "" ? Path.GetFileNameWithoutExtension(FilePath) : Detail.Title;
            }
        }
        public string Duration
        {
            get
            {
                return $"{(Detail.Duration.Hours == 0 ? null : $"{Detail.Duration.Hours}:D2:")}{Detail.Duration.Minutes:D2}:{Detail.Duration.Seconds:D2}";
            }
        }
    }
}
