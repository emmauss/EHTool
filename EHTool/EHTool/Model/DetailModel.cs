using System.Collections.Generic;

namespace EHTool.EHTool.Model
{
    public class DetailModel
    {
        public List<CommentModel> CommentList { get; internal set; }
        public int DetailPageCount { get; internal set; }
        public HeaderModel HeaderModel { get; internal set; }
        public List<ImageListModel> ImageList { get; internal set; }
        public int MaxImageCount { get; internal set; }
        public List<UpLoadModel> UpLoadModel { get; internal set; }
        public int TorrentCount { get; internal set; }
        public double RateValue { get; internal set; }
    }
}
