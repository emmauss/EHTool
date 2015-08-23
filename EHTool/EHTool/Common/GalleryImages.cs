using System.Collections.Generic;
using System.Threading.Tasks;
using EHTool.EHTool.Model;
using HtmlAgilityPack;

using static EHTool.Common.Helpers.HttpHelper;
using static EHTool.Common.Helpers.CookieHelper;

namespace EHTool.EHTool.Common
{
    public class GalleryImages : HostLinkModel
    {
        private IEnumerable<ImageListModel> _imagePageList;

        public GalleryImages(IEnumerable<ImageListModel> list)
        {
            _imagePageList = list;
        }

        public async Task<string> GetImageLink(string uri)
        {
            var htmlstring = await GetStringWithCookie(uri, Cookie);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlstring);
            return doc.GetElementbyId("img").Attributes["src"].Value;
        }

        public async Task<IEnumerable<string>> GetImageLinkList()
        {
            List<string> list = new List<string>();
            foreach (var item in _imagePageList)
            {
                list.Add(await GetImageLink(item.ImagePage));
            }
            return list;
        }
    }
}
