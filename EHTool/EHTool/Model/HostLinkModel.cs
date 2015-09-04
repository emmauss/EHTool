using System.Net.NetworkInformation;
using EHTool.EHTool.Entities;

namespace EHTool.EHTool.Model
{
    public abstract class HostLinkModel
    {
        private const string EHentaiLink = "http://g.e-hentai.org/";
        private const string ExHentaiLink = "http://exhentai.org/";

        public ServerTypes ServerType { get; internal set; }

        protected bool NetworkAvailable => NetworkInterface.GetIsNetworkAvailable();
        protected string HostLink =>
            ServerType == ServerTypes.EHentai ? EHentaiLink : ExHentaiLink;
    }
}
