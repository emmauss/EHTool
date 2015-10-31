﻿using EHTool.Shared.Entities;
using EHTool.Shared.Extension;
using EHTool.Shared.Model;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

using static System.Text.RegularExpressions.Regex;
using static HtmlAgilityPack.HtmlEntity;
using static EHTool.Shared.Helpers.HttpHelper;
using static Common.Helpers.CacheHelper;
using static EHTool.Shared.Helpers.CookieHelper;

namespace EHTool.Shared
{
    public class Gallery : HostLinkModel
    {
        public Gallery() : this(ServerTypes.EHentai) { }
        protected int MaxPageCount;
        public Gallery(ServerTypes type)
        {
            ServerType = type;
        }

        public async Task<IEnumerable<GalleryListModel>> GetGalleryList()
        {
            if (ServerType == ServerTypes.ExHentai && !CheckCookie())
            {
                throw new ExHentaiAccessException();
            }
            string folderName = ServerType.ToString();
            string htmlStr = "";
            if (NetworkAvailable)
            {
                htmlStr = await GetStringWithCookie($"{HostLink}?page=0", (ServerType == ServerTypes.ExHentai ? Cookie : null) + Unconfig);
                await SaveTextCache(folderName, "Main", htmlStr);
            }
            else
            {
                htmlStr = await GetTextCache(folderName, "Main");
            }
            return string.IsNullOrEmpty(htmlStr) ? new List<GalleryListModel>() : GetGalleryListFromString(htmlStr);

        }


        public async Task<IEnumerable<GalleryListModel>> GetGalleryList(int page) =>
            await GetGalleryList(null, page);

        public async Task<IEnumerable<GalleryListModel>> GetGalleryList(GallerySearchOption option, int page = 0)
        {
            var htmlStr = NetworkAvailable ? await GetStringWithCookie($"{HostLink}?page={page}{option?.GetLinkExtension()}", (ServerType == ServerTypes.ExHentai ? Cookie : null) + Unconfig) : "";
            return string.IsNullOrEmpty(htmlStr) ? new List<GalleryListModel>() : GetGalleryListFromString(htmlStr);
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
