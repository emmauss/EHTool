using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Extension;
using EHTool.EHTool.Model;
using HtmlAgilityPack;

using static Common.Helpers.CacheHelper;
using static System.Text.RegularExpressions.Regex;
using static HtmlAgilityPack.HtmlEntity;
using static Common.Helpers.HttpHelper;
using static EHTool.Common.Helpers.CookieHelper;

namespace EHTool.EHTool.Common
{
    public class GalleryDetail : HostLinkModel
    {

        protected string _id;
        protected string _token;

        public GalleryDetail(string id, string token)
        {
            _id = id;
            _token = token;
        }


        private int GetMaxImageCount(string Str)
        {
            var mates = Matches(Str, "[0-9][0-9]{0,}");
            int returnint;
            if (int.TryParse(mates[2].Value, out returnint))
            {
                return returnint;
            }
            else
            {
                return 0;
            }
        }


        public async Task<IEnumerable<ImageListModel>> GetImagePageList(IEnumerable<ImageListModel> pageList, int pageCount)
        {
            var list = pageList;
            for (int i = 1; i < pageCount; i++)
            {
                var temp = await GetDetail(i);
                list = list.Concat(temp.ImageList);
            }
            return list;
        }

        public async Task<DetailModel> GetDetail(int page = 0)
        {
            string link = $"{HostLink}g/{_id}/{_token}/?p={page}";
            string htmlStr = await CheckIfCached(link, page == 0);
            var detailItem = htmlStr != "" ? GetDetailFromString(htmlStr) : null;
            return detailItem;
        }
        private async Task<string> CheckIfCached(string link, bool canCache)
        {
            var htmlStr = "";
            if (canCache)
            {
                if (NetworkAvailable)
                {
                    htmlStr = await GetStringWithCookie(link, Cookie + Unconfig);
                    await SaveTextCache(ServerType.ToString(), _id, htmlStr);
                }
                else
                {
                    htmlStr = await GetTextCache(ServerType.ToString(), _id);
                }
            }
            else
            {
                htmlStr = NetworkAvailable ? await GetStringWithCookie(link, Cookie + Unconfig) : "";
            }
            return htmlStr;
        }


        public async Task<IEnumerable<TorrentModel>> GetTorrentList()
        {
            var link = $"{HostLink}gallerytorrents.php?gid={_id}&t={_token}";

            var htmlstr = await GetStringWithCookie(link, Cookie + Unconfig);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlstr);
            var torrentInfoDiv = doc.GetElementbyId("torrentinfo").ChildNodes[1];
            return from a in torrentInfoDiv.ChildNodes
                   where a.HasChildNodes && a.Name == "div"
                   from b in a.ChildNodes
                   where b.Name == "table"
                   select new TorrentModel()
                   {
                       PostDate = b.GetElements("tr")[0].GetElements("td")[0].InnerText,
                       Size = b.GetElements("tr")[0].GetElements("td")[1].InnerText,
                       Seeds = b.GetElements("tr")[0].GetElements("td")[3].InnerText,
                       Peers = b.GetElements("tr")[0].GetElements("td")[4].InnerText,
                       Downloads = b.GetElements("tr")[0].GetElements("td")[5].InnerText,
                       Uploader = b.GetElements("tr")[1].GetElements("td")[0].InnerText,
                       Name = b.GetElements("tr")[2].Element("td").Element("a").InnerText,
                       TorrentLink = b.GetElements("tr")[2].Element("td").Element("a").Attributes["href"].Value,
                   };
        }



        private DetailModel GetDetailFromString(string htmlstring)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlstring);
            return new DetailModel
            {
                HeaderModel = new HeaderModel
                {
                    ImageLink = (doc.GetElementbyId("gd1").Element("img").Attributes["src"].Value),
                    TitleEn = DeEntitize(doc.GetElementbyId("gn").InnerText),
                    TitleJp = DeEntitize(doc.GetElementbyId("gj").InnerText),
                    TagModel = (from a in doc.GetElementbyId("taglist").FirstChild.ChildNodes
                                select new TagModel
                                {
                                    Name = DeEntitize(a.FirstChild.InnerText),
                                    Value = (from b in a.LastChild.ChildNodes
                                             select DeEntitize(b.InnerText)).ToArray(),
                                }).ToList(),
                },
                TorrentCount = int.Parse(Match(doc.GetElementbyId("gd5").InnerHtml, "Torrent Download \\( ([0-9]+) \\)").Groups[1].Value),
                RateValue = double.Parse(Match(doc.GetElementbyId("rating_label").InnerText, "([0-9]+.[0-9]+)").Value),
                UpLoadModel = (from a in doc.GetElementbyId("gdd").FirstChild.ChildNodes
                               select new UpLoadModel
                               {
                                   Value = DeEntitize(a.InnerText),
                               }).ToList(),
                MaxImageCount = GetMaxImageCount(doc.DocumentNode.GetNodebyClassName("gpc").InnerText),
                DetailPageCount = doc.DocumentNode.GetNodebyClassName("ptt").FirstChild.ChildNodes.Count - 2,
                ImageList = (from a in doc.GetElementbyId("gdt").ChildNodes
                             where a.HasChildNodes
                             select new ImageListModel
                             {
                                 ImageName = (a.InnerText),
                                 ImageIndex = a.FirstChild.FirstChild.Attributes["alt"].Value,
                                 ImageLink = a.FirstChild.FirstChild.Attributes["src"].Value,
                                 ImagePage = (a.FirstChild.Attributes["href"].Value) + "?",
                             }).ToList(),
                CommentList = (from a in doc.GetElementbyId("cdiv").ChildNodes
                               where a.HasChildNodes && a.FirstChild.Name == "div"
                               select new CommentModel
                               {
                                   Poster = DeEntitize(a.GetNodebyClassName("c3").InnerText),
                                   Content = DeEntitize(a.GetNodebyClassName("c6").InnerText),
                                   Score = a.GetNodebyClassName("c5 nosel") != null ? a.GetNodebyClassName("c5 nosel").InnerText : a.GetNodebyClassName("c4 nosel") != null ? a.GetNodebyClassName("c4 nosel").InnerText : "",
                                   Base = a.GetNodebyClassName("c7") == null ? "" : a.GetNodebyClassName("c7").InnerText,
                               }).ToList(),
            };
        }
    }
}
