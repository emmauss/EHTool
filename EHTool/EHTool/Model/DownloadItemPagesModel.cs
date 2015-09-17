using EHTool.EHTool.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
