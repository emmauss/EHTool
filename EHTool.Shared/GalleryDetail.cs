using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EHTool.Shared.Model;
using HtmlAgilityPack;
using EHTool.Shared.Entities;
using EHTool.Shared.Extension;

using static Common.Helpers.CacheHelper;
using static System.Text.RegularExpressions.Regex;
using static HtmlAgilityPack.HtmlEntity;
using static EHTool.Shared.Helpers.HttpHelper;
using static EHTool.Shared.Helpers.CookieHelper;
using System;
using System.Net.Http;

namespace EHTool.Shared
{
    public class GalleryDetail : HostLinkModel
    {
        public string Link { get; protected set; }
        public string Id { get; protected set; }
        public string Token { get; protected set; }
        public GalleryListModel ListItem { get; protected set; }
        public GalleryDetail()
        {

        }

        public GalleryDetail(string id, string token, ServerTypes type)
        {
            Id = id;
            Token = token;
            ServerType = type;
            Link = $"{HostLink}g/{Id}/{Token}";
        }

        public GalleryDetail(GalleryListModel item):this(item.ID,item.Token,item.ServerType)
        {
            ListItem = item;
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

        public async Task<IEnumerable<ImageListModel>> GetImagePageList()
        {
            /// TODO:Solve the problem about the cookie will send more than once
            /// Below doesn't work well because GetStringWithCookie well send Cookie more than once
            /// I have no idea why,can anyone help me?
            ///
            //var detailHtmlStr = await GetStringWithCookie(Link, Cookie + ";uconfig=tl_m-uh_y-tr_2-ts_m-dm_t-ar_0-xns_0-rc_0-rx_0-ry_0-cs_a-to_a-pn_0-sc_0-cats_0-prn_y-ms_n-mt_n-sa_y-oi_n-qb_n-tf_n-hp_-hk_-xl_");
            //HtmlDocument doc = new HtmlDocument();                       
            //doc.LoadHtml(detailHtmlStr);
            //var detailPageCount = int.Parse(doc.DocumentNode.GetNodeByClassName("ptt").FirstChild.ChildNodes[doc.DocumentNode.GetNodeByClassName("ptt").FirstChild.ChildNodes.Count - 2].InnerText);
            //var nodeList = doc.GetElementbyId("gdt").GetNodesByClassName("gdtm");
            //if (detailPageCount > 1)
            //{
            //    string[] pagelinklist = new string[detailPageCount - 1];
            //    for (int i = 1; i < detailPageCount; i++)
            //    {
            //        pagelinklist[i - 1] = $"{HostLink}g/{Id}/{Token}/?p={i}";
            //    }
            //    var downloads = pagelinklist.Select(link => GetStringWithCookie(link, Cookie + ";uconfig=tl_m-uh_y-tr_2-ts_m-dm_t-ar_0-xns_0-rc_0-rx_0-ry_0-cs_a-to_a-pn_0-sc_0-cats_0-prn_y-ms_n-mt_n-sa_y-oi_n-qb_n-tf_n-hp_-hk_-xl_"));
            //    var task = Task.WhenAll(downloads);                                            
            //    var htmls = await task;
            //    if (task.Status == TaskStatus.RanToCompletion)
            //    {
            //        foreach (var item in htmls)
            //        {
            //            HtmlDocument tempdoc = new HtmlDocument();
            //            tempdoc.LoadHtml(detailHtmlStr);
            //            nodeList = nodeList.Concat(tempdoc.GetElementbyId("gdt").GetNodesByClassName("gdtm"));
            //        }
            //    }
            //    else
            //    {
            //        throw new System.Net.WebException();
            //    }
            //}
            //return from node in nodeList
            //       where node.HasChildNodes
            //       select new ImageListModel
            //       {
            //           ServerType = ServerType,
            //           ImageName = node.InnerText,
            //           ImageIndex = int.Parse(node.GetNodeByName("img").Attributes["alt"].Value),
            //           ImagePage = $"{node.GetNodeByName("a").Attributes["href"].Value}?",
            //       };

            var detail = await GetDetail();
            IEnumerable<ImageListModel> list = detail.ImageList;
            if (detail.DetailPageCount > 1)
            {
                string[] pagelinklist = new string[detail.DetailPageCount - 1];
                for (int i = 1; i < detail.DetailPageCount; i++)
                {
                    pagelinklist[i - 1] = $"{HostLink}g/{Id}/{Token}/?p={i}";
                }
                var downloads = pagelinklist.Select(link => GetStringWithCookie(link, Cookie + Unconfig));
                var task = Task.WhenAll(downloads);
                var htmls = await task;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    foreach (var item in htmls)
                    {
                        list = list.Concat(GetDetailFromString(item).ImageList);
                    }
                    list = list.Distinct((a) => a.ImagePage);
                    list = list.OrderBy((a) => a.ImageIndex);
                }
                else
                {
                    throw new System.Net.WebException();
                }
            }
            return list;
        }


        //public async Task<IEnumerable<ImageListModel>> GetImagePageList(IEnumerable<ImageListModel> pageList, int pageCount)
        //{
        //    var list = pageList;
        //    for (int i = 1; i < pageCount; i++)
        //    {
        //        var temp = await GetDetail(i);
        //        list = list.Concat(temp.ImageList);
        //    }
        //    return list;
        //}

        public async Task<DetailModel> GetDetail(int page = 0)
        {
            string link = $"{HostLink}g/{Id}/{Token}/?p={page}";
            string htmlStr = await CheckIfCached(link, page == 0);
            var detailItem = string.IsNullOrEmpty(htmlStr) ? null : GetDetailFromString(htmlStr);
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
                    await SaveTextCache(ServerType.ToString(), Id, htmlStr);
                }
                else
                {
                    htmlStr = await GetTextCache(ServerType.ToString(), Id);
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
            var link = $"{HostLink}gallerytorrents.php?gid={Id}&t={Token}";

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
                       PostDate = b.GetElements("tr").ToList()[0].GetElements("td").ToList()[0].InnerText,
                       Size = b.GetElements("tr").ToList()[0].GetElements("td").ToList()[1].InnerText,
                       Seeds = b.GetElements("tr").ToList()[0].GetElements("td").ToList()[3].InnerText,
                       Peers = b.GetElements("tr").ToList()[0].GetElements("td").ToList()[4].InnerText,
                       Downloads = b.GetElements("tr").ToList()[0].GetElements("td").ToList()[5].InnerText,
                       Uploader = b.GetElements("tr").ToList()[1].GetElements("td").ToList()[0].InnerText,
                       Name = b.GetElements("tr").ToList()[2].Element("td").Element("a").InnerText,
                       TorrentLink = b.GetElements("tr").ToList()[2].Element("td").Element("a").Attributes["href"].Value,
                   };
        }



        private DetailModel GetDetailFromString(string htmlstring)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlstring);
            return new DetailModel
            {
                Header = new HeaderModel
                {
                    ImageLink = (doc.GetElementbyId("gd1").Element("img").Attributes["src"].Value),
                    TitleEn = DeEntitize(doc.GetElementbyId("gn").InnerText),
                    TitleJp = DeEntitize(doc.GetElementbyId("gj").InnerText),
                    Tags = (from a in doc.GetElementbyId("taglist").FirstChild.ChildNodes
                            select new TagModel
                            {
                                Name = DeEntitize(a.FirstChild.InnerText),
                                Value = (from b in a.LastChild.ChildNodes
                                         select new TagValueModel()
                                         {
                                             Value = DeEntitize(b.InnerText),
                                             FullValue = DeEntitize(a.FirstChild.InnerText) + DeEntitize(b.InnerText),
                                         }).ToList(),
                            }).ToList(),
                },
                TorrentCount = int.Parse(Match(doc.GetElementbyId("gd5").InnerHtml, "Torrent Download \\( ([0-9]+) \\)").Groups[1].Value),
                RateValue = double.Parse(Match(doc.GetElementbyId("rating_label").InnerText, "([0-9]+.[0-9]+)").Value),
                UpLoadInformation = (from a in doc.GetElementbyId("gdd").FirstChild.ChildNodes
                                     select DeEntitize(a.InnerText)).ToArray(),
                MaxImageCount = GetMaxImageCount(doc.DocumentNode.GetNodeByClassName("gpc").InnerText),
                DetailPageCount = int.Parse(doc.DocumentNode.GetNodeByClassName("ptt").FirstChild.ChildNodes[doc.DocumentNode.GetNodeByClassName("ptt").FirstChild.ChildNodes.Count - 2].InnerText),
                ImageList = (from a in doc.GetElementbyId("gdt").ChildNodes
                             where a.HasChildNodes
                             select new ImageListModel
                             {
                                 ServerType = ServerType,
                                 ImageName = a.InnerText,
                                 ImageIndex = int.Parse(a.GetNodeByName("img").Attributes["alt"].Value),
                                 ImageLink = a.GetNodeByName("img").Attributes["src"].Value,
                                 ImagePage = $"{a.GetNodeByName("a").Attributes["href"].Value}?",
                             }).ToList(),
                CommentList = (from a in doc.GetElementbyId("cdiv").ChildNodes
                               where a.HasChildNodes && a.FirstChild.Name == "div"
                               select new CommentModel
                               {
                                   Poster = DeEntitize(a.GetNodeByClassName("c3").InnerText),
                                   Content = DeEntitize(a.GetNodeByClassName("c6").InnerText),
                                   Score = a.GetNodeByClassName("c5 nosel") != null ? a.GetNodeByClassName("c5 nosel").InnerText : a.GetNodeByClassName("c4 nosel") != null ? a.GetNodeByClassName("c4 nosel").InnerText : "",
                                   Base = a.GetNodeByClassName("c7") == null ? "" : a.GetNodeByClassName("c7").InnerText,
                               }).ToList(),
            };
        }
    }

}
