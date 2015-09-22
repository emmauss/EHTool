using System.Collections.Generic;

namespace EHTool.EHTool.Model
{
    public class TagModel
    {
        public string Name { get; internal set; }
        public List<TagValueModel> Value { get; internal set; }
    }
}
