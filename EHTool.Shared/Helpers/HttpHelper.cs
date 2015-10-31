using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EHTool.Shared.Helpers
{
    static class HttpHelper
    {

        internal static async Task<byte[]> GetByteArrayWith(string method, string link, string cookie, string referer)
        {
            var req = WebRequest.CreateHttp(link);
#if WINDOWS_UWP
            req.Headers["Cookie"] = cookie;
            req.Headers["Referer"] = referer;
#else
            //if (!string.IsNullOrEmpty(cookie))
            //{
            //    req.Headers[HttpRequestHeader.Cookie] = cookie;
            //}
            //req.Headers[HttpRequestHeader.Referer] = referer;
#endif
            req.Method = method;
            var res = await req.GetResponseAsync();
            using (var resStream = res.GetResponseStream())
            {
                return await ConvertHelper.StreamToBytes(resStream);
            }
        }


        internal async static Task<string> GetStringWithCookie(string uriString, string cookie)
        {
            using (var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None, UseCookies = false, })
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Cookie", cookie);
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                return await client.GetStringAsync(uriString);
            }
        }
    }
}
