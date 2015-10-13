using System;
using System.Collections.Generic;
using System.Text;

namespace EHTool.Shared.Model
{
    public class TagModel
    {
        public string Name { get; internal set; }
        public List<TagValueModel> Value { get; internal set; }
    }
    public class TagValueModel
    {
        public string Value { get; set; }
        public string FullValue { get; set; }
    }

}
