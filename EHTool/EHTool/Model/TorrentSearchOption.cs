using System;
using EHTool.EHTool.Interface;

namespace EHTool.EHTool.Model
{
    public class TorrentSearchOption : ISearchOption
    {
        public string KeyWord { get; set; }

        public string GetLinkExtension()
        {
            throw new NotImplementedException();
        }
    }
}
