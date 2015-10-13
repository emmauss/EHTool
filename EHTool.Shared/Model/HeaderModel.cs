using System.Collections.Generic;

namespace EHTool.Shared.Model
{
    public class HeaderModel : BaseModel
    {
        public List<TagModel> Tags { get; internal set; }
        public string TitleEn { get; internal set; }
        public string TitleJp { get; internal set; }
    }
}
