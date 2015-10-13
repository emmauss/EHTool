using EHTool.Shared.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using static EHTool.Shared.Helpers.HttpHelper;
using static EHTool.Shared.Helpers.CookieHelper;

namespace EHTool.Shared
{
    public class GalleryImages : HostLinkModel
    {
        private IEnumerable<ImageListModel> _imagePageList;

        public GalleryImages()
        {

        }

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
