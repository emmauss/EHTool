using System.Runtime.Serialization;

namespace EHTool.Shared.Model
{
    [DataContract]
    public class GalleryListModel : BaseModel
    {
        [DataMember]
        public string FileCount { get; internal set; }
        [DataMember]
        public string Link { get; internal set; }
        [DataMember]
        public string Title { get; internal set; }
        [DataMember]
        public string Type { get; internal set; }
        [DataMember]
        public string Token { get; internal set; }
        [DataMember]
        public string ID { get; internal set; }
    }
}
