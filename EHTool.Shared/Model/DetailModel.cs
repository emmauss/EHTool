using System.Collections.Generic;

namespace EHTool.Shared.Model
{
    public class DetailModel
    {
        public List<CommentModel> CommentList { get; internal set; }
        public int DetailPageCount { get; internal set; }
        public HeaderModel Header { get; internal set; }
        public List<ImageListModel> ImageList { get; internal set; }
        public int MaxImageCount { get; internal set; }
        public string[] UpLoadInformation { get; internal set; }
        public int TorrentCount { get; internal set; }
        public double RateValue { get; internal set; }
    }
}
