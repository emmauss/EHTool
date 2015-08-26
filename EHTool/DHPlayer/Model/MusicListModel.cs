using System;
using System.IO;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace EHTool.DHPlayer.Model
{
    public class MusicListModel : MainListModel
    {
        public MusicProperties Detail { get; set; }
        public string Genre
        {
            get
            {
                return Detail.Genre.Count == 0 ? null : Detail.Genre[0];
            }
        }
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
