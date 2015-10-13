using EHTool.EHTool.Entities;
using EHTool.Shared.Model;
using System.Runtime.Serialization;

namespace EHTool.EHTool.Model
{
    [DataContract]
    public class DownloadItemPagesModel
    {
        [DataMember]
        public DownloadState State { get; internal set; }
        [DataMember]
        public string Link { get; internal set; }
        public DownloadItemPagesModel()
        {

        }
        public DownloadItemPagesModel(ImageListModel item)
        {
            Link = item.ImagePage;
            State = DownloadState.UnDownloaded;
        }
    }
}
