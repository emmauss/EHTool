using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EHTool.EHTool.Entities;
using EHTool.EHTool.Model;
using HtmlAgilityPack;
using EHTool.Common.Extension;

using static EHTool.Common.Helpers.CacheHelper;
using static System.Text.RegularExpressions.Regex;
using static HtmlAgilityPack.HtmlEntity;
using static EHTool.Common.Helpers.HttpHelper;
using static EHTool.Common.Helpers.CookieHelper;

namespace EHTool.EHTool.Common
{

    public class Gallery : HostLinkModel
    {
        public Gallery() : this(ServerTypes.EHentai) { }

        public Gallery(ServerTypes type)
        {
            ServerType = type;
        }
        


        public async Task<IEnumerable<GalleryListModel>> GetGalleryList()
        {
            string folderName = ServerType.ToString();
            string htmlStr = "";
            if (NetworkAvailable)
            {
                htmlStr = await GetStringWithCookie($"{HostLink}?page=0", Cookie + Unconfig);
                await SaveTextCache(folderName, "Main", htmlStr);
            }
            else
            {
                htmlStr = await GetTextCache(folderName, "Main");
            }
            return htmlStr != "" ? GetGalleryListFromString(htmlStr) : null;
            
        }


        public async Task<IEnumerable<GalleryListModel>> GetGalleryList(int page)
        {
            return await GetGalleryList(null, page);
        }

        public async Task<IEnumerable<GalleryListModel>> GetGalleryList(GallerySearchOption option, int page = 0)
        {
            var htmlStr = NetworkAvailable ? await GetStringWithCookie($"{HostLink}?page={page}{option?.GetLinkExtension()}", Cookie + Unconfig) : "";
            return htmlStr != "" ? GetGalleryListFromString(htmlStr) : null;
        }

        private IEnumerable<GalleryListModel> GetGalleryListFromString(string htmlString)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlString);
            var htmlnode = doc.DocumentNode.GetNodebyClassName("itg");
            return from a in htmlnode.ChildNodes
                   where a.HasChildNodes
                   select new GalleryListModel
                   {
                       Title = DeEntitize(a.GetNodebyClassName("id2").InnerText),
                       ImageLink = (a.GetNodebyClassName("id3").Element("a").Element("img").Attributes["src"].Value),
                       Link = (a.GetNodebyClassName("id2").Element("a").Attributes["href"].Value),
                       FliesCount = (a.GetNodebyClassName("id42").InnerText),
                       LinkMatch = Match(a.GetNodebyClassName("id2").Element("a").Attributes["href"].Value, "/g/([^/]+)/([^-]+)/"),
                       SaveFolder = ServerType.ToString(),
                   };
        }

    }
}
