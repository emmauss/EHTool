using System.Collections.Generic;

namespace EHTool.EHTool.Model
{
    public class HeaderModel : BaseModel
    {
        public List<TagModel> Tags { get; internal set; }
        public string TitleEn { get; internal set; }
        public string TitleJp { get; internal set; }
    }
}
