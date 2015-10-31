using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using EHTool.Shared.Model;
using EHTool.Shared.Entities;
using System.Threading.Tasks;
using EHTool.Shared.Extension;

using static System.Text.RegularExpressions.Regex;
using static HtmlAgilityPack.HtmlEntity;
using static EHTool.Shared.Helpers.HttpHelper;
using static EHTool.Shared.Helpers.CookieHelper;

namespace EHTool.Shared
{
    public class TagGallery : HostLinkModel
    {
        protected int MaxPageCount;
        protected string _tagValue;
        public TagGallery() : this(null, ServerTypes.EHentai)
        {

        }
        public TagGallery(string tagValue, ServerTypes type)
        {
            _tagValue = tagValue;
            ServerType = type;
        }
        public async Task<IEnumerable<GalleryListModel>> GetGalleryList() => await GetGalleryList(0);

        public async Task<IEnumerable<GalleryListModel>> GetGalleryList(int page)
        {
            var link = $"{HostLink}tag/{_tagValue}/{page}";
            string folderName = ServerType.ToString();
            string htmlStr = "";
            if (NetworkAvailable)
            {
                htmlStr = await GetStringWithCookie(link, (ServerType == ServerTypes.ExHentai ? Cookie : null) + Unconfig);
            }
            return htmlStr != "" ? GetGalleryListFromString(htmlStr) : new List<GalleryListModel>();
        }

        protected IEnumerable<GalleryListModel> GetGalleryListFromString(string htmlString)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlString);
            var ptt = doc.DocumentNode.GetNodeByClassName("ptt").FirstChild;
            MaxPageCount = int.Parse(ptt.ChildNodes[ptt.ChildNodes.Count - 2].InnerText);
            var htmlnode = doc.DocumentNode.GetNodeByClassName("itg");
            return from a in htmlnode.ChildNodes
                   where a.HasChildNodes
                   select new GalleryListModel
                   {
                       Title = DeEntitize(a.GetNodeByClassName("id2").InnerText),
                       ImageLink = (a.GetNodeByClassName("id3").Element("a").Element("img").Attributes["src"].Value),
                       Link = (a.GetNodeByClassName("id2").Element("a").Attributes["href"].Value),
                       FileCount = (a.GetNodeByClassName("id42").InnerText),
                       Token = Match(a.GetNodeByClassName("id2").Element("a").Attributes["href"].Value, "/g/([^/]+)/([^-]+)/").Groups[2].Value,
                       ID = Match(a.GetNodeByClassName("id2").Element("a").Attributes["href"].Value, "/g/([^/]+)/([^-]+)/").Groups[1].Value,
                       Type = a.GetNodeByClassName("id41").Attributes["title"].Value,
                       ServerType = ServerType,
                   };
        }

    }
}
